using System;
using System.Net;
using System.Text;
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
        Dictionary<ulong, Playlist> servers = new Dictionary<ulong, Playlist>();
        
        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("노래방 명령어 도움말")
            .AddField("시작", "봇을 음성채팅방으로 초대합니다. 가장 먼저 이것을 먼저 해야 다른 노래방 명령어를 사용 가능합니다.")
            .AddField("등록 [검색어나 URL]", "노래를 재생목록에 추가합니다. Youtube와 Spotify에서 검색합니다. (Spotify가 우선)")
            .AddField("등록 [y/s] [검색어나 URL]", "노래를 재생목록에 추가합니다. Youtube와 Spotify에서 검색합니다. (y는 Youtube, s는 Spotify)")
            .AddField("재생", "등록된 노래들을 재생합니다.")
            .AddField("정지", "재생중인 노래를 일지정지합니다.")
            .AddField("다음", "다음 노래로 넘깁니다.")
            .AddField("초기화", "재생목록의 모든 곡을 삭제합니다.")
            .AddField("종료", "노래방을 종료합니다. 재생목록이 사라집니다.")
            .WithColor(new Color(0xbe33ff));
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }
        [Command("시작")]
        public async Task start()
        {
            try
            {
                SocketGuildUser user = Context.User as SocketGuildUser;
                
                SocketVoiceChannel channel = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id);
                await ReplyAsync("음성채팅방으로 들어갔습니다.");
                await channel.ConnectAsync(false, false, true);
            }
            catch
            {
                await ReplyAsync("먼저 음성채팅방에 들어가고 시작해주세요.");
            }
        }

        [Command("종료")]
        public async Task end()
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
                SocketVoiceChannel channel = Context.Guild.GetVoiceChannel(user.VoiceChannel.Id);
                await channel.DisconnectAsync();
                await ReplyAsync("음성채팅방에서 떠났습니다.");
            }
            catch
            {
                await ReplyAsync("아직 들어간 음성채팅방이 없습니다.");
            }
        }
    }
    struct Playlist
    {
        SocketGuildChannel channel;
        List<string> playlist;
        public Playlist(SocketGuildChannel soundChannel)
        {
            channel = soundChannel;
            playlist = new List<string>();
        }
        public bool Add(string urlOrSearch, out string title)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            try
            {
                string download = client.DownloadString(urlOrSearch);
                playlist.Add(download);
            }
            catch
            {

            }
            title = "";
            return false;
            
        }
    }    
}