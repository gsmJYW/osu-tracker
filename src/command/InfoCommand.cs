using Discord.Commands;
using osu_tracker.api;
using osu_tracker.embed;
using System;
using System.Data;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class InfoCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("info")]
        public async Task Info(params string[] args)
        {
            string username = string.Join(" ", args);

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

            var infoEmbed = new InfoEmbed(user);
            await ReplyAsync(embed: infoEmbed.Build());
        }
    }
}
