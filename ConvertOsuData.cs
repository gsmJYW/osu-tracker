using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace osu_tracker
{
    class ConvertOsuData
    {
        // 유저명 또는 유저 id로 유저 정보를 불러옴
        public static UserInfo GetUserInfo(object user)
        {
            string userInfoJson;

            try
            {
                user = Regex.Replace(user.ToString(), @"[^0-9 a-z A-Z \s \[ \] \- _]+", "").Trim(); // 닉네임에 포함 불가능한 문자 삭제

                // api에 유저 정보 요청
                using (WebClient wc = new WebClient())
                {
                    userInfoJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user?k={0}&u={1}", Program.key, user));
                }

                return JsonConvert.DeserializeObject<List<UserInfo>>(userInfoJson)[0];
            }
            catch
            {
                throw new Exception("플레이어를 찾을 수 없습니다.");
            }
        }

        // 비트맵 id로 맵 정보를 불러옴
        public static MapInfo getMapInfo(int beatmap_id)
        {
            string mapInfoJson;

            try
            {
                // api에 유저 정보 요청
                using (WebClient wc = new WebClient())
                {
                    mapInfoJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_beatmaps?k={0}&b={1}", Program.key, beatmap_id));
                }
            }
            catch
            {
                throw new Exception("플레이어를 찾을 수 없습니다.");
            }

            return JsonConvert.DeserializeObject<List<MapInfo>>(mapInfoJson)[0];
        }

        // 문자열로 된 시각을 오프셋으로 변환
        public static DateTimeOffset dateToOffset(string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9); // 한국 시간 = UTC +9
            long dateTimeMs = (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs);
        }

        // 문자열로 된 랭크를 이미지 링크로 변환
        public static string rankToImageUrl(string rank)
        {
            switch (rank)
            {
                case "XH":
                    return "https://imgur.com/UzDVLCF.png";

                case "X":
                    return "https://imgur.com/7hMmYYW.png";

                case "SH":
                    return "https://imgur.com/AMT9Iyy.png";

                case "S":
                    return "https://imgur.com/NXxWkeX.png";

                case "A":
                    return "https://imgur.com/QCzekYf.png";

                case "B":
                    return "https://imgur.com/xq2Q4wB.png";

                case "C":
                    return "https://imgur.com/fL372Ks.png";

                default:
                    return "https://imgur.com/Dxlcytu.png";
            }
        }

        // 노트 판정 개수로 정확도 계산
        public static double getAccuracy(int count300, int count100, int count50, int countMiss)
        {
            return (50.0 * count50 + 100.0 * count100 + 300.0 * count300) / (300.0 * (countMiss + count50 + count100 + count300)) * 100.0;
        }

        // 정수로 쓰여져있는 모드 값을 문자열로 변환
        public static string modsToString(int mods)
        {
            bool[] modsBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            string modsStr = "";

            for (int i = 1; i <= modsBinary.Length; i++)
            {
                if (modsBinary[modsBinary.Length - i])
                {
                    switch (i)
                    {
                        case 1:
                            modsStr += "NF";
                            break;

                        case 2:
                            modsStr += "EZ";
                            break;

                        case 3:
                            modsStr += "TD";
                            break;

                        case 4:
                            modsStr += "HD";
                            break;

                        case 5:
                            modsStr += "HR";
                            break;

                        case 6:
                            modsStr += "SD";
                            break;

                        case 7:
                            modsStr += "DT";
                            break;

                        case 9:
                            modsStr += "HT";
                            break;

                        case 10:
                            modsStr += "NC";
                            break;

                        case 11:
                            modsStr += "FL";
                            break;

                        case 13:
                            modsStr += "SO";
                            break;

                        case 15:
                            modsStr += "PF";
                            break;
                    }
                }
            }

            if (modsStr.Contains("NC"))
                modsStr = modsStr.Replace("DT", "");

            if (modsStr.Contains("PF"))
                modsStr = modsStr.Replace("SD", "");

            if (modsStr == "")
                return modsStr;
            else
                return "+" + modsStr;
        }
    }
}
