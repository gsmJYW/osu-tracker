using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
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

        public static string api_key, bot_token, mysql_server, mysql_port, mysql_database, mysql_uid, mysql_password, mysql_charset;

        static void Main(string[] args)
        {
            try
            {
                api_key = args[0];
                bot_token = args[1];

                mysql_server = args[2];
                mysql_port = args[3];
                mysql_database = args[4];
                mysql_uid = args[5];
                mysql_password = args[6];
            }
            catch
            {
                Console.WriteLine("프로그램 실행 시 다음 매개변수가 필요합니다:\n" +
                    "[osu!API v1 키] [디스코드 봇 토큰] [MySQL 서버] [MySQL 포트] [MySQL DB] [MySQL 유저] [MySQL 비밀번호] <MySQL 인코딩>"
                    );
                return;
            }

            try
            {
                mysql_charset = args[7];
            }
            catch
            {
                mysql_charset = "UTF8";
            }

            // mysql 연결 시도
            try
            {
                new Sql(mysql_server, mysql_port, mysql_database, mysql_uid, mysql_password, mysql_charset);
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
            DataTable userTable = Sql.Get("SELECT user_id FROM targets GROUP BY user_id");

            // 타겟 유저 마다 검사
            foreach (DataRow userRow in userTable.Rows)
            {
                try
                {
                    UserBest userBest;
                    User user = User.Search((int)userRow["user_id"]);

                    // 이전 pp 기록이 있는지 확인
                    DataTable ppHistorySearchTable = Sql.Get(
                        "SELECT p.user_id FROM pphistories p, targets t " +
                        "WHERE p.user_id = {0} AND p.user_id = t.user_id", user.user_id
                        );

                    // 이전 pp 기록이 없을 경우 새로 삽입하고 다음 타겟 검사
                    if (ppHistorySearchTable.Rows.Count == 0)
                    {
                        userBest = new UserBest(user.user_id);
                        Sql.Execute("INSERT INTO pphistories VALUES ({0}, {1}, {2}, {3})", user.user_id, userBest.pp_sum, user.pp_raw, user.pp_rank);

                        continue;
                    }

                    double previous_pp_raw = Convert.ToDouble(Sql.Get("SELECT pp_raw FROM pphistories WHERE user_id = {0}", user.user_id).Rows[0]["pp_raw"]);

                    // pp 변화가 없을 경우 다음 타겟 검사
                    if (previous_pp_raw.IsCloseTo(user.pp_raw))
                    {
                        continue;
                    }

                    // pp 변화가 있을 경우 새로운 베퍼포가 있는지 확인
                    userBest = new UserBest(user.user_id);
                    userBest.GetNewBest();

                    // 새로운 베퍼포가 없을 경우 다음 타겟 검사
                    if (userBest.newBest == null)
                    {
                        continue;
                    }

                    ScoreEmbed scoreEmbed = new ScoreEmbed(userBest);
                    DataTable guildTable = Sql.Get("SELECT guild_id FROM targets WHERE user_id = '{0}'", user.user_id);

                    foreach (DataRow guildRow in guildTable.Rows)
                    {
                        ulong guild_id = ulong.Parse(guildRow["guild_id"].ToString());
                        SocketGuild guild = _client.GetGuild(guild_id);

                        // 'osu!tracker' 채널을 탐색해서 있으면 거기로 전송, 없으면 채널 생성 후 전송
                        try
                        {
                            SocketTextChannel channelToSend = null;
                            bool isThereOsuTrackerChannel = false;

                            IReadOnlyCollection<SocketTextChannel> channelList = guild.TextChannels;

                            foreach (SocketTextChannel channel in channelList)
                            {
                                if (channel.Name.ToLower() == "osu-tracker")
                                {
                                    isThereOsuTrackerChannel = true;
                                    channelToSend = channel;
                                    break;
                                }
                            }

                            if (!isThereOsuTrackerChannel)
                            {
                                RestTextChannel CreatedChannel = (await guild.CreateTextChannelAsync("osu-tracker"));
                                channelToSend = guild.GetTextChannel(CreatedChannel.Id);
                            }

                            await channelToSend.SendMessageAsync(embed: scoreEmbed.Build());
                        }
                        // 실패 시 채널이 삭제된 것으로 판단하고 DB에서 삭제
                        catch
                        {
                            Sql.Execute("DELETE FROM targets WHERE guild_id = '{0}'", guild_id);

                            // 해당 서버에서 삭제한 유저가 타겟에 더 이상 없을 경우 점수 기록도 삭제
                            DataTable targetSearchTable = Sql.Get("SELECT user_id FROM targets WHERE user_id = {0}", user.user_id);

                            if (targetSearchTable.Rows.Count == 0)
                            {
                                Sql.Execute("DELETE FROM pphistories WHERE user_id = {0}", user.user_id);
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
            await _client.LoginAsync(TokenType.Bot, bot_token);
            await _client.StartAsync();
            
            Thread.Sleep(5000);

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
