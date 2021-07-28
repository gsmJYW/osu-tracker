using Discord;
using osu_tracker.api;
using osu_tracker.region;

namespace osu_tracker.embed
{
    internal class InfoEmbed : EmbedBuilder
    {
        public InfoEmbed(User user, Language lang)
        {
            string mainModString;
            
            WithColor(new Color(0xFF69B4));
            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl($"https://osu.ppy.sh/users/{user.user_id}")
                .WithIconUrl($"https://www.countryflags.io/{user.country.ToLower()}/flat/64.png");
            });

            if (user.pp_rank == 0)
            {
                WithDescription(lang.Select("no_score") + "\n\u200B");
                AddField(lang.Select("level"), $"{user.level:0.##}", true);
                AddField("\u200B", "\u200B", true);
            }
            else
            {
                var userBest = new UserBest(user.user_id);
                userBest.GetMainMods();

                if (userBest.mainMods == 0)
                {
                    mainModString = "NM";
                }
                else
                {
                    var mainModList = userBest.mainMods.ToModList();
                    mainModString = string.Join("", mainModList);
                }

                if (user.pp_raw == 0)
                {
                    WithDescription(lang.Select("inactive_player") + "​\n\u200B");

                    AddField(lang.Select("pp_rank"), $"#{user.pp_rank}", true);
                    AddField(lang.Select("main_mods"), mainModString, true);
                    AddField("\u200B", "\u200B", true);
                    AddField(lang.Select("accuracy"), $"{user.accuracy:0.##}%", true);
                    AddField(lang.Select("level"), $"{user.level:0.##}", true);
                }
                else
                {
                    WithDescription(user.user_id == 10901226 | user.user_id == 14977949 ? lang.Select("osu_tracker_developer") + "\n\u200B" : "​");

                    AddField(lang.Select("performance"), $"{user.pp_raw:0.##}pp", true);
                    AddField(lang.Select("main_mods"), mainModString, true);
                    AddField("\u200B", "\u200B", true);

                    AddField(lang.Select("pp_rank"), $"#{user.pp_rank}", true);
                    AddField(lang.Select("pp_country_rank"), $"#{user.pp_country_rank}", true);
                    AddField("\u200B", "\u200B", true);

                    AddField(lang.Select("accuracy"), $"{user.accuracy:0.##}%", true);
                    AddField(lang.Select("level"), $"{user.level:0.##}", true);
                }
            }

            AddField("\u200B", "\u200B", true);

            AddField(lang.Select("playcount"), $"{user.playcount}\n​", true);
            AddField(lang.Select("total_play_time"), $"{user.total_seconds_played / 3600} hours", true);
            AddField("\u200B", "\u200B", true);

            WithThumbnailUrl($"https://a.ppy.sh/{user.user_id}");
            
            WithFooter(footer => { footer
                .WithText(lang.Select("join_date"));
            });
            WithTimestamp(user.join_date.ToDateTimeOffset());
        }
    }
}
