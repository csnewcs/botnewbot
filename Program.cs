using System;
using System.IO;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Discord.Commands;
using System.Reflection;
using System.Security.Permissions;

namespace bot
{
    class Program
    {
        Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        DiscordSocketClient client;
        CommandService command;
        static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();
        async Task mainAsync()
        {
            DiscordSocketConfig config = new DiscordSocketConfig{MessageCacheSize = 200};
            CommandServiceConfig serviceConfig = new CommandServiceConfig{};
            command = new CommandService(serviceConfig);
            client = new DiscordSocketClient(config);
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
            client.UserJoined += personJoinedGuild;
            await command.AddModulesAsync(assembly:Assembly.GetEntryAssembly(),
                                        services: null);
            await Task.Delay(-1); //봇 꺼지지 말라고 기다리기
        }
        async Task messageReceived(SocketMessage msg) //메세지 받았을 때
        {
            if (!msg.Author.IsBot)
            {
                if (msg.Channel is SocketGuildChannel) //기본적으로 서버만 지원
                {
                    SocketUserMessage message = msg as SocketUserMessage;
                    if (message == null) return;
                    int argPos = 0;
                    if (!message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))  return; //접두사 $없으면 리턴, 접두사가 언급하는거면 리턴(왜 있는거지?)

                    var channel = msg.Channel as SocketGuildChannel;
                    var guild = channel.Guild;
                    JObject config = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                    var guildUser = guild.GetUser(msg.Author.Id);
                    ulong roleId = (ulong)config["useBot"];
                    bool hasRole = false;
                    foreach (var role in guildUser.Roles)
                    {
                        if (role.Id == roleId)
                        {
                            hasRole = true;
                            break;
                        }
                    }
                    if (hasRole) //명령 수행할 부분
                    {
                        SocketCommandContext context = new SocketCommandContext(client, message);
                        Help help = new Help();
                        var result = await command.ExecuteAsync(context: context, argPos: argPos, services: null);
                    }
                    else return;
                }
                else
                {
                    if (setting.ContainsKey(msg.Author.Id)) //세팅 값에 있는지 확인 후 있으면 설정 이어가기
                    {
                        SocketGuild guild = client.GetGuild(setting[msg.Author.Id]);
                        ulong guildId = guild.Id;
                        if (server[msg.Author.Id].addServer(guild, msg.Author, msg.Content))
                        {
                            JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json")); //설정 json가져오기
                            await msg.Author.SendMessageAsync("설정이 완료되었습니다. 그럼 이제 서버원들과 함께 즐기세요!\n당신은 이 봇의 관리자이며 \"$관리자명령어\" 를 통해 관리자 전용 명령어를 확인할 수 있습니다.");
                            if (json["noticeBot"].ToString() != "0") //봇이 공지를 할 수 있으면
                            {
                                IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["noticeBot"].ToString()));
                                await channel.SendMessageAsync("@everyone\n이 봇을 데려와주셔서 감사드립니다. 명령어를 사용하기 위한 접두사는 \"$\"이며 명령어들은 \"$명령어\"를 통해 확인하실 수 있습니다.");
                            }
                            setting.Remove(msg.Author.Id);
                        }
                        else return;
                    }
                    else return;
                }
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel deletedMessageChannel) //메세지 삭제될 때
        {
            if (deletedMessageChannel is SocketGuildChannel) //서비인지 확인
            {
                SocketGuild guild = (deletedMessageChannel as SocketTextChannel).Guild;
                JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                if (json["deleteMessage"].ToString() != "0") //메세지가 삭제되었을 알리는지 확인
                {
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["deleteMessage"].ToString()));
                    SocketGuildUser user = guild.GetUser(msg.Value.Author.Id);
                    string nickname = getNickname(user);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle($"{nickname}님의 메세지가 삭제됨")
                    .WithColor(new Color(0xff0000)) //빨간색
                    .AddField("내용", msg.Value, true)
                    .AddField("위치", deletedMessageChannel.Name);
                    Embed embed = embedBuilder.Build();
                    await channel.SendMessageAsync("", embed:embed);
                }
            }
        }
        async Task messageEdited(Cacheable<IMessage, ulong> beforeMsg, SocketMessage afterMsg, ISocketMessageChannel editedMessageChannel) //메세지 수정될 때
        {
            if (editedMessageChannel is SocketTextChannel) //서버인지 확인
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
            await guild.Owner.SendMessageAsync("설정 전 정리를 하고 있습니다. 잠시만 기다려주세요");
            foreach (SocketGuildUser user in guild.Users) //유저 추가
            {
                if (!user.IsBot) File.WriteAllText($"servers/{guild.Id}/{user.Id}","{\"money\":100}");
            }
            File.Delete($"servers/{guild.Id}/{guild.OwnerId}"); //소유자는 최대 권한 (0(블랙리스트): 명령어 사용 불가, 1(유저): 명령어 사용 가능, 2(관리자): 관리자 명령어 사용 가능, 3(소유자): 소유자 명령어 사용 가능, 유저 이하로 강등되지 않음)
            File.WriteAllText($"servers/{guild.Id}/{guild.OwnerId}","{\"owner\":true, \"money\":100}");
            await guild.Owner.SendMessageAsync("초기 설정을 시작합니다.");

            server[guild.OwnerId].addServer(guild, guild.Owner);
        }
        Task personJoinedGuild(SocketGuildUser user)
        {
            if (!user.IsBot) File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}","{\"power\":0, \"money\":100}");
            return Task.CompletedTask;
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
