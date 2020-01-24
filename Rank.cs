using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
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
        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("순위 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("나","자기 자신의 순위를 봅니다. (사용법: 순위 나)");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("나")]
        public async Task me()
        {
            string dirPath = $"servers/{Context.Guild.Id}";
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            JObject files = new JObject();
            foreach (var fileName in dirInfo.GetFiles())
            {
                if (fileName.Name != $"config.json")
                {
                    string temp = File.ReadAllText($"{dirPath}/{fileName.Name}");
                    files.Add(fileName.Name, JObject.Parse(temp));
                }
            }
            int rank = 1;
            ulong first = (ulong)files[Context.User.Id.ToString()]["money"];
            foreach (var file in files)
            {
                ulong person = (ulong)file.Value["money"];
                if (person > first)
                {
                    rank++;
                    first = person;
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

        [Command("모두")]
        public async Task all()
        {

        }

        [Command("상위권")]
        public async Task top()
        {
            
        }
    }
}