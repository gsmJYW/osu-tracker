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
            if (Math.Abs(a - b) < 0.0001)
                return true;
            else
                return false;
        }

        // 문자열로 된 시각을 오프셋으로 변환
        public static DateTimeOffset ToDateTimeOffset(this string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null).AddHours(9); // 한국 시간 = UTC +9
            long dateTimeMs = (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeMs);
        }
        
        // 모드에 해당하는 문자열
        public static string ToModString(this int mods)
        {
            bool[] modBinary = Convert.ToString(mods, 2).Select(s => s.Equals('1')).ToArray(); // 10진수를 2진 비트 배열로 저장
            string modString = "";

            for (int i = 1; i <= modBinary.Length; i++)
            {
                if (modBinary[modBinary.Length - i])
                {
                    switch (i)
                    {
                        case 1:
                            modString += "NF";
                            break;

                        case 2:
                            modString += "EZ";
                            break;

                        case 3:
                            modString += "TD";
                            break;

                        case 4:
                            modString += "HD";
                            break;

                        case 5:
                            modString += "HR";
                            break;

                        case 6:
                            modString += "SD";
                            break;

                        case 7:
                            modString += "DT";
                            break;

                        case 9:
                            modString += "HT";
                            break;

                        case 10:
                            modString += "NC";
                            break;

                        case 11:
                            modString += "FL";
                            break;

                        case 13:
                            modString += "SO";
                            break;

                        case 15:
                            modString += "PF";
                            break;
                    }
                }
            }

            if (modString.Contains("NC"))
                modString = modString.Replace("DT", "");

            if (modString.Contains("PF"))
                modString = modString.Replace("SD", "");

            return modString;
        }

        // 특정 이름을 가진 채널을 검색, 있을 경우 검색된 채널 반환, 없을 경우 생성한 후 생성한 채널 반환
        public async static Task<SocketTextChannel> CreateChannelIfNotExist(this SocketGuild guild, string name)
        {
            try
            {
                SocketTextChannel osuTrackerChannel = null;
                bool isThereOsuTrackerChannel = false;

                IReadOnlyCollection<SocketTextChannel> channelList = guild.TextChannels;

                foreach (SocketTextChannel channel in channelList)
                {
                    if (channel.Name.ToLower() == name.ToLower())
                    {
                        isThereOsuTrackerChannel = true;
                        osuTrackerChannel = channel;
                        break;
                    }
                }

                if (!isThereOsuTrackerChannel)
                {
                    ulong osuTrackerChannelId = (await guild.CreateTextChannelAsync("osu-tracker")).Id;
                    osuTrackerChannel = guild.GetTextChannel(osuTrackerChannelId);
                }

                return osuTrackerChannel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
