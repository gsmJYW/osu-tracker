using Discord;
using osu_tracker.api;

namespace osu_tracker.embed
{
    class RecentEmbed : EmbedBuilder
    {
        public RecentEmbed(Score recent)
        {
            User user = User.Search(recent.user_id);
            Beatmap beatmap = Beatmap.Search(recent.beatmap_id, recent.enabled_mods);

            string modsString = recent.enabled_mods.ToModString();

            if (modsString.Length > 0)
            {
                modsString = " +" + modsString;
            }

            WithColor(new Color(0xFF69B4));
            
            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl("https://osu.ppy.sh/users/" + user.user_id)
                .WithIconUrl("https://www.countryflags.io/" + user.country.ToLower() + "/flat/64.png");
            });

            WithTitle(string.Format("\n{0} - {1}", beatmap.artist, beatmap.title));
            WithDescription(string.Format("**[{0}]{1} {2:0.##}🟊**\n​", beatmap.version, modsString, beatmap.difficultyrating));
            WithUrl(string.Format("https://osu.ppy.sh/beatmapsets/{0}#osu/{1}", beatmap.beatmapset_id, recent.beatmap_id));

            WithThumbnailUrl(recent.RankImageUrl());
            WithImageUrl(string.Format("https://assets.ppy.sh/beatmaps/{0}/covers/cover.jpg", beatmap.beatmapset_id));

            WithTimestamp(recent.date.ToDateTimeOffset());

            AddField("점수", recent.score, true);
            AddField("콤보", string.Format("{0}/{1}", recent.maxcombo, beatmap.max_combo), true);
            AddField("정확도", string.Format("{0:0.##}%", recent.Accuracy()), true);
            
            AddField("300", "x" + recent.count300, true);
            AddField("100", "x" + recent.count100, true);
            AddField("\u200B", "\u200B", true);
            
            AddField("50", "x" + recent.count50, true);
            AddField("미스", "x" + recent.countmiss, true);
            AddField("\u200B", "\u200B", true);
        }
    }
}