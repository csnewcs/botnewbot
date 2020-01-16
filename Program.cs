using System;
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
        Dictionary<ulong, short> setting = new Dictionary<ulong, short>();
        DiscordSocketClient client = new DiscordSocketClient();
        static void Main(string[] args) => new Program().mainAsync().GetAwaiter().GetResult();
        async Task mainAsync()
        {
            string token = File.ReadAllText("config.txt");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            client.Log += log;
            client.Ready += ready;
            client.GuildAvailable += guildAvailable;
            client.MessageReceived += messageReceived;
            client.MessageDeleted += messageDeleted;
            client.MessageUpdated += messageEdited;
            client.JoinedGuild += joinedGuild;
            await Task.Delay(-1);
        }
        async Task messageReceived(SocketMessage msg)
        {
            if (!msg.Author.IsBot)
            {
                if (msg.Channel is SocketGuildChannel)
                {
                    await msg.Channel.SendMessageAsync("Server");
                }
                else
                {
                    await msg.Channel.SendMessageAsync("DM");
                }
            }
        }
        async Task messageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            
        }
        async Task messageEdited(Cacheable<IMessage, ulong> beforeMsg, SocketMessage afterMsg, ISocketMessageChannel channel)
        {

        }
        Task log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }
        async Task joinedGuild(SocketGuild guild)
        {
            
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
