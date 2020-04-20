using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;

using Lavalink4NET;
using Lavalink4NET.Logging;
using Lavalink4NET.DiscordNet;

using Microsoft.Extensions.DependencyInjection;

namespace bot
{
    public class Program
    {
        private static void Main() => RunAsync().GetAwaiter().GetResult();
        private static async Task RunAsync()
        {
            using (var serviceProvider = ConfigureServices())
            {
                var bot = serviceProvider.GetRequiredService<botnewbot>();
                var audio = serviceProvider.GetRequiredService<IAudioService>();
                var logger = serviceProvider.GetRequiredService<ILogger>() as EventLogger;
                logger.LogMessage += Log;

                await bot.mainAsync();
                await audio.InitializeAsync();

                logger.Log(bot, "Example Bot is running. Press [Q] to stop.");

                await Task.Delay(-1);
            }
        }
        private static void Log(object sender, LogMessageEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{args.Source.GetType().Name} {args.Level}] ");

            Console.ResetColor();
            Console.WriteLine(args.Message);

            if (args.Exception != null)
            {
                Console.WriteLine(args.Exception);
            }
        }
        private static ServiceProvider ConfigureServices() => new ServiceCollection()
            // Bot
            .AddSingleton<botnewbot>()

            // Discord
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()

            // Lavalink
            .AddSingleton<IAudioService, LavalinkNode>()
            .AddSingleton<IDiscordClientWrapper, DiscordClientWrapper>()
            .AddSingleton<ILogger, EventLogger>()

            .AddSingleton(new LavalinkNodeOptions
            {
                // Your Node Configuration
            })

            // Request Caching for Lavalink

            .BuildServiceProvider();
    }
    public class botnewbot : ModuleBase<SocketCommandContext>
    {
        const string version = "1.0"; // 버전을 저장, 파일에 저장할까도 고민중
        Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        DiscordSocketClient client;
        CommandService command;
        IServiceProvider provider;
        Dictionary<ulong, int> people = new Dictionary<ulong, int>();

        public botnewbot(IServiceProvider provider)
        {
            client = provider.GetRequiredService<DiscordSocketClient>();
            command = provider.GetRequiredService<CommandService>();
            this.provider = provider;
        }

        public async Task mainAsync() //기본 세팅
        {
            DiscordSocketConfig config = new DiscordSocketConfig{MessageCacheSize = 100};
            CommandServiceConfig serviceConfig = new CommandServiceConfig{};
            command = new CommandService(serviceConfig);
            client = new DiscordSocketClient(config);

            string[] botConfig = File.ReadAllLines("config.txt"); //봇의 정보 가져오기
            await client.LoginAsync(TokenType.Bot, botConfig[0]); //봇 로그인과 시작
            await client.StartAsync();

            
            await command.AddModulesAsync(assembly:Assembly.GetEntryAssembly(), services: provider);

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

            Thread thread = new Thread(minus);
            Season ss = new Season();
            Thread mkdt = new Thread(() => ss.mkdt(client));
            thread.Start();
            mkdt.Start();
            
            
            //--------공지 날리기---------\\
            while (true)
            {
                Console.WriteLine("공지를 날리실거면 notice.txt에 내용을 적고 아무 키나 누르세요...  ");
                Console.ReadKey();
                Console.WriteLine();
                DirectoryInfo dir = new DirectoryInfo("servers");
                foreach (var a in dir.GetDirectories())
                {
                    JObject server = JObject.Parse(File.ReadAllText($"servers/{a.Name}/config.json"));
                    if ((ulong)server["noticeBot"] != 0)
                    {
                        SocketGuild guild = client.GetGuild(ulong.Parse(a.Name));
                        SocketTextChannel channel = guild.GetChannel((ulong)server["noticeBot"]) as SocketTextChannel;
                        await channel.SendMessageAsync(File.ReadAllText("notice.txt"));
                    }
                }
                File.WriteAllText("notice.txt", "");
                Console.WriteLine("공지 전송 완료");
            }
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

                    addMoney(guildUser, msg);

                    int argPos = 0;
                    if (!message.HasCharPrefix('$', ref argPos))  return; //접두사 $없으면 리턴


                    GC.Collect();
                    if (coolDown(msg.Author.Id))
                    {
                        var a = await msg.Channel.SendMessageAsync("아직 명령어를 입력할 수 없습니다.");
                        await Task.Delay(500);
                        await a.DeleteAsync();
                        return;
                    }


                    string[] split = msg.Content.Split(' ');
                    switch(split[0])
                    {
                        case "$초기설정":
                            await reset(guildUser);
                            break;
                    }
                    SocketCommandContext context = new SocketCommandContext(client, message);

                    var result = await command.ExecuteAsync(context: context, argPos: argPos, services: provider);
                    if (result.Error.HasValue)
                    {
                        Console.WriteLine($"{result.Error}: {result.ErrorReason}");
                        await msg.Channel.SendMessageAsync($"{result.Error}: {result.ErrorReason}");
                    }
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
                            await msg.Author.SendMessageAsync("설정이 완료되었습니다. 그럼 이제 서버원들과 함께 즐기세요!\n당신은 이 봇의 관리자이며 \"$명령어 관리자\" 를 통해 관리자 전용 명령어를 확인할 수 있습니다.");
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
        void addMoney(SocketGuildUser guildUser, SocketMessage msg)
        {
            Random random = new Random();
            int getByte = (System.Text.Encoding.Default.GetBytes(msg.Content).Length) / (random.Next(3, 16)) + 1;
            Console.WriteLine("바이트 수: " + (System.Text.Encoding.Default.GetBytes(msg.Content).Length));
            Console.WriteLine("실제 얻은거: " + getByte);
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
                if (afterMsg.Embeds.Count > 0 && beforeMsg.Value.Embeds.Count <= 0)
                {
                    return;
                }
                if (json["editMessage"].ToString() != "0" && !string.IsNullOrEmpty(afterMsg.Content)) 
                {
                    if (beforeMsg.Value.Author.IsBot) return;
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
            await guild.Owner.SendMessageAsync("초기 설정을 시작합니다.");

            server[guild.OwnerId].addServer(guild, guild.Owner);
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
            return Task.CompletedTask;
        }
        public static string getNickname(SocketGuildUser guild)
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
        public static string unit(ulong money)
        {
            string moneyString = money.ToString();
            int length = moneyString.Length;
            List<string> array = new List<string>();
            string temp = "";
            int start = length % 4;
            for (int i = 0; i < start; i++) //4로 정확히 나눠지지 않을거니까
            {
                temp += moneyString[i];
            }
            array.Add(temp);
            for (int i = 0; i < length / 4; i++) //4개씩 묶기
            {
                array.Add($"{moneyString[i * 4 + start]}{moneyString[i * 4 + 1 + start]}{moneyString[i * 4 + 2 + start]}{moneyString[i * 4 + 3 + start]}");
            }
            string unitString = "만억조경";
            temp = "";
            for (int i = 0; i < array.Count; i++) //단위 붙이기
            {
                if (array[i] == "0000")
                {
                    continue;
                }
                temp += (int.Parse(array[i]).ToString());
                if (i != array.Count - 1) 
                {
                    temp += unitString[array.Count - i - 2]; //Count니까 1빼고, 일, 십, 백, 천 빠졌으니 또 1빠짐
                }
                temp += " ";
            }
            return temp;
        }
        private async Task reset(SocketGuildUser user)
        {
            if (!hasPermission(user, Permission.Admin))
            {
                return;
            }
            SocketGuild guild = user.Guild;
            File.Delete($"servers/{guild.Id}/config.json");
            setting.Add(user.Id, guild.Id);
            server.Add(user.Id, new Server());
            await user.SendMessageAsync("초기 설정을 시작합니다.");
            server[user.Id].addServer(guild, user);
        }
        public static bool isOver(SocketGuildUser first, SocketGuildUser second) //위에 있는 역할일수록 수가 큼
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles, two = second.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in two)
            {
                if (a.Position > twoTop) twoTop = a.Position;
            }
            return oneTop > twoTop;
        }
        public static bool isOver(SocketGuildUser first, IReadOnlyCollection<SocketUser> second)
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in second)
            {
                foreach (var b in (a as SocketGuildUser).Roles)
                {
                    if (b.Position > twoTop) twoTop = b.Position;
                }
            }
            return oneTop > twoTop;
        }
        public static bool isOver(SocketGuildUser first, IReadOnlyCollection<SocketRole> second)
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in second)
            {
                if (a.Position > twoTop) twoTop = a.Position;
            }
            return oneTop > twoTop;
        }
        public static bool hasPermission(SocketGuildUser user, Permission p)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return true;
            }
            bool[] permissions = new bool[5];
            foreach (var a in user.Roles) //더 빠른 알고리즘이 생각났지만 코드가 너무 더러워 질 것 같ㄷ...
            {
                bool change = false;
                if (!permissions[0])
                {
                    permissions[0] = a.Permissions.BanMembers;
                    change = true;
                }
                if (!permissions[1])
                {
                    permissions[1] = a.Permissions.BanMembers;
                    change = true;
                }
                if (!permissions[2])
                {
                    permissions[2] = a.Permissions.BanMembers;
                    change = true;
                }
                if (!permissions[3])
                {
                    permissions[3] = a.Permissions.BanMembers;
                    change = true;
                }
                if (!permissions[4])
                {
                    permissions[4] = a.Permissions.BanMembers;
                    change = true;
                }
                if (!change) break;
            }
            return permissions[(int)p];
        }
        public enum Permission //며칠 전에 이거 책에서 봐서 다행이네
        {
            BanUser,
            KickUser,
            MuteUser,
            ManageRole,
            Admin
        }


        
    }
}
