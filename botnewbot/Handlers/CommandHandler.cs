using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using botnewbot.BotData;
using botnewbot.Services;

namespace botnewbot.Handlers
{
    public class CommandHandler
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private ServiceProvider _services;
        public async Task setProvider(ServiceProvider provider)
        {
            _commands = provider.GetService<CommandService>();
            _client = provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += messageRecieved;
            _services = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        private async Task messageRecieved(SocketMessage message)
        {
            if (message.Author.IsBot || message.Author.IsWebhook) return;
            SocketUserMessage msg = message as SocketUserMessage;
            SocketGuildUser user = msg.Author as SocketGuildUser;
            if(user == null) return;
            int argPos = 0;
            if(!msg.HasStringPrefix(BotConfig.Prefix, ref argPos)) return;
            var context = new SocketCommandContext(_client, msg);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                LoggingService.Log(result.ErrorReason, LogSeverity.Error);
            }
        }
    }
}