using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace bot
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("도움말")]
        [Summary("이 봇이 사용 가능한 명령어에 대해 알려주는 명령어")]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("이 봇이 사용 가능한 명령어")
            .AddField("도움말", "이 봇이 사용 가능한 명령어들 보여줌.");
            await ReplyAsync("", embed:builder.Build());
        }
    }
}