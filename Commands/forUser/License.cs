using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace bot
{
    ///////////////////////
    // 여기는 은행 관련된 곳 //
    ///////////////////////
    [Group("라이센스")]
    public class License : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("이 봇의 라이센스 및 이 봇 제작시 사용된 프로그램들의 라이센스")]
        public async Task license()
        {
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("이 봇과 봇에 사용된 Nuget들의 라이센스")
            .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)))
            .AddField("botnewbot (이 봇)", "라이센스: MIT, 소스코드: https://github.com/csnewcs/botnewbot")
            .AddField("Discord.Net", "라이센스: MIT, 소스코드: https://github.com/discord-net/Discord.Net")
            .AddField("Newtonsoft.Json", "라이센스: MIT, 소스코드: https://github.com/JamesNK/Newtonsoft.Json")
            .AddField("Lavalink4NET.Discord.Net", "라이센스: MIT, 소스코드: https://github.com/angelobreuer/Lavalink4NET");
            await ReplyAsync("", embed: builder.Build());
        }
    }
}