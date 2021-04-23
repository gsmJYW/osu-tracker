using osu_tracker.api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.IO;
using System.Linq;
using Tarczynski.NtpDateTime;
using System.Drawing.Imaging;

namespace osu_tracker.image
{
    class ScoreImage
    {
        static Dictionary<string, MemoryStream> asset = new Dictionary<string, MemoryStream>();
        
        bool recentImage, bestImage;
        UserBest userBest;
        Score score;

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

            SaveImage("nofail", "https://imgur.com/r5hvACb.png");
            SaveImage("easy", "https://imgur.com/4fV6ILw.png");
            SaveImage("hidden", "https://imgur.com/JF40zcs.png");
            SaveImage("hardrock", "https://imgur.com/aMDWaJX.png");
            SaveImage("suddendeath", "https://imgur.com/7nAvSh0.png");
            SaveImage("doubletime", "https://imgur.com/jJwtoMI.png");
            SaveImage("halftime", "https://imgur.com/cyHBpFo.png");
            SaveImage("nightcore", "https://imgur.com/6XXtQp0.png");
            SaveImage("flashlight", "https://imgur.com/OXPXkI5.png");
            SaveImage("spunout", "https://imgur.com/QrDW6vh.png");
            SaveImage("perfect", "https://imgur.com/upWagxt.png");

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

        public Image DrawImage()
        {
            if (bestImage)
            {
                score = userBest.newBest;
            }

            User user = User.Search(score.user_id);
            Beatmap beatmap = Beatmap.Search(score.beatmap_id, score.enabled_mods);

            Image wallpaper = ParseImage("https://assets.ppy.sh/beatmaps/" + beatmap.beatmapset_id + "/covers/cover.jpg");
            StackBlur.StackBlur.Process((Bitmap)wallpaper, 8);

            Graphics graphics = Graphics.FromImage(wallpaper);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(144, 0, 0, 0)), 0, 0, wallpaper.Width, wallpaper.Height);

            graphics.DrawImage(Image.FromStream(asset[score.rank]), 23, 28, 168.82f, 204.7f);

            Font large = new Font("Verdana", 19, FontStyle.Regular);
            Font medium = new Font("Verdana", 14, FontStyle.Regular);
            Font arrow = new Font("Arial", 14, FontStyle.Regular);

            string songName = beatmap.artist + " - " + beatmap.title;

            if (songName.Length >= 33)
            {
                songName = songName.Substring(0, 33) + "...";
            }

            graphics.DrawString(songName, large, new SolidBrush(Color.White), new Point(208, 25));

            if (beatmap.version.Length >= 22)
            {
                beatmap.version = beatmap.version.Substring(0, 22) + "...";
            }

            graphics.DrawString("[" + beatmap.version + "]", large, new SolidBrush(Color.White), new Point(207, 58));

            List<string> modList = ToModList(score.enabled_mods);
            SizeF diffNameSize = graphics.MeasureString(beatmap.version, large);
            int index = 0;

            foreach (string mod in modList)
            {
                graphics.DrawImage(Image.FromStream(asset[mod]), 236 + diffNameSize.Width + index * 45, 63, 42.5f, 30);
                index++;
            }

            SizeF starRatingSize = graphics.MeasureString(string.Format("{0:0.00}", beatmap.difficultyrating), large);
            graphics.DrawString(string.Format("{0:0.00}", beatmap.difficultyrating), large, new SolidBrush(Color.Yellow), new Point(238 + (int)diffNameSize.Width + index * 45, 59));
            graphics.DrawImage(Image.FromStream(asset["star"]), 238 + diffNameSize.Width + index * 45 + starRatingSize.Width, 63, 27, 27);

            Rectangle scoreRec = new Rectangle()
            {
                Width = wallpaper.Width - 25,
                Height = wallpaper.Height,
                X = 0,
                Y = 25
            };

            Rectangle accuracyRec = new Rectangle()
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

            graphics.DrawString(string.Format("{0:0.00}%", score.Accuracy()), large, new SolidBrush(Color.White), accuracyRec, new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            });

            graphics.DrawImage(Image.FromStream(asset["hit300"]), 206, 110, 51.5f, 30);
            graphics.DrawString("x" + score.count300, medium, new SolidBrush(Color.White), new Point(260, 115));

            graphics.DrawImage(Image.FromStream(asset["hit100"]), 208, 152, 48.5f, 28.5f);
            graphics.DrawString("x" + score.count100, medium, new SolidBrush(Color.White), new Point(260, 157));

            graphics.DrawString(string.Format("x{0}/{1}", score.maxcombo, beatmap.max_combo), large, new SolidBrush(Color.White), new Point(208, 191));

            graphics.DrawImage(Image.FromStream(asset["hit50"]), 326, 110, 35.5f, 28.5f);
            graphics.DrawString("x" + score.count50, medium, new SolidBrush(Color.White), new Point(364, 115));

            graphics.DrawImage(Image.FromStream(asset["hit0"]), 328, 150, 30, 30);
            graphics.DrawString("x" + score.countmiss, medium, new SolidBrush(Color.White), new Point(364, 157));

            Rectangle rightBottom = new Rectangle()
            {
                Width = wallpaper.Width - 25,
                Height = wallpaper.Height - 28,
                X = 0,
                Y = 0
            };

            if (bestImage)
            {
                Image profile;

                try
                {
                    profile = ParseImage("https://a.ppy.sh/" + user.user_id);
                }
                catch
                {
                    profile = Image.FromStream(asset["avatar-guest"]);
                }

                graphics.DrawImage(profile, 423, 115, 100, 100);
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 3), 421.5f, 113.5f, 103, 103);

                graphics.DrawString(user.username, large, new SolidBrush(Color.White), 536, 104);

                SizeF rankSize = graphics.MeasureString("#" + userBest.pp_rank, large);
                SizeF ppSize = graphics.MeasureString(Math.Round(userBest.pp_raw) + "PP", large);

                graphics.DrawString("#" + userBest.pp_rank, large, new SolidBrush(Color.White), 533, 146);
                int gainedRank = userBest.previous_pp_rank - userBest.pp_rank;

                if (gainedRank >= 0)
                {
                    graphics.DrawString("▲", arrow, new SolidBrush(Color.LimeGreen), 538 + rankSize.Width, 153);
                }
                else
                {
                    graphics.DrawString("▼", arrow, new SolidBrush(Color.IndianRed), 538 + rankSize.Width, 153);
                }

                graphics.DrawString(Math.Abs(gainedRank).ToString(), medium, new SolidBrush(Color.White), 556 + rankSize.Width, 152);

                graphics.DrawString(Math.Round(userBest.pp_raw) + "PP", large, new SolidBrush(Color.White), 535, 189);
                int gainedPP = (int)Math.Round(userBest.pp_raw) - (int)Math.Round(userBest.previous_pp_raw);

                if (gainedPP >= 0)
                {
                    graphics.DrawString("▲", arrow, new SolidBrush(Color.LimeGreen), 538 + ppSize.Width, 195);
                }
                else
                {
                    graphics.DrawString("▼", arrow, new SolidBrush(Color.IndianRed), 538 + ppSize.Width, 195);
                }

                graphics.DrawString(Math.Abs(gainedPP).ToString(), medium, new SolidBrush(Color.White), 556 + ppSize.Width, 194);

                graphics.DrawString(Math.Round(score.pp) + "PP", large, new SolidBrush(Color.White), rightBottom, new StringFormat()
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                });
            }
            else if (recentImage)
            {
                DateTime date = DateTime.ParseExact(score.date, "yyyy-MM-dd H:m:s", null);
                DateTime now = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now.FromNtp());

                TimeSpan timeSpan = now.Subtract(date);

                graphics.DrawString(timeSpan.Hours + "h " + timeSpan.Minutes + "m ago", large, new SolidBrush(Color.White), rightBottom, new StringFormat()
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Far
                });
            }

            return wallpaper;
        }

        public static Image ParseImage(string url)
        {
            byte[] bytes = new WebClient().DownloadData(url);
            using MemoryStream memoryStream = new MemoryStream(bytes);
            return Image.FromStream(memoryStream);
        }

        public static void SaveImage(string key, string url)
        {
            Image image = ParseImage(url);
            asset.Add(key, new MemoryStream());
            image.Save(asset[key], ImageFormat.Png);
        }

        // 모드에 해당하는 아이콘 링크
        public static List<string> ToModList(int mods)
        {
            bool[] modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            List<string> modList = new List<string>();

            for (int i = 1; i <= modBinary.Length; i++)
            {
                if (modBinary[modBinary.Length - i])
                {
                    switch (i)
                    {
                        case 1:
                            modList.Add("nofail");
                            break;

                        case 2:
                            modList.Add("easy");
                            break;

                        /*
                        case 3:
                            modList.Add("TD");
                            break;
                        */

                        case 4:
                            modList.Add("hidden");
                            break;

                        case 5:
                            modList.Add("hardrock");
                            break;

                        case 6:
                            modList.Add("suddendeath");
                            break;

                        case 7:
                            modList.Add("doubletime");
                            break;

                        case 9:
                            modList.Add("halftime");
                            break;

                        case 10:
                            modList.Add("nightcore");
                            break;

                        case 11:
                            modList.Add("flashlight");
                            break;

                        case 13:
                            modList.Add("spunout");
                            break;

                        case 15:
                            modList.Add("perfect");
                            break;
                    }
                }
            }

            if (modList.Contains("NC"))
            {
                modList.Remove("DT");
            }

            if (modList.Contains("PF"))
            {
                modList.Remove("SD");
            }

            return modList;
        }
    }
}
