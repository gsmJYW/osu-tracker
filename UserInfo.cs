namespace osu_tracker
{
    class UserInfo
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public int playcount { get; set; }
        public int pp_rank { get; set; }
        public int pp_country_rank { get; set; }
        public double pp_raw { get; set; }
        public double level { get; set; }
        public double accuracy { get; set; }
        public string country { get; set; }
        public string join_date { get; set; }
    }
}
