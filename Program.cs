using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using osu_tracker.api;
using osu_tracker.embed;

namespace osu_tracker
{
    class Program : ModuleBase<ShardedCommandContext>
    {
        private static DiscordShardedClient _client;

        private CommandService _commands;
        private IServiceProvider _services;

        static void Main(string[] args)
        {
            try
            {
                new Config();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            // mysql 연결 시도
            try
            {
                new Sql();
            }
            // 실패 시 종료
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            new Program()
                .RunBotAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async Task CheckNewBest()
        {
            DataTable users = Sql.Get("SELECT user_id FROM targets GROUP BY user_id");

            // 타겟 유저 마다 검사
            foreach (DataRow user in users.Rows)
            {
                try
                {
                    int user_id = (int)user["user_id"];

                    UserBest userBest = new UserBest(user_id);
                    Score newBest = userBest.NewBest();

                    // 새로운 베퍼포가 있을 경우
                    if (newBest!= null)
                    {
                        ScoreEmbed scoreInfoEmbed = new ScoreEmbed(newBest);
                        DataTable channels = Sql.Get("SELECT guild_id, channel_id FROM targets WHERE user_id = '{0}'", user_id);

                        foreach (DataRow channel in channels.Rows)
                        {
                            ulong guild_id = ulong.Parse(channel["guild_id"].ToString());
                            ulong channel_id = ulong.Parse(channel["channel_id"].ToString());

                            // 스코어 정보 전송
                            try
                            {
                                var socketTextChannel = _client.GetGuild(guild_id).GetTextChannel(channel_id);
                                await socketTextChannel.SendMessageAsync(embed: scoreInfoEmbed.Build());
                            }
                            // 실패 시 채널이 삭제된 것으로 판단하고 DB에서 삭제
                            catch (NullReferenceException)
                            {
                                Sql.Execute("DELETE FROM targets WHERE channel_id = '{0}'", channel_id);

                                // 해당 서버에서 삭제한 유저가 타겟에 더 이상 없을 경우 점수 기록도 삭제
                                DataTable findUser = Sql.Get("SELECT user_id FROM targets WHERE user_id = {0}", user_id);

                                if (findUser.Rows.Count == 0)
                                {
                                    Sql.Execute("DELETE FROM pphistories WHERE user_id = {0}", user_id);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Thread.Sleep(1000);
            await Task.Factory.StartNew(() => CheckNewBest());
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            _client.Log += _client_Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, Config.bot_token);
            await _client.StartAsync();
            await CheckNewBest();
            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new ShardedCommandContext(_client, message);

            if (message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            string guild = (message.Channel as SocketGuildChannel).Guild.Id.ToString();

            if (message.HasStringPrefix(">", ref argPos))
            {
                var channel = _client.GetGuild(ulong.Parse(guild)).GetTextChannel(message.Channel.Id);

                if (message.ToString().Contains("\\"))
                {
                    await channel.SendMessageAsync("**이스케이프 문자**를 입력하지 마세요.");
                    return;
                }

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    string err = result.ErrorReason;

                    if (err.Contains("incomplete") || err.Contains("whitespace"))
                    {
                        await channel.SendMessageAsync("**큰 따옴표**가 제대로 닫히지 않았습니다.");
                    }
                    else
                    {
                        Console.WriteLine(err);
                    }
                }
            }
        }
    }
}
