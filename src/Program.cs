using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using osu_tracker.api;
using osu_tracker.image;
// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.BoxingAllocation
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace osu_tracker
{
    internal class Program : ModuleBase<ShardedCommandContext>
    {
        private static DiscordShardedClient _client;

        private CommandService _commands;
        private IServiceProvider _services;

        public static string prefix, api_key, bot_token, mysql_server, mysql_port, mysql_database, mysql_uid, mysql_password;

        private static void Main(string[] args)
        {
            // 환경설정 매개변수
            try
            {
                prefix = args[0];
                api_key = args[1];
                bot_token = args[2];

                mysql_server = args[3];
                mysql_port = args[4];
                mysql_database = args[5];
                mysql_uid = args[6];
                mysql_password = args[7];
            }
            catch
            {
                Console.WriteLine("프로그램 실행 시 다음 매개변수가 필요합니다:\n" +
                    "[명령어 접두사] [osu!API v1 키] [디스코드 봇 토큰] [MySQL 서버 주소] [MySQL 포트 번호] [MySQL DB 이름] [MySQL 유저 id] [MySQL 비밀번호]"
                    );
                return;
            }

            try
            {
                Sql.Connect(mysql_server, mysql_port, mysql_database, mysql_uid, mysql_password);
                ScoreImage.LoadAssets();
            }
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

        private static async Task CheckNewBest()
        {
            var userTable = Sql.Get("SELECT user_id FROM targets GROUP BY user_id");

            // 타겟 유저 마다 검사
            foreach (DataRow userRow in userTable.Rows)
            {
                try
                {
                    UserBest userBest;
                    
                    var uRow = userRow["user_id"];
                    var user = User.Search(uRow);

                    // 이전 pp 기록이 있는지 확인
                    // ReSharper disable once HeapView.ObjectAllocation
                    var ppHistorySearchTable = Sql.Get(
                        "SELECT p.user_id FROM pphistories p, targets t " +
                        // ReSharper disable once HeapView.BoxingAllocation
                        "WHERE p.user_id = {0} AND p.user_id = t.user_id", user.user_id
                        );

                    // 이전 pp 기록이 없을 경우 새로 삽입하고 다음 타겟 검사
                    if (ppHistorySearchTable.Rows.Count == 0)
                    {
                        userBest = new UserBest(user.user_id);
                        Sql.Execute("INSERT INTO pphistories VALUES ({0}, {1}, {2}, {3})", user.user_id, userBest.pp_sum, user.pp_raw, user.pp_rank);

                        continue;
                    }

                    var previous_pp_raw = Convert.ToDouble(Sql.Get("SELECT pp_raw FROM pphistories WHERE user_id = {0}", user.user_id).Rows[0]["pp_raw"]);

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

                    var recentImage = new ScoreImage(userBest);
                    var guildTable = Sql.Get("SELECT guild_id FROM targets WHERE user_id = '{0}'", user.user_id);
                    
                    foreach (DataRow guildRow in guildTable.Rows)
                    {
                        var guild_id = ulong.Parse(guildRow["guild_id"].ToString());
                        var guild = _client.GetGuild(guild_id);

                        try
                        {
                            var osuTrackerChannel = await guild.CreateChannelIfNotExist("osu-tracker");

                            await using var memoryStream = new MemoryStream();
                            recentImage.DrawImage().Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await osuTrackerChannel.SendFileAsync(memoryStream, "userBest.png", "");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Sql.Execute("DELETE FROM pphistories WHERE user_id NOT IN (SELECT user_id FROM targets)");
            await Task.Factory.StartNew(() => CheckNewBest());
        }

        [SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += _client_Log;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.LeftGuild += _client_LeftGuild;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, bot_token);
            await _client.StartAsync();
            await _client.SetGameAsync(">help");

            await CheckNewBest();
            await Task.Delay(-1);
        }

        private async Task _client_JoinedGuild(SocketGuild guild)
        {
            // 서버 입장 시 권한 확인
            var permission = guild.GetUser(_client.CurrentUser.Id).GuildPermissions;

            try
            {
                // 관리자 권한이 있는지 확인
                if (!permission.Administrator)
                {
                    await guild.DefaultChannel.SendMessageAsync("https://discord.com/api/oauth2/authorize?client_id=755681499214381136&permissions=8&scope=bot");
                    await guild.DefaultChannel.SendMessageAsync(_client.CurrentUser.Mention + "은(는) **관리자 권한**이 필요합니다.\n권한이 부족해 서버에서 내보냅니다, 다시 초대해주세요.");
                    await guild.LeaveAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Task _client_LeftGuild(SocketGuild guild)
        {
            // 서버에서 추방됐거나 서버가 삭제됐을 경우 해당 길드에서 추적하던 유저 제거
            Sql.Execute("DELETE FROM targets WHERE guild_id = '{0}'", guild.Id);
            return Task.CompletedTask;
        }

        private static Task _client_Log(LogMessage arg)
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

            if (message == null)
            {
                Console.WriteLine("message is null");
                return;
            }
            
            if (message.Author.IsBot)
            {
                return;
            }

            var argPos = 0;

            if (message.Channel is not SocketGuildChannel messageChannel)
            {
                Console.WriteLine("channel is null");
                return;
            }  
            var guild = messageChannel.Guild.Id.ToString();

            if (message.HasStringPrefix(prefix, ref argPos))
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
                    var err = result.ErrorReason;

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
