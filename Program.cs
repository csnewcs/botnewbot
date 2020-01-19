using System;
using System.IO;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace bot
{
    class Program
    {
        Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        DiscordSocketClient client;
        static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();
        async Task mainAsync()
        {
            DiscordSocketConfig config = new DiscordSocketConfig{MessageCacheSize = 200};
            DiscordSocketClient client = new DiscordSocketClient(config);
            this.client = client;
            string token = File.ReadAllText("config.txt"); //토큰 가져오기
            await this.client.LoginAsync(TokenType.Bot, token); //봇 로그인과 시작
            await this.client.StartAsync();
            this.client.Log += log; //이벤트 설정
            this.client.Ready += ready;
            this.client.GuildAvailable += guildAvailable;
            this.client.MessageReceived += messageReceived;
            this.client.MessageDeleted += messageDeleted;
            this.client.MessageUpdated += messageEdited;
            this.client.JoinedGuild += joinedGuild;
            this.client.LeftGuild += leftGuild;
            this.client.UserJoined += personJoinedGuild;
            await Task.Delay(-1); //봇 꺼지지 말라고 기다리기
        }
        async Task messageReceived(SocketMessage msg) //메세지 받았을 때
        {
            if (!msg.Author.IsBot)
            {
                if (msg.Channel is SocketGuildChannel)
                {
                    var channel = msg.Channel as SocketGuildChannel;
                    var guild = channel.Guild;
                }
                else
                {
                    if (setting.ContainsKey(msg.Author.Id)) //세팅 값에 있는지 확인 후 있으면 설정 이어가기
                    {
                        SocketGuild guild = client.GetGuild(setting[msg.Author.Id]);
                        ulong guildId = guild.Id;
                        if (server[msg.Author.Id].addServer(guild, msg.Author, msg.Content))
                        {
                            JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                            await msg.Author.SendMessageAsync("설정이 완료되었습니다. 그럼 이제 서버원들과 함께 즐기세요!\n당신은 이 봇의 관리자이며 \"$관리자명령어\" 를 통해 관리자 전용 명령어를 확인할 수 있습니다.");
                            if (json["noticeBot"].ToString() != "0") 
                            {
                                IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["noticeBot"].ToString()));
                                await channel.SendMessageAsync("@everyone\n이 봇을 데려와주셔서 감사드립니다. 명령어를 사용하기 위한 접두사는 \"$\"이며 명령어들은 \"$명령어\"를 통해 확인하실 수 있습니다.");
                            }
                            setting.Remove(msg.Author.Id);
                        }
                    }
                }
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel deletedMessageChannel) //메세지 삭제될 때
        {
            if (deletedMessageChannel is SocketGuildChannel)
            {
                SocketGuild guild = (deletedMessageChannel as SocketTextChannel).Guild;
                JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                if (json["deleteMessage"].ToString() != "0") 
                {
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["deleteMessage"].ToString()));
                    SocketGuildUser user = guild.GetUser(msg.Value.Author.Id);
                    string nickname = getNickname(user);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle($"{nickname}님의 메세지가 삭제됨")
                    .WithColor(new Color(0xff0000))
                    .AddField("내용", msg.Value, true)
                    .AddField("위치", deletedMessageChannel.Name);
                    Embed embed = embedBuilder.Build();
                    await channel.SendMessageAsync("", embed:embed);
                }
            }
        }
        async Task messageEdited(Cacheable<IMessage, ulong> beforeMsg, SocketMessage afterMsg, ISocketMessageChannel editedMessageChannel) //메세지 수정될 때
        {
            if (editedMessageChannel is SocketTextChannel)
            {
                SocketGuild guild = (editedMessageChannel as SocketTextChannel).Guild;
                JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                if (json["editMessage"].ToString() != "0" && !string.IsNullOrEmpty(afterMsg.Content)) 
                {
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["editMessage"].ToString()));
                    SocketGuildUser user = guild.GetUser(beforeMsg.Value.Author.Id);
                    string nickname = getNickname(user);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle($"{nickname}님의 메세지가 수정됨")
                    .WithColor(new Color(0x880088))
                    .AddField("이전 내용", beforeMsg.Value, true)
                    .AddField("현재 내용", afterMsg.Content, true)
                    .AddField("위치", editedMessageChannel.Name);
                    Embed embed = embedBuilder.Build();
                    await channel.SendMessageAsync("", embed:embed);
                }
            }
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
        async Task personJoinedGuild(SocketGuildUser user)
        {
            File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}","{\"power\":0, \"money\":100}");
            await user.Guild.SystemChannel.SendMessageAsync("새로운 유저 " + user.Mention + "님이 오셨어요!");
        }
        Task leftGuild(SocketGuild guild)
        {
            Directory.Delete("servers/" + guild.Id.ToString(),true);
            return Task.CompletedTask;
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
        string getNickname(SocketGuildUser guild)
        {
            if (string.IsNullOrEmpty(guild.Nickname))
            {
                return guild.Username;
            }
            else
            {
                return guild.Nickname;
            }
        }
    }
}
