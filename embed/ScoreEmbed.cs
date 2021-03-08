using Discord;
using osu_tracker.api;

namespace osu_tracker.embed
{
    class ScoreEmbed : EmbedBuilder
    {
        public ScoreEmbed(UserBest userBest)
        {
            Score best = userBest.newBest;
            User user = User.Search(best.user_id);
            Beatmap beatmap = Beatmap.Search(best.beatmap_id, best.enabled_mods);

            string modsString = best.enabled_mods.ToModString();

            if (modsString.Length > 0)
            {
                modsString = " +" + modsString;
            }

            WithTitle(string.Format("\n{0} - {1}", beatmap.artist, beatmap.title));
            WithDescription(string.Format("**[{0}]{1} {2:0.##}🟊**\n​", beatmap.version, modsString, beatmap.difficultyrating));
            WithUrl(string.Format("https://osu.ppy.sh/beatmapsets/{0}#osu/{1}", beatmap.beatmapset_id, best.beatmap_id));
            WithColor(new Color(0xFF69B4));
            WithTimestamp(best.date.ToDateTimeOffset());
            WithFooter(footer => { footer
                    .WithText((best.index + 1) + "번째 탑 플레이");
            });
            WithThumbnailUrl(best.RankImageUrl());
            WithImageUrl(string.Format("https://assets.ppy.sh/beatmaps/{0}/covers/cover.jpg", beatmap.beatmapset_id));
            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl("https://osu.ppy.sh/users/" + user.user_id)
                .WithIconUrl("https://www.countryflags.io/" + user.country.ToLower() + "/flat/64.png");
            });

            AddField("점수", best.score, true);
            AddField("퍼포먼스", string.Format("{0:0.##}pp", best.pp), true);

            if (userBest.previous_pp_raw == 0 && userBest.previous_pp_rank != 0)
            {
                AddField("퍼포먼스 변화", string.Format("{0:0.##}pp (복귀 유저)", userBest.pp_raw));
            }
            else
            {
                AddField("퍼포먼스 변화", string.Format("{0:0.##}pp ({1}{2:0.##})",
                    userBest.pp_raw,
                    userBest.pp_raw >= userBest.previous_pp_raw ? "+" : "",
                    userBest.pp_raw - userBest.previous_pp_raw
                    ), true
                );
            }
            
            AddField("콤보", string.Format("{0}/{1}", best.maxcombo, beatmap.max_combo), true);
            AddField("정확도", string.Format("{0:0.##}%", best.Accuracy()), true);

            if (userBest.previous_pp_rank == 0)
            {
                AddField("순위 변화", string.Format("#{0} (신규 유저)", userBest.pp_rank), true);
            }
            else
            {
                AddField("순위 변화", string.Format("#{0} ({1}{2})",
                    userBest.pp_rank,
                    userBest.previous_pp_rank >= userBest.pp_rank ? "+" : "",
                    userBest.previous_pp_rank - userBest.pp_rank
                    ), true
                );
            }

            AddField("300", "x" + best.count300, true);
            AddField("100", "x" + best.count100, true);
            AddField("\u200B", "\u200B", true);
            AddField("50", "x" + best.count50, true);
            AddField("미스", "x" + best.countmiss, true);
            AddField("\u200B", "\u200B", true);

        }
    }
}