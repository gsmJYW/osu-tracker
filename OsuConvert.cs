using Newtonsoft.Json;
using osu_tracker.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace osu_tracker
{
    class OsuConvert
    {
        // 유저명 또는 유저 id로 유저 정보를 불러옴
        public static User ToUser(object username)
        {
            string userJson;

            try
            {
                username = Regex.Replace(username.ToString(), @"[^0-9 a-z A-Z \s \[ \] \- _]+", "").Trim(); // 닉네임에 포함 불가능한 문자 삭제

                // api에 유저 정보 요청
                using (WebClient wc = new WebClient())
                {
                    userJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user?k={0}&u={1}", Config.api_key, username));
                }

                return JsonConvert.DeserializeObject<List<User>>(userJson)[0];
            }
            catch
            {
                throw new Exception("플레이어를 찾을 수 없습니다.");
            }
        }

        // 비트맵 id로 맵 정보를 불러옴
        public static Beatmap ToBeatmap(int beatmap_id, int mods)
        {
            string beatmapJson;
            bool[] modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장

            // 스타레이팅에 영향을 주는 모드들만 계산
            int difficultyChangingMods = 0;

            for (int i = 1; i <= modBinary.Length; i++)
            {
                if (modBinary[modBinary.Length - i])
                {
                    switch (i)
                    {
                        case 2:
                            difficultyChangingMods += 2; // EZ
                            break;

                        case 5:
                            difficultyChangingMods += 16; // HR
                            break;

                        case 7:
                            difficultyChangingMods += 64; // DT
                            break;

                        case 9:
                            difficultyChangingMods += 256; // HT
                            break;
                    }
                }
            }

            // api에 비트맵 정보 요청
            using (WebClient wc = new WebClient())
            {
                beatmapJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_beatmaps?k={0}&b={1}&mods={2}", Config.api_key, beatmap_id, difficultyChangingMods));
            }

            return JsonConvert.DeserializeObject<List<Beatmap>>(beatmapJson)[0];
        }

        // 문자열로 된 시각을 오프셋으로 변환
        public static DateTimeOffset ToOffset(string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9); // 한국 시간 = UTC +9
            long dateTimeMs = (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs);
        }

        // 모드에 해당하는 문자열
        public static string ToModString(int mods)
        {
            bool[] modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            string modString = "";

            for (int i = 1; i <= modBinary.Length; i++)
            {
                if (modBinary[modBinary.Length - i])
                {
                    switch (i)
                    {
                        case 1:
                            modString += "NF";
                            break;

                        case 2:
                            modString += "EZ";
                            break;

                        case 3:
                            modString += "TD";
                            break;

                        case 4:
                            modString += "HD";
                            break;

                        case 5:
                            modString += "HR";
                            break;

                        case 6:
                            modString += "SD";
                            break;

                        case 7:
                            modString += "DT";
                            break;

                        case 9:
                            modString += "HT";
                            break;

                        case 10:
                            modString += "NC";
                            break;

                        case 11:
                            modString += "FL";
                            break;

                        case 13:
                            modString += "SO";
                            break;

                        case 15:
                            modString += "PF";
                            break;
                    }
                }
            }

            if (modString.Contains("NC"))
                modString = modString.Replace("DT", "");

            if (modString.Contains("PF"))
                modString = modString.Replace("SD", "");
            
            return modString;
        }
    }
}