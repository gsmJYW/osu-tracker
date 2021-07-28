using Discord.Commands;
using osu_tracker.api;
using osu_tracker.region;
using System;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class MeCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("me")]
        public async Task Recent(params string[] args)
        {
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var discordUser = Context.User;
            var username = string.Join(" ", args);

            var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", discordUser.Id);

            if (username.Length == 0)
            {
                if (userTable.Rows.Count > 0)
                {
                    Sql.Execute("DELETE FROM users WHERE discord_id = '{0}'", discordUser.Id);
                    await ReplyAsync(lang.Select("user_info_deleted").Replace("username", discordUser.Username));
                }
                else
                {
                    await ReplyAsync(lang.Select("no_user_info"));
                }

                return;
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

            if (userTable.Rows.Count > 0)
            {
                Sql.Execute("UPDATE users SET user_id = '{0}' WHERE discord_id = '{1}'", user.user_id, discordUser.Id);
            }
            else
            {
                Sql.Execute("INSERT INTO users VALUES ('{0}', '{1}')", discordUser.Id, user.user_id);
            }

            await ReplyAsync(lang.Select("user_info_registered").Replace("username", discordUser.Username));
        }
    }
}
