using Discord;
using System.Collections.Generic;
using System.Data;
using osu_tracker.api;
using System.Threading.Tasks;
using osu_tracker.region;

namespace osu_tracker.embed
{
    internal class ListEmbed : EmbedBuilder
    {
        public ListEmbed(DataTable userTable, Language lang)
        {
            WithColor(new Color(0xFF69B4));

            WithTitle(lang.Select("tracking_players"));
            WithDescription("\u200B");

            if (userTable.Rows.Count == 0)
            {
                AddField(lang.Select("no_tracking_players"), lang.Select("how_to_track"));
            }
            else
            {
                var userList = new List<User>();

                // db에서 받아온 DataTable을 UserInfo 리스트로 변환
                Parallel.For(0, userTable.Rows.Count,
                    (i) => {
                        var user = User.Search(userTable.Rows[i]["user_id"].ToString());

                        // 기록이 없어서 순위가 0인 플레이어를 맨 아래로 정렬하기 위해 순위를 int 최댓값으로 임시 지정
                        if (user.pp_rank == 0)
                        {
                            user.pp_rank = int.MaxValue;
                        }

                        userList.Add(user);
                    });

                // 랭크 순으로 정렬해서 embed에 추가
                userList.Sort((x, y) => x.pp_rank.CompareTo(y.pp_rank));

                foreach (var user in userList)
                {
                    // int 최댓값으로 바꿨던 순위를 0으로 복구
                    if (user.pp_rank == int.MaxValue)
                    {
                        user.pp_rank = 0;
                    }

                    AddField(
                        user.username, 
                        $"{(user.pp_raw == 0 && user.pp_rank != 0 ? "?" : $"{user.pp_raw:0.##}")}pp (#{(user.pp_rank == 0 ? "?" : user.pp_rank.ToString())})", 
                        true
                    );
                }
            }
        }
    }
}
