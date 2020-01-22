using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace bot
{
    [Group("도박")]
    public class Gamble : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {

        }
        [Command("제비뽑기")]
        public async Task draw(uint money, uint select) //제비뽑기
        {
            if (select < 1 || select > 8)
            {
                await ReplyAsync("제비는 1~8번까지 있습니다.");
                return;
            }
            if (money % 100 != 0 || money == 0)
            {
                await ReplyAsync("100BNB 단위로만 제비뽑기가 가능해요");
                return;
            }
            Program program = new Program();
            if (minusMoney(Context.User as SocketGuildUser, money))
            {
                await ReplyAsync("가지고 있는 돈 보다 많은 돈을 쓸 수 없습니다.");
                return;
            }
            int[] multi = new int[] {0, 10, 20, 40, 80, 160, 320, 640};
            Random rd = new Random();
            int temp = 0;
            foreach (int i in multi) //그냥 랜덤으로 한번에 끝내려다가 그래도 제비뽑기니까 섞음
            {
                int rd1 = rd.Next(0, 8);
                int rd2 = rd.Next(0, 8);
                temp = multi[rd1];
                multi[rd1] = multi[rd2];
                multi[rd2] = temp;
            }
            uint result = (uint)(money / 100 * multi[select]);
            plusMoney(Context.User as SocketGuildUser, result);
            EmbedBuilder builder = new EmbedBuilder()
            .AddField(program.getNickname(Context.User as SocketGuildUser) + "님의 제비뽑기 결과", $"× {multi[select]}%를 뽑으셔서 {money}BNB가 {result}BNB가 되었습니다.")
            .WithColor(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
            await ReplyAsync("", embed:builder.Build());
        }
        [Command("슬롯머신")]
        public async Task slot(int money) //슬롯머신
        {

        }
        private void plusMoney(SocketGuildUser user, uint plus)
        {
            JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            getUser["money"] = (uint)getUser["money"] + plus;
            File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}", getUser.ToString());
        }
        private bool minusMoney(SocketGuildUser user, uint minus)
        {
            JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            uint money = (uint)getUser["money"];
            if (minus > money)
            {
                return true;
            }
            getUser["money"] = money - minus;
            File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}", getUser.ToString());
            return false;
        }
    }
}