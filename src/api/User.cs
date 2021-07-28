using Newtonsoft.Json;
using osu_tracker.region;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace osu_tracker.api
{
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public int playcount { get; set; }
        public int pp_rank { get; set; }
        public int total_seconds_played { get; set; }
        public int pp_country_rank { get; set; }
        public double pp_raw { get; set; }
        public double level { get; set; }
        public double accuracy { get; set; }
        public string country { get; set; }
        public string join_date { get; set; }

        // 유저명 또는 유저 id로 유저 정보를 불러옴
        public static User Search(object username, Language lang = null)
        {
            try
            {
                username = Regex.Replace(username.ToString() ?? throw new InvalidOperationException(), @"[^0-9 a-z A-Z \s \[ \] \- _]+", "").Trim(); // 닉네임이나 id에 포함 불가능한 문자 삭제
                var userJson = new WebClient().DownloadString($"https://osu.ppy.sh/api/get_user?k={Program.api_key}&u={username}"); // api에 유저 정보 요청

                var resp = JsonConvert.DeserializeObject<List<User>>(userJson);

                if (resp == null)
                {
                    throw new Exception();
                }
                return resp[0];
            }
            catch
            {
                throw new Exception(lang.Select("player_not_found"));
            }
        }
    }
}
