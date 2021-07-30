using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

using botnewbot.Handlers;

namespace botnewbot.Commands
{
    public class Bank : ModuleBase<SocketCommandContext>
    {
        private readonly SqlHandler _sql;
        public Bank(SqlHandler sql)
        {
            _sql = sql;
        } 
        [Command("은행")]
        [Alias("bank")]
        public async Task bank()
        {
            var money = _sql.getUserMoney(Context.User.Id, Context.Guild.Id);
            var guildUser = Context.User as SocketGuildUser;
            string nickname = guildUser.Nickname == null ? Context.User.Username : guildUser.Nickname;
            await ReplyAsync($"현재 {nickname}님께는 {money}BNB를 가지고 있어요.");
        }
    }
}