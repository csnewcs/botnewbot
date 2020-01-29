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
    class Punish
    {
        public async Task help(SocketGuildUser user, SocketMessage msg)
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("처벌 명령어 사용법")
            .WithColor(new Color(0xbe33ff))
            .AddField("뮤트", "지정한 사람의 마이크를 없앱니다. 음성채팅방에서 다른 누구도 그 사람의 목소리를 들을 수 없습니다. (사용법: $처벌 뮤트 [뮤트 시킬 사용자 언급(여러 명 가능)])")
            .AddField("킥", "지정한 사람을 서버에서 쫓아냅니다. 단 다시 들어올 수 있습니다. (사용법: $처벌 킥 [킥 시킬 사용자 언급(여러 명 가능)])")
            .AddField("밴", "지정한 사람을 서버에서 쫓아냅니다. 단 밴이 풀릴 때 까지 다시 들어올 수 없습니다. (사용법: $처벌 밴 [밴 시킬 사용자 언급(여러 명 가능)])");
            await user.SendMessageAsync("", embed:builder.Build());
            await msg.Channel.SendMessageAsync("DM으로 결과를 전송했습니다.");
        }
        public async Task mute(SocketGuildUser user, SocketMessage msg)
        {
            try
            {
                var muteUsers = msg.MentionedUsers;
                if (muteUsers.Count == 0) return;
                Random rd = new Random();
                Program program = new Program();
                foreach (var muteUser in muteUsers)
                {
                    GuildUserProperties properties = new GuildUserProperties{Mute = true};
                    await (muteUser as SocketGuildUser).ModifyAsync(m => {m.Mute = true;});
                }
                if (muteUsers.Count != 1)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{program.getNickname(muteUsers.First() as SocketGuildUser)}외 {muteUsers.Count}분의 뮤트 처리가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
                else 
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{program.getNickname(muteUsers.First() as SocketGuildUser)}님의 뮤트 처리가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
            }
            catch
            {
                await msg.Channel.SendMessageAsync("저런 그분은 음성채팅에 있지 않아요.");
            }
        }

        public async Task kick(SocketGuildUser user, SocketMessage msg)
        {
            var kickUsers = msg.MentionedUsers;

            foreach (var kickUser in  kickUsers)
            {
                await (kickUser as SocketGuildUser).KickAsync();
            }
            Random rd = new Random();
            Program program = new Program();
            if (kickUsers.Count != 1)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{program.getNickname(kickUsers.First() as SocketGuildUser)}외 {kickUsers.Count}분의 킥 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
            else 
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{program.getNickname(kickUsers.First() as SocketGuildUser)}님의 킥 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
        }
        
        public async Task ban(SocketGuildUser user, SocketMessage msg)
        {
            var banUsers = msg.MentionedUsers;

            foreach (var banUser in  banUsers)
            {
                await (banUser as SocketGuildUser).BanAsync();
            }
            Random rd = new Random();
            Program program = new Program();
            if (banUsers.Count != 1)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{program.getNickname(banUsers.First() as SocketGuildUser)}외 {banUsers.Count}분의 밴 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
            else 
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("작업 완료", $"{program.getNickname(banUsers.First() as SocketGuildUser)}님의 밴 처리가 완료되었습니다.");
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
        }
    }
}