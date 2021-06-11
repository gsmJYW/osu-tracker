using System.ComponentModel;


namespace osu_tracker.region
{
    public class KO
    {
        private KO(string display)
        {
            this.display = display;
        }
        
        private readonly string display;
        
        public override string ToString()
        {
            return display;
        }

        public static readonly KO performance = new("퍼포먼스");
        public static readonly KO main_mode = new("주력 모드");
        public static readonly KO pp_rank = new("순위");
        public static readonly KO pp_country_rank = new("국가 순위");
        public static readonly KO accuracy = new("정확도");
        public static readonly KO level = new("레벨");
        public static readonly KO playcount = new("플레이 횟수");
        public static readonly KO total_seconds_played = new("플레이 시간");
        public static readonly KO join_date = new("가입 일시");
    }
    public class EN
    {
        private EN(string display)
        {
            this.display = display;
        }
        
        private readonly string display;
        
        public override string ToString()
        {
            return display;
        }
        // korean
        public static readonly EN performance = new ("performance");
        public static readonly EN main_mode = new ("main mode");
        public static readonly EN pp_rank = new ("pp rank");
        public static readonly EN pp_country_rank = new("pp country rank");
        public static readonly EN accuracy = new ("accuracy");
        public static readonly EN level = new ("level");
        public static readonly EN playcount = new("play count");
        public static readonly EN total_seconds_played = new("total play time");
        public static readonly EN join_date = new("Join date");
    }

    public class Languages
    {
        public string select(string language, string value)
        {
            return (language, value) switch
            {
                ("ko", "performance") => Kr_Performance.ToString(),
                ("ko", "mode") => Kr_MainMode.ToString(),
                ("ko", "pp_rank") => Kr_PPRank.ToString(),
                ("ko", "pp_country_rank") => Kr_PPCountryRank.ToString(),
                ("ko", "accuracy") => Kr_Accuracy.ToString(),
                ("ko", "level") => Kr_Level.ToString(),
                ("ko", "playcount") => Kr_Playcount.ToString(),
                ("ko", "total_seconds_played") => Kr_TotalSecondsPlayed.ToString(),
                ("ko", "join_date") => Kr_JoinDate.ToString(),
                ("en", "performance") => En_Performance.ToString(),
                ("en", "mode") => En_MainMode.ToString(),
                ("en", "pp_rank") => En_PPRank.ToString(),
                ("en", "pp_country_rank") => En_PPCountryRank.ToString(),
                ("en", "accuracy") => En_Accuracy.ToString(),
                ("en", "level") => En_Level.ToString(),
                ("en", "playcount") => En_Playcount.ToString(),
                ("en", "total_seconds_played") => En_TotalSecondsPlayed.ToString(),
                ("en", "join_date") => En_JoinDate.ToString(),
                _ => throw new WarningException($"This {language} and {value} is not support.")
            };
        }
        private static KO Kr_Performance => KO.performance;
        private static KO Kr_MainMode => KO.main_mode;
        private static KO Kr_PPRank => KO.pp_rank;
        private static KO Kr_PPCountryRank =>  KO.pp_country_rank;
        private static KO Kr_Accuracy => KO.accuracy;
        private static KO Kr_Level => KO.level;
        private static KO Kr_Playcount => KO.playcount;
        private static KO Kr_TotalSecondsPlayed => KO.total_seconds_played;
        private static KO Kr_JoinDate => KO.join_date;
        
        private static EN En_Performance => EN.performance;
        private static EN En_MainMode => EN.main_mode;
        private static EN En_PPRank => EN.pp_rank;
        private static EN En_PPCountryRank =>  EN.pp_country_rank;
        private static EN En_Accuracy => EN.accuracy;
        private static EN En_Level => EN.level;
        private static EN En_Playcount => EN.playcount;
        private static EN En_TotalSecondsPlayed => EN.total_seconds_played;
        private static EN En_JoinDate => EN.join_date;
    }
}
