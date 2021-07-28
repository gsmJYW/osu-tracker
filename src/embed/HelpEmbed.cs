using Discord;
using osu_tracker.region;

namespace osu_tracker.embed
{
    internal class HelpEmbed : EmbedBuilder
    {
        public HelpEmbed(string avatarUrl, Language lang)
        {
            WithColor(new Color(0x527788));
            WithThumbnailUrl(avatarUrl);

            switch (lang.language)
            {
                case "kr":
                    AddField($"{Program.prefix}help", "osu!tracker의 사용법을 알려줍니다.");
                    AddField($"{Program.prefix}lang 언어", "해당 서버에서 사용할 언어를 변경합니다.\n관리자만 사용하실 수 있습니다.\n현재 지원 중인 언어: `en`, `kr`");
                    AddField($"{Program.prefix}info 유저명", "플레이어의 정보를 보여줍니다.");
                    AddField($"{Program.prefix}track 유저명",
                      @"이 서버에서 추적할 플레이어를 추가합니다.
                플레이어가 자신의 상위 100 pp 기록을 갱신할 때마다
                해당 스코어 정보를 #osu-tracker 채널에 보냅니다.
                이미 추적 중인 플레이어를 그만 추적하려면
                해당 플레이어의 유저명을 입력하세요.");
                    AddField($"{Program.prefix}recent 유저명", "플레이어의 최근 플레이 기록을 보여줍니다.");
                    AddField($"{Program.prefix}me 유저명",
                     $@"자신의 플레이어 정보를 등록합니다.
                등록함으로써 명령어 사용 시마다 유저명을 입력하지 않아도 됩니다.
                이미 등록한 플레이어 정보를 삭제하려면 `{Program.prefix}me`를 입력하세요.");
                    AddField($"{Program.prefix}list", "이 서버에서 추적 중인 플레이어들을 보여줍니다.");
                    break;

                default:
                    AddField($"{Program.prefix}help", "Shows how to use osu!tracker.");
                    AddField($"{Program.prefix}lang language", "Changes the language for this server.\nOnly administrators can use this feature.\nLanguages currently supported: `en`, `kr`");
                    AddField($"{Program.prefix}info username", "Shows the player's information.");
                    AddField($"{Program.prefix}track username",
                      @"Adds a player to track from this server.
                Everytime the player sets a top 100 pp score,
                Information of the score will be sent to #osu-tracker.
                If you want to stop tracking a player,
                type a username of the player again.");
                    AddField($"{Program.prefix}recent username", "Shows the player's recent play.");
                    AddField($"{Program.prefix}me username",
                     $@"Registers your user info,
                so that you don't have to type a username everytime you use a command.
                If you want to delete your user info, just type `{Program.prefix}me`");
                    AddField($"{Program.prefix}list", "Shows players being tracked from this server.");
                    break;
            }
        }
    }
}
