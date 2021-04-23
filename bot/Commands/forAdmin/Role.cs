using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using botnewbot.Support;

namespace bot
{
    /////////////////////////////////
    // 여기는 사람들의 역할을 관리하는 곳 //
    /////////////////////////////////
    [Group("역할")]
    public class Role : ModuleBase<SocketCommandContext>
    {
        Support support = new Support();
        Permission permission = new Permission();
        [Command]
        public async Task helpRole()
        {
            await ReplyAsync($"이 명령어는 이전되었습니다. '{Program.prefix}명령어'를 사용해 주세요");
            /*EmbedBuilder build = new EmbedBuilder()
            .WithTitle("역할 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("부여", "사람 혹은 사람들에게 역할을 부여합니다.\n(사용법: $역할 부여 [역할 부여할 사람 멘션 (여러명 가능)] [@부여할 역할 멘션 (여러개 가능)])")
            .AddField("부여 모두", "모든 사람에게 역할을 부여합니다.\n(사용법: $역할 부여 모두 [부여할 역할 멘션 (여러개 가능)])")
            .AddField("강탈", "사람 혹은 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 강탈 [역할을 없앨 사람 멘션 (여러명 가능)] [@빼앗을 역할 멘션 (여러개 가능)])")
            .AddField("강탈 모두", "모든 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 강탈 모두 [빼앗을 역할 멘션 (여러개 가능)])");
            await Context.User.SendMessageAsync("", embed:build.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");*/
        }
        [Command("부여", true)]
        public async Task giveRole()
        {
            // try
            // {
            //     SocketGuildUser user = Context.User as SocketGuildUser;
            //     SocketMessage message = Context.Message;
            //     if (!(permission.compareRolePosition(user, message.MentionedRoles) && support.hasPermission(user, Support.Permission.ManageRole)))
            //     {
            //         await ReplyAsync("봇의 권한이 없거나 명령어를 사용하는 사람의 권한이 없어요.");
            //         return;
            //     }
            //     var mentionedUsers = message.MentionedUsers;
            //     var mentionedRoles = message.MentionedRoles;
            //     if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0)
            //     {
            //         await ReplyAsync("유저와 역할을 멘션했는지 확인하세요");
            //         return;
            //     } 
            //     foreach (SocketGuildUser a in mentionedUsers)
            //     {
            //         await a.AddRolesAsync(mentionedRoles);
            //     }
            //     if (mentionedUsers.Count == 1)
            //     {
            //         if (mentionedRoles.Count == 1)
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //         else
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //     }
            //     else
            //     {
            //         if (mentionedRoles.Count == 1)
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //         else
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //     }
            // }
            // catch (Exception e)
            // {
            //     await ReplyAsync("```" + e.ToString() + "```");
            // }
        }
        [Command("강탈", true)]
        public async Task ridRole()
        {
            // SocketMessage message = Context.Message;
            // SocketGuildUser user = Context.User as SocketGuildUser;
            // var mentionedUsers = message.MentionedUsers;
            // var mentionedRoles = message.MentionedRoles;
            // if (!(support.hasPermission(user, Support.Permission.ManageRole) && support.isOver(user, mentionedRoles)))
            // {
            //     return;
            // }
            // if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0) return;

            // foreach (SocketGuildUser a in mentionedUsers)
            // {
            //     await a.RemoveRolesAsync(mentionedRoles);
            // }
            // if (mentionedUsers.Count == 1)
            // {
            //     if (mentionedRoles.Count == 1)
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 강탈 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            //     else
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 강탈 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들의 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            // }
            // else
            // {
            //     if (mentionedRoles.Count == 1)
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 강탈 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            //     else
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 강탈 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            // }
        }
    }
}