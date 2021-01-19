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
        Support support;
        public Help(Support _support) => support = _support;

        [Command]
        [Summary("기본 명령어 도움말")]
        public async Task help()
        {
            var prefix = Program.prefix;
            SocketUser user = Context.User; 
            EmbedBuilder builder = new EmbedBuilder()
            .WithColor(new Color(0xbe33ff))
            .WithTitle("어떤 명령어에 관한 도움말을 보실건가요?")
            .AddField($"{prefix}은행", "은행에 관한 명령어를 보시려면 :moneybag:")
            .AddField($"{prefix}도박", $"도박에 관한 명령어를 보시려면 :money_with_wings:")
            .AddField($"{prefix}순위", $"순위에 관한 명령어를 보시려면 :trophy:")
            .AddField($"{prefix}노래방", $"노래방에 관한 명령어를 보시려면 :microphone:")
            .AddField("관리자용 명령어", "관리할 때 쓰이는 명령어를 보시려면 :tools:")
            .WithUrl("https://github.com/csnewcs/botnewbot/wiki/%EB%AA%85%EB%A0%B9%EC%96%B4")
            .WithFooter("csnewcs 제작");
            var message = await ReplyAsync("", embed:builder.Build());
            try
            {
                Emoji[] emojis = new Emoji[5] {
                    new Emoji("\U0001f4b0"), new Emoji("\U0001f4b8"), new Emoji("\U0001f3c6"), new Emoji("\U0001f3a4"), new Emoji("\U0001f6e0"), 
                };
                support.helpMessages.Add(message.Id, user.Id);
                await message.AddReactionsAsync(emojis);

            }
            catch (Exception e) {await ReplyAsync(e.ToString());}
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