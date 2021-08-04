using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using botnewbot.Handlers;

namespace botnewbot.Commands
{
    public class Draw : ModuleBase<SocketCommandContext>
    {
        private readonly SqlHandler _sql;
        public Draw(SqlHandler sql)
        {
            _sql = sql;
        }

        [Command("제비뽑기")]
        [Alias("draw")]
        public async Task draw(ulong money, byte selected)
        {
            ulong userMoney = _sql.getUserMoney(Context.User.Id, Context.Guild.Id);
            if (userMoney < money)
            {
                await ReplyAsync($"{money}BNB를 걸려고 하셨지만 {userMoney}BNB밖에 없어 도박을 할 수 없어요.");
                return;
            }
            userMoney -= money;
            Random rd = new Random();
            int[] percentages = new int[9]
            {
                0, 5, 10, 20, 40, 80, 160, 200, 240
            };
            for(int i = 0; i < percentages.Length; i++) //shuffle
            {
                int a = rd.Next(0, 9);
                int b = rd.Next(0, 9);
                int temp = percentages[a];
                percentages[a] = percentages[b];
                percentages[b] = temp;
            }
            ulong after = (ulong)Math.Round((double)money / 100 * percentages[selected - 1]);
            userMoney += after;
            _sql.setUserMoney(Context.User.Id, Context.Guild.Id, userMoney);
            await ReplyAsync($"{Context.User.Mention}님의 제비뽑기 결과\n{percentages[selected - 1]}%를 뽑아 {money}BNB가 {after}BNB가 되었어요.");
        }
    }
}