﻿using System;
using System.IO;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Collections;
using System.Collections.Generic;

namespace bot
{
    class Program
    {
        Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        DiscordSocketClient client = new DiscordSocketClient();
        static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();
        async Task mainAsync()
        {
            string token = File.ReadAllText("config.txt"); //토큰 가져오기
            await client.LoginAsync(TokenType.Bot, token); //봇 로그인과 시작
            await client.StartAsync();
            client.Log += log; //이벤트 설정
            client.Ready += ready;
            client.GuildAvailable += guildAvailable;
            client.MessageReceived += messageReceived;
            client.MessageDeleted += messageDeleted;
            client.MessageUpdated += messageEdited;
            client.JoinedGuild += joinedGuild;
            client.LeftGuild += leftGuild;
            await Task.Delay(-1); //봇 꺼지지 말라고 기다리기
        }
        async Task messageReceived(SocketMessage msg) //메세지 받았을 때
        {
            if (!msg.Author.IsBot)
            {
                if (msg.Channel is SocketGuildChannel)
                {
                    await msg.Channel.SendMessageAsync("Server");
                    var channel = msg.Channel as SocketGuildChannel;
                    var guild = channel.Guild;
                }
                else
                {
                    if (setting.ContainsKey(msg.Author.Id)) //세팅 값에 있는지 확인 후 있으면 설정 이어가기
                    {
                        SocketGuild guild = client.GetGuild(setting[msg.Author.Id]);
                        ulong guildId = guild.Id;
                        server[msg.Author.Id].addServer(guild, msg.Author, msg.Content);
                    }
                }
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel) //메세지 삭제될 때
        {
            
        }
        async Task messageEdited(Cacheable<IMessage, ulong> beforeMsg, SocketMessage afterMsg, ISocketMessageChannel channel) //메세지 수정될 때
        {

        }
        Task log(LogMessage log) //로그 출력
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }
        async Task joinedGuild(SocketGuild guild) //서버에 처음 들어갔을 때
        {
            setting.Add(guild.OwnerId, guild.Id); // (서버 주인 ID, 서버 ID)
            server.Add(guild.OwnerId, new Server()); //(서버 주인 ID, 서버 설정 클래스)
            Directory.CreateDirectory("servers/" + guild.Id.ToString()); //servers/서버 ID가 이름인 디렉터리 생성
            await guild.Owner.SendMessageAsync("초기 설정을 시작합니다.");

            server[guild.OwnerId].addServer(guild, guild.Owner);
        }
        async Task leftGuild(SocketGuild guild)
        {
            Directory.Delete("servers/" + guild.Id.ToString(), true);
        }
        Task guildAvailable(SocketGuild guild)
        {
            guild.DefaultChannel.SendMessageAsync("");
            return Task.CompletedTask;
        }
        Task ready()
        {
            return Task.CompletedTask;
        }
    }
}
