using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace osu_tracker.api
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Score
    {
        public int beatmap_id { get; set; }
        public int maxcombo { get; set; }
        public int count300 { get; set; }
        // ReSharper disable once UnusedMember.Global
        public int countgeki { get; set; }
        public int count100 { get; set; }
        // ReSharper disable once UnusedMember.Global
        public int countkatu { get; set; }
        public int count50 { get; set; }
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

        public static Score UserRecent(object username)
        {
            // ReSharper disable once NotAccessedVariable
            User user;
            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                username = Regex.Replace(username.ToString(), @"[^0-9 a-z A-Z \s \[ \] \- _]+", "").Trim(); // 닉네임이나 id에 포함 불가능한 문자 삭제
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var userJson = new WebClient().DownloadString($"https://osu.ppy.sh/api/get_user?k={Program.api_key}&u={username}"); // api에 유저 정보 요청
                
                var uj = JsonConvert.DeserializeObject<List<User>>(userJson);

                if (uj != null)
                {
                    // ReSharper disable once RedundantAssignment
                    user = uj[0];
                }
            }
            catch
            {
                throw new Exception("플레이어를 찾을 수 없습니다.");
            }

            try
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var userRecentJson = new WebClient().DownloadString($"https://osu.ppy.sh/api/get_user_recent?k={Program.api_key}&u={username}&limit=1"); // api에 유저 정보 요청
                var resp =  JsonConvert.DeserializeObject<List<Score>>(userRecentJson);

                if (resp == null)
                {
                    throw new Exception("최근 24시간 플레이 기록이 없습니다.");
                }
                return resp[0];
            }
            catch
            {
                throw new Exception("최근 24시간 플레이 기록이 없습니다.");
            }
        }
    }
}