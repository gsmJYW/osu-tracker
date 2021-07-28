using System;
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

        public static readonly KO no_score = new("등록한 스코어 없음");
        public static readonly KO performance = new("퍼포먼스");
        public static readonly KO main_mods = new("주력 모드");
        public static readonly KO pp_rank = new("순위");
        public static readonly KO pp_country_rank = new("국가 순위");
        public static readonly KO accuracy = new("정확도");
        public static readonly KO level = new("레벨");
        public static readonly KO playcount = new("플레이 횟수");
        public static readonly KO total_play_time = new("플레이 시간");
        public static readonly KO join_date = new("가입 일시");
        public static readonly KO inactive_player = new("장기간 활동이 없는 유저");
        public static readonly KO osu_tracker_developer = new("osu!tracker 개발자");
        public static readonly KO player_not_found = new("플레이어를 찾을 수 없습니다.");
        public static readonly KO no_recent_play = new("최근 24시간 플레이 기록이 없습니다.");
        public static readonly KO no_username = new($"**유저명**을 입력하지 않으셨습니다.\n매번 유저명을 입력하고 싶지 않으시면, `{Program.prefix}me 유저명`을 입력하세요.");
        public static readonly KO no_user_info = new($"유저 정보를 등록하지 않으셨습니다.\n등록하려면 `{Program.prefix}me 유저명`을 입력하세요.");
        public static readonly KO user_info_registered = new("**username**님의 유저 정보를 등록 했습니다.");
        public static readonly KO user_info_deleted = new("**username**님의 유저 정보를 삭제했습니다.");
        public static readonly KO tracking_started = new("**guild**에서 **username**님을 추적합니다.");
        public static readonly KO tracking_stopped = new("더 이상 **guild**에서 **username**님을 추적하지 않습니다.");
        public static readonly KO tracking_players = new("추적 중인 플레이어");
        public static readonly KO no_tracking_players = new("이 서버에서 추적 중인 플레이어가 없습니다.");
        public static readonly KO how_to_track = new($"`{Program.prefix}track 유저명`으로 추가하실 수 있습니다.");
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
        // English
        public static readonly EN no_score = new("the player hasn't set a score");
        public static readonly EN performance = new ("performance");
        public static readonly EN main_mods = new ("main mods");
        public static readonly EN pp_rank = new ("pp rank");
        public static readonly EN pp_country_rank = new("pp country rank");
        public static readonly EN accuracy = new ("accuracy");
        public static readonly EN level = new ("level");
        public static readonly EN playcount = new("playcount");
        public static readonly EN total_play_time = new("total play time");
        public static readonly EN join_date = new("join date");
        public static readonly EN inactive_player = new("an inactive player");
        public static readonly EN osu_tracker_developer = new("the developer of osu!tracker");
        public static readonly EN player_not_found = new("Couldn't find the player");
        public static readonly EN no_recent_play = new("The player hasn't played a game for 24 hours.");
        public static readonly EN no_username = new($"Please provide a **username**.\nIf you don't want to type a username everytime, use `{Program.prefix}me username`.");
        public static readonly EN no_user_info = new($"You didn't register your user info\nTo register, use `{Program.prefix}me username`.");
        public static readonly EN user_info_registered = new("Registered **username**'s user info");
        public static readonly EN user_info_deleted = new("Deleted **username**'s user info");
        public static readonly EN tracking_started = new("Now tracking **username** from **guild**");
        public static readonly EN tracking_stopped = new("Stopped tracking **username** from **guild**");
        public static readonly EN tracking_players = new("Players being tracked");
        public static readonly EN no_tracking_players = new("There's no player being tracked from this server.");
        public static readonly EN how_to_track = new($"To add, use `{Program.prefix}track username`");
    }

    public class Language
    {
        public string language;

        public Language()
        {
            language = "en";
        }

        public Language(object language)
        {
            switch (language.ToString().ToLower())
            {
                case "en":
                case "kr":
                    this.language = language.ToString();
                    break;

                default:
                    throw new Exception("No such language.");
            }
        }

        public string Select(string value)
        {
            return (language, value) switch
            {
                ("kr", "performance") => Kr_Performance.ToString(),
                ("kr", "main_mods") => Kr_MainMods.ToString(),
                ("kr", "pp_rank") => Kr_PPRank.ToString(),
                ("kr", "pp_country_rank") => Kr_PPCountryRank.ToString(),
                ("kr", "accuracy") => Kr_Accuracy.ToString(),
                ("kr", "level") => Kr_Level.ToString(),
                ("kr", "playcount") => Kr_Playcount.ToString(),
                ("kr", "total_play_time") => Kr_TotalPlayTime.ToString(),
                ("kr", "join_date") => Kr_JoinDate.ToString(),
                ("kr", "no_score") => Kr_NoScore.ToString(),
                ("kr", "inactive_player") => Kr_InactivePlayer.ToString(),
                ("kr", "osu_tracker_developer") => Kr_OsuTrackerDeveloper.ToString(),
                ("kr", "player_not_found") => Kr_PlayerNotFound.ToString(),
                ("kr", "no_recent_play") => Kr_NoRecentPlay.ToString(),
                ("kr", "no_username") => Kr_NoUsername.ToString(),
                ("kr", "no_user_info") => Kr_NoUserInfo.ToString(),
                ("kr", "user_info_registered") => Kr_UserInfoRegistered.ToString(),
                ("kr", "user_info_deleted") => Kr_UserInfoDeleted.ToString(),
                ("kr", "tracking_started") => Kr_TrackingStarted.ToString(),
                ("kr", "tracking_stopped") => Kr_TrackingStopped.ToString(),
                ("kr", "tracking_players") => Kr_TrackingPlayers.ToString(),
                ("kr", "no_tracking_players") => Kr_NoTrackingPlayers.ToString(),
                ("kr", "how_to_track") => Kr_HowToTrack.ToString(),

                ("en", "performance") => En_Performance.ToString(),
                ("en", "main_mods") => En_MainMods.ToString(),
                ("en", "pp_rank") => En_PPRank.ToString(),
                ("en", "pp_country_rank") => En_PPCountryRank.ToString(),
                ("en", "accuracy") => En_Accuracy.ToString(),
                ("en", "level") => En_Level.ToString(),
                ("en", "playcount") => En_Playcount.ToString(),
                ("en", "total_play_time") => En_TotalPlayTime.ToString(),
                ("en", "join_date") => En_JoinDate.ToString(),
                ("en", "no_score") => En_NoScore.ToString(),
                ("en", "inactive_player") => En_InactivePlayer.ToString(),
                ("en", "osu_tracker_developer") => En_OsuTrackerDeveloper.ToString(),
                ("en", "player_not_found") => En_PlayerNotFound.ToString(),
                ("en", "no_recent_play") => En_NoRecentPlay.ToString(),
                ("en", "no_username") => En_NoUsername.ToString(),
                ("en", "no_user_info") => En_NoUserInfo.ToString(),
                ("en", "user_info_registered") => En_UserInfoRegistered.ToString(),
                ("en", "user_info_deleted") => En_UserInfoDeleted.ToString(),
                ("en", "tracking_started") => En_TrackingStarted.ToString(),
                ("en", "tracking_stopped") => En_TrackingStopped.ToString(),
                ("en", "tracking_players") => En_TrackingPlayers.ToString(),
                ("en", "no_tracking_players") => En_NoTrackingPlayers.ToString(),
                ("en", "how_to_track") => En_HowToTrack.ToString(),
                _ => throw new WarningException($"{language} - {value} is not supported.")
            };
        }
        private static KO Kr_Performance => KO.performance;
        private static KO Kr_MainMods => KO.main_mods;
        private static KO Kr_PPRank => KO.pp_rank;
        private static KO Kr_PPCountryRank =>  KO.pp_country_rank;
        private static KO Kr_Accuracy => KO.accuracy;
        private static KO Kr_Level => KO.level;
        private static KO Kr_Playcount => KO.playcount;
        private static KO Kr_TotalPlayTime => KO.total_play_time;
        private static KO Kr_JoinDate => KO.join_date;
        private static KO Kr_NoScore => KO.no_score;
        private static KO Kr_InactivePlayer => KO.inactive_player;
        private static KO Kr_OsuTrackerDeveloper => KO.osu_tracker_developer;
        private static KO Kr_PlayerNotFound => KO.player_not_found;
        private static KO Kr_NoRecentPlay => KO.no_recent_play;
        private static KO Kr_NoUsername => KO.no_username;
        private static KO Kr_NoUserInfo => KO.no_user_info;
        private static KO Kr_UserInfoRegistered => KO.user_info_registered;
        private static KO Kr_UserInfoDeleted => KO.user_info_deleted;
        private static KO Kr_TrackingStarted => KO.tracking_started;
        private static KO Kr_TrackingStopped => KO.tracking_stopped;
        private static KO Kr_TrackingPlayers => KO.tracking_players;
        private static KO Kr_NoTrackingPlayers => KO.no_tracking_players;
        private static KO Kr_HowToTrack => KO.how_to_track;

        private static EN En_Performance => EN.performance;
        private static EN En_MainMods => EN.main_mods;
        private static EN En_PPRank => EN.pp_rank;
        private static EN En_PPCountryRank =>  EN.pp_country_rank;
        private static EN En_Accuracy => EN.accuracy;
        private static EN En_Level => EN.level;
        private static EN En_Playcount => EN.playcount;
        private static EN En_TotalPlayTime => EN.total_play_time;
        private static EN En_JoinDate => EN.join_date;
        private static EN En_NoScore => EN.no_score;
        private static EN En_InactivePlayer => EN.inactive_player;
        private static EN En_OsuTrackerDeveloper => EN.osu_tracker_developer;
        private static EN En_PlayerNotFound => EN.player_not_found;
        private static EN En_NoRecentPlay => EN.no_recent_play;
        private static EN En_NoUsername => EN.no_username;
        private static EN En_NoUserInfo => EN.no_user_info;
        private static EN En_UserInfoRegistered => EN.user_info_registered;
        private static EN En_UserInfoDeleted => EN.user_info_deleted;
        private static EN En_TrackingStarted => EN.tracking_started;
        private static EN En_TrackingStopped => EN.tracking_stopped;
        private static EN En_TrackingPlayers => EN.tracking_players;
        private static EN En_NoTrackingPlayers => EN.no_tracking_players;
        private static EN En_HowToTrack => EN.how_to_track;
    }
}
