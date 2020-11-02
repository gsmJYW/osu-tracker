namespace osu_tracker.api
{
    class Score
    {
        public int beatmap_id { get; set; }
        public int maxcombo { get; set; }
        public int count50 { get; set; }
        public int count100 { get; set; }
        public int count300 { get; set; }
        public int countmiss { get; set; }
        public int enabled_mods { get; set; }
        public int user_id { get; set; }
        public int index { get; set; }
        public ulong score { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }

        // 노트 판정 개수로 정확도 계산
        public double Accuracy()
        {
            return (50.0 * count50 + 100.0 * count100 + 300.0 * count300) / (300.0 * (countmiss + count50 + count100 + count300)) * 100.0;
        }

        // 랭크에 해당하는 이미지 url
        public string RankImageUrl()
        {
            switch (rank)
            {
                case "XH":
                    return "https://imgur.com/UzDVLCF.png";

                case "X":
                    return "https://imgur.com/7hMmYYW.png";

                case "SH":
                    return "https://imgur.com/AMT9Iyy.png";

                case "S":
                    return "https://imgur.com/NXxWkeX.png";

                case "A":
                    return "https://imgur.com/QCzekYf.png";

                case "B":
                    return "https://imgur.com/xq2Q4wB.png";

                case "C":
                    return "https://imgur.com/fL372Ks.png";

                default:
                    return "https://imgur.com/Dxlcytu.png";
            }
        }
    }
}