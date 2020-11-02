namespace osu_tracker.api
{
    class Beatmap
    {
        public string artist { get; set; }
        public int beatmapset_id { get; set; }
        public string title { get; set; }
        public string version { get; set; }
        public int max_combo { get; set; }
        public double difficultyrating { get; set; }
    }
}
