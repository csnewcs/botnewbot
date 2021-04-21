using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using botnetbot.Support;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;

using Victoria;
using Victoria.EventArgs;
using Microsoft.Extensions.DependencyInjection;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

using csnewcs.Sql;
using csnewcs.Game.GoStop;

using botnewbot.Support;


namespace bot
{
    public sealed class Program
    {
        const int version = 1; // 버전을 저장, 파일에 저장할까도 고민중
        
        DiscordSocketClient client;
        CommandService command;
        LavaNode lavaNode;
        ServiceProvider _services;
        Money _money;
        private User _user;
        private Game _game;
        private Guild _guild;
        Support support;
        //public static Support _support
        //{
        //    get {return support;}
        //}


        Dictionary<ulong, int> people = new Dictionary<ulong, int>();
        public static string prefix = "";

        private static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();

        public async Task mainAsync() //기본 세팅
        {
            DiscordSocketConfig config = new DiscordSocketConfig{MessageCacheSize = 100};
            CommandServiceConfig serviceConfig = new CommandServiceConfig{};
            
            _services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>(new DiscordSocketClient(config))
                .AddSingleton<CommandService>(new CommandService(serviceConfig))
                .AddSingleton<Support>(new Support())
                .AddSingleton(new Money())
                .AddSingleton(new User())
                .AddSingleton(new Guild())
		        // Other services DiscordSocketClient, CommandService, etc
                .AddLavaNode(x => {
                    x.SelfDeaf = false;
                }).BuildServiceProvider();
            client = _services.GetRequiredService<DiscordSocketClient>();
            command = _services.GetRequiredService<CommandService>();
            lavaNode = _services.GetRequiredService<LavaNode>();
            support = _services.GetRequiredService<Support>();
            _money = _services.GetRequiredService<Money>();
            _user = _services.GetRequiredService<User>();
            _guild = _services.GetRequiredService<Guild>();


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
            client.ReactionAdded += reactionAdded;
            // client.LeftGuild += leftGuild;
            // client.UserJoined += personJoinedGuild;
            

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
            // Season ss = new Season();
            // Thread mkdt = new Thread(() => ss.mkdt(client));
            Thread version = new Thread(checkVersion);
            thread.Start();
            // mkdt.Start();
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
            // DirectoryInfo dir = new DirectoryInfo("servers");
            
            //     foreach (var a in dir.GetDirectories())
            //     {
            //         JObject server = JObject.Parse(File.ReadAllText($"servers/{a.Name}/config.json"));
            //         if ((ulong)server["noticeBot"] != 0)
            //         {
            //             SocketGuild guild = client.GetGuild(ulong.Parse(a.Name));
            //             SocketTextChannel channel = guild.GetChannel((ulong)server["noticeBot"]) as SocketTextChannel;
            //             try
            //             {
            //                 channel.SendMessageAsync(send);
            //             }
            //             catch {}
            //         }
            //     }
            //     File.WriteAllText("notice.txt", "");
            //     Console.WriteLine("공지 전송 완료");
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

                    // DirectoryInfo dtinfo = new DirectoryInfo($"servers/{guild.Id}");
                    // FileInfo finfo = new FileInfo($"servers/{guild.Id}/{guildUser.Id}");
                    if (!_guild.exists(guildUser.Guild))
                    {
                        _guild.add(guildUser.Guild);
                    }
                    else if (!_user.exist(guildUser))
                    {
                        _user.add(guildUser);
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
                    // if (split[0] == prefix + "초기설정") await support.reset(guildUser);
                    SocketCommandContext context = new SocketCommandContext(client, message);

                    var result = await command.ExecuteAsync(context: context, argPos: argPos, services: _services);
                    if (!result.IsSuccess) await msg.Channel.SendMessageAsync(result.Error.ToString());
                }
                else
                {
                    try
                    {
                        if (!support.turnPlayer.ContainsKey(msg.Author.Id)) return;

                        SocketGuildChannel channel = support.turnPlayer[msg.Author.Id];
                        var gostopgame = support.goStopGame[channel];
                        var gostopPlayer = gostopgame.getPlayer(msg.Author.Id);
                        int selected = 0;

                        if(!int.TryParse(msg.Content, out selected))
                        {
                            await msg.Channel.SendMessageAsync("숫자만 입력해주세요.");
                            return;
                        }


                        Hwatu selectedHwatu = gostopPlayer.hwatus[selected - 1];
                        Hwatu[] canGet = gostopgame.Field.canGet(selectedHwatu);
                        int eat = 0;
                        bool wasSelected = false;

                        if(support.selectFieldGet.ContainsKey(msg.Author.Id))
                        {
                            gostopgame.selectHwatu(support.selectFieldGet[msg.Author.Id][selected - 1]);
                            support.selectGet.Remove(msg.Author.Id);
                            return;
                        }
                        else if(support.selectGet.ContainsKey(msg.Author.Id))
                        {
                            selectedHwatu = support.selectGet[msg.Author.Id].Key;
                            canGet = gostopgame.Field.canGet(selectedHwatu);
                            eat = Array.IndexOf(canGet, support.selectGet[msg.Author.Id].Value[selected - 1]);
                            Console.WriteLine(selectedHwatu.toKR() + support.selectGet[msg.Author.Id].Value[selected - 1].toKR());
                            wasSelected = true;
                            support.selectGet.Remove(msg.Author.Id);
                            // return;
                        }

                        if(canGet.Length > 1 && !wasSelected)
                        {
                            KeyValuePair<Hwatu, Hwatu[]> putNGet = new KeyValuePair<Hwatu, Hwatu[]>(selectedHwatu, canGet);
                            support.selectGet.Add(msg.Author.Id, putNGet);
                            string sendMessage = $"```{selectedHwatu.toKR()}를 가지고 먹을 화투를 선택하세요\n";

                            for(int i = 1; i <= canGet.Length; i++)
                            {
                                sendMessage += $"{i}: {canGet[i - 1].toKR()}\n";
                            }
                            sendMessage += "```";

                            await msg.Author.SendMessageAsync(sendMessage);
                            return;
                        }
                        try
                        {
                            support.goStopGame[channel].turnPlayerPutHwatu(selectedHwatu, eat);
                        }
                        catch (Exception e)
                        {
                            if(e.Message != "Please Select Hwatu")
                            {
                                Console.WriteLine(e);
                                return;
                            }
                            
                            support.selectFieldGet.Add(msg.Author.Id, canGet);
                            string sendMessage = $"```{selectedHwatu.toKR()}를 가지고 먹을 화투를 선택하세요\n";

                            for(int i = 1; i <= canGet.Length; i++)
                            {
                                sendMessage += $"{i}: {canGet[i - 1].toKR()}\n";
                            }
                            sendMessage += "```";

                            await msg.Author.SendMessageAsync(sendMessage);
                            return;
                        }
                        string path = $"GoStop/{channel.Id}/{msg.Author.Id}";

                        gostopPlayer = gostopgame.getPlayer(msg.Author.Id);
                        
                        gostopgame.getImage().Save($"GoStop/{channel.Id}/pan.png", new PngEncoder());
                        gostopPlayer.getHwatusImage().Save(path + ".png", new PngEncoder());
                        // gostopPlayer.getScoreHwatusImage().Save(path + "score.png", new PngEncoder());
                        // gostopgame.Field.getFieldImage().Save($"GoStop/{channel.Id}/field.png", new PngEncoder());
                        int score = gostopPlayer.getScore();
                        await msg.Author.SendFileAsync($"GoStop/{channel.Id}/pan.png", "현재 상황");
                        // await msg.Author.SendFileAsync($"{path}score.png", $"당신의 점수판\n현재 점수: {score}");
                        User user = new User();
                        await ((SocketTextChannel)channel).SendFileAsync($"GoStop/{channel.Id}/pan.png", $"{user.getNickName(channel.Guild.GetUser(msg.Author.Id))}의 턴 종료, 현재 상황");
                        // await ((SocketTextChannel)channel).SendFileAsync($"GoStop/{channel.Id}/field.png", "");

                        gostopgame = support.goStopGame[channel];
                        gostopPlayer = gostopgame.turn;
                        
                        string send = "당신의 차례입니다. 아래 목록에서 낼 것을 골라 번호를 입력하세요. \n```";
                        int index = 1;
                        foreach(var hwatu in gostopPlayer.hwatus)
                        {
                            send += $"{index}: {hwatu.toKR()}\n";
                            index++;
                        }
                        send += "```";
                        // support.goStopGame[channel].Field.getFieldImage().Save(path + "field.png", new PngEncoder());

                        SocketUser next = client.GetUser(gostopPlayer.id);
                        support.turnPlayer.Remove(msg.Author.Id);
                        support.turnPlayer.Add(gostopPlayer.id, channel);
                        await next.SendFileAsync($"GoStop/{channel.Id}/pan.png");
                        await next.SendFileAsync($"GoStop/{channel.Id}/{next.Id}.png");
                        await next.SendMessageAsync(send);
                    } 
                    catch(Exception e) {Console.WriteLine(e);}
                        

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
            _money.addMoney(guildUser, getByte);
            // string path = $"servers/{guildUser.Guild.Id}/{guildUser.Id}";
            // JObject user = JObject.Parse(File.ReadAllText(path));
            // ulong money = (ulong)user["money"] + (ulong)getByte;
            // user["money"] = money;
            // File.WriteAllText(path, user.ToString());
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
                // foreach(var a in Program.support.timer.ToArray())
                // {
                //     var key = a.Key;
                //     Console.WriteLine(a.Value);
                //     Program.support.timer[key]--;
                //     if(a.Value == 1)
                //     {
                //         Program.support.timer.Remove(key);
                //         Gamble gamble = new Gamble(Program.support);
                //         await gamble.startGoStop(key);
                //     }
                // }
                System.Threading.Thread.Sleep(1000);
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel deletedMessageChannel) //메세지 삭제될 때
        {
            if (deletedMessageChannel is SocketGuildChannel) //서버인지 확인
            {
                SocketGuild guild = (deletedMessageChannel as SocketTextChannel).Guild;
                JObject json  = support.getGuildConfig(guild);
                if (json["deleteMessage"].ToString() != "0") //메세지가 삭제되었을 알리는지 확인
                {
                    if (msg.Value.Author.IsBot || !msg.HasValue) return;
                    // if(msg.Value.Author != msg.Value)
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["deleteMessage"].ToString()));
                    SocketGuildUser guildUser = guild.GetUser(msg.Value.Author.Id);
                    User user = new User();
                    string nickname = user.getNickName(guildUser);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle($"{nickname}님의 메세지가 삭제됨")
                    .WithColor(new Discord.Color(0xff0000)) //빨간색
                    .AddField("내용", msg.Value, true)
                    .AddField("위치", deletedMessageChannel.Name);
                    Embed embed = embedBuilder.Build();
                    await channel.SendMessageAsync("", embed:embed);
                }
            }
        }
        async Task messageEdited(Cacheable<IMessage, ulong> beforeMsg, SocketMessage afterMsg, ISocketMessageChannel editedMessageChannel) //메세지 수정될 때
        {
            // Console.WriteLine("메세지 업데이트");
            // if (beforeMsg.Value.Reactions.Count != afterMsg.Reactions.Count)
            // {
            //     Console.WriteLine("들어옴");
            //     bool person = false;
            //     string emojiName = "";
            //     foreach (var reaction in afterMsg.Reactions)
            //     {
            //         if (!reaction.Value.IsMe)
            //         {
            //             person = true;
            //             emojiName = reaction.Key.Name;
            //         }
            //         Console.WriteLine(reaction.Key.Name);
            //     }
            //     await afterMsg.Channel.SendMessageAsync("이모지 변경됨 " + emojiName);
            // }
            
            if (beforeMsg.Value.Content == afterMsg.Content) return;
            if (editedMessageChannel is SocketTextChannel) //서버인지 확인
            {
                SocketGuild guild = (editedMessageChannel as SocketTextChannel).Guild;
                JObject json = support.getGuildConfig(guild);
                // JObject json = JObject.Parse(File.ReadAllText($"servers/{guild.Id}/config.json"));
                if (afterMsg.Embeds.Count > 0 && beforeMsg.Value.Embeds.Count <= 0)
                {
                    return;
                }
                if (json["editMessage"].ToString() != "0" && !string.IsNullOrEmpty(afterMsg.Content)) 
                {
                    // if (beforeMsg.Value.Author.IsBot) return;
                    IMessageChannel channel = guild.GetTextChannel(ulong.Parse(json["editMessage"].ToString()));
                    SocketGuildUser guildUser = guild.GetUser(beforeMsg.Value.Author.Id);
                    User user = new User();
                    string nickname = user.getNickName(guildUser);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    .WithTitle($"{nickname}님의 메세지가 수정됨")
                    .WithColor(new Discord.Color(0x880088))
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
        Task joinedGuild(SocketGuild guild) //서버에 처음 들어갔을 때
        {
            _guild.add(guild);
            return Task.CompletedTask;

            // // setting.Add(guild.OwnerId, guild.Id); // (서버 주인 ID, 서버 ID)
            // // server.Add(guild.OwnerId, new Server()); //(서버 주인 ID, 서버 설정 클래스)
            // Directory.CreateDirectory("servers/" + guild.Id.ToString()); //servers/서버 ID가 이름인 디렉터리 생성
            // // Console.WriteLine(guild.OwnerId);
            // // SocketGuildUser owner = guild.GetUser(guild.OwnerId);
            // // await owner.SendMessageAsync("설정 전 정리를 하고 있습니다. 잠시만 기다려주세요");
            // foreach (SocketGuildUser user in guild.Users) //유저 추가
            // {
            //     if (!user.IsBot) File.WriteAllText($"servers/{guild.Id}/{user.Id}","{\"money\":100}");
            // }
            // File.WriteAllText("servers/" + guild.Id.ToString() + "/config.json", @"{
            //     ""editMessage"": 0,
            //     ""deleteMessage"": 0,
            //     ""noticeBot"": 0
            // }");
            // await guild.DefaultChannel.SendMessageAsync($"안녕하세요? botnewbot입니다. 이 봇의 접두사는 '{prefix}' 이며, '{prefix}명령어' 로 사용 가능한 명령어를 확인할 수 있습니다.\n서버의 관리자들은 '{prefix}명령어 관리자' 로 서버 관리에 관한 명령어를 확인할 수 있습니다.");
            // // await owner.SendMessageAsync("초기 설정을 시작합니다.");

            // // server[guild.OwnerId].addServer(guild, guild.Owner);
        }
        async Task reactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            // Console.WriteLine("이모지 변경 감지");
            // if (message.Value.Author != client.) 
            // {
            //     return;
            //     // return Task.CompletedTask;
            // }
            // Console.WriteLine("봇이 보낸 거 맞음");
            if(support.helpMessages.ContainsKey(message.Value.Id))
            {
                // Console.WriteLine("메세지가 감지됨");
                EmbedBuilder builder= new EmbedBuilder();
                string[] emojis = new string[5] {
                    "\U0001f4b0", "\U0001f4b8", "\U0001f3c6", "\U0001f3a4", "\U0001f6e0" 
                };
                if (reaction.UserId == support.helpMessages[message.Value.Id])
                {
                    string prefix = Program.prefix;
                    // Console.WriteLine(reaction.Emote.Name);
                    if (reaction.Emote.Name == emojis[0])
                    {
                            builder.AddField($"{prefix}은행", "현재 자신이 돈이 얼마나 있는지 확인합니다.");
                    }
                    else if (reaction.Emote.Name == emojis[1])
                    {
                        builder.AddField($"{prefix}도박 제비뽑기 <걸 돈> <선택할 제비>", "```제비뽑기로 도박을 합니다. 1~9번 제비가 있으며, 건 돈의 0%~250%를 돌려받을 수 있습니다. (최소 10BNB를 걸어야 도박이 가능합니다)```")
                                        .AddField($"{prefix}도박 슬롯머신 <걸 돈>", "```슬롯머신으로 도박을 합니다. 건 돈의 0배~30배를 돌려받을 수 있습니다. (최소 10BNB를 걸어야 도박이 가능합니다)```")
                                        .AddField($"{prefix}도박 슬롯머신 <판당 걸 돈> <연속으로 돌릴 횟수>", "```슬롯머신을 여러 번 돌립니다. 한 번에 최대 100번까지 돌릴 수 있으며, 결과는 DM으로 전송됩니다.```");
                    }
                    else if (reaction.Emote.Name == emojis[2])
                    {
                        builder.AddField($"{prefix}순위 나", "```이 서버 내에서 나의 순위를 확인합니다.```")
                                        .AddField($"{prefix}순위 상위권", "```이 서버의 상위 5명을 확인합니다.```")
                                        .AddField($"{prefix}순위 모두", "```이 서버에 속해있는 모든 사람들의 순위를 확인합니다. 결과는 DM으로 전송됩니다.```");
                    }
                    else if (reaction.Emote.Name == emojis[3])
                    {
                        builder.AddField($"{prefix}노래방 들어와", "```사용자가 있는 통화방으로 봇이 들어갑니다.```")
                                        .AddField($"{prefix}노래방 등록 <검색어 | URL>", "```노래를 등록합니다. 재생하고 있는 것이 없다면 바로 재생하고, 아니면 재생목록에 추가됩니다.```")
                                        .AddField($"{prefix}노래방 일괄등록 <검색어>,[검색어]", "```반점(,)을 기준으로 URL혹은 검색어를 나눠 노래를 등록합니다.```")
                                        .AddField($"{prefix}노래방 재생목록 [페이지]", "```현재 재생목록을 보여줍니다. 한 페이지에 10곡씩```")
                                        .AddField($"{prefix}노래방 멈춰", "```노래를 일시 정지시킵니다.```")
                                        .AddField($"{prefix}노래방 재생", "```일시정지된 것을 다시 재생시킵니다.```");
                    }
                    else if (reaction.Emote.Name == emojis[4])
                    {
                        builder.WithTitle("관리자용 명령어")
                                        .AddField($"{prefix}역할 부여 <@역할을 부여할 사람> <@부여될 역할>", "```언급한 사람들에게 언급한 역할들을 부여합니다. 한 번에 여러 사람에게 하는 것도 가능하고, 한 번에 여러 역할을 부여하는 것도 가능합니다.```")
                                        .AddField($"{prefix}역할 강탈 <@역할을 없앨 사람> <@없앨 역할>", "```언급할 사람에게서 언급한 역할을 제거합니다. 한 번에 여러 사람에게 하는 것도 가능하고, 한 번에 여러 역할을 없애는 것도 가능합니다..```")
                                        .AddField($"{prefix}처벌 뮤트 <@음소거 시킬 사람>", "```언급한 사람들의 마이크를 강제로 꺼버립니다. 한 번에 여러 명을 음소거 시키는 것도 가능합니다.```")
                                        .AddField($"{prefix}처벌 킥 <@추방할 사람>", "```언급한 사람들을 서버에서 추방시킵니다. 한 번에 여러 명을 추방하는 것도 가능합니다.```")
                                        .AddField($"{prefix}처벌 밴 <@차단할 사람>", "```언급한 사람들을 서버에서 차단시킵니다. 한 번에 여러 명을 차단하는 것도 가능합니다.```")
                                        .AddField($"{prefix}처벌해제 뮤트 <@음소거된 사람>", "```언급한 사람들의 마이크를 다시 킵니다. 한 번에 여러 명을 푸는 것도 가능합니다.```")
                                        .AddField($"{prefix}처벌해제 밴 목록", "```이 서버에서 벤을 당한 사람들 목록을 불러옵니다. 결과는 DM으로 전송됩니다.```")
                                        .AddField($"{prefix}처벌해제 밴 <차단을 풀 사람의 ID>", "```ID를 준 사람의 차단을 해제합니다. 밴 목록에서 ID를 가져와주세요.```")
                                        .AddField($"{prefix}처벌해제 밴 모두", "```이 서버에서 차단 당한 사람 모두를 해제합니다.```")
                                        .WithFooter("당연히 권한 없는 사람이 하면 아무 일도 일어나지 않아요.");
                    }
                    builder.WithColor(0xbe33ff);
                    support.helpMessages.Remove(message.Id);
                    await message.Value.ModifyAsync(m => {m.Embed = builder.Build();});
                }
            }
            
            // return Task.CompletedTask;
        }
        Task personJoinedGuild(SocketGuildUser user)
        {
            if (!user.IsBot) _user.add(user);
            return Task.CompletedTask;
        }
        // Task leftGuild(SocketGuild guild)
        // {
        //     Directory.Delete("servers/" + guild.Id.ToString(),true);
        //     return Task.CompletedTask;
        // }
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
            var servers = client.Guilds;
            Dictionary<string, string[]> tables = new Dictionary<string, string[]>();
            foreach (var server in servers)
            {
                tables.Add("guild_" + server.Id.ToString(), new string[] {
                    "id:varchar(20) NOT NULL",
                    "money:BIGINT",
                    "serverinfo:TEXT"
                });
            }
            SqlHelper sqlHelper = new SqlHelper("localhost", "botnewbot", tables);
            support.changeSqlHelper(sqlHelper);
            _money.setSqlHelper(sqlHelper);
            _user.setSqlHelper(sqlHelper);
            _guild.setSqlHelper(sqlHelper);
            return Task.CompletedTask;
        }
        

        private void checkVersion()
        {
            string lastversion = "";
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
                    if (tags.Count > version && lastversion != tags.Last["name"].ToString())
                    {
                        Console.WriteLine("새로운 버전 {0}이(가) 나왔습니다!", tags.Last["name"]);
                        sendNotice($"이 봇의 새로운 버전 {tags.Last["name"]}가 나왔습니다!\n서버장님께 봇 업데이트를 요청해보는건 어떨까요?\n자세한 설명: https://github.com/csnewcs/botnewbot/releases/tag/{tags.Last["name"]}");
                        lastversion = tags.Last["name"].ToString();
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
