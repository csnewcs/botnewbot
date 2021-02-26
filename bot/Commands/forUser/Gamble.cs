using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

// using SixLabors.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
// using SixLabors.ImageSharp
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

using Newtonsoft.Json.Linq
;
using csnewcs.Game.GoStop;

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
            .WithColor(new Discord.Color(0xbe33ff))
            .AddField("제비뽑기", "1번 ~ 9번 제비를 뽑아 건 돈의 0% ~ 220%를 돌려받습니다.\n(사용법: $도박 제비뽑기 [걸 돈] [선택한 제비 번호])")
            .AddField("슬롯머신", "1번 ~ 9번까지의 랜덤한 숫자 3개가 나옵니다.\n나온 숫자에 의해 건 돈의 0배 ~ 43배를 돌려받습니다.\n(사용법: $도박 슬롯머신 [걸 돈])")
            .AddField("슬롯머신(연속)", "슬롯머신과 같습니다. 단 연속으로(100번까지) 돌립니다.\n결과 중 일부는 DM으로 전송됩니다.\n(사용법: $도박 슬롯머신 [(판당)걸 돈] [돌릴 수(0이면 일반 슬롯머신으로 간주)])");
            await ReplyAsync("", embed:build.Build());
        }
        [Command("제비뽑기")]
        public async Task draw(long _money, ulong select) //제비뽑기
        {
            double money = (double)_money;
            try
            {
                // await ReplyAsync($"{money} / {select}");
                if (select < 1 || select > 9)
                {
                    await ReplyAsync("제비는 1~9번까지 있습니다.");
                    return;
                }
                if (money < 10)
                {
                    await ReplyAsync("최소 10BNB는 걸어야 합니다.");
                    return;
                }
                
                if (minusMoney(Context.User as SocketGuildUser, (long)money))
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
                long result = (long)Math.Round(money / 100 * (double)multi[select - 1]);
                plusMoney(Context.User as SocketGuildUser, result);
                EmbedBuilder builder = new EmbedBuilder()
                .AddField(support.getNickname(Context.User as SocketGuildUser) + "님의 제비뽑기 결과", $"× {multi[select - 1]}%를 뽑으셔서 {support.unit(_money)}BNB가 {support.unit(result)}BNB가 되었습니다.")
                .WithColor(rd.Next(0, 255), rd.Next(0, 255), rd.Next(0, 255));
                await ReplyAsync("", embed:builder.Build());
            }
            catch (Exception e) {await ReplyAsync(e.ToString());}
        }
        [Command("슬롯머신")]
        public async Task slot(long _money, uint loop = 0) //슬롯머신
        {
            if (_money < 10)
            {
                await ReplyAsync("최소 10BNB를 걸어야 합니다.");
            }
            double money = (double)_money;
            if (loop != 0)
            {
                await manySlot(money, loop, Context.User as SocketGuildUser, Context.Message);
                return;
            }
            
            if (minusMoney(Context.User as SocketGuildUser, _money))
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
            double result = 0;
            result = Math.Round(result);
            if (one == two && one == three && two == three) //숫자 3개 모두 일치
            {
                if (one == 6) result = (long)Math.Round(money * 300);
                else result = money * (one + 9) * (one + 9); 
                plusMoney(Context.User as SocketGuildUser, (long)result);
                builder.AddField("축하 드립니다!", $"숫자 3개를 모두 {one + 1}으로 맞추셨습니다! 거셨던 {support.unit(_money)} BNB가 {support.unit((long)result)} BNB가 되어 돌아갑니다!");
            }
            else if (one == two || one == three) //숫자 2개 일치 (첫번째 숫자가 들어감)
            {
                long avg = (long)(one + two + three + 3);
                if (one == 6) result = money * 45;
                else result = money * (avg + 4); //1 ~ 9배
                plusMoney(Context.User as SocketGuildUser, (long)result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {one + 1}으로 맞추셨습니다. 거셨던 {support.unit(_money )} BNB가 {support.unit((long)result)} BNB가 되어 돌아갑니다.");
            }
            else if (two == three) //숫자 2개 일치 (첫번째 숫자가 들어가지 않음)
            {
                long avg = (long)(one + two + three + 3);
                if (two == 6) result = money * 45;
                else result = money * (avg + 2);
                plusMoney(Context.User as SocketGuildUser, (long)result);
                builder.AddField("축하 드립니다.", $"숫자 2개를 {two + 1}으로 맞추셨습니다. 거셨던 {support.unit(_money )} BNB가 {support.unit((long)result)} BNB가 되어 돌아갑니다.");
            }
            else
            {
                builder.AddField("저런", $"숫자 3개가 모두 맞지 않습니다. 거셨던 {support.unit(_money)} BNB가 소멸 되었습니다.");
            }
            await ReplyAsync("", embed:builder.Build());
        }
        
        private async Task manySlot(double money, uint loop, SocketGuildUser user, SocketMessage msg)
        {
            if (money <  10)
            {
                await ReplyAsync("최소 10BNB를 걸어야 합니다.");
                return;
            }
            if (loop > 100)
            {
                await ReplyAsync("100번까지만 연속 슬롯머신이 가능합니다.");
                return;
            }
            
            if (minusMoney(Context.User as SocketGuildUser, (long)Math.Round(money * loop)))
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
                    double temp = 0;
                    if (one == 6) temp = money * 300;
                    else temp = (long)money * (one + 9) * ( one + 9);
                    result += (long)Math.Round(temp);
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit((long)Math.Round(temp))} BNB)\n";
                }
                else if (one == two || one == three) //숫자 2개 일치 (첫번째 숫자가 들어감)
                {
                    double temp = 0;
                    long avg = (one + two + three + 3);
                    if (one == 6) temp = money * 45;
                    else temp = money * (avg + 3);
                    result += (long)Math.Round(temp);
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit((long)Math.Round(temp))} BNB)\n";
                }
                else if (two == three) //숫자 2개 일치 (첫번째 숫자가 들어가지 않음)
                {
                    double temp = 0;
                    long avg = (one + two + three+3);
                    if (two == 6) temp = money * 45;
                    else temp = money * (avg + 3);
                    result += (long)Math.Round(temp);
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(+ {support.unit((long)Math.Round(temp))} BNB)\n";
                }
                else
                {
                    results += $"{i}번째 결과: {number[one]}{number[two]}{number[three]}(실패)\n";
                }
                if (i % 25 == 0 && i != loop) results += ";";
            }
            plusMoney(user, result);
            string log = "";
            
            if (result > money * 10 * loop) log = $"{support.unit(result - ((long)money * loop * 10))} BNB 이득";
            else log = $"{support.unit(((long)money * 10 * loop) - result)} BNB 손해";

            string[] array = results.Split(';');
            uint color = (uint)rd.Next(0x000000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"{support.getNickname(user)}님의 {loop}번 연속 슬롯머신 결과")
                .WithColor(new Discord.Color(color));
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
            .WithColor(new Discord.Color(color))
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

        [Command("고스톱")]
        public async Task goStop(string type)
        {
            try
            {
                EmbedBuilder builder = new EmbedBuilder();
                Random rd = new Random();
                builder.WithColor(new Discord.Color((uint)rd.Next(0, 0xffffff)));
                SocketGuildChannel channel = Context.Channel as SocketGuildChannel;
                SocketGuild guild = Context.Guild;

                if (type == "시작")
                {
                    if (support.goStopGame.ContainsKey(Context.Channel as SocketGuildChannel))
                    {
                        Console.WriteLine(support.goStopGame[Context.Channel as SocketGuildChannel].players[0].id);
                        builder.AddField("저런", "이미 게임이 진행중이에요. 나중에 다시 시도해주세요.");
                    }
                    else if(support.tempUsers.ContainsKey(Context.Channel as SocketGuildChannel))
                    {
                        builder.AddField("저런", $"이미 시작을 했어요 '{Context.Message.Content[0]}도박 고스톱 참가' 로 게임에 참가해주세요!");
                    }
                    else
                    {
                        System.Collections.Generic.List<ulong> list = new System.Collections.Generic.List<ulong>();
                        list.Add(Context.User.Id);
                        support.tempUsers.Add(Context.Channel as SocketGuildChannel, list);
                        builder.AddField("성공", $"게임이 10초 후 시작됩니다. 참가하실 분들은 '{Context.Message.Content[0]}도박 고스톱 참가'로 게임에 참가해주세요.");
                        new Thread(() => countdown(channel)).Start();
                    }
                }
                else if (type == "참가")
                {
                    if (support.goStopGame.ContainsKey(channel))
                    {
                        builder.AddField("저런", "이미 게임이 진행중이에요. 다른 게임이 시작하면 다시 시도해주세요.");
                    }
                    else if(!support.tempUsers.ContainsKey(channel))
                    {
                        builder.AddField("저런", $"아직 게임 시작을 하지 않았어요 '{Context.Message.Content[0]}도박 고스톱 시작'으로 먼저 게임을 시작해주세요.");
                    }
                    else if (support.tempUsers[channel].Count > 3)
                    {
                        builder.AddField("저런", "이미 사람이 꽉 찼어요. 나중에 다시 시도해주세요.");
                    }
                    else if (support.tempUsers[channel].Contains(Context.User.Id))
                    {
                        builder.AddField("저런", "이미 게임에 참가하셨어요.");
                    }
                    else
                    {
                        support.tempUsers[channel].Add(Context.User.Id);
                        builder.AddField("성공", "게임에 참가하셨습니다. 게임이 시작될 때 까지 잠시만 기다려주세요.");
                    }
                }
               await ReplyAsync("", false, builder.Build());
            }
            catch(Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
        public async Task startGoStop(SocketGuildChannel channel)
        {
            try
            {
                Random rd = new Random();
                EmbedBuilder builder = new EmbedBuilder().WithColor((uint)rd.Next(0, 0xffffff));
                SocketTextChannel textChannel = (SocketTextChannel)channel;

                var players = support.tempUsers[channel];
                
                if (players.Count < 2)
                {
                    builder.AddField("실패!", "사람이 너무 없어요. 적어도 2명 이상이 플레이 해야 합니다!");
                    await textChannel.SendMessageAsync("", false, builder.Build());
                    support.tempUsers.Remove(channel);
                    return;
                }
                else if(players.Count > 4)
                {
                    builder.AddField("실패!", "사람이 너무 많아요. 최대 4명 까지 한 번에 플레이가 가능해요.");
                    await textChannel.SendMessageAsync("", false, builder.Build());
                    support.tempUsers.Remove(channel);
                    return;
                }

                 Discord.Rest.RestTextChannel createChannel = null;
                try
                {
                    createChannel = await Context.Guild.CreateTextChannelAsync($"{DateTime.Now}고스톱 채널", c => c.Position = channel.Position - 1);
                    OverwritePermissions everyone = OverwritePermissions.DenyAll(createChannel).Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow, addReactions: PermValue.Allow);
                    OverwritePermissions playersAndBot = everyone.Modify(sendMessages: PermValue.Allow, attachFiles: PermValue.Allow, embedLinks: PermValue.Allow);

                    await createChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, everyone);
                    await createChannel.AddPermissionOverwriteAsync(Context.Guild.GetUser(Context.Client.CurrentUser.Id), playersAndBot);

                    foreach (var player in support.tempUsers[channel])
                    {
                        await createChannel.AddPermissionOverwriteAsync(Context.Guild.GetUser(player), playersAndBot);
                    }
                }
                catch
                {
                    await textChannel.SendMessageAsync("새 채널을 만들고 권한을 설정하는 데 실패햐였습니다. 봇의 권한을 확인해 주세요");
                    support.goStopGame.Remove(channel);
                    return;
                }

                channel = Context.Guild.GetChannel(createChannel.Id);
                support.goStopGame.Add(channel, new GoStop(support.tempUsers[Context.Channel as SocketGuildChannel].ToArray()));

                Player first = support.goStopGame[channel].turn;
                SocketGuildUser firstTrun = channel.Guild.GetUser(first.id);

                support.turnPlayer.Add(firstTrun.Id, channel);
                SocketGuild guild = channel.Guild;
                string path = $"GoStop/{channel.Id}/";
                DirectoryInfo dtInfo = new DirectoryInfo("GoStop/" + channel.Id + "/");
                if (dtInfo.Exists)
                {
                    foreach (var file in dtInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                {
                    dtInfo.Create();
                }
                
                foreach(var player in players)
                {
                    string filePath = path + player + ".png";
                    support.goStopGame[channel].getPlayer(player).getHwatusImage().Save(filePath, new PngEncoder());
                    await guild.GetUser(player).SendFileAsync(filePath, "당신의 패입니다.");
                    GC.Collect();
                }
                
                string send = "당신의 차례입니다. 아래 목록에서 낼 것을 골라 번호를 입력하세요. \n```";
                int index = 1;
                foreach(var hwatu in first.hwatus)
                {
                    send += $"{index}: {hwatu.toKR()}\n";
                    index++;
                }
                send += "```";
                support.goStopGame[channel].Field.getFieldImage().Save(path + "field.png", new PngEncoder());
                
                textChannel = channel as SocketTextChannel;

                await textChannel.SendFileAsync(path + "field.png", "게임 시작!");
                await firstTrun.SendFileAsync($"{path}/field.png");
                await firstTrun.SendMessageAsync(send);                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            // await ((SocketTextChannel)channel).SendMessageAsync("", false, builder.Build());
        }
        async void countdown(SocketGuildChannel channel)
        {
            await Task.Delay(10000);
            await startGoStop(channel);
        }
    }
}