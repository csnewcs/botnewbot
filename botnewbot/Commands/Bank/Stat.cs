using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using botnewbot.Handlers;

namespace botnewbot.Commands
{
    public class Stat : ModuleBase<SocketCommandContext>
    {
        private readonly SqlHandler _sqlHandler;
        public Stat(SqlHandler sqlHandler)
        {
            _sqlHandler = sqlHandler;
        }
        [Command("통계")]
        [Alias("stat")]
        public async Task stat()
        {
            Dictionary<ulong, ulong> money = new Dictionary<ulong, ulong>();
            uint allUserCount = 0;
            uint thisUserCount = 0;
            // ulong thisGuild;
            ulong allGuildsAllMoney = 0;
            ulong thisGuildAllMoney = 0;
            var guilds = Context.Client.Guilds;
            foreach(var guild in guilds)
            {
                if(_sqlHandler.getAllMoneyFromOneGuild(guild.Id, out money))
                {
                    ulong thisGuild = 0;
                    foreach(var onemoney in money)
                    {
                        thisGuild += onemoney.Value;
                    }
                    allGuildsAllMoney += thisGuild;
                    allUserCount += (uint)money.Count;
                    if(guild.Id == Context.Guild.Id) 
                    {
                        thisGuildAllMoney = thisGuild;
                        thisUserCount = (uint)money.Count;
                    }
                }
            }
            ulong allGuildsAvgMoney = allGuildsAllMoney / allUserCount;
            ulong thisGuildAvgMoney = thisGuildAllMoney / thisUserCount;
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("통계")
            .WithTimestamp(DateTime.Now)
            .AddField("현재 시중에 있는 모든 돈", $"{allGuildsAllMoney}BNB")
            .AddField("현재 모든 사용자가 가진 돈의 평균", $"{allGuildsAvgMoney}BNB")
            .AddField("현재 이 서버의 모든 돈", $"{thisGuildAllMoney}BNB")
            .AddField("현재 이 서버의 사용자들이 가진 돈의 평균", $"{thisGuildAvgMoney}BNB");
            await ReplyAsync("", embed: builder.Build());
        }
    }
}