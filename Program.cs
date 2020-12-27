using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;

using Victoria;
using Microsoft.Extensions.DependencyInjection;


namespace bot
{
    public sealed class Program
    {
        const int version = 1; // 버전을 저장, 파일에 저장할까도 고민중
        
        DiscordSocketClient client;
        CommandService command;
        LavaNode lavaNode;
        ServiceProvider _services;
        Support support = new Support();

        Dictionary<ulong, int> people = new Dictionary<ulong, int>();
        static string prefix = "";

        private static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();

        public async Task mainAsync() //기본 세팅
        {
            DiscordSocketConfig config = new DiscordSocketConfig{MessageCacheSize = 100};
            CommandServiceConfig serviceConfig = new CommandServiceConfig{};
            
            _services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>(new DiscordSocketClient(config))
                .AddSingleton<CommandService>(new CommandService(serviceConfig))
		        // Other services DiscordSocketClient, CommandService, etc
                .AddLavaNode(x => {
                    x.SelfDeaf = false;
                }).BuildServiceProvider();
            client = _services.GetRequiredService<DiscordSocketClient>();
            command = _services.GetRequiredService<CommandService>();
            lavaNode = _services.GetRequiredService<LavaNode>();
            lavaNode.OnLog += log;
            LavaLinkEvents events = new LavaLinkEvents();
            lavaNode.OnTrackEnded += events.TrackEnded;
            

            Console.WriteLine("공지를 날리실거면 notice.txt에 내용을 적고 아무 키나 누르세요...  ");



            // command = new CommandService(serviceConfig);
            // client = new DiscordSocketClient(config);
            //----------이벤트 설정-----------\\
            client.Log += log; 
            client.Ready += ready;
            client.GuildAvailable += guildAvailable;
            client.MessageReceived += messageReceived;
            client.MessageDeleted += messageDeleted;
            client.MessageUpdated += messageEdited;
            client.JoinedGuild += joinedGuild;
            client.LeftGuild += leftGuild;
            client.UserJoined += personJoinedGuild;
            

            string[] botConfig = new string[0];
            try
            {
                botConfig = File.ReadAllLines("config.txt"); //봇의 정보 가져오기
                prefix = botConfig[1];
            }
            catch
            {
                FileStream fs = new FileStream("config.txt", FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(fs);
                Console.WriteLine("봇 초기 설정을 시작합니다.\n봇의 토큰을 입력하세요.");
                writer.WriteLine(Console.ReadLine());
                Console.WriteLine("봇 사용 시 사용할 접두사를 입력하세요.");
                writer.WriteLine(Console.ReadLine());
                writer.Close();
                fs.Close();
                botConfig = File.ReadAllLines("config.txt"); //봇의 정보 가져오기
            }
            await client.LoginAsync(TokenType.Bot, botConfig[0]); //봇 로그인과 시작
            await client.StartAsync();
            await client.SetGameAsync($"'{prefix}명령어'로 명령어 확인!");
            
            prefix = botConfig[1];
            Thread thread = new Thread(minus);
            Season ss = new Season();
            Thread mkdt = new Thread(() => ss.mkdt(client));
            Thread version = new Thread(checkVersion);
            thread.Start();
            mkdt.Start();
            version.Start();
            
            await command.AddModulesAsync(assembly:Assembly.GetEntryAssembly(), services: _services);

            
            //--------공지 날리기---------\\
            while (true)
            {
                Console.ReadKey();
                Console.WriteLine();
                sendNotice();
                Console.WriteLine("공지를 날리실거면 notice.txt에 내용을 적고 아무 키나 누르세요...  ");
            }
        }
        void sendNotice(string send = "")
        {
            
            if (string.IsNullOrEmpty(send))
            {
                try
                {
                    send = File.ReadAllText("notice.txt");
                }
                catch
                {
                    Console.WriteLine("공지를 전송 할 수 없습니다. ./notice.txt 파일을 확인해 주세요");
                }
            }
            DirectoryInfo dir = new DirectoryInfo("servers");
                foreach (var a in dir.GetDirectories())
                {
                    JObject server = JObject.Parse(File.ReadAllText($"servers/{a.Name}/config.json"));
                    if ((ulong)server["noticeBot"] != 0)
                    {
                        SocketGuild guild = client.GetGuild(ulong.Parse(a.Name));
                        SocketTextChannel channel = guild.GetChannel((ulong)server["noticeBot"]) as SocketTextChannel;
                        try
                        {
                            channel.SendMessageAsync(send);
                        }
                        catch {}
                    }
                }
                File.WriteAllText("notice.txt", "");
                Console.WriteLine("공지 전송 완료");
        }
        async Task messageReceived(SocketMessage msg) //메세지 받았을 때
        {
            if (!msg.Author.IsBot) //봇이면 바로 보내고
            {
                if (msg.Channel is SocketGuildChannel) //기본적으로 서버만 지원
                {
                    SocketUserMessage message = msg as SocketUserMessage;
                    if (message == null) return;
                    var channel = msg.Channel as SocketGuildChannel;
                    var guild = channel.Guild;
                    var guildUser = msg.Author as SocketGuildUser;

                    DirectoryInfo dtinfo = new DirectoryInfo($"servers/{guild.Id}");
                    FileInfo finfo = new FileInfo($"servers/{guild.Id}/{guildUser.Id}");
                    if (!finfo.Exists)
                    {
                        if (!dtinfo.Exists)
                        {
                            dtinfo.Create();
                            File.WriteAllText(dtinfo.FullName + "/config.json", @"{
                                ""editMessage"": 0,
                                ""deleteMessage"": 0,
                                ""noticeBot"": 0
                            }");
                            foreach (var a in guild.Users)
                            {
                                if (a.IsBot) continue;
                                File.WriteAllText($"servers/{guild.Id}/{a.Id}", @"{
                                ""money"": 100
                                }");
                            }
                        }
                        else File.WriteAllText(finfo.FullName, @"{""editMessage"": 0,""deleteMessage"": 0,""noticeBot"": 0}");
                    }

                    addMoney(guildUser, msg);

                    int argPos = 0;
                    if (!message.HasStringPrefix(prefix, ref argPos))  return; //접두사 $없으면 리턴


                    GC.Collect();
                    if (coolDown(msg.Author.Id))
                    {
                        var a = await msg.Channel.SendMessageAsync("아직 명령어를 입력할 수 없습니다.");
                        await Task.Delay(500);
                        await a.DeleteAsync();
                        return;
                    }


                    string[] split = msg.Content.Split(' ');
                    if (split[0] == prefix + "초기설정") await support.reset(guildUser);
                    SocketCommandContext context = new SocketCommandContext(client, message);

                    var result = await command.ExecuteAsync(context: context, argPos: argPos, services: _services);
                    if (!result.IsSuccess) await msg.Channel.SendMessageAsync(result.Error.ToString());
                }
                else
                {
                    // if (support.setting.ContainsKey(msg.Author.Id)) //세팅 값에 있는지 확인 후 있으면 설정 이어가기
                    // {
                    //     SocketGuild guild = client.GetGuild(support.setting[msg.Author.Id]);
                    //     ulong guildId = guild.Id;
                    //     if (support.server[msg.Author.Id].addServer(guild, msg.Author, msg.Content))
                    //     {
                    //         JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json")); //설정 json가져오기
                    //         await msg.Author.SendMessageAsync("설정이 완료되었습니다. 그럼 이제 서버원들과 함께 즐기세요!\n당신은 이 봇의 관리자이며 \"$명령어 관리자\" 를 통해 관리자 전용 명령어를 확인할 수 있습니다.");
                    //         if (json["noticeBot"].ToString() != "0") //봇이 공지를 할 수 있으면
                    //         {
                    //             IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["noticeBot"].ToString()));
                    //             await channel.SendMessageAsync("@everyone\n이 봇을 데려와주셔서 감사드립니다. 명령어를 사용하기 위한 접두사는 \"$\"이며 명령어들은 \"$명령어\"를 통해 확인하실 수 있습니다.");
                    //         }
                    //         support.setting.Remove(msg.Author.Id);
                    //     }
                    //     else return;
                    // }
                    return;
                }
            }
        }
        void addMoney(SocketGuildUser guildUser, SocketMessage msg)
        {
            Random random = new Random();
            int getByte = (System.Text.Encoding.Default.GetBytes(msg.Content).Length) * (random.Next(3, 16)) + 1;
            Console.WriteLine("bytes: " + (System.Text.Encoding.Default.GetBytes(msg.Content).Length) + "     get BNB: " + getByte);
            string path = $"servers/{guildUser.Guild.Id}/{guildUser.Id}";
            JObject user = JObject.Parse(File.ReadAllText(path));
            ulong money = (ulong)user["money"] + (ulong)getByte;
            user["money"] = money;
            File.WriteAllText(path, user.ToString());
        }
        bool coolDown(ulong Id) //명령어는 3초에 한 번씩
        {
            if (people.ContainsKey(Id))
            {
                return true;
            }
            else
            {
                people.Add(Id, 3);
                return false;
            }
        }
        void minus()
        {
            while(true)
            {
                var temp = people.Keys.ToList();
                foreach(ulong key in temp)
                {
                    people[key]--;
                    if (people[key] == 0) 
                    {
                        people.Remove(key);
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel deletedMessageChannel) //메세지 삭제될 때
        {
            if (deletedMessageChannel is SocketGuildChannel) //서버인지 확인
            {
                SocketGuild guild = (deletedMessageChannel as SocketTextChannel).Guild;
                JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                if (json["deleteMessage"].ToString() != "0") //메세지가 삭제되었을 알리는지 확인
                {
                    if (msg.Value.Author.IsBot) return;
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["deleteMessage"].ToString()));
                    SocketGuildUser user = guild.GetUser(msg.Value.Author.Id);
                    string nickname = support.getNickname(user);
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
                if (afterMsg.Embeds.Count > 0 && beforeMsg.Value.Embeds.Count <= 0)
                {
                    return;
                }
                if (json["editMessage"].ToString() != "0" && !string.IsNullOrEmpty(afterMsg.Content)) 
                {
                    if (beforeMsg.Value.Author.IsBot) return;
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["editMessage"].ToString()));
                    SocketGuildUser user = guild.GetUser(beforeMsg.Value.Author.Id);
                    string nickname = support.getNickname(user);
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
            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            Console.WriteLine(log);
            sw.WriteLine(log);
            sw.Close();
            fs.Close();
            return Task.CompletedTask;
        }
        async Task joinedGuild(SocketGuild guild) //서버에 처음 들어갔을 때
        {
            // setting.Add(guild.OwnerId, guild.Id); // (서버 주인 ID, 서버 ID)
            // server.Add(guild.OwnerId, new Server()); //(서버 주인 ID, 서버 설정 클래스)
            Directory.CreateDirectory("servers/" + guild.Id.ToString()); //servers/서버 ID가 이름인 디렉터리 생성
            // Console.WriteLine(guild.OwnerId);
            // SocketGuildUser owner = guild.GetUser(guild.OwnerId);
            // await owner.SendMessageAsync("설정 전 정리를 하고 있습니다. 잠시만 기다려주세요");
            foreach (SocketGuildUser user in guild.Users) //유저 추가
            {
                if (!user.IsBot) File.WriteAllText($"servers/{guild.Id}/{user.Id}","{\"money\":100}");
            }
            File.WriteAllText("servers/" + guild.Id.ToString() + "/config.json", @"{
                ""editMessage"": 0,
                ""deleteMessage"": 0,
                ""noticeBot"": 0
            }");
            await guild.DefaultChannel.SendMessageAsync($"안녕하세요? botnewbot입니다. 이 봇의 접두사는 '{prefix}' 이며, '{prefix}명령어' 로 사용 가능한 명령어를 확인할 수 있습니다.\n서버의 관리자들은 '{prefix}명령어 관리자' 로 서버 관리에 관한 명령어를 확인할 수 있습니다.");
            // await owner.SendMessageAsync("초기 설정을 시작합니다.");

            // server[guild.OwnerId].addServer(guild, guild.Owner);
        }
        Task personJoinedGuild(SocketGuildUser user)
        {
            if (!user.IsBot) File.WriteAllText($"servers/{user.Guild.Id}/{user.Id}","{\"money\":100}");
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
            if (!lavaNode.IsConnected) 
            {
                lavaNode.ConnectAsync();
            }
            return Task.CompletedTask;
        }
        

        private void checkVersion()
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            while(true)
            {
                try
                {
                    Thread.Sleep(1000 * 60 * 60); //1시간 기다리기
                    client.Headers.Add("user-agent", "botnewbot");
                    string download = client.DownloadString("https://api.github.com/repos/csnewcs/botnewbot/tags");
                    if (string.IsNullOrEmpty(download)) continue;
                    JArray tags = JArray.Parse(download);
                    if (tags.Count > version)
                    {
                        Console.WriteLine("새로운 버전 {0}이(가) 나왔습니다!", tags.Last["name"]);
                        sendNotice($"이 봇의 새로운 버전 {tags.Last["name"]}가 나왔습니다!\n서버장님께 봇 업데이트를 요청해보는건 어떨까요?\n자세한 설명: https://github.com/csnewcs/botnewbot/releases/tag/{tags.Last["name"]}");
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(1000 * 60 * 60 * 6); //6시간 기다리기
                }
            }
        }
    }
}
