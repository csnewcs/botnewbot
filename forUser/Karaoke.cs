using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Audio;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using Lavalink4NET;
using Lavalink4NET.DiscordNet;


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
            .AddField("등록 [검색어나 URL]", "노래를 재생목록에 추가합니다. Youtube에서 검색합니다.")
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
                if (allPlaylist.addPlaylist(Context.Guild.Id, channel))
                {
                    await ReplyAsync("이미 등록되어 있습니다");
                    return;
                }
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

        [Command("등록", true)]
        public async Task add()
        {
            string title = "";
            string[] split = Context.Message.Content.Split(' ');
            string urlOrSearch = "";
            string imageUrl = "";
            for (int i = 2; i < split.Length; i++)
            {
                urlOrSearch += split[i] + " ";
            }
            if (allPlaylist.addMusic(Context.Guild.Id, urlOrSearch, out title, out imageUrl))
            {
                Random random = new Random();
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)random.Next(0x000000, 0xffffff))
                .AddField(title + " 추가 완료", Program.getNickname(Context.User as SocketGuildUser) + "님에 의해 노래 추가 완료")
                .WithImageUrl(imageUrl);
                
                await ReplyAsync("", embed: builder.Build());
            }
            else
            {
                await ReplyAsync("실패");
            }
        }
        [Command("재생")]
        public async Task play()
        {
            var audioService = new LavalinkNode(new LavalinkNodeOptions(), new DiscordClientWrapper(Context.Client));
            audioService.InitializeAsync();
            var asdf = audioService.GetTrackAsync("");
        }
    }
    static class allPlaylist
    {
        static ConcurrentDictionary<ulong, Playlist> servers = new ConcurrentDictionary<ulong, Playlist>();
        public static bool addPlaylist(ulong guildId, SocketVoiceChannel channel)
        {
            return !servers.TryAdd(guildId, new Playlist(channel, guildId));
        }
        public static bool addMusic(ulong guildId, string urlOrSearch, out string title, out string url)
        {
            return servers[guildId].Add(urlOrSearch, out title, out url);
        }
    }
    struct Playlist
    {
        ulong guildId;
        SocketGuildChannel channel;
        List<string> playlist;
        public Playlist(SocketGuildChannel soundChannel, ulong id)
        {
            channel = soundChannel;
            playlist = new List<string>();
            guildId = id;
        }
        public bool Add(string urlOrSearch, out string title, out string imageUrl)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer() {
            ApiKey = System.IO.File.ReadAllLines("config.txt")[1],
            ApplicationName = "botnewbot"
            });
            bool re = false;
            string tempTitle = "";
            string tempUrl = "";
            // 유튜브 검색 설정
            tempTitle = "Youtube";
            var listRequest = youtubeService.Search.List("snippet");
            listRequest.MaxResults = 50;
            listRequest.Q = urlOrSearch;
            try
            {
                // 검색
                var searchResult = listRequest.Execute();
                foreach (var search in searchResult.Items)
                {
                    if (search.Id.Kind == "youtube#video")
                    {
                        tempTitle = search.Snippet.Title;
                        tempUrl = search.Snippet.Thumbnails.Default__.Url;
                        playlist.Add(search.Id.VideoId.ToString());
                        break;
                    }
                    // else if (search.Id.Kind == "youtube#playlist")
                    // {
                    //     tempTitle = search.Snippet.Title;
                    //     tempUrl = search.Snippet.Thumbnails.Default__.Url;
                    // }
                }
                re = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            imageUrl = tempUrl;
            title = tempTitle;
            return re;
        }
        public async Task playMusic(DiscordSocketClient client)
        {
            

        }
    }    
}