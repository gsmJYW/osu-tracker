using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord;
using System.Data;

namespace osu_tracker
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help(params string[] args)
        {
            var builder = new EmbedBuilder()
                .WithColor(new Color(0x527788))
                .WithThumbnailUrl(Program.botImageUrl)
                .AddField(">help", "osu!tracker의 사용법을 알려줍니다.")
                .AddField(">info [유저명/유저 id]", "플레이어의 정보를 알려줍니다.")
                .AddField(">track [유저명/유저 id]",
                    "이 채널에서 추적할 플레이어를 추가합니다.\n" +
                    "추가한 플레이어가 상위 pp 기록을 갱신할 때마다\n" +
                    "해당 스코어 정보를 채널에 알려줍니다.\n" +
                    "플레이어는 여러 명 추가할 수 있습니다.\n" +
                    "이미 추적 중인 플레이어를 입력할 경우 추적을 중지합니다.")
                .AddField(">list", "이 채널에서 추적 중인 플레이어들을 알려줍니다.");

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(embed: embed);
        }

        [Command("info")]
        public async Task Info(params string[] args)
        {
            string user = string.Join(" ", args);
            
            if (user.Length == 0)
            {
                await ReplyAsync("**유저명** 또는 **유저 id**를 입력하세요.");
                return;
            }

            UserInfo userInfo;

            try
            {
                userInfo = ConvertOsuData.GetUserInfo(user);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            var userInfoEmbed = new UserInfoEmbed(userInfo);
            await ReplyAsync(embed: userInfoEmbed.embed);
        }

        [Command("track")]
        public async Task Track(params string[] args)
        {
            string user = string.Join(" ", args); // 띄어쓰기 있는 닉네임 처리
            string guild_id = Context.Guild.Id.ToString();
            string channel_id = Context.Channel.Id.ToString();

            if (user.Length == 0)
            {
                await ReplyAsync("**유저명** 또는 **유저 id**를 입력하세요.");
                return;
            }

            UserInfo userInfo;

            try
            {
                userInfo = ConvertOsuData.GetUserInfo(user);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            // 해당 플레이어가 등록된 서버 목록
            DataTable findGuild = Sql.Get("SELECT channel_id FROM targets WHERE user_id = {0} AND guild_id = '{1}'", userInfo.user_id, guild_id);

            // 서버에 없던 유저일 경우 새로 추가
            if (findGuild.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO targets VALUES ({0}, '{1}', '{2}')", userInfo.user_id, guild_id, channel_id);
            }
            // 이미 해당 서버에 등록된 유저일 경우
            else
            {
                // 이미 해당 채널에 등록된 유저일 경우 삭제
                if (channel_id.Equals(findGuild.Rows[0]["channel_id"].ToString()))
                {
                    Sql.Execute("DELETE FROM targets WHERE user_id = {0} AND guild_id = '{1}'", userInfo.user_id, guild_id);

                    // 해당 서버에서 삭제한 유저가 타겟에 더 이상 없을 경우 점수 기록도 삭제
                    DataTable findUser = Sql.Get("SELECT guild_id FROM targets WHERE user_id = {0}", userInfo.user_id);

                    if (findUser.Rows.Count == 0)
                        Sql.Execute("DELETE FROM pphistories WHERE user_id = {0}", userInfo.user_id);

                    await ReplyAsync(string.Format("더 이상 **#{0}**에서 **{1}**님을 추적하지 않습니다.", Context.Channel.Name, userInfo.username));
                    return;
                }
                // 그 외의 경우 업데이트
                else
                {
                    Sql.Execute("UPDATE targets SET channel_id = '{0}' WHERE user_id = {1} AND guild_id = '{2}'", channel_id, userInfo.user_id, guild_id);
                }
            }

            await ReplyAsync(string.Format("**#{0}**에서 **{1}**님을 추적합니다.", Context.Channel.Name, userInfo.username));
        }

        [Command("list")]
        public async Task List(params string[] args)
        {
            string channel_id = Context.Channel.Id.ToString();
            DataTable users = Sql.Get("SELECT user_id FROM targets WHERE channel_id = '{0}'", channel_id); // 해당 채널에서 추적 중인 플레이어 목록

            var userListEmbed = new UserListEmbed(users);
            await ReplyAsync(embed: userListEmbed.embed);
        }
    }
}