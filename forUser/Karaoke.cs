using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Rest;


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
            .AddField("등록 (검색어|주소)", "유튜브에서 검색해 음악을 등록합니다. 검색어 대신 주소를 입력해 등록할 수도 있습니다.")
            .AddField("볼륨 (0~100)", "음악의 볼륨을 설정합니다. 0%~100% 사이로 설정할 수 있습니다.")
            .AddField("나와", "들어가 있던 음성채팅방에서 나옵니다.")
            .WithColor(new Color(0xbe33ff));
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");
        }

        [Command("들어와")]
        public async Task JoinAsync() 
        {
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
                // if (Context.Client.)
                await ReplyAsync($"{voiceState.VoiceChannel.Name}에 들어가는데 성공했습니다!");
            }
            catch (Exception exception) 
            {
                await ReplyAsync( "에러\n```"+ exception.Message + "```");
            }
        }
        [Command("나와")]
        public async Task leaveAsync()
        {
            IVoiceChannel voiceChannel;
            voiceChannel = _lavaNode.GetPlayer(Context.Guild).VoiceChannel;
            // if (_lavaNode.GetPlayer(Context.Guild).VoiceState ==)
            await _lavaNode.LeaveAsync(voiceChannel);
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff))
                .AddField("성공", $"{voiceChannel.Name} 음성채널을 나왔습니다.");
            await ReplyAsync("", false, builder.Build());
        }
        [Command("등록", true)]
        public async Task addSongAsync()
        {
            var player = _lavaNode.GetPlayer(Context.Guild);
            string query = Context.Message.ToString().Substring(8);
            try
            {
                // await ReplyAsync("준비 중인 기능입니다.");
                var result = Uri.IsWellFormedUriString(query, UriKind.Absolute) ? _lavaNode.SearchAsync(query).Result : _lavaNode.SearchYouTubeAsync(query).Result;
                if (result.Tracks.Count == 0)
                {
                    await ReplyAsync("검색 결과가 없습니다. 검색어를 수정해 주세요"); 
                    return;
                } 
                LavaTrack track = result.Tracks.FirstOrDefault();
                Random rd = new Random();

                if (track.Duration.Minutes > 15)
                {
                    await ReplyAsync("16분 이상의 음악은 등록할 수 없습니다.");
                    return;
                }
                player.Queue.Enqueue(track);
                EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle("추가 성공")
                    .WithColor((uint)rd.Next(0x000000, 0xffffff));
                if (player.Queue.Count == 1)
                {
                    embedBuilder.AddField("재생 시작", $"{track.Title}의 재생을 시작합니다.");
                    await player.PlayAsync(track);
                }
                else
                {
                    embedBuilder.AddField("대기열 추가됨", $"{track.Title} 을(를) {player.Queue.Count}번째 순서로 등록");
                }
                await ReplyAsync("", false, embedBuilder.Build());
            }
            catch(Exception e)
            {
                EmbedBuilder builder = new EmbedBuilder()
                     .WithTitle("오류 발생")
                     .AddField("에러 참조", e.ToString());
                await ReplyAsync("", false, builder.Build());                     
            }
        }
        [Command("볼륨")]
        public async Task setVolumeAsync(ushort volume)
        {
            if (volume > 100)
            {
                await ReplyAsync("볼륨은 0~100까지 설정할 수 있습니다.");
                return;
            }
            Random rd = new Random();
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor((uint)rd.Next(0x000000, 0xffffff));
            try
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                if (player == null)
                {
                    await ReplyAsync("현재 들어가 있는 음성채널이 없습니다.");
                    return;
                }
                await player.UpdateVolumeAsync(volume);
                builder.AddField("성공", $"플레이어의 볼륨을 {volume} (으)로 설정했습니다.");
                await ReplyAsync("", false, builder.Build());
            }
            catch(Exception e)
            {
                builder.WithTitle("오류 발생");
                builder.AddField("오류 참조",e.ToString());
                await ReplyAsync("", false, builder.Build());
            }
        }
    }
}