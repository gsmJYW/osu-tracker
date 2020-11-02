using Discord.Commands;
using osu_tracker.api;
using System;
using System.Data;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class TrackCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("track")]
        public async Task Track(params string[] args)
        {
            string username = string.Join(" ", args);
            string guild_id = Context.Guild.Id.ToString();
            string channel_id = Context.Channel.Id.ToString();

            if (username.Length == 0)
            {
                await ReplyAsync("**유저명** 또는 **유저 id**를 입력하세요.");
                return;
            }

            User user;

            try
            {
                user = OsuConvert.ToUser(username);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            // 해당 플레이어가 등록된 서버 목록
            DataTable findGuild = Sql.Get("SELECT channel_id FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);

            // 서버에 없던 유저일 경우 새로 추가
            if (findGuild.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO targets VALUES ({0}, '{1}', '{2}')", user.user_id, guild_id, channel_id);
            }
            // 이미 해당 서버에 등록된 유저일 경우
            else
            {
                // 이미 해당 채널에 등록된 유저일 경우 삭제
                if (channel_id.Equals(findGuild.Rows[0]["channel_id"].ToString()))
                {
                    Sql.Execute("DELETE FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);

                    // 해당 서버에서 삭제한 유저가 타겟에 더 이상 없을 경우 점수 기록도 삭제
                    DataTable findUser = Sql.Get("SELECT guild_id FROM targets WHERE user_id = {0}", user.user_id);

                    if (findUser.Rows.Count == 0)
                        Sql.Execute("DELETE FROM pphistories WHERE user_id = {0}", user.user_id);

                    await ReplyAsync(string.Format("더 이상 **#{0}**에서 **{1}**님을 추적하지 않습니다.", Context.Channel.Name, user.username));
                    return;
                }
                // 그 외의 경우 업데이트
                else
                {
                    Sql.Execute("UPDATE targets SET channel_id = '{0}' WHERE user_id = {1} AND guild_id = '{2}'", channel_id, user.user_id, guild_id);
                }
            }

            await ReplyAsync(string.Format("**#{0}**에서 **{1}**님을 추적합니다.", Context.Channel.Name, user.username));
        }
    }
}
