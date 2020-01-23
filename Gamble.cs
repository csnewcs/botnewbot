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
            EmbedBuilder build = new EmbedBuilder()
            .WithTitle("역할 설정 명령어")
            .WithColor(new Color(0xbe33ff))
            .AddField("제비뽑기", "1번~8번 제비를 뽑아 건 돈의 0% ~ 640%를 돌려받습니다. (사용법: $도박 제비뽑기 [걸 돈] [선택한 제비 번호])")
            .AddField("슬롯머신", "1번 ~ 9번까지의 랜덤한 숫자 3개가 나옵니다. 나온 숫자에 의해 건 돈의 0% ~ ?%를 돌려받습니다. (사용법: $도박 슬롯머신 [걸 돈])");
            await Context.User.SendMessageAsync("", embed:build.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        [Command("제비뽑기")]
        public async Task draw(ulong money, ulong select) //제비뽑기
        {
            if (select < 1 || select > 8)
            {
                await ReplyAsync("제비는 1~8번까지 있습니다.");
                return;
            }
            if (money % 100 != 0 || money == 0)
            {
                await ReplyAsync("100BNB 단위로만 도박이 가능합니다.");
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
            ulong result = money / 100 * (ulong)multi[select - 1];
            plusMoney(Context.User as SocketGuildUser, result);
            EmbedBuilder builder = new EmbedBuilder()
            .AddField(program.getNickname(Context.User as SocketGuildUser) + "님의 제비뽑기 결과", $"× {multi[select - 1]}%를 뽑으셔서 {money}BNB가 {result}BNB가 되었습니다.")
            .WithColor(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
            await ReplyAsync("", embed:builder.Build());
        }
        [Command("슬롯머신")]
        public async Task slot(ulong money) //슬롯머신
        {
            if (money % 100 != 0 || money == 0)
            {
                await ReplyAsync("100BNB 단위로만 도박이 가능합니다.");
                return;
            }
            Program program = new Program();
            if (minusMoney(Context.User as SocketGuildUser, money))
            {
                await ReplyAsync("가지고 있는 돈 보다 많은 돈을 쓸 수 없습니다.");
                return;
            }
            Random rd = new Random();
            
            string[] number = new string[9] {":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:" ,":eight:" ,":nine:"};
            int one = rd.Next(0, 9);
            int two = rd.Next(0, 9);
            int three = rd.Next(0, 9);
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle(number[one] + number[two] + number[three])
            .WithColor(rd.Next(0, 256), rd.Next(0, 256), rd.Next(0, 256));
            ulong result = 0;
            if (one == two && one == three && two == three) //숫자 3개 모두 일치
            {
                if (one == 7) result = money * 98;
                else result = money * (ulong)one ^ 2; //1 ~ 81배
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다!", $"숫자 3개를 모두 {one + 1}으로 맞추셨습니다! 거셨던 {money} BNB가 {result} BNB가 되어 돌아갑니다!");
            }
            else if (one == two || one == three) //숫자 2개 일치 (첫번째 숫자가 들어감)
            {
                if (one == 7) result = money * 14;
                else result = money * ((ulong)one + 1); //1 ~ 9배
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {one + 1}으로 맞추셨습니다. 거셨던 {money} BNB가 {result} BNB가 되어 돌아갑니다.");
            }
            else if (two == three) //숫자 2개 일치 (첫번째 숫자가 들어가지 않음)
            {
                if (two == 7) result = money * 14;
                else result = money * ((ulong)two + 1);
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {two + 1}으로 맞추셨습니다! 거셨던 {money} BNB가 {result} BNB가 되어 돌아갑니다.");
            }
            else
            {
                builder.AddField("저런", $"숫자 3개가 모두 맞지 않습니다. 거셨던 {money} BNB가 소멸 되었습니다.");
            }
            await ReplyAsync("", embed:builder.Build());
        }
        private void plusMoney(SocketGuildUser user, ulong plus)
        {
            JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            getUser["money"] = (ulong)getUser["money"] + plus;
            File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}", getUser.ToString());
        }
        private bool minusMoney(SocketGuildUser user, ulong minus)
        {
            JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            ulong money = (ulong)getUser["money"];
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