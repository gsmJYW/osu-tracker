using Discord;
using System.Collections.Generic;
using System.Data;
using osu_tracker.api;
using System.Threading.Tasks;

namespace osu_tracker.embed
{
    class ListEmbed : EmbedBuilder
    {
        public ListEmbed(DataTable userTable)
        {
            WithTitle("추적 중인 플레이어");
            WithColor(new Color(0xFF69B4));
            WithDescription("\u200B");

            if (userTable.Rows.Count == 0)
            {
                AddField("이 채널에서 추적 중인 플레이어가 없습니다.", ">track [유저명/유저 id]로 추가할 수 있습니다.");
            }
            else
            {
                List<User> userList = new List<User>();

                // db에서 받아온 DataTable을 UserInfo 리스트로 변환
                Parallel.For(0, userTable.Rows.Count,
                    (i) => {
                        User user = User.Search(userTable.Rows[i]["user_id"].ToString());

                        // 기록이 없어서 순위가 0인 플레이어를 맨 아래로 정렬하기 위해 순위를 int 최댓값으로 임시 지정
                        if (user.pp_rank == 0)
                        {
                            user.pp_rank = int.MaxValue;
                        }

                        userList.Add(user);
                    });

                // 랭크 순으로 정렬해서 embed에 추가
                userList.Sort((x, y) => x.pp_rank.CompareTo(y.pp_rank));

                foreach (User user in userList)
                {
                    // int 최댓값으로 바꿨던 순위 0으로 복구
                    if (user.pp_rank == int.MaxValue)
                    {
                        user.pp_rank = 0;
                    }

                    AddField(user.username, string.Format("{0}pp ({1})",
                        user.pp_raw == 0 && user.pp_rank != 0 ? "?" : string.Format("{0:0.##}", user.pp_raw),
                        user.pp_rank == 0 ? "?" : user.pp_rank.ToString()
                        ), true
                    );
                }
            }
        }
    }
}
