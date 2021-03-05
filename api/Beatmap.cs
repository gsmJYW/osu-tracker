using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace osu_tracker.api
{
    public class Beatmap
    {
        public string artist { get; set; }
        public int beatmapset_id { get; set; }
        public string title { get; set; }
        public string version { get; set; }
        public int max_combo { get; set; }
        public double difficultyrating { get; set; }
        
        // 비트맵 id로 맵 정보를 불러옴
        public static Beatmap Search(int beatmap_id, int mods)
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

            beatmapJson = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_beatmaps?k={0}&b={1}&mods={2}", Program.api_key, beatmap_id, difficultyChangingMods)); // api에 비트맵 정보 요청
            return JsonConvert.DeserializeObject<List<Beatmap>>(beatmapJson)[0];
        }
    }
}
