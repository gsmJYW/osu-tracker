using Discord.Commands;
using osu_tracker.api;
using osu_tracker.embed;
using System;
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
                await ReplyAsync("**유저명** 또는 **유저 id**를 입력하세요.");
                return;
            }

            User user;

            try
            {
                user = OsuConvert.ToUser(username);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            var userEmbed = new UserEmbed(user);
            await ReplyAsync(embed: userEmbed.Build());
        }
    }
}
