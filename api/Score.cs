using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace osu_tracker.api
{
    class Score
    {
        public int beatmap_id { get; set; }
        public int maxcombo { get; set; }
        public int count50 { get; set; }
        public int count100 { get; set; }
        public int count300 { get; set; }
        public int countmiss { get; set; }
        public int enabled_mods { get; set; }
        public int user_id { get; set; }
        public int index { get; set; }
        public ulong score { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }

        // 노트 판정 개수로 정확도 계산
        public double Accuracy()
        {
            return (50.0 * count50 + 100.0 * count100 + 300.0 * count300) / (300.0 * (countmiss + count50 + count100 + count300)) * 100.0;
        }

        // 랭크에 해당하는 이미지 url
        public string RankImageUrl()
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

                case "D":
                    return "https://imgur.com/Dxlcytu.png";

                // F (페일)
                default:
                    return "https://imgur.com/C0FLjUs.png";
            }
        }

        public static Score UserRecent(object username)
        {
            User user;

            try
            {
                username = Regex.Replace(username.ToString(), @"[^0-9 a-z A-Z \s \[ \] \- _]+", "").Trim(); // 닉네임이나 id에 포함 불가능한 문자 삭제
                string userJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user?k={0}&u={1}", Program.api_key, username)); // api에 유저 정보 요청

                user = JsonConvert.DeserializeObject<List<User>>(userJson)[0];
            }
            catch
            {
                throw new Exception("플레이어를 찾을 수 없습니다.");
            }

            try
            {
                string userRecentJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user_recent?k={0}&u={1}&limit=1", Program.api_key, username)); // api에 유저 정보 요청
                return JsonConvert.DeserializeObject<List<Score>>(userRecentJson)[0];
            }
            catch
            {
                throw new Exception("최근 24시간 플레이 기록이 없습니다.");
            }
        }
    }
}