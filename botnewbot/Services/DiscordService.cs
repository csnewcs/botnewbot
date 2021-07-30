using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using botnewbot.BotData;
using botnewbot.Handlers;

namespace botnewbot.Services
{
    public class DiscordService
    {
        private ServiceProvider _provider;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _commandHandler;
        private InteractionHandler _interactionHandler;

        public DiscordService()
        {
            _provider = ConfigureServices();
            _client = _provider.GetService<DiscordSocketClient>();
            _commands = _provider.GetService<CommandService>();
            _commandHandler = _provider.GetService<CommandHandler>();
            _interactionHandler = _provider.GetService<InteractionHandler>();
        }
        public async Task Init()
        {
            BotConfig.Init();
            await _commandHandler.setProvider(_provider);
            _client.Ready += ready;
            _client.Log += discordLog;
            _client.InteractionCreated += _interactionHandler.interactionCreated;
            _commands.Log += discordLog;
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), _provider);
            await _client.LoginAsync(TokenType.Bot, BotConfig.BotToken);
            await _client.StartAsync();
            await Task.Delay(-1);
        }
        private async Task ready()
        {
            //나중에 Victoria가 되면 여기에 넣기
        }
        private Task discordLog(LogMessage msg)
        {
            if(msg.Message != null) LoggingService.Log(msg.Message, msg.Severity);
            return Task.CompletedTask;
        }

        public ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }
    }
}