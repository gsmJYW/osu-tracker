using Discord.Commands;
using osu_tracker.region;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class LangCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("lang")]
        public async Task Lang(params string[] args)
        {
            var guild_id = Context.Guild.Id;
            var user = Context.Guild.GetUser(Context.User.Id);

            if (args.Length == 0)
            {
                var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = '{guild_id}'");

                if (guilds.Rows.Count == 0)
                {
                    await ReplyAsync("language: `en`");
                }
                else
                {
                    await ReplyAsync($"language: `{guilds.Rows[0]["lang"]}`");
                }

                return;
            }

            if (!user.GuildPermissions.Administrator)
            {
                await ReplyAsync("**Only administrators** can change the language.");
                return;
            }

            Language lang;

            try
            {
                if (args[0] == "en")
                {
                    Sql.Execute($"DELETE FROM guilds WHERE id = '{guild_id}'");
                }
                else
                {
                    lang = new(args[0]);
                    Sql.Execute($"REPLACE INTO guilds VALUES ('{guild_id}', '{args[0]}')");
                }

                await ReplyAsync($"Changed the language: `{args[0]}`");
            }
            catch
            {
                await ReplyAsync($"No such language: `{args[0]}`");
            }
        }
    }
}
