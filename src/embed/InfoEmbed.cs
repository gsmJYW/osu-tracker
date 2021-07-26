using Discord;
using osu_tracker.api;
using osu_tracker.region;
// ReSharper disable HeapView.BoxingAllocation


namespace osu_tracker.embed
{
    internal class InfoEmbed : EmbedBuilder
    {
        public InfoEmbed(User user, string userLanguage)
        {
            Languages language = new Languages();
            string mainModString;
        
            if (user.pp_rank == 0)
            {
                mainModString = "없음";
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
            }

            WithColor(new Color(0xFF69B4));

            WithAuthor(author => { author
                .WithName(user.username)
                .WithUrl($"https://osu.ppy.sh/users/{user.user_id}")
                .WithIconUrl($"https://www.countryflags.io/{user.country.ToLower()}/flat/64.png");
            });

            if (user.pp_rank == 0)
            {
                WithDescription(language.select(userLanguage, "no_user_play_history") + "\n\u200B");
                AddField(language.select(userLanguage, "level"), $"{user.level:0.##}", true);
                AddField("\u200B", "\u200B", true);
            }
            else if (user.pp_raw == 0)
            {
                WithDescription(language.select(userLanguage, "lomg_time_no_active") + "​\n\u200B");

                AddField(language.select(userLanguage, "pp_rank"), $"#{user.pp_rank}", true);
                AddField(language.select(userLanguage, "main_mode"), mainModString, true);
                AddField("\u200B", "\u200B", true);
                AddField(language.select(userLanguage, "accuracy"), $"{user.accuracy:0.##}%", true);
                AddField(language.select(userLanguage, "level"), $"{user.level:0.##}", true);
            }
            else
            {
                WithDescription(user.user_id == 10901226 | user.user_id == 14977949 ? language.select(userLanguage, "i_am_osu_tracker_developer") + "!\n\u200B" : "​");

                AddField(language.select(userLanguage, "performance"), $"{user.pp_raw:0.##}pp", true);
                AddField(language.select(userLanguage, "main_mode"), mainModString, true);
                AddField("\u200B", "\u200B", true);

                AddField(language.select(userLanguage, "pp_rank"), $"#{user.pp_rank}", true);
                AddField(language.select(userLanguage, "pp_country_rank"), $"#{user.pp_country_rank}", true);
                AddField("\u200B", "\u200B", true);
                
                AddField(language.select(userLanguage, "accuracy"), $"{user.accuracy:0.##}%", true);
                AddField(language.select(userLanguage, "level"), $"{user.level:0.##}", true);
            }

            AddField("\u200B", "\u200B", true);

            AddField(language.select(userLanguage, "playcount"), $"{user.playcount}\n​", true);
            AddField(language.select(userLanguage, "total_seconds_played"), $"{user.total_seconds_played / 3600} hours", true);
            AddField("\u200B", "\u200B", true);

            WithThumbnailUrl($"https://a.ppy.sh/{user.user_id}");
            
            WithFooter(footer => { footer
                .WithText(language.select(userLanguage, "join_date"));
            });
            WithTimestamp(user.join_date.ToDateTimeOffset());
        }
    }
}
