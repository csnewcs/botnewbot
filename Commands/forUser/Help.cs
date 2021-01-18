using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace bot
{
    /////////////////////
    // 여긴 도움말 보는 곳 //
    /////////////////////
    [Group("명령어")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("기본 명령어 도움말")]
        public async Task help()
        {
            var prefix = Program.prefix;
            SocketUser user = Context.User; 
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color(0xbe33ff))
            .WithTitle("이 봇이 사용 가능한 명령어")
            .AddField($"{prefix}은행", "```가지고 있는 돈이 얼마나 있는지 알려줍니다. (단위: BNB)```")
            .AddField($"{prefix}도박", $"```돈을 걸고 간단한 게임을 하는 것입니다. 도박이니 당연히 운입니다.\n자세한 도움말: {prefix}도박```")
            .AddField($"{prefix}순위", $"```서버 내에서 순위를 확인하는 것입니다.\n공동순위일 경우 ID순으로 정렬됩니다.\n자세한 도움말: {prefix}순위```")
            .AddField($"{prefix}노래방", $"이 봇으로 노래를 들을 수 있습니다.\n자세한 도움말: {prefix}노래방")
            .WithUrl("https://github.com/csnewcs/botnewbot/wiki/%EB%AA%85%EB%A0%B9%EC%96%B4")
            .WithFooter("csnewcs 제작");
            await user.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("관리자")]
        [Summary("관리자 명령어 도움말")]
        public async Task adminHelp()
        {           
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xbe33ff))
                .WithTitle("이 봇의 관리자가 사용 가능한 명령어")
                .AddField("역할", "사용자들의 역할을 관리하는 명령어입니다.\n봇의 역할보다 **상위에 있는 사람이나 역할은 건들 수 없으니** 봇의 역할의 위치를 적절히 설정해주세요.\n(자세한 설명은 \"$역할\"을 통해 확인해 주세요.)")
                .AddField("처벌", "유저를 뮤트시키거나 킥하거나 밴시킵니다.\n물론 봇 보다 **상위 유저는 관리할 수 없습니다.**\n(자세한 설명은 \"$처벌\"을 통해 확인해 주세요.)")
                .AddField("처벌해제", "유저의 뮤트나 밴을 해제시킵니다.\n(자세한 설명은 \"$처벌해제\"를 통해 확인해 주세요.)")
                .AddField("초기설정", "봇의 초기 설정을 다시 합니다.\n(사용법: $초기설정)")
                .WithUrl("https://github.com/csnewcs/botnewbot/wiki/%EB%AA%85%EB%A0%B9%EC%96%B4")
                .WithFooter("관리 권한이 없는 멤버는 이 명령어를 쳐도 아무 일도 일어나지 않습니다");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
    }
}