using osu_tracker.api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.IO;
using System.Linq;
using Tarczynski.NtpDateTime;
using System.Threading;
using System.Drawing.Imaging;

namespace osu_tracker.image
{
    class ScoreImage
    {
        static Dictionary<string, MemoryStream> asset = new Dictionary<string, MemoryStream>();

        static Image XH, X, SH, S, A, B, C, D, F,
            nofail, easy, hidden, hardrock, suddendeath, doubletime, halftime, nightcore, flashlight, spunout, perfect,
            hit300, hit100, hit50, hit0,
            avatarGuest;
        
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

            graphics.DrawImage(Image.FromStream(asset[score.rank]), 23, 25, 168.82f, 204.7f);

            Font large = new Font("CENTURY GOTHIC", 19, FontStyle.Regular);
            Font medium = new Font("CENTURY GOTHIC", 14, FontStyle.Regular);

            string songName = beatmap.artist + " - " + beatmap.title;

            if (songName.Length >= 33)
            {
                songName = songName.Substring(0, 33) + "...";
            }

            graphics.DrawString(songName, large, new SolidBrush(Color.White), new Point(206, 17));

            if (beatmap.version.Length >= 22)
            {
                beatmap.version = beatmap.version.Substring(0, 22) + "...";
            }

            graphics.DrawString("[" + beatmap.version + "]", large, new SolidBrush(Color.White), new Point(203, 55));

            List<string> modList = ToModList(score.enabled_mods);
            SizeF diffNameSize = graphics.MeasureString(beatmap.version, large);
            int index = 0;

            foreach (string mod in modList)
            {
                graphics.DrawImage(Image.FromStream(asset[mod]), 220 + diffNameSize.Width + index * 45, 60, 42.5f, 30);
                index++;
            }

            graphics.DrawString(string.Format("{0:0.00}", beatmap.difficultyrating) + "★", large, new SolidBrush(Color.Yellow), new Point(220 + (int)diffNameSize.Width + index * 45, 58));

            Rectangle scoreRec = new Rectangle()
            {
                Width = wallpaper.Width - 17,
                Height = wallpaper.Height,
                X = 0,
                Y = 17
            };

            Rectangle accuracyRec = new Rectangle()
            {
                Width = wallpaper.Width - 17,
                Height = wallpaper.Height,
                X = 0,
                Y = 55
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

            graphics.DrawImage(Image.FromStream(asset["hit300"]), 206, 115, 51.5f, 30);
            graphics.DrawString("x" + score.count300, medium, new SolidBrush(Color.White), new Point(260, 120));

            graphics.DrawImage(Image.FromStream(asset["hit100"]), 208, 155, 48.5f, 28.5f);
            graphics.DrawString("x" + score.count100, medium, new SolidBrush(Color.White), new Point(260, 160));

            graphics.DrawString(string.Format("x{0}/{1}", score.maxcombo, beatmap.max_combo), large, new SolidBrush(Color.White), new Point(206, 191));

            graphics.DrawImage(Image.FromStream(asset["hit50"]), 326, 115, 35.5f, 28.5f);
            graphics.DrawString("x" + score.count50, medium, new SolidBrush(Color.White), new Point(364, 120));

            graphics.DrawImage(Image.FromStream(asset["hit0"]), 328, 155, 30, 30);
            graphics.DrawString("x" + score.countmiss, medium, new SolidBrush(Color.White), new Point(364, 160));

            Rectangle rightBottom = new Rectangle()
            {
                Width = wallpaper.Width - 17,
                Height = wallpaper.Height - 22,
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

                graphics.DrawImage(profile, 426, 121, 100, 100);
                graphics.DrawRectangle(new Pen(new SolidBrush(Color.White), 3), 424.5f, 119.5f, 103, 103);

                graphics.DrawString(user.username, large, new SolidBrush(Color.White), 536, 111);

                SizeF rankSize = graphics.MeasureString("#" + userBest.pp_rank, large);
                SizeF ppSize = graphics.MeasureString(Math.Round(userBest.pp_raw) + "PP", large);

                graphics.DrawString("#" + userBest.pp_rank, large, new SolidBrush(Color.White), 533, 151);
                int gainedRank = userBest.previous_pp_rank - userBest.pp_rank;

                if (gainedRank >= 0)
                {
                    graphics.DrawString("▲", medium, new SolidBrush(Color.LimeGreen), 533 + rankSize.Width, 158);
                }
                else
                {
                    graphics.DrawString("▼", medium, new SolidBrush(Color.IndianRed), 533 + rankSize.Width, 158);
                }

                graphics.DrawString(Math.Abs(gainedRank).ToString(), medium, new SolidBrush(Color.White), 553 + rankSize.Width, 158);

                graphics.DrawString(Math.Round(userBest.pp_raw) + "PP", large, new SolidBrush(Color.White), 535, 191);
                int gainedPP = (int)Math.Round(userBest.pp_raw) - (int)Math.Round(userBest.previous_pp_raw);

                if (gainedPP >= 0)
                {
                    graphics.DrawString("▲", medium, new SolidBrush(Color.LimeGreen), 533 + ppSize.Width, 198);
                }
                else
                {
                    graphics.DrawString("▼", medium, new SolidBrush(Color.IndianRed), 533 + ppSize.Width, 198);
                }

                graphics.DrawString(Math.Abs(gainedPP).ToString(), medium, new SolidBrush(Color.White), 553 + ppSize.Width, 198);

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
