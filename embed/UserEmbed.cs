using Discord;
using osu_tracker.api;

namespace osu_tracker.embed
{
    class UserEmbed : EmbedBuilder
    {
        public UserEmbed(User user)
        {
            UserBest userBest = new UserBest(user.user_id);
            string ppString, ppRankString, ppCountryRankString, modString = OsuConvert.ToModString(userBest.MainMods());

            if (modString.Length == 0)
            {
                modString = "노모드";
            }

            // inactive 플레이어
            if (user.pp_raw == 0)
            {
                ppString = "활동 없음";
                ppRankString = "활동 없음";
                ppCountryRankString = "활동 없음";
            }
            else
            {
                ppString = string.Format("{0:0.##}pp", user.pp_raw);
                ppRankString = "#" + user.pp_rank;
                ppCountryRankString = "#" + user.pp_country_rank;
            }

            WithColor(new Color(0xFF69B4));
            WithDescription("​");
            WithTimestamp(OsuConvert.ToDateTimeOffset(user.join_date));
            WithFooter(footer => { footer
                .WithText("가입 일시");
            });
            WithThumbnailUrl("https://a.ppy.sh/" + user.user_id);
            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl("https://osu.ppy.sh/users/" + user.user_id)
                .WithIconUrl("https://github.com/ppy/osu/blob/master/assets/lazer.png?raw=true");
            });
            AddField("퍼포먼스", ppString, true);
            AddField("주력 모드", modString, true);
            AddField("\u200B", "\u200B", true);
            AddField("순위", ppRankString, true);
            AddField("국가 순위", string.Format(":flag_{0}: {1}", user.country.ToLower(), ppCountryRankString), true);
            AddField("\u200B", "\u200B", true);
            AddField("정확도", string.Format("{0:0.##}%", user.accuracy), true);
            AddField("레벨", string.Format("{0:0.##}", user.level), true);
            AddField("\u200B", "\u200B", true);
            AddField("플레이 횟수", user.playcount + "회", true);
            AddField("플레이 시간", user.total_seconds_played / 3600 + "시간", true);
            AddField("\u200B", "\u200B", true);
        }
    }
}
