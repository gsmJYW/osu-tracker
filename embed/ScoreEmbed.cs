using Discord;
using osu_tracker.api;

namespace osu_tracker.embed
{
    class ScoreEmbed : EmbedBuilder
    {
        public ScoreEmbed(Score score)
        {
            User user = OsuConvert.ToUser(score.user_id);
            Beatmap beatmap = OsuConvert.ToBeatmap(score.beatmap_id, score.enabled_mods);

            string modsString = OsuConvert.ToModString(score.enabled_mods);
            if (modsString.Length > 0)
            {
                modsString = " +" + modsString;
            }

            WithTitle(string.Format("\n{0} - {1}", beatmap.artist, beatmap.title));
            WithDescription(string.Format("**[{0}]{1} {2:0.##}🟊**\n​", beatmap.version, modsString, beatmap.difficultyrating));
            WithUrl(string.Format("https://osu.ppy.sh/beatmapsets/{0}#osu/{1}", beatmap.beatmapset_id, score.beatmap_id));
            WithColor(new Color(0xFF69B4));
            WithTimestamp(OsuConvert.ToOffset(score.date));
            WithFooter(footer => { footer
                    .WithText((score.index + 1) + "번째 탑 플레이");
            });
            WithThumbnailUrl(score.RankImageUrl());
            WithImageUrl(string.Format("https://assets.ppy.sh/beatmaps/{0}/covers/cover.jpg", beatmap.beatmapset_id));
            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl("https://osu.ppy.sh/users/" + user.user_id)
                .WithIconUrl("https://github.com/ppy/osu/blob/master/assets/lazer.png?raw=true");
            });
            AddField("점수", score.score, true);
            AddField("퍼포먼스", string.Format("{0:0.##}pp", score.pp), true);
            AddField("\u200B", "\u200B", true);
            AddField("콤보", string.Format("{0}/{1}", score.maxcombo, beatmap.max_combo), true);
            AddField("정확도", string.Format("{0:0.##}%", score.Accuracy()), true);
            AddField("\u200B", "\u200B", true);
            AddField("300", "x" + score.count300, true);
            AddField("100", "x" + score.count100, true);
            AddField("\u200B", "\u200B", true);
            AddField("50", "x" + score.count50, true);
            AddField("미스", "x" + score.countmiss, true);
            AddField("\u200B", "\u200B", true);
        }
    }
}