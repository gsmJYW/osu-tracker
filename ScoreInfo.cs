namespace osu_tracker
{
    class ScoreInfo
    {
        public int beatmap_id { get; set; }
        public int maxcombo { get; set; }
        public int count50 { get; set; }
        public int count100 { get; set; }
        public int count300 { get; set; }
        public int countmiss { get; set; }
        public int enabled_mods { get; set; }
        public int user_id { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }
    }
}
