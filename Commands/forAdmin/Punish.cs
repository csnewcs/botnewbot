using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace bot
{
    ////////////////////////////////////
    // 여기는 처벌 관련된 명령어 사용하는 곳 //
    ///////////////////////////////////
    [Group("처벌")]
    public class Punish : ModuleBase<SocketCommandContext>
    {
        Support support = new Support();
        [Command]
        public async Task help() //명령어: $처벌
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("처벌 명령어 사용법")
            .WithColor(new Color(0xbe33ff))
            .AddField("뮤트", "지정한 사람의 마이크를 없앱니다.\n음성채팅방에서 다른 누구도 그 사람의 목소리를 들을 수 없습니다.\n(지정한 사람이 음성채팅방에 있어야 사용 가능)\n(사용법: $처벌 뮤트 [뮤트 시킬 사용자 언급(여러 명 가능)])")
            .AddField("킥", "지정한 사람을 서버에서 쫓아냅니다.\n단 다시 들어올 수 있습니다.\n(사용법: $처벌 킥 [킥 시킬 사용자 언급(여러 명 가능)])")
            .AddField("밴", "지정한 사람을 서버에서 쫓아냅니다.\n단 밴이 풀릴 때 까지 다시 들어올 수 없습니다.\n(사용법: $처벌 밴 [밴 시킬 사용자 언급(여러 명 가능)])");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        [Command("뮤트", true)]
        public async Task mute() //명령어: $처벌 뮤트 <누군가를 멘션>
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            SocketMessage msg = Context.Message;
            var muteUsers = msg.MentionedUsers;
            if (!(support.hasPermission(user, Support.Permission.MuteUser) && support.isOver(user, muteUsers)) || muteUsers.Count == 0)
            {
                return;
            }
            try
            {
                Random rd = new Random();
                foreach (var muteUser in muteUsers)
                {
                    await (muteUser as SocketGuildUser).ModifyAsync(m => {m.Mute = true;});
                }
                if (muteUsers.Count != 1)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{support.getNickname(muteUsers.First() as SocketGuildUser)}외 {muteUsers.Count}분의 뮤트 처리가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
                else 
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{support.getNickname(muteUsers.First() as SocketGuildUser)}님의 뮤트 처리가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
            }
            catch
            {
                await msg.Channel.SendMessageAsync("저런 그분은 음성채팅에 있지 않아요.");
            }
        }
        [Command("킥", true)]
        public async Task kick()
        {
            SocketMessage msg = Context.Message;
            var kickUsers = msg.MentionedUsers;
            SocketGuildUser user = Context.User as SocketGuildUser;
            if (!(support.hasPermission(user, Support.Permission.KickUser) && support.isOver(user, kickUsers)) || kickUsers.Count == 0)
            {
                return;
            }
            Random rd = new Random();
            if (kickUsers.Count != 1)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{support.getNickname(kickUsers.First() as SocketGuildUser)}외 {kickUsers.Count}분의 킥 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
            else 
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{support.getNickname(kickUsers.First() as SocketGuildUser)}님의 킥 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }   
        }
        [Command("밴", true)]
        public async Task ban()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            SocketMessage msg = Context.Message;
            var banUsers = msg.MentionedUsers;
            if (!(support.hasPermission(user, Support.Permission.BanUser) && support.isOver(user, banUsers)) || banUsers.Count == 0)
            {
                return;
            }
            foreach (var a in banUsers)
            {
                await (a as SocketGuildUser).BanAsync();
            }
            Random rd = new Random();
            if (banUsers.Count != 1)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{support.getNickname(banUsers.First() as SocketGuildUser)}외 {banUsers.Count}분의 밴 처리가 완료되었습니다.");

                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
            else 
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{support.getNickname(banUsers.First() as SocketGuildUser)}님의 밴 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
        }
    }
}