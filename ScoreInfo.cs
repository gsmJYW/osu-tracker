using System;
using System.Linq;

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
        public ulong score { get; set; }
        public string date { get; set; }
        public string rank { get; set; }
        public double pp { get; set; }

        // 노트 판정 개수로 정확도 계산
        public double Accuracy()
        {
            return (50.0 * count50 + 100.0 * count100 + 300.0 * count300) / (300.0 * (countmiss + count50 + count100 + count300)) * 100.0;
        }

        // 문자열로 된 랭크를 이미지 링크로 변환
        public string rankImageUrl()
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

        // 정수로 쓰여져있는 모드 값을 문자열로 변환
        public string modsString()
        {
            bool[] modsBinary = Convert.ToString(enabled_mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            string modsStr = "";

            for (int i = 1; i <= modsBinary.Length; i++)
            {
                if (modsBinary[modsBinary.Length - i])
                {
                    switch (i)
                    {
                        case 1:
                            modsStr += "NF";
                            break;

                        case 2:
                            modsStr += "EZ";
                            break;

                        case 3:
                            modsStr += "TD";
                            break;

                        case 4:
                            modsStr += "HD";
                            break;

                        case 5:
                            modsStr += "HR";
                            break;

                        case 6:
                            modsStr += "SD";
                            break;

                        case 7:
                            modsStr += "DT";
                            break;

                        case 9:
                            modsStr += "HT";
                            break;

                        case 10:
                            modsStr += "NC";
                            break;

                        case 11:
                            modsStr += "FL";
                            break;

                        case 13:
                            modsStr += "SO";
                            break;

                        case 15:
                            modsStr += "PF";
                            break;
                    }
                }
            }

            if (modsStr.Contains("NC"))
                modsStr = modsStr.Replace("DT", "");

            if (modsStr.Contains("PF"))
                modsStr = modsStr.Replace("SD", "");

            if (modsStr == "")
                return modsStr;
            else
                return "+" + modsStr;
        }
    }
}