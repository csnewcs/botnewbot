using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace bot
{
    ////////////////////////////
    // 여긴 사람들의 순위를 보는 곳 //
    ////////////////////////////
    [Group("순위")]
    public class Rank : ModuleBase<SocketCommandContext>
    {
        JObject json = new JObject();
        KeyValuePair<ulong, ulong>[] people;

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
            makeJson(Context.Guild.Id);
            sort();
            int rank = 1;
            foreach (var a in people)
            {
                if (Context.User.Id == a.Key) break;
                rank++;
            }
            Random rd = new Random();
            string nickName = Program.getNickname(Context.User as SocketGuildUser);
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)))
            .AddField($"{nickName}님의 순위는", $"{rank}등입니다.");
            await ReplyAsync("", embed:builder.Build());
        }

        [Command("모두")]
        public async Task all()
        {
            makeJson(Context.Guild.Id);
            sort();
            Random rd = new Random();
            uint color = (uint)rd.Next(0x000000, 0xffffff);
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}서버의 순위")
            .WithColor(new Color(color));
            int count = 0;
            int index = 0;
            string users = "";
            foreach (var a in people)
            {
                string nickName = Program.getNickname(Context.Guild.GetUser(a.Key)); //해당 사람의 닉네임 얻기
                users += $"{count+1}등\n{nickName}: ({Program.unit(a.Value)} BNB)\n\n"; 

                if (index % 20 == 0 && index != users.Length - 1)
                {
                    builder.AddField($"순위({count})", users);
                    users = "";
                    count++;
                    if (count % 20 == 0)
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
            makeJson(Context.Guild.Id);
            sort();
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle($"{Context.Guild.Name}서버의 순위")
            .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)));
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    string nickName = Program.getNickname(Context.Guild.GetUser(people[i].Key));
                    builder.AddField($"{i + 1}등", $"{nickName}: {Program.unit(people[i].Value)} BNB");
                }
                catch
                {
                    builder.AddField($"{i+1}등", "사람이... 읎어요!");
                }
            }
            await ReplyAsync("", embed:builder.Build());
        }
        private void makeJson(ulong guildId)
        {
            json = new JObject();
            string dirPath = $"servers/{guildId}";
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileName in dirInfo.GetFiles())
            {
                if (fileName.Name != $"config.json")
                {
                    string temp = File.ReadAllText($"{dirPath}/{fileName.Name}");
                    json.Add(fileName.Name, JObject.Parse(temp)); //ID: {"money":1234}
                }
            }
        }
        private void sort()
        {
            people =  new KeyValuePair<ulong, ulong>[json.Count];//rank-1: {ID:MONEY}
            int i = 0;
            foreach(var person in json)
            {
                people[i] = new KeyValuePair<ulong, ulong>(ulong.Parse(person.Key), (ulong)person.Value["money"]);
            }
            for (i = 1; i < people.Length; i++) //삽입 정렬
            {
                for (int j = 0; j < i; j++)
                {
                    if (people[i].Value < people[j].Value) swap(i, j);
                }
            }
        }
        private void swap(int a, int b)
        {
            KeyValuePair<ulong, ulong> temp = people[a];
            people[a] = people[b];
            people[b] = temp;
        }
    }
}