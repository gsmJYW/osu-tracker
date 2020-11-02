using Discord;
using System.Collections.Generic;
using System.Data;
using osu_tracker.api;

namespace osu_tracker.embed
{
    class UserListEmbed : EmbedBuilder
    {
        public UserListEmbed(DataTable users)
        {
            WithTitle("추적 중인 플레이어");
            WithColor(new Color(0xFF69B4));
            WithDescription("\u200B");

            if (users.Rows.Count == 0)
            {
                AddField("이 채널에서 추적 중인 플레이어가 없습니다.", ">track [유저명/유저 id]로 추가할 수 있습니다.");
            }
            else
            {
                // db에서 받아온 DataTable을 UserInfo 리스트로 변환
                List<User> userInfos = new List<User>();

                foreach (DataRow user in users.Rows)
                {
                    User userInfo = OsuConvert.ToUser(user["user_id"].ToString());
                    userInfos.Add(userInfo);
                }

                // 랭크 순으로 정렬해서 embed에 추가
                userInfos.Sort((x, y) => x.pp_rank.CompareTo(y.pp_rank));

                foreach (User userInfo in userInfos)
                {
                    AddField(userInfo.username, string.Format("{0:0.0#}pp (#{1})", userInfo.pp_raw, userInfo.pp_rank));
                }
            }
        }
    }
}
