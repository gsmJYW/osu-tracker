using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public static MapInfo GetMapInfo(int beatmap_id)
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
        public static DateTimeOffset DateToOffset(string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9); // 한국 시간 = UTC +9
            long dateTimeMs = (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs);
        }
    }
}