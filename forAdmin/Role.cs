using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace bot
{
    /////////////////////////////////
    // 여기는 사람들의 역할을 관리하는 곳 //
    /////////////////////////////////
    public class Role : ModuleBase<SocketCommandContext>
    {
        [Command("역할")]
        public async Task helpRole()
        {
            EmbedBuilder build = new EmbedBuilder()
            .WithTitle("역할 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("부여", "사람 혹은 사람들에게 역할을 부여합니다.\n(사용법: $역할 부여 [역할 부여할 사람 멘션 (여러명 가능)] [@부여할 역할 멘션 (여러개 가능)])")
            .AddField("부여 모두", "모든 사람에게 역할을 부여합니다.\n(사용법: $역할 부여 모두 [부여할 역할 멘션 (여러개 가능)])")
            .AddField("강탈", "사람 혹은 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 강탈 [역할을 없앨 사람 멘션 (여러명 가능)] [@빼앗을 역할 멘션 (여러개 가능)])")
            .AddField("강탈 모두", "모든 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 강탈 모두 [빼앗을 역할 멘션 (여러개 가능)])");
            await Context.User.SendMessageAsync("", embed:build.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        public async Task giveRole(SocketGuildUser user, SocketMessage message)
        {
            if (Program.isOver(user.Roles, message.MentionedRoles))
            {
                bool can = false;
                foreach (var a in message.MentionedUsers)
                {
                    if (Program.isOver(user.Roles, (a as SocketGuildUser).Roles))
                    {
                        can = true;
                        break;
                    }
                }
                if (can)
                {
                    if (!canManage(user, message.MentionedRoles)) return;
                    else await single(message);
                }
            }
        }
        private async Task single(SocketMessage message)
        {
            var mentionedUsers = message.MentionedUsers;
            var mentionedRoles = message.MentionedRoles;
            if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0)
            {
                await ReplyAsync("유저와 역할을 멘션했는지 확인하세요");
                return;
            } 
            foreach (SocketGuildUser a in mentionedUsers)
            {
                await a.AddRolesAsync(mentionedRoles);
            }
            if (mentionedUsers.Count == 1)
            {
                if (mentionedRoles.Count == 1)
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 부여 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
                else
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 부여 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
            }
            else
            {
                if (mentionedRoles.Count == 1)
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 부여 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
                else
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 부여 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
            }
        }
        
        public async Task ridRole(SocketGuildUser user, SocketMessage message)
        {
            if (Program.isOver(user.Roles, message.MentionedRoles))
            {
                bool can = false;
                foreach (var a in message.MentionedUsers)
                {
                    if (Program.isOver(user.Roles, (a as SocketGuildUser).Roles))
                    {
                        can = true;
                        break;
                    }
                }
                if (can)
                {
                    if (!canManage(user, message.MentionedRoles)) return;
                    else await singleRid(message);
                }
            }
        }
        private async Task singleRid(SocketMessage message)
        {
            var mentionedUsers = message.MentionedUsers;
            var mentionedRoles = message.MentionedRoles;
            if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0) return;
            foreach (SocketGuildUser a in mentionedUsers)
            {
                await a.RemoveRolesAsync(mentionedRoles);
            }
            if (mentionedUsers.Count == 1)
            {
                if (mentionedRoles.Count == 1)
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 강탈 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
                else
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 강탈 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들의 사라졌습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
            }
            else
            {
                if (mentionedRoles.Count == 1)
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 강탈 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
                else
                {
                    Random rd = new Random();
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
                    .AddField("역할 강탈 완료", $"{Program.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들이 사라졌습니다.");
                    await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
                }
            }
        }
        private bool canManage(SocketGuildUser user, IReadOnlyCollection<SocketRole> roles)
        {
            bool canManage = false;
            if (user.Id == user.Guild.OwnerId) return true;
            foreach (var a in user.Roles)
            {
                if (a.Permissions.ManageRoles)
                {
                    canManage = true;
                    break;
                }
            }
            if (!canManage) return false;
            return Program.isOver(user.Roles, roles);
        }
    }
}