using Discord.Commands;
using osu_tracker.api;
using osu_tracker.embed;
using osu_tracker.image;
using System;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable HeapView.BoxingAllocation

namespace osu_tracker.command
{
    public class RecentCommand : ModuleBase<ShardedCommandContext>
    {
        [Command("recent")]
        public async Task Recent(params string[] args)
        {
            var username = string.Join(" ", args);

            if (username.Length == 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation
                var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", Context.User.Id);

                if (userTable.Rows.Count > 0)
                {
                    username = userTable.Rows[0]["user_id"].ToString();
                }
                else
                {
                    await ReplyAsync("**유저명**을 입력하지 않으셨습니다.\n유저명을 생략하고 싶으시면 `>me 유저명`으로 유저 정보를 등록하세요.");
                    return;
                }
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

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            await using var memoryStream = new MemoryStream();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var recentImage = new ScoreImage(recent);
            recentImage.DrawImage().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(memoryStream, "recent.png", "");
        }
    }
}
