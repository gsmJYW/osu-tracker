using System;
using System.Threading.Tasks;
using Discord.Commands;
using osu_tracker.api;
using osu_tracker.embed;
using osu_tracker.region;

namespace osu_tracker.command
{
    public class InfoCommand : ModuleBase<ShardedCommandContext>
    {   
        [Command("info")]
        public async Task Info(params string[] args)
        {
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var username = string.Join(" ", args);

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

            var infoEmbed = new InfoEmbed(user, lang);
            await ReplyAsync(embed: infoEmbed.Build());
        }
    }
}
