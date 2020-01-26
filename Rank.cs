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
    [Group("순위")]
    public class Rank : ModuleBase<SocketCommandContext>
    {
        JObject json = new JObject();
        SortedDictionary<int, KeyValuePair<string, JToken>> allRank = new SortedDictionary<int, KeyValuePair<string, JToken>>();

        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("순위 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("나","자기 자신의 순위를 봅니다. (사용법: 순위 나)")
            .AddField("모두","서버 내부 사람 전체의 순위를 봅니다. (결과는 DM으로 전송됩니다.) (사용법: 순위 모두)")
            .AddField("상위권","서버 내부 사람 상위 5명의 순위를 봅니다. (사용법: 순위 상위권)");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("나")]
        public async Task me()
        {
            try
            {
                makeJson(Context.Guild.Id);
                sort();
                KeyValuePair<string, JToken> find = new KeyValuePair<string, JToken>(Context.User.Id.ToString(), json["money"]);
                int rank = 0;
                foreach (var a in allRank)
                {
                    if (a.Value.Key == find.Key)
                    {
                        rank = a.Key;
                        break;
                    }
                }
                Random rd = new Random();
                Program program = new Program();
                string nickName = program.getNickname(Context.User as SocketGuildUser);
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)))
                .AddField($"{nickName}님의 순위는", $"{rank}등입니다.");
                await ReplyAsync("", embed:builder.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            Program program = new Program();
            foreach (var a in allRank)
            {
                string nickName = program.getNickname(Context.Guild.GetUser(ulong.Parse(a.Value.Key)));
                builder.AddField(a.Key + "등", nickName + ": (" + program.unit((ulong)a.Value.Value["money"]) + " BNB)");
                if (a.Key % 25 == 0 && a.Key != allRank.Count)
                {
                    await Context.User.SendMessageAsync("", embed:builder.Build());
                    builder = new EmbedBuilder()
                    .WithTitle($"{Context.Guild.Name}서버의 순위")
                    .WithColor(new Color(color));
                }
            }
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
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
            Program program = new Program();
            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    string nickName = program.getNickname(Context.Guild.GetUser(ulong.Parse(allRank[i].Key)));
                    builder.AddField(i + "등", nickName + ": (" + program.unit((ulong)allRank[i].Value["money"]) + " BNB)");
                }
                catch
                {
                    builder.AddField(i + "등", "사람이... 읎어요!");
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
                    json.Add(fileName.Name, JObject.Parse(temp));
                }
            }
        }
        private void sort()
        {
            foreach (var person in json)
            {
                ulong first = (ulong)person.Value["money"];
                int rank = 1;
                foreach (var file in json)
                {
                    ulong money = (ulong)file.Value["money"];
                    if (money > first)
                    {
                        rank++;
                        first = money;
                    }
                }
                while(true)
                {
                    try
                    {
                        allRank.Add(rank, person);
                        break;
                    }
                    catch {rank++;}
                }
            }
        }
    }
}