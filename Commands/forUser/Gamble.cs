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
    ///////////////////
    // 여긴 도박하는 곳 //
    ///////////////////
    [Group("도박")]
    public class Gamble : ModuleBase<SocketCommandContext>
    {
        private readonly Support support;
        public Gamble(Support _support) => support = _support;
        [Command]
        public async Task help()
        {
            EmbedBuilder build = new EmbedBuilder()
            .WithTitle("도박 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("제비뽑기", "1번 ~ 9번 제비를 뽑아 건 돈의 0% ~ 220%를 돌려받습니다.\n(사용법: $도박 제비뽑기 [걸 돈] [선택한 제비 번호])")
            .AddField("슬롯머신", "1번 ~ 9번까지의 랜덤한 숫자 3개가 나옵니다.\n나온 숫자에 의해 건 돈의 0배 ~ 43배를 돌려받습니다.\n(사용법: $도박 슬롯머신 [걸 돈])")
            .AddField("슬롯머신(연속)", "슬롯머신과 같습니다. 단 연속으로(100번까지) 돌립니다.\n결과 중 일부는 DM으로 전송됩니다.\n(사용법: $도박 슬롯머신 [(판당)걸 돈] [돌릴 수(0이면 일반 슬롯머신으로 간주)])");
            await Context.User.SendMessageAsync("", embed:build.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        [Command("제비뽑기")]
        public async Task draw(long money, ulong select) //제비뽑기
        {
            try
            {
                await ReplyAsync($"{money} / {select}");
                if (select < 1 || select > 9)
                {
                    await ReplyAsync("제비는 1~9번까지 있습니다.");
                    return;
                }
                if (money % 10 != 0 || money == 0)
                {
                    await ReplyAsync("10BNB 단위로만 도박이 가능합니다.");
                    return;
                }
                
                if (minusMoney(Context.User as SocketGuildUser, money))
                {
                    await ReplyAsync("가지고 있는 돈 보다 많은 돈을 쓸 수 없습니다.");
                    return;
                }
                int[] multi = new int[] {0, 5, 10, 20, 40, 80, 160, 200, 250};
                Random rd = new Random();
                int temp = 0;
                foreach (int i in multi) //그냥 랜덤으로 한번에 끝내려다가 그래도 제비뽑기니까 섞음
                {
                    int rd1 = rd.Next(0, 9);
                    int rd2 = rd.Next(0, 9);
                    temp = multi[rd1];
                    multi[rd1] = multi[rd2];
                    multi[rd2] = temp;
                }
                long result = money / 100 * (long)multi[select - 1];
                plusMoney(Context.User as SocketGuildUser, result);
                EmbedBuilder builder = new EmbedBuilder()
                .AddField(support.getNickname(Context.User as SocketGuildUser) + "님의 제비뽑기 결과", $"× {multi[select - 1]}%를 뽑으셔서 {support.unit(money)}BNB가 {support.unit(result)}BNB가 되었습니다.")
                .WithColor(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
                await ReplyAsync("", embed:builder.Build());
            }
            catch (Exception e) {await ReplyAsync(e.ToString());}
        }
        [Command("슬롯머신")]
        public async Task slot(long money, uint loop = 0) //슬롯머신
        {
            if (loop != 0)
            {
                await manySlot(money, loop, Context.User as SocketGuildUser, Context.Message);
                return;
            }
            if (money % 10 != 0 || money == 0)
            {
                await ReplyAsync("10BNB 단위로만 도박이 가능합니다.");
                return;
            }
            
            if (minusMoney(Context.User as SocketGuildUser, money))
            {
                await ReplyAsync("가지고 있는 돈 보다 많은 돈을 쓸 수 없습니다.");
                return;
            }
            Random rd = new Random();
            money = money / 10;
            string[] number = new string[9] {":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:" ,":eight:" ,":nine:"};
            int one = rd.Next(0, 9);
            int two = rd.Next(0, 9);
            int three = rd.Next(0, 9);
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle(number[one] + number[two] + number[three])
            .WithColor(rd.Next(0, 256), rd.Next(0, 256), rd.Next(0, 256));
            long result = 0;
            if (one == two && one == three && two == three) //숫자 3개 모두 일치
            {
                if (one == 6) result = money * 300;
                else result = money * (one + 9) * (one + 9); 
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다!", $"숫자 3개를 모두 {one + 1}으로 맞추셨습니다! 거셨던 {support.unit(money * 3)} BNB가 {support.unit(result)} BNB가 되어 돌아갑니다!");
            }
            else if (one == two || one == three) //숫자 2개 일치 (첫번째 숫자가 들어감)
            {
                long avg = (long)(one + two + three + 3);
                if (one == 6) result = money * 45;
                else result = money * (avg + 4); //1 ~ 9배
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {one + 1}으로 맞추셨습니다. 거셨던 {support.unit(money * 3)} BNB가 {support.unit(result)} BNB가 되어 돌아갑니다.");
            }
            else if (two == three) //숫자 2개 일치 (첫번째 숫자가 들어가지 않음)
            {
                long avg = (long)(one + two + three + 3);
                if (two == 6) result = money * 45;
                else result = money * (avg + 2);
                plusMoney(Context.User as SocketGuildUser, result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {two + 1}으로 맞추셨습니다. 거셨던 {support.unit(money * 3)} BNB가 {support.unit(result)} BNB가 되어 돌아갑니다.");
            }
            else
            {
                builder.AddField("저런", $"숫자 3개가 모두 맞지 않습니다. 거셨던 {support.unit(money * 3)} BNB가 소멸 되었습니다.");
            }
            await ReplyAsync("", embed:builder.Build());
        }
        
        private async Task manySlot(long money, uint loop, SocketGuildUser user, SocketMessage msg)
        {
            if (loop > 100)
            {
                await ReplyAsync("100번까지만 연속 슬롯머신이 가능합니다.");
                return;
            }
            if (money % 10 != 0 || money == 0)
            {
                await ReplyAsync("10BNB 단위로만 도박이 가능합니다.");
                return;
            }
            
            if (minusMoney(Context.User as SocketGuildUser, money * loop))
            {
                await ReplyAsync("가지고 있는 돈 보다 많은 돈을 쓸 수 없습니다.");
                return;
            }
            money = money / 10;
            Random rd = new Random();
            
            string[] number = new string[9] {":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:" ,":eight:" ,":nine:"};
            long result = 0;
            string results = "";
            for (int i = 1; i <= loop; i++)
            {
                int one = rd.Next(0, 9);
                int two = rd.Next(0, 9);
                int three = rd.Next(0, 9);
                if (one == two && one == three && two == three) //숫자 3개 모두 일치
                {
                    long temp = 0;
                    if (one == 6) temp = money * 300;
                    else temp = money * (one + 9) * ( one + 9);
                    result += temp;
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit(temp)} BNB)\n";
                }
                else if (one == two || one == three) //숫자 2개 일치 (첫번째 숫자가 들어감)
                {
                    long temp = 0;
                    long avg = (one + two + three + 3);
                    if (one == 6) temp = money * 45;
                    else temp = money * (avg + 3);
                    result += temp;
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit(temp)} BNB)\n";
                }
                else if (two == three) //숫자 2개 일치 (첫번째 숫자가 들어가지 않음)
                {
                    long temp = 0;
                    long avg = (one + two + three+3);
                    if (two == 6) temp = money * 45;
                    else temp = money * (avg + 3);
                    result += temp;
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit(temp)} BNB)\n";
                }
                else
                {
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(실패)\n";
                }
                if (i % 25 == 0 && i != loop) results += ";";
            }
            plusMoney(user, result);
            string log = "";
            money *= 3;
            if (result > money * loop) log = $"{support.unit(result - (money * loop))} BNB 이득";
            else log = $"{support.unit((money * loop) - result)} BNB 손해";
            string[] array = results.Split(';');
            uint color = (uint)rd.Next(0x000000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"{support.getNickname(user)}님의 {loop}번 연속 슬롯머신 결과")
                .WithColor(new Color(color));
            if (array.Length > 1)
            {
                int i = 1;
                foreach (string split in array)
                {
                    builder.AddField($"결과 목록({i})", split);
                    i++;
                }
                builder.AddField("결론", log);
            }
            else
            {
                builder.AddField("결과 목록", results)
                .AddField("결론", log);
            }
            EmbedBuilder serverSend = new EmbedBuilder()
            .WithColor(new Color(color))
            .AddField($"{support.getNickname(user)}님의 {loop}번 연속 슬롯머신 결과", log);
            await msg.Author.SendMessageAsync("", embed:builder.Build());
            await msg.Channel.SendMessageAsync("DM으로 결과를 전송했습니다.", embed:serverSend.Build());
        }
        private void plusMoney(SocketGuildUser user, long plus)
        {
            long money = (long)support.getMoney(user);
            money += plus;
            support.setMoney(user, money);
            // JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            // getUser["money"] = (ulong)getUser["money"] + plus;
            // File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}", getUser.ToString());
        }
        private bool minusMoney(SocketGuildUser user, long minus)
        {
            // JObject getUser = JObject.Parse(File.ReadAllText($"servers/{user.Guild.Id}/{user.Id}"));
            long money = (long)support.getMoney(user);
            if (minus > money)
            {
                return true;
            }
            money -= minus;
            support.setMoney(user, money);
            // getUser["money"] = money - minus;
            // File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}", getUser.ToString());
            return false;
        }
    }
}