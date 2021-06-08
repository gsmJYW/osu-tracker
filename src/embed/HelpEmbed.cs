using Discord;

namespace osu_tracker.embed
{
    internal class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl)
        {
            WithColor(new Color(0x527788));
            WithThumbnailUrl(avatarUrl);

            AddField($"{Program.prefix}help", "osu!tracker의 사용법을 알려줍니다.");
            AddField($"{Program.prefix}info 유저명", "플레이어의 정보를 보여줍니다.");
            AddField($"{Program.prefix}track 유저명",
              @"이 서버에서 추적할 플레이어를 추가합니다.
                플레이어가 자신의 상위 pp 기록을 갱신할 때마다
                해당 스코어 정보를 #osu-tracker 채널에 보냅니다.
                이미 추적 중인 플레이어를 그만 추적하려면
                해당 플레이어의 유저명을 입력하세요.");
            AddField($"{Program.prefix}recent 유저명", "플레이어의 최근 플레이 기록을 보여줍니다.");
            AddField($"{Program.prefix}me 유저명",
             $@"자신의 플레이어 정보를 등록합니다.
                등록함으로써 다른 명령어들을 사용할 때
                유저명을 생략할 수 있습니다.
                이미 등록한 플레이어 정보를 삭제하려면
                유저명을 쓰지 말고 `{Program.prefix}me`를 입력하세요.");
            AddField($"{Program.prefix}list", "이 서버에서 추적 중인 플레이어들을 보여줍니다.");
        }
    }
}
