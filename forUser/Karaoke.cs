using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Rest;
using Lavalink4NET.Player;
using Lavalink4NET.DiscordNet;


namespace bot
{
    [Group("노래방")]
    public class Karaoke : ModuleBase<SocketCommandContext>
    {
        LavalinkNode audioService;
        
        [Command]
        public async Task help()
        {
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("노래방 명령어 도움말")
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

        [Command("종료")]
        public async Task disconnect()
        {
            var player = await GetPlayerAsync(Context.Guild, Context.User.Id, Context.Client);
            
            if (player == null)
            {
                return;
            }
            await player.StopAsync(true);
            await ReplyAsync("종료됨");
        }
        
        [Command("등록")]
        public async Task play([Remainder]string urlOrSearch)
        {
            Console.WriteLine(Context.Guild.Id);   
            try
            {

                var player = await GetPlayerAsync(Context.Guild, Context.User.Id, Context.Client);
                if (player == null)
                {
                    return;
                }
                
                var track = await audioService.GetTrackAsync(urlOrSearch, SearchMode.YouTube);
                if (track == null)
                {
                    await ReplyAsync("결과가 없습니다. URL이나 검색어를 바꿔주세요");
                    return;
                }
                var position = await player.PlayAsync(track, enqueue: true);
                Random rd = new Random();
                if (position == 0)
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .AddField("노래 재생 시작", Program.getNickname(Context.User as SocketGuildUser) + "님에 의해 " + track.Title + " 재생 시작")
                    .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)));
                }
                else
                {
                    EmbedBuilder builder = new EmbedBuilder()
                    .AddField("노래를 재생목록에 추가", Program.getNickname(Context.User as SocketGuildUser) + "님에 의해 " + track.Title + "을 재생 목록에 추가")
                    .WithColor(new Color((uint)rd.Next(0x000000, 0xffffff)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        
        [Command("정지")]
        public async Task stop()
        {
            var player = await GetPlayerAsync(Context.Guild, Context.User.Id, Context.Client);
            await player.PauseAsync();
            await ReplyAsync("일시정지 완료");
        }

        [Command("재생")]
        public async Task restart()
        {
            var player = await GetPlayerAsync(Context.Guild, Context.User.Id, Context.Client);
            await player.ReplayAsync();
            await ReplyAsync("재생 시작");
        }
        
        private async Task startTrack(object sender, Lavalink4NET.Events.TrackStartedEventArgs track)
        {
            var channel = Channels.getChannel(track.Player.GuildId);
            await ReplyAsync(track.Player.CurrentTrack.Title + "재생 시작");
        }

        private async Task<VoteLavalinkPlayer> GetPlayerAsync(SocketGuild guild, ulong userId, DiscordSocketClient client, bool connectToVoiceChannel = true)
        {
            Console.WriteLine(guild.Id);
            VoteLavalinkPlayer player = null;
            try
            {
                player = audioService.GetPlayer<VoteLavalinkPlayer>(guild.Id);
            }
            catch
            {
                audioService = new LavalinkNode(new LavalinkNodeOptions{
                    Password = "youshallnotpass",
                    DisconnectOnStop = false,
                    BufferSize = 1024 * 1024,
                    RestUri = "http://localhost:8080",
                    WebSocketUri = "ws://localhost:8080"
                }, new DiscordClientWrapper(client));
                audioService.TrackStarted += startTrack;
                await audioService.InitializeAsync();
                player = audioService.GetPlayer<VoteLavalinkPlayer>(guild.Id);
                
            }

            if (player != null && player.State != PlayerState.NotConnected && player.State != PlayerState.Destroyed)
            {
                return player;
            }

            var user = guild.GetUser(Context.User.Id);
            Console.WriteLine(user.VoiceChannel.Id);
            return await audioService.JoinAsync<VoteLavalinkPlayer>(guild.Id, user.VoiceChannel.Id);
        }
        static class Channels
        {
            static Dictionary<ulong, SocketGuildChannel> dict = new Dictionary<ulong, SocketGuildChannel>();
            public static void addChannel(SocketGuildChannel ch)
            {
                dict.Add(ch.Guild.Id, ch);
            }
            public static SocketGuildChannel getChannel(ulong guild)
            {
                return dict[guild];
            }
        }
    }
}