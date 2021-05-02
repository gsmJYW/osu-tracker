using Discord.Commands;
using osu_tracker.api;
using osu_tracker.embed;
using osu_tracker.image;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace osu_tracker.command
{
    public class RecentCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("recent")]
        public async Task Recent(params string[] args)
        {
            string username = string.Join(" ", args);

            if (username.Length == 0)
            {
                await ReplyAsync("**유저명** 또는 **유저 id**를 입력하세요.");
                return;
            }

            Score recent;

            try
            {
                recent = Score.UserRecent(username);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            using MemoryStream memoryStream = new MemoryStream();

            ScoreImage recentImage = new ScoreImage(recent);
            recentImage.DrawImage().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(memoryStream, "recent.png", "");
        }
    }
}
