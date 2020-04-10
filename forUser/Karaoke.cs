using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace bot
{
    [Group("노래방")]
    public class Karaoke : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("노래방 명령어 도움말")
            .AddField("시작", "봇을 음성채팅방으로 초대합니다. 가장 먼저 이것을 먼저 해야 다른 노래방 명령어를 사용 가능합니다.")
            .AddField("등록 [검색어나 URL]", "노래를 재생목록에 추가합니다. Youtube와 Spotify에서 검색합니다.")
            .AddField("재생", "등록된 노래들을 재생합니다.")
            .AddField("정지", "재생중인 노래를 일지정지합니다.")
            .AddField("다음", "다음 노래로 넘깁니다.")
            .AddField("초기화", "재생목록의 모든 곡을 삭제합니다.")
            .AddField("종료", "노래방을 종료합니다. 재생목록이 사라집니다.")
            .WithColor(new Color(0xbe33ff));
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        struct ServerSoundList
        {
            SocketGuildChannel channel;
            string search;
            List<string> playlist;
            public ServerSoundList(SocketGuildChannel soundChannel, string searchText)
            {
                channel = soundChannel;
                search = searchText;
                playlist = new List<string>();
            }
            public void clear()
            {
                search = "";
                playlist.Clear();
            }
        }
    }
}