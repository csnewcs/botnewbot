using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using botnewbot.Support;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

using botnewbot.Support;
using Newtonsoft.Json.Linq;

namespace bot
{
    ////////////////////////////
    // 여긴 사람들의 순위를 보는 곳 //
    ////////////////////////////
    [Group("순위")]
    public class Rank : ModuleBase<SocketCommandContext>
    {
        Support support;
        public Rank(Support _support)
        {
            support = _support;
        }

        // Dictionary<SocketGuildUser, long> people = new Dictionary<SocketGuildUser, long>();
        // JObject json = new JObject();
        // KeyValuePair<SocketGuildUser, long>[] people;
        List<KeyValuePair<SocketGuildUser, long>> people;

        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("순위 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("나","자기 자신의 순위를 봅니다.\n(사용법: 순위 나)")
            .AddField("모두","서버 내부 사람 전체의 순위를 봅니다.\n(결과는 DM으로 전송됩니다.) (사용법: 순위 모두)")
            .AddField("상위권","서버 내부 사람 상위 5명의 순위를 봅니다.\n(사용법: 순위 상위권)");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("나")]
        public async Task me()
        {
            User user = new User();
            makeDict(Context.Guild);
            sort();
            int rank = 1;
            foreach (var a in people)
            {
                if (Context.User as SocketGuildUser == a.Key) break;
                rank++;
            }
            Random rd = new Random();
            string nickName = user.getNickName(Context.User as SocketGuildUser);
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)))
            .AddField($"{nickName}님의 순위는", $"{rank}등입니다.");
            await ReplyAsync("", embed:builder.Build());
        }

        [Command("모두")]
        public async Task all()
        {
            User user = new User();
            Money money = new Money();
            makeDict(Context.Guild);
            sort();
            Random rd = new Random();
            uint color = (uint)rd.Next(0x000000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}서버의 순위")
            .WithColor(new Color(color));
            int count = 0;
            int fields = 1;
            string users = "";
            foreach (var a in people)
            {
                string nickName = user.getNickName(a.Key); //해당 사람의 닉네임 얻기
                users += $"{count+1}등\n{nickName}: ({money.unit(a.Value)} BNB)\n\n"; 
                count++;

                if (count % 20 == 0 && count != users.Length - 1)
                {
                    builder.AddField($"순위({fields})", users);
                    users = "";
                    fields++;
                    if (fields % 5 == 0)
                    {
                        await Context.User.SendMessageAsync("", embed:builder.Build());
                        builder = new EmbedBuilder()
                        .WithTitle($"{Context.Guild.Name}서버의 순위")
                        .WithColor(new Color(color));
                    }
                }

            }
            if (users != "") builder.AddField($"순위({count})", users);
            await ReplyAsync("DM으로 결과를 전송했습니다.");
            await Context.User.SendMessageAsync("", embed:builder.Build());
        }

        [Command("상위권")]
        public async Task top()
        {
            User user = new User();
            Money money = new Money();
            makeDict(Context.Guild);
            sort();
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}서버의 순위")
            .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)));
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    string nickName = user.getNickName(people[i].Key);
                    builder.AddField($"{i + 1}등", $"{nickName}: {money.unit(people[i].Value)} BNB");
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    builder.AddField($"{i+1}등", "사람이... 읎어요!");
                }
            }
            await ReplyAsync("", embed:builder.Build());
        }
        private void makeDict(SocketGuild guild)
        {
            Money money = new Money();
            var users = guild.Users;
            // int count = 0;
            // foreach (var user in users) if (!user.IsBot) count++;

            people = new List<KeyValuePair<SocketGuildUser, long>>();
            // int index = 0;
            foreach (SocketGuildUser user in users)
            {
                try
                {
                    if (!user.IsBot) people.Add(new KeyValuePair<SocketGuildUser, long>(user, money.getUserMoney(user)));
                }
                catch{}
            }

            // string dirPath = $"servers/{guildId}";
            // DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            // foreach (var fileName in dirInfo.GetFiles())
            // {
                // if (fileName.Name != $"config.json")
                // {
                    // string temp = File.ReadAllText($"{dirPath}/{fileName.Name}");
                    // JToken token = JToken.Parse(temp);
                    // json.Add(fileName.Name, token); //ID: {"money":1234}
                // }
            // }
        }
        private void sort()
        {
            // people =  new KeyValuePair<ulong, long>[people.Count];//rank-1: {ID:MONEY}
            // int i = 0;
            // foreach(var person in people)
            // {
            //     people[i] = new KeyValuePair<ulong, long>(person.Key.Id, (long)person.Value);
            //     i++;
            // }
            for (int i = 1; i < people.Count; i++) //삽입 정렬
            {
                // Console.WriteLine(people[i]);
                for (int j = 0; j  < i; j++)
                {
                    if (people[i].Value > people[j].Value) swap(i, j);
                }
            }
        }
        private void swap(int a, int b)
        {
            var temp = people[a];
            people[a] = people[b];
            people[b] = temp;
        }
    }
}