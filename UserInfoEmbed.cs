using Discord;

namespace osu_tracker
{
    class UserInfoEmbed
    {
        public Embed embed;

        public UserInfoEmbed(UserInfo userInfo)
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xFF69B4))
                .WithDescription("​")
                .WithTimestamp(ConvertOsuData.dateToOffset(userInfo.join_date))
                .WithFooter(footer => { footer
                    .WithText("가입 일시");
                })
                .WithThumbnailUrl("https://a.ppy.sh/" + userInfo.user_id)
                .WithAuthor(author => { author
                    .WithName(userInfo.username)
                    .WithUrl("https://osu.ppy.sh/users/" + userInfo.user_id)
                    .WithIconUrl("https://github.com/ppy/osu/blob/master/assets/lazer.png?raw=true");
                })
                .AddField("퍼포먼스", string.Format("{0:0.0#}pp", userInfo.pp_raw))
                .AddField("순위", "#" + userInfo.pp_rank, true)
                .AddField("국가", string.Format(":flag_{0}: #{1}", userInfo.country.ToLower(), userInfo.pp_country_rank), true)
                .AddField("\u200B", "\u200B", true)
                .AddField("정확도", string.Format("{0:0.0#}%", userInfo.accuracy), true)
                .AddField("플레이 횟수", userInfo.playcount + "회", true)
                .AddField("\u200B", "\u200B", true)
                .AddField("레벨", string.Format("{0:0.0#}", userInfo.level) + "\n\u200B");

            embed = builder.Build();
        }
    }
}
