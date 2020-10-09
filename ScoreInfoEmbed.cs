using Discord;
using static osu_tracker.ConvertOsuData;

namespace osu_tracker
{
    class ScoreInfoEmbed
    {
        public Embed embed;

        public ScoreInfoEmbed(UserBest userBest)
        {
            ScoreInfo scoreInfo = userBest.newScore;

            UserInfo userInfo = GetUserInfo(scoreInfo.user_id);
            MapInfo mapInfo = GetMapInfo(scoreInfo.beatmap_id);

            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle(string.Format("\n{0} - {1}", mapInfo.artist, mapInfo.title))
                .WithDescription(string.Format("**[{0}] {1}**\n​", mapInfo.version, scoreInfo.modsString()))
                .WithUrl(string.Format("https://osu.ppy.sh/beatmapsets/{0}#osu/{1}", mapInfo.beatmapset_id, scoreInfo.beatmap_id))
                .WithColor(new Color(0xFF69B4))
                .WithTimestamp(DateToOffset(scoreInfo.date))
                .WithFooter(footer => { footer
                    .WithText((userBest.newScoreIndex + 1) + "번째 탑 플레이");
                })
                .WithThumbnailUrl(scoreInfo.rankImageUrl())
                .WithImageUrl(string.Format("https://assets.ppy.sh/beatmaps/{0}/covers/cover.jpg", mapInfo.beatmapset_id))
                .WithAuthor(author => { author
                    .WithName(userInfo.username)
                    .WithUrl("https://osu.ppy.sh/users/" + userInfo.user_id)
                    .WithIconUrl("https://github.com/ppy/osu/blob/master/assets/lazer.png?raw=true");
                })
                .AddField("점수", scoreInfo.score, true)
                .AddField("퍼포먼스", string.Format("{0:0.0#}pp", scoreInfo.pp), true)
                .AddField("\u200B", "\u200B", true)
                .AddField("콤보", string.Format("{0}/{1}", scoreInfo.maxcombo, mapInfo.max_combo), true)
                .AddField("정확도", string.Format("{0:0.0#}%", scoreInfo.Accuracy()), true)
                .AddField("\u200B", "\u200B", true)
                .AddField("300", "x" + scoreInfo.count300, true)
                .AddField("100", "x" + scoreInfo.count100, true)
                .AddField("\u200B", "\u200B", true)
                .AddField("50", "x" + scoreInfo.count50, true)
                .AddField("미스", "x" + scoreInfo.countmiss, true)
                .AddField("\u200B", "\u200B", true);

            embed = builder.Build();
        }
    }
}