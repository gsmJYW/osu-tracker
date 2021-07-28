using Discord.Commands;
using osu_tracker.embed;
using osu_tracker.region;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class ListCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("list")]
        public async Task List(params string[] args)
        {
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var guild_id = Context.Guild.Id.ToString();
            var users = Sql.Get("SELECT user_id FROM targets WHERE guild_id = '{0}'", guild_id); // 해당 서버에서 추적 중인 플레이어 목록

            var listEmbed = new ListEmbed(users, lang);
            await ReplyAsync(embed: listEmbed.Build());
        }
    }
}
