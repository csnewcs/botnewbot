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
            .WithColor(new Color(0xbe33ff))
            .WithTitle("이 봇이 사용 가능한 명령어")
            .AddField("은행", "가지고 있는 돈이 얼마나 있는지 알려줍니다. (단위: BNB)")
            .AddField ("도박", "돈을 걸고 간단한 게임을 하는 것입니다. 도박이니 당연히 운입니다. (자세한 도움말: $도박)");
            await user.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("관리자")]
        [Summary("관리자 명령어 도움말")]
        public async Task adminHelp()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            SocketGuild guild = user.Guild; //서버 정보 얻기
            JObject config = JObject.Parse(File.ReadAllText($"servers/{guild.Id.ToString()}/config.json")); //그 서버의 설정 파일
            SocketRole adminRole = guild.GetRole(ulong.Parse((config["adminBot"].ToString()))); //그 서버에서 지정한 관리자 검색
            bool isAdmin = false;
            foreach(var role in user.Roles) //그냥 try catch로 바꿀까 고민중... (역할 줬을 때 성공하면 다시 뺏고 리턴 실패하면 작업) (근데 사람 역할을 수백개씩 가지고 있진 않을테니 아마 이대로 갈 듯)
            {
                if (role == adminRole) 
                {
                    isAdmin = true; 
                    break; //그래도 최적화라고 성공하면 나가기
                }
            }
            if (isAdmin)
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xbe33ff))
                .WithTitle("이 봇의 관리자가 사용 가능한 명령어")
                .AddField("역할", "사용자들의 역할을 관리하는 명령어입니다. 봇의 역할보다 상위에 있는 사람이나 역할은 건들 수 없으니 봇의 역할의 위치를 적절히 설정해주세요. (자세한 설명은 \"$역할\"을 통해 확인해 주세요.)");
                await user.SendMessageAsync("", embed:builder.Build());
                await ReplyAsync("DM으로 결과를 전송했습니다.");
            }
        }
    }
}