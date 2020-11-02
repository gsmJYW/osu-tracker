using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace osu_tracker.api
{
    class UserBest
    {
        int user_id;
        List<Score> bestList;

        public UserBest(int user_id)
        {
            string userBest;
            this.user_id = user_id;

            // api에 베퍼포 정보 요청
            using (WebClient wc = new WebClient())
            {
                userBest = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user_best?k={0}&u={1}&limit=100", Config.api_key, user_id));
            }

            bestList = JsonConvert.DeserializeObject<List<Score>>(userBest);
        }

        // 새로운 베퍼포
        public Score NewBest()
        {
            Score newBest = null;
            double previous_pp_sum, pp_sum = 0.0;

            foreach (Score best in bestList)
            {
                pp_sum += best.pp;
            }

            // 점수 정보에 해당 유저가 있는지 확인
            DataTable findUser = Sql.Get("SELECT user_id FROM pphistories WHERE user_id = {0}", user_id);

            // 없을 경우 새로 삽입
            if (findUser.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO pphistories VALUES ({0}, {1})", user_id, pp_sum);
            }
            // 있을 경우 불러옴
            else
            {
                previous_pp_sum = double.Parse(Sql.Get("SELECT previous_pp_sum FROM pphistories WHERE user_id = {0}", user_id).Rows[0]["previous_pp_sum"].ToString());

                if (Compare(pp_sum, previous_pp_sum) == 1)
                {
                    Sql.Execute("UPDATE pphistories SET previous_pp_sum = {0} WHERE user_id = {1}", pp_sum, user_id);

                    newBest = bestList.OrderByDescending(
                        x => DateTime.ParseExact(x.date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9)
                    ).FirstOrDefault();

                    newBest.index = bestList.IndexOf(newBest);
                }
            }

            return newBest;
        }

        // 주력 모드
        public int MainMods()
        {
            int weight = 0;
            Dictionary<int, double> modList = new Dictionary<int, double>();

            foreach (Score best in bestList)
            {
                int mods = best.enabled_mods;
                bool[] modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장

                // 불필요한 모드 삭제
                for (int i = 1; i <= modBinary.Length; i++)
                {
                    if (modBinary[modBinary.Length - i])
                    {
                        switch (i)
                        {
                            case 1:
                                mods -= 1; // NF
                                break;

                            case 6:
                                mods -= 32; // SD
                                break;

                            case 10:
                                mods -= 512; // NC
                                break;

                            case 13:
                                mods -= 4096; // SO
                                break;

                            case 15:
                                mods -= 16384; // PF
                                break;
                        }
                    }
                }

                double ppWeighted = best.pp * Math.Pow(0.95, weight);

                try
                {
                    modList[mods] += ppWeighted;
                }
                catch
                {
                    modList.Add(mods, ppWeighted);
                }

                weight++;
            }

            int mainMods = modList.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return mainMods;
        }

        // 같으면 0, 왼쪽이 크면 1, 오른쪽이 크면 2
        public int Compare(double a, double b)
        {
            if (Math.Abs(a - b) < 0.0001)
                return 0;
            else if (a > b)
                return 1;
            else
                return 2;
        }
    }
}