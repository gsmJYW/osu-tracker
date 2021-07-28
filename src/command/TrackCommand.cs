using Discord.Commands;
using osu_tracker.api;
using osu_tracker.region;
using System;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class TrackCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("track")]
        public async Task Track(params string[] args)
        {
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var username = string.Join(" ", args);
            var guild_id = Context.Guild.Id;

            if (username.Length == 0)
            {
                var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", Context.User.Id);

                if (userTable.Rows.Count > 0)
                {
                    username = userTable.Rows[0]["user_id"].ToString();
                }
                else
                {
                    await ReplyAsync(lang.Select("no_username"));
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
            var targetSearchTable = Sql.Get("SELECT guild_id FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
            
            // 해당 서버에 없던 유저일 경우 추가
            if (targetSearchTable.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO targets VALUES ({0}, '{1}')", user.user_id, guild_id);
                await ReplyAsync(lang.Select("tracking_started").Replace("guild", Context.Guild.Name).Replace("username", user.username));
            }
            // 이미 해당 서버에 등록된 유저일 경우 삭제
            else 
            {
                Sql.Execute("DELETE FROM targets WHERE user_id = {0} AND guild_id = '{1}'", user.user_id, guild_id);
                await ReplyAsync(lang.Select("tracking_stopped").Replace("guild", Context.Guild.Name).Replace("username", user.username));
            }
            return;
        }
    }
}
