using Discord;
using System.Collections.Generic;
using System.Data;

namespace osu_tracker
{
    class UserListEmbed
    {
        public Embed embed;

        public UserListEmbed(DataTable users)
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("추적 중인 플레이어")
                .WithColor(new Color(0xFF69B4))
                .WithDescription("\u200B");

            if (users.Rows.Count == 0)
            {
                builder.AddField("이 채널에서 추적 중인 플레이어가 없습니다.", ">track [유저명/유저 id]로 추가할 수 있습니다.");
            }
            else
            {
                // db에서 받아온 DataTable을 UserInfo 리스트로 변환
                List<UserInfo> userInfos = new List<UserInfo>();

                foreach (DataRow user in users.Rows)
                {
                    UserInfo userInfo = ConvertOsuData.GetUserInfo(user["user_id"].ToString());
                    userInfos.Add(userInfo);
                }

                // 랭크 순으로 정렬해서 embed에 추가
                userInfos.Sort((x, y) => x.pp_rank.CompareTo(y.pp_rank));

                foreach (UserInfo userInfo in userInfos)
                {
                    builder.AddField(userInfo.username, string.Format("{0:0.0#}pp (#{1})", userInfo.pp_raw, userInfo.pp_rank));
                }
            }

            embed = builder.Build();
        }
    }
}
