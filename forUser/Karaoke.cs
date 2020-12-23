using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Microsoft.Extensions.DependencyInjection;


namespace bot
{
    [Group("노래방")]
    public class Karaoke : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public Karaoke(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("노래방 명령어 도움말")
            .AddField("들어와", "사용자가 있는 음성채팅방에 봇이 들어갑니다. 음악을 등록하기 전에 해야합니다.")
            .WithColor(new Color(0xbe33ff));
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("들어와")]
        public async Task JoinAsync() 
        {
            Console.WriteLine("명령어 인식");
            if (_lavaNode.HasPlayer(Context.Guild)) 
            {
                await ReplyAsync("이미 다른 음성 채널에 들어가 있습니다.");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null) 
            {
                await ReplyAsync("먼저 음성채팅방에 들어가 주세요.");
                return;
            }

            try 
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"{voiceState.VoiceChannel.Name}에 들어가는데 성공했습니다!");
            }
            catch (Exception exception) 
            {
                await ReplyAsync( "에러\n```"+ exception.Message + "```");
            }
        }
    }
}