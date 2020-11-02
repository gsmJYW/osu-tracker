using Discord.Commands;
using osu_tracker.embed;
using System.Data;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class ListCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("list")]
        public async Task List(params string[] args)
        {
            string guild_id = Context.Guild.Id.ToString();
            DataTable users = Sql.Get("SELECT user_id FROM targets WHERE guild_id = '{0}'", guild_id); // 해당 서버에서 추적 중인 플레이어 목록

            var userListEmbed = new UserListEmbed(users);
            await ReplyAsync(embed: userListEmbed.Build());
        }
    }
}
