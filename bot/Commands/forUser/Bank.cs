using System;
using System.IO;
using System.Threading.Tasks;
using botnetbot.Support;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using botnewbot.Support;
using Newtonsoft.Json.Linq;

namespace bot
{
    ///////////////////////
    // 여기는 은행 관련된 곳 //
    ///////////////////////
    [Group("은행")]
    public class Bank : ModuleBase<SocketCommandContext>
    {
        private Money _money;
        
        
        public Bank(Money money)
        {
            _money = money;
        }

        [Command]
        [Summary("자기 돈 확인하기")]
        public async Task bank()
        {
            User user = new User();
            SocketGuildUser guildUser = Context.User as SocketGuildUser;
            // JObject json = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            string nickname = user.getNickName(guildUser);
            string moneyString = _money.unit(_money.getUserMoney(guildUser));
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(rd.Next(0, 256), rd.Next(0, 256), rd.Next(0, 256))
            .AddField(nickname + "님의 통장엔...", moneyString + " BNB가 있습니다.");
            await ReplyAsync("", embed:builder.Build());
        }
    }
}