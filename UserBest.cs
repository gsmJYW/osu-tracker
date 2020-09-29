using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace osu_tracker
{
    class UserBest
    {
        public int newScoreIndex;
        public double previous_pp_sum, pp_sum = 0.0;
        public string scores;
        public ScoreInfo newScore = null;

        public UserBest(string user_id)
        {
            // api에 베퍼포 정보 요청
            using (WebClient wc = new WebClient())
            {
                scores = new WebClient().DownloadString(string.Format("https://osu.ppy.sh/api/get_user_best?k={0}&u={1}&limit=100", Program.key, user_id));
            }

            List<ScoreInfo> scoreInfoList = JsonConvert.DeserializeObject<List<ScoreInfo>>(scores);

            foreach (ScoreInfo scoreInfo in scoreInfoList)
            {
                pp_sum += scoreInfo.pp;
            }

            // 점수 정보에 해당 유저가 있는지 확인
            DataTable findUser = Sql.Get("SELECT user_id FROM pphistories WHERE user_id = {0}", user_id);

            // 없을 경우 새로 삽입
            if (findUser.Rows.Count == 0)
            {
                Sql.Execute("INSERT INTO pphistories VALUES ({0}, {1})", user_id, pp_sum);
                previous_pp_sum = pp_sum;
            }
            // 있을 경우 불러옴
            else
            {
                previous_pp_sum = double.Parse(Sql.Get("SELECT previous_pp_sum FROM pphistories WHERE user_id = {0}", user_id).Rows[0]["previous_pp_sum"].ToString());

                if (Compare(pp_sum, previous_pp_sum) == 1)
                {
                    newScore = scoreInfoList.OrderByDescending(
                        x => DateTime.ParseExact(x.date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9)
                    ).FirstOrDefault();

                    newScoreIndex = scoreInfoList.IndexOf(newScore);
                }

                Sql.Execute("UPDATE pphistories SET previous_pp_sum = {0} WHERE user_id = {1}", pp_sum, user_id);
            }
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