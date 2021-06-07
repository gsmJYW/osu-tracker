using Discord.Commands;
using osu_tracker.embed;
using System.Data;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace osu_tracker.command
{
    // ReSharper disable once UnusedType.Global
    public class ListCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("list")]
        public async Task List(params string[] args)
        {
            var guild_id = Context.Guild.Id.ToString();
            // ReSharper disable once HeapView.ObjectAllocation
            var users = Sql.Get("SELECT user_id FROM targets WHERE guild_id = '{0}'", guild_id); // 해당 서버에서 추적 중인 플레이어 목록

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var listEmbed = new ListEmbed(users);
            await ReplyAsync(embed: listEmbed.Build());
        }
    }
}
