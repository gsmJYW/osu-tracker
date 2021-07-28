using Discord.Commands;
using osu_tracker.embed;
using osu_tracker.region;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class HelpCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("help")]
        public async Task Help(params string[] args)
        {
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var helpEmbed = new HelpEmbed(avatarUrl, lang);

            await Context.Channel.SendMessageAsync(embed: helpEmbed.Build());
        }
    }
}
