using Discord;

namespace osu_tracker.embed
{
    class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl)
        {
            WithColor(new Color(0x527788));
            
            WithThumbnailUrl(avatarUrl);
            
            AddField(">help", "osu!tracker의 사용법을 알려줍니다.");
            AddField(">info 유저명/유저id", "플레이어의 정보를 보여줍니다.");
            AddField(">track 유저명/유저id",
                    "이 서버에서 추적할 플레이어를 추가합니다.\n" +
                    "플레이어가 자신의 상위 pp 기록을 갱신할 때마다\n" +
                    "해당 스코어 정보를 #osu-tracker 채널에 보냅니다.\n" +
                    "이미 추적 중인 플레이어일 경우 추적을 중지합니다.");
            AddField(">recent 유저명/유저id", "플레이어의 최근 플레이 기록을 보여줍니다.");
            AddField(">list", "이 서버에서 추적 중인 플레이어들을 보여줍니다.");
        }
    }
}
