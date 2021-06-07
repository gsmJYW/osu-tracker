using osu_tracker.api;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Net;
using System.IO;
using System.Linq;
using Tarczynski.NtpDateTime;
using System.Drawing.Imaging;
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace osu_tracker.image
{
    internal class ScoreImage
    {
        static Dictionary<string, MemoryStream> asset = new Dictionary<string, MemoryStream>();

        private readonly bool recentImage;
        private readonly bool bestImage;
        private readonly UserBest userBest;
        private Score score;

        public static void LoadAssets()
        {
            SaveImage("XH", "https://imgur.com/UzDVLCF.png");
            SaveImage("X", "https://imgur.com/7hMmYYW.png");
            SaveImage("SH", "https://imgur.com/AMT9Iyy.png");
            SaveImage("S", "https://imgur.com/NXxWkeX.png");
            SaveImage("A", "https://imgur.com/QCzekYf.png");
            SaveImage("B", "https://imgur.com/xq2Q4wB.png");
            SaveImage("C", "https://imgur.com/fL372Ks.png");
            SaveImage("D", "https://imgur.com/Dxlcytu.png");
            SaveImage("F", "https://imgur.com/C0FLjUs.png");

            SaveImage("NF", "https://imgur.com/r5hvACb.png");
            SaveImage("EZ", "https://imgur.com/4fV6ILw.png");
            SaveImage("HD", "https://imgur.com/JF40zcs.png");
            SaveImage("HR", "https://imgur.com/aMDWaJX.png");
            SaveImage("SD", "https://imgur.com/7nAvSh0.png");
            SaveImage("DT", "https://imgur.com/jJwtoMI.png");
            SaveImage("HT", "https://imgur.com/cyHBpFo.png");
            SaveImage("NC", "https://imgur.com/6XXtQp0.png");
            SaveImage("FL", "https://imgur.com/OXPXkI5.png");
            SaveImage("SO", "https://imgur.com/QrDW6vh.png");
            SaveImage("PF", "https://imgur.com/upWagxt.png");

            SaveImage("hit300", "https://imgur.com/mP9f2pW.png");
            SaveImage("hit100", "https://imgur.com/NYYpHxv.png");
            SaveImage("hit50", "https://imgur.com/oCrtIJU.png");
            SaveImage("hit0", "https://imgur.com/rOcF4GM.png");

            SaveImage("star", "https://imgur.com/PxYvnx7.png");
            SaveImage("avatar-guest", "https://osu.ppy.sh/images/layout/avatar-guest@2x.png");
        }

        public ScoreImage(Score score)
        {
            this.score = score;
            recentImage = true;
        }

        public ScoreImage(UserBest userBest)
        {
            this.userBest = userBest;
            bestImage = true;
        }

        [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
        public Image DrawImage()
        {
            if (bestImage)
            {
                score = userBest.newBest;
            }

            // ReSharper disable once HeapView.BoxingAllocation
            var user = User.Search(score.user_id);
            var beatmap = Beatmap.Search(score.beatmap_id, score.enabled_mods);

            var wallpaper = ParseImage($"https://assets.ppy.sh/beatmaps/{beatmap.beatmapset_id}/covers/cover.jpg");
            StackBlur.StackBlur.Process((Bitmap)wallpaper, 8);

            var graphics = Graphics.FromImage(wallpaper);
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(144, 0, 0, 0)), 0, 0, wallpaper.Width, wallpaper.Height);

            graphics.DrawImage(Image.FromStream(asset[score.rank]), 23, 28, 168.82f, 204.7f);

            var large = new Font("Verdana", 19, FontStyle.Regular);
            var medium = new Font("Verdana", 14, FontStyle.Regular);
            var arrow = new Font("Arial", 14, FontStyle.Regular);

            var songName = $"{beatmap.artist} - {beatmap.title}";
            var songNameSize = graphics.MeasureString(songName, large);

            if (songNameSize.Width > 480)
            {
                songName = $"{songName[..27]}...";
            }

            graphics.DrawString(songName, large, new SolidBrush(Color.White), new Point(208, 25));

            var diffName = beatmap.version;
            // ReSharper disable once HeapView.BoxingAllocation
            var starRating = $"{beatmap.difficultyrating:0.00}";

            var diffNameSize = graphics.MeasureString(diffName, large);
            var starRatingSize = graphics.MeasureString(starRating, large);

            var modList = score.enabled_mods.ToModList();

            var enumerable = modList.ToList();

            if (diffNameSize.Width + starRatingSize.Width + enumerable.Count * 45 > 480)
            {
                diffName = $"{diffName[..17]}...";
            }

            graphics.DrawString($"[{diffName}]", large, new SolidBrush(Color.White), new Point(207, 58));
            diffNameSize = graphics.MeasureString(diffName, large);

            var index = 0;

            // ReSharper disable once HeapView.ObjectAllocation.Possible
            foreach (var mod in enumerable)
            {
                graphics.DrawImage(Image.FromStream(asset[mod]), 236 + diffNameSize.Width + index * 45, 63, 42.5f, 30);
                index++;
            }
            graphics.DrawString(starRating, large, new SolidBrush(Color.Yellow), new Point(238 + (int)diffNameSize.Width + index * 45, 59));
            graphics.DrawImage(Image.FromStream(asset["star"]), 238 + diffNameSize.Width + index * 45 + starRatingSize.Width, 63, 27, 27);

            // ReSharper disable once RedundantEmptyObjectCreationArgumentList
            var scoreRec = new Rectangle()
            {
                Width = wallpaper.Width - 25,
                Height = wallpaper.Height,
                X = 0,
                Y = 25
            };

            // ReSharper disable once RedundantEmptyObjectCreationArgumentList
            var accuracyRec = new Rectangle()
            {
                Width = wallpaper.Width - 25,
                Height = wallpaper.Height,
                X = 0,
                Y = 58
            };
            
            graphics.DrawString(score.score.ToString(), large, new SolidBrush(Color.White), scoreRec, new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            });

            // ReSharper disable once HeapView.BoxingAllocation
            graphics.DrawString($"{score.Accuracy():0.00}%", large, new SolidBrush(Color.White), accuracyRec, new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            });

            graphics.DrawImage(Image.FromStream(asset["hit300"]), 206, 110, 51.5f, 30);
            // ReSharper disable once HeapView.BoxingAllocation
            graphics.DrawString($"x{score.count300}", medium, new SolidBrush(Color.White), new Point(260, 115));

            graphics.DrawImage(Image.FromStream(asset["hit100"]), 208, 152, 48.5f, 28.5f);
            // ReSharper disable once HeapView.BoxingAllocation
            graphics.DrawString($"x{score.count100}", medium, new SolidBrush(Color.White), new Point(260, 157));
            
            graphics.DrawString($"x{score.maxcombo}/{beatmap.max_combo}", large, new SolidBrush(Color.White), new Point(208, 191));

            graphics.DrawImage(Image.FromStream(asset["hit50"]), 326, 110, 35.5f, 28.5f);
            graphics.DrawString($"x{score.count50}", medium, new SolidBrush(Color.White), new Point(364, 115));

            graphics.DrawImage(Image.FromStream(asset["hit0"]), 328, 152, 30, 30);
            graphics.DrawString($"x{score.countmiss}", medium, new SolidBrush(Color.White), new Point(364, 157));

            // ReSharper disable once RedundantEmptyObjectCreationArgumentList
            var rightBottom = new Rectangle()
            {
                Width = wallpaper.Width - 25,
                Height = wallpaper.Height - 27,
                X = 0,
                Y = 0
            };

            if (bestImage)
            {
                Image profile;

                try
                {
                    profile = ParseImage($"https://a.ppy.sh/{user.user_id}");
                }
                catch
                {
                    profile = Image.FromStream(asset["avatar-guest"]);
                }

                graphics.DrawImage(profile, 423, 116, 100, 100);
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 3), 421.5f, 114.5f, 103, 103);

                graphics.DrawString(user.username, large, new SolidBrush(Color.White), 536, 105);

                var rankSize = graphics.MeasureString($"#{userBest.pp_rank}", large);
                var ppSize = graphics.MeasureString($"{Math.Round(userBest.pp_raw)}PP", large);

                graphics.DrawString($"#{userBest.pp_rank}", large, new SolidBrush(Color.White), 533, 148);
                var gainedRank = userBest.previous_pp_rank - userBest.pp_rank;

                if (gainedRank >= 0)
                {
                    graphics.DrawString("▲", arrow, new SolidBrush(Color.LimeGreen), 538 + rankSize.Width, 156);
                }
                else
                {
                    graphics.DrawString("▼", arrow, new SolidBrush(Color.IndianRed), 538 + rankSize.Width, 156);
                }

                graphics.DrawString(Math.Abs(gainedRank).ToString(), medium, new SolidBrush(Color.White), 556 + rankSize.Width, 155);

                graphics.DrawString($"{Math.Round(userBest.pp_raw)}PP", large, new SolidBrush(Color.White), 535, 191);
                var gainedPP = (int)Math.Round(userBest.pp_raw) - (int)Math.Round(userBest.previous_pp_raw);

                if (gainedPP >= 0)
                {
                    graphics.DrawString("▲", arrow, new SolidBrush(Color.LimeGreen), 538 + ppSize.Width, 197);
                }
                else
                {
                    graphics.DrawString("▼", arrow, new SolidBrush(Color.IndianRed), 538 + ppSize.Width, 197);
                }

                graphics.DrawString(Math.Abs(gainedPP).ToString(), medium, new SolidBrush(Color.White), 556 + ppSize.Width, 196);

                graphics.DrawString($"{Math.Round(score.pp)}PP", large, new SolidBrush(Color.White), rightBottom, new StringFormat()
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                });
            }
            else if (recentImage)
            {
                var date = DateTime.ParseExact(score.date, "yyyy-MM-dd H:m:s", null);
                var now = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.FromNtp());

                var timeSpan = now.Subtract(date);

                // ReSharper disable once RedundantEmptyObjectCreationArgumentList
                graphics.DrawString($"{timeSpan.Hours}h {timeSpan.Minutes}m ago", large, new SolidBrush(Color.White), rightBottom, new StringFormat()
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                });
            }

            return wallpaper;
        }

        private static Image ParseImage(string url)
        {
            var bytes = new WebClient().DownloadData(url);
            using var memoryStream = new MemoryStream(bytes);
            return Image.FromStream(memoryStream);
        }

        private static void SaveImage(string key, string url)
        {
            var image = ParseImage(url);
            asset.Add(key, new MemoryStream());
            image.Save(asset[key], ImageFormat.Png);
        }
    }
}
