using Discord.Commands;
using osu_tracker.embed;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    // ReSharper disable once UnusedType.Global
    public class HelpCommand : ModuleBase<ShardedCommandContext>
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedParameter.Global
        [Command("help")]
        public async Task Help(params string[] args)
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var helpEmbed = new HelpEmbed(avatarUrl);

            await Context.Channel.SendMessageAsync(embed: helpEmbed.Build());
        }
    }
}
