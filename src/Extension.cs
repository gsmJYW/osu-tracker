using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace osu_tracker
{
    public static class Extension
    {
        // double 소숫점 3자리까지 비교
        public static bool IsCloseTo(this double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }

        // 문자열로 된 시각을 오프셋으로 변환
        public static DateTimeOffset ToDateTimeOffset(this string date)
        {
            var dateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9); // 한국 시간 = UTC +9
            var dateTimeMs = (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs);
        }
        
        // 2진법으로 된 모드 목록을 문자열 리스트로 반환
        public static IEnumerable<string> ToModList(this int mods)
        {
            var modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            var modList = new List<string>();

            for (var i = 1; i <= modBinary.Length; i++)
            {
                if (modBinary[^i])
                {
                    switch (i)
                    {
                        case 1:
                            modList.Add("NF");
                            break;

                        case 2:
                            modList.Add("EZ");
                            break;

                        //case 3:
                        //    modList.Add("TD");
                        //    break;

                        case 4:
                            modList.Add("HD");
                            break;

                        case 5:
                            modList.Add("HR");
                            break;

                        case 6:
                            modList.Add("SD");
                            break;

                        case 7:
                            modList.Add("DT");
                            break;

                        case 9:
                            modList.Add("HT");
                            break;

                        case 10:
                            modList.Add("NC");
                            break;

                        case 11:
                            modList.Add("FL");
                            break;

                        case 13:
                            modList.Add("SO");
                            break;

                        case 15:
                            modList.Add("PF");
                            break;
                    }
                }
            }

            if (modList.Contains("NC"))
                modList.Remove("DT");

            if (modList.Contains("PF"))
                modList.Remove("SD");

            return modList;
        }

        // 특정 이름을 가진 채널을 검색, 있을 경우 검색된 채널 반환, 없을 경우 생성한 후 생성한 채널 반환
        public static async Task<SocketTextChannel> CreateChannelIfNotExist(this SocketGuild guild, string name)
        {
            SocketTextChannel osuTrackerChannel = null;
            var isThereOsuTrackerChannel = false;

            var channelList = guild.TextChannels;

            foreach (var channel in channelList)
            {
                if (!string.Equals(channel.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                isThereOsuTrackerChannel = true;
                osuTrackerChannel = channel;
                break;
            }

            if (isThereOsuTrackerChannel)
                return osuTrackerChannel;

            var osuTrackerChannelId = (await guild.CreateTextChannelAsync("osu-tracker")).Id;
            osuTrackerChannel = guild.GetTextChannel(osuTrackerChannelId);

            return osuTrackerChannel;
        }
    }
}
