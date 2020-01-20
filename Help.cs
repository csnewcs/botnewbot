using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace bot
{
    [Group("명령어")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("기본 명령어 도움말")]
        public async Task help()
        {
            SocketUser user = Context.User; 
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("이 봇이 사용 가능한 명령어")
            .AddField("읎어요!", "???: 영 좋지 않은 타이밍에 누르셨군요.");
            await user.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("관리자")]
        [Summary("관리자 명령어 도움말")]
        public async Task adminHelp()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            Console.WriteLine(user);
            SocketGuild guild = user.Guild;
            JObject config = JObject.Parse(File.ReadAllText($"servers/{guild.Id.ToString()}/config.json"));
            SocketRole adminRole = guild.GetRole(ulong.Parse((config["adminBot"].ToString())));
            Console.WriteLine(adminRole.Name);
            bool isAdmin = false;
            foreach(var role in user.Roles)
            {
                if (role == adminRole) 
                {
                    isAdmin = true; 
                    break;
                }
            }
            if (isAdmin)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("이 봇의 관리자가 사용 가능한 명령어")
                .AddField("읎어요!", "???: 영 좋지 않은 타이밍에 누르셨군요.");
                await user.SendMessageAsync("", embed:builder.Build());
                await ReplyAsync("DM으로 결과를 전송했습니다.");
            }
            else Console.WriteLine("권한 없음");
        }
    }
}