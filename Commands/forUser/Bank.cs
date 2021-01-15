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
    [Group("은행")]
    public class Bank : ModuleBase<SocketCommandContext>
    {
        private readonly Support support;
        
        public Bank(Support sup)
        {
            support = sup;
        }

        [Command]
        [Summary("자기 돈 확인하기")]
        public async Task bank()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            // JObject json = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            string nickname = support.getNickname(user);
            string moneyString = support.unit(support.getMoney(user));
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(rd.Next(0, 256), rd.Next(0, 256), rd.Next(0, 256))
            .AddField(nickname + "님의 통장엔...", moneyString + " BNB가 있습니다.");
            await ReplyAsync("", embed:builder.Build());
        }
    }
}