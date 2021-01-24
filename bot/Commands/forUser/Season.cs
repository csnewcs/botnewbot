// using System;
// using System.IO;
// using System.Linq;
// using System.Threading;
// using System.Collections;
// using Discord;
// using Discord.Commands;
// using Discord.WebSocket;
// using Newtonsoft.Json.Linq;

// namespace bot
// {
//     class Season : ModuleBase<SocketCommandContext>
//     {
//         Support support = new Support();
//         public void mkdt(DiscordSocketClient client)
//         {
//             int last = 0;
//             while (true)
//             {
//                 DateTime time = DateTime.Now;
//                 int now = time.Month;
//                 if ((now == 4 || now == 7 || now == 11 || now == 1) && now != last)
//                 {
//                     DirectoryInfo servers = new DirectoryInfo("servers");
//                     foreach (DirectoryInfo server in servers.GetDirectories()) //
//                     {
//                         var users = server.GetFiles();
//                         SortedList list = new SortedList();
//                         foreach (var user in users)
//                         {
//                             if (user.Name == "config.json" || user.Name == "season.json") continue;
//                             string result = user.OpenText().ReadToEnd();
//                             JObject jObject = JObject.Parse(result);
//                             ulong money = (ulong)jObject["money"];
//                             list.Add(money, ulong.Parse(user.Name));
//                         }
                        
//                         JArray save = new JArray();
//                         try
//                         {
//                             save = JArray.Parse(File.ReadAllText($"servers/{server.Name}/season.json"));
//                         }
//                         catch
//                         {
                            
//                         }
//                         string date = $"{time.Year}.{time.Month}.{time.Day}";
//                         SocketGuild guild = client.GetGuild(ulong.Parse(server.Name));
//                         SocketGuildUser guildUser = guild.GetUser((ulong)list.GetByIndex(1));
//                         JObject single = new JObject();
//                         single.Add("date", date);
//                         int four = now / 3;
//                         int year = time.Year;
//                         if (four == 0) 
//                         {
//                             four = 4;
//                             year--;
//                         }
//                         string name = $"{year}년 {four}/4 분기 시즌";
//                         single.Add("name", name);
//                         single.Add("username", support.getNickname(guildUser));
//                         single.Add("money", (ulong)list.GetKey(1));
//                         save.Add(single);
//                         File.WriteAllText($"servers/{server.Name}/season.json", save.ToString());
//                         last = now;
//                         Console.WriteLine("{0} 저장 완료", name);
//                     }
//                 }
//                 Thread.Sleep(3600000); //1시간
//             }
//         }
//     }
// }