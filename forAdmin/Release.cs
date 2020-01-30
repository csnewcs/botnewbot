using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Discord;
using Discord.WebSocket;

namespace bot
{
    class Release
    {
        public async Task help(SocketGuildUser user, SocketMessage msg)
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("처벌해제 명령어 사용법")
            .WithColor(new Color(0xbe33ff))
            .AddField("뮤트", "지정한 사람의 뮤트를 해제합니다.\n(사용법: $처벌해제 뮤트 [뮤트를 풀 사용자 언급(여러 명 가능)])")
            .AddField("뮤트 모두", "모든 사람의 뮤트를 해제합니다.\n(사용법: $처벌해제 뮤트 모두)")
            .AddField("밴 목록", "이 서버에서 밴 당한 사람들의 목록을 보여줍니다.\n결과는 DM으로 전송됩니다.\n(사용법: $처벌해제 밴)")
            .AddField("밴", "지정한 사람의 밴을 해제합니다.\n(밴 목록으로 그 사람의 ID를 알아와주세요.)\n(사용법: $처벌해제 밴 [밴을 풀 사람의 ID])")
            .AddField("밴 모두", "모든 사람의 밴을 해제합니다.\n(사용법: $처벌해제 밴 모두)");
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
                    await (muteUser as SocketGuildUser).ModifyAsync(m => {m.Mute = false;});
                }
                if (muteUsers.Count != 1)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{program.getNickname(muteUsers.First() as SocketGuildUser)}외 {muteUsers.Count}분의 뮤트 해제가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
                else 
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor((uint)rd.Next(0x000000, 0xffffff))
                    .AddField("작업 완료", $"{program.getNickname(muteUsers.First() as SocketGuildUser)}님의 뮤트 해제가 완료되었습니다.");
                    await msg.Channel.SendMessageAsync("", embed:builder.Build());
                }
            }
            catch
            {
                await msg.Channel.SendMessageAsync("저런 그분은 음성채팅에 있지 않아요.");
            }
        }
        public async Task ban(SocketGuild guild, SocketMessage msg, string[] split)
        {
            if (split.Length == 2)
            {
                await banList(guild, msg);
                return;
            }
            else
            {
                if (split[2] == "모두")
                {
                    await allBan(guild, msg);
                    return;
                }
                ulong id = ulong.Parse(split[2]);
                var bannedUser = guild.GetBanAsync(id).Result;
                await guild.RemoveBanAsync(id);
                Random rd = new Random();
                EmbedBuilder builder = new EmbedBuilder()
                .AddField("작업 완료", $"{bannedUser.User.Username}님의 밴 해제가 완료되었습니다.")
                .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)));
                await msg.Channel.SendMessageAsync("", embed:builder.Build());
            }
        }
        private async Task banList(SocketGuild guild, SocketMessage msg)
        {
            int index = 1;
            var getBans = guild.GetBansAsync();
            Random rd = new Random();
            uint color = (uint)rd.Next(0x00000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color(color));
            foreach (var a in getBans.Result)
            {
                builder.AddField(a.User.Username, $"유저ID: {a.User.Id}\n이유: {a.Reason}");
                index++;
                if (index % 20 == 0 && index != getBans.Id)
                {
                    await msg.Author.SendMessageAsync("", embed:builder.Build());
                    builder = new EmbedBuilder()
                    .WithColor(new Color(color));
                }
            }
            await msg.Channel.SendMessageAsync("DM으로 결과를 전송했습니다.");
            await msg.Author.SendMessageAsync("", embed:builder.Build());
        }
        private async Task allBan(SocketGuild guild, SocketMessage msg)
        {
            var banPeople = guild.GetBansAsync().Result;
            foreach (var banPerson in banPeople)
            {
                await guild.RemoveBanAsync(banPerson.User.Id);
            }
            Random rd = new Random();
            uint color = (uint)rd.Next(0x000000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color(color))
            .AddField("밴 해제 완료", $"{banPeople.First().User.Username}외 {banPeople.Count - 1}분의 밴 해제가 완료되었습니다.");
            await msg.Channel.SendMessageAsync("", embed:builder.Build());
        }
    }
}