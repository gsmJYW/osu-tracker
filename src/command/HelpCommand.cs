using Discord.Commands;
using osu_tracker.embed;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class HelpCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("help")]
        public async Task Help(params string[] args)
        {
            string avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var helpEmbed = new HelpEmbed(avatarUrl);

            await Context.Channel.SendMessageAsync(embed: helpEmbed.Build());
        }
    }
}
