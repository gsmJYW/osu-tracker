using Discord.Commands;
using osu_tracker.api;
using osu_tracker.image;
using osu_tracker.region;
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
            Language lang = new();
            var guilds = Sql.Get($"SELECT * FROM guilds WHERE id = {Context.Guild.Id}");

            if (guilds.Rows.Count > 0)
            {
                lang = new(guilds.Rows[0]["lang"]);
            }

            var username = string.Join(" ", args);

            if (username.Length == 0)
            {
                var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", Context.User.Id);

                if (userTable.Rows.Count > 0)
                {
                    username = userTable.Rows[0]["user_id"].ToString();
                }
                else
                {
                    await ReplyAsync(lang.Select("no_username"));
                    return;
                }
            }

            Score recent;

            try
            {
                recent = Score.UserRecent(username, lang);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            await using var memoryStream = new MemoryStream();

            var recentImage = new ScoreImage(recent);
            recentImage.DrawImage().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(memoryStream, "recent.png", "");
        }
    }
}
