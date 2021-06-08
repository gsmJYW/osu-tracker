using Discord.Commands;
using osu_tracker.api;
using System;
using System.Data;
using System.Threading.Tasks;
// ReSharper disable HeapView.BoxingAllocation

namespace osu_tracker.command
{
    // ReSharper disable once UnusedType.Global
    public class TrackCommand : ModuleBase<ShardedCommandContext>
    {
        // ReSharper disable once UnusedMember.Global
        [Command("track")]
        public async Task Track(params string[] args)
        {
            var username = string.Join(" ", args);
            var guild_id = Context.Guild.Id;

            if (username.Length == 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation
                var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", Context.User.Id);

                if (userTable.Rows.Count > 0)
                {
                    username = userTable.Rows[0]["user_id"].ToString();
                }
                else
                {
                    await ReplyAsync($"**유저명**을 입력하지 않으셨습니다.\n유저명을 생략하고 싶으시면 `{Program.prefix}me 유저명`으로 유저 정보를 등록하세요.");
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
            // ReSharper disable once HeapView.ObjectAllocation
            var targetSearchTable = Sql.Get("SELECT guild_id FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
            
            // 해당 서버에 없던 유저일 경우 추가
            if (targetSearchTable.Rows.Count == 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation
                Sql.Execute("INSERT INTO targets VALUES ({0}, '{1}')", user.user_id, guild_id);
                await ReplyAsync($"**{Context.Guild.Name}**에서 **{user.username}**님을 추적합니다.");
            }
            // 이미 해당 서버에 등록된 유저일 경우 삭제
            else 
            {
                // ReSharper disable once HeapView.ObjectAllocation
                Sql.Execute("DELETE FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
                await ReplyAsync($"더 이상 **{Context.Guild.Name}**에서 **{user.username}**님을 추적하지 않습니다.");
            }
            // ReSharper disable once RedundantJumpStatement
            return;
        }
    }
}
