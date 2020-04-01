using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace bot
{
    [Group("은행")]
    public class Bank : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("자기 돈 확인하기")]
        public async Task bank()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            JObject json = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            Program program = new Program();
            string nickname = program.getNickname(user);
            string moneyString = program.unit((ulong)json["money"]);
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(rd.Next(0, 256), rd.Next(0, 256), rd.Next(0, 256))
            .AddField(nickname + "님의 통장엔...", moneyString + " BNB가 있습니다.");
            await ReplyAsync("", embed:builder.Build());
        }
    }
}