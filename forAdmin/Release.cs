using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Discord;
using Discord.Commands;
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
            .AddField("뮤트", "지정한 사람의 뮤트를 해제합니다. (사용법: $처벌해제 뮤트 [뮤트를 풀 사용자 언급(여러 명 가능)])")
            .AddField("밴", "지정한 사람의 밴을 해제합니다. (사용법: $처벌해제 밴 [밴을 풀 사용자 언급(여러 명 가능)])");
            await user.SendMessageAsync("", embed:builder.Build());
            await msg.Channel.SendMessageAsync("DM으로 결과를 전송했습니다.");
        }
    }
}