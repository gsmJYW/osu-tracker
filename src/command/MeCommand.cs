using Discord;
using Discord.Commands;
using Discord.WebSocket;
using osu_tracker.api;
using System;
using System.Data;
using System.Threading.Tasks;
// ReSharper disable HeapView.BoxingAllocation

namespace osu_tracker.command
{
    // ReSharper disable once UnusedType.Global
    public class MeCommand : ModuleBase<ShardedCommandContext>
    {
        // ReSharper disable once UnusedMember.Global
        [Command("me")]
        public async Task Recent(params string[] args)
        {
            var discordUser = Context.User;
            var username = string.Join(" ", args);

            // ReSharper disable once HeapView.ObjectAllocation
            var userTable = Sql.Get("SELECT * FROM users WHERE discord_id = '{0}'", discordUser.Id);

            if (username.Length == 0)
            {
                if (userTable.Rows.Count > 0)
                {
                    // ReSharper disable once HeapView.ObjectAllocation
                    Sql.Execute("DELETE FROM users WHERE discord_id = '{0}'", discordUser.Id);
                    await ReplyAsync($"**{discordUser.Username}**님의 유저 정보를 삭제 했습니다.");
                }
                else
                {
                    await ReplyAsync("삭제할 유저 정보가 없습니다.");
                }

                return;
            }

            User user;

            try
            {
                user = User.Search(username);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                return;
            }

            if (userTable.Rows.Count > 0)
            {
                // ReSharper disable once HeapView.ObjectAllocation
                Sql.Execute("UPDATE users SET user_id = '{0}' WHERE discord_id = '{1}'", user.user_id, discordUser.Id);
            }
            else
            {
                // ReSharper disable once HeapView.ObjectAllocation
                Sql.Execute("INSERT INTO users VALUES ('{0}', '{1}')", discordUser.Id, user.user_id);
            }

            await ReplyAsync($"**{discordUser.Username}**님의 유저 정보를 등록 했습니다.");
        }
    }
}
