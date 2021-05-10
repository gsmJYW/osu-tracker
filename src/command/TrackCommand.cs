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
            ulong guild_id = Context.Guild.Id;

            if (username.Length == 0)
            {
                DataTable userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", Context.User.Id);

                if (userTable.Rows.Count > 0)
                {
                    username = userTable.Rows[0]["user_id"].ToString();
                }
                else
                {
                    await ReplyAsync("**유저명**을 입력하지 않으셨습니다.\n유저명을 생략하고 싶으시면 `>me 유저명`으로 유저 정보를 등록하세요.");
                    return;
                }
            }

            User user;

            try
            {
                user = User.Search(username);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            // 해당 서버에서 추적 중인 플레이어인지 확인
            DataTable targetSearchTable = Sql.Get("SELECT guild_id FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
            
            // 해당 서버에 없던 유저일 경우 추가
            if (targetSearchTable.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO targets VALUES ({0}, '{1}')", user.user_id, guild_id);
                await ReplyAsync(string.Format("**{0}**에서 **{1}**님을 추적합니다.", Context.Guild.Name, user.username));
            }
            // 이미 해당 서버에 등록된 유저일 경우 삭제
            else 
            {
                Sql.Execute("DELETE FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
                await ReplyAsync(string.Format("더 이상 **{0}**에서 **{1}**님을 추적하지 않습니다.", Context.Guild.Name, user.username));
            }

            return;
        }
    }
}
