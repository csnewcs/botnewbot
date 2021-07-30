using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.WebSocket;

using botnewbot.Services;
using botnewbot.Commands;

namespace botnewbot.Handlers
{
    public class InteractionHandler
    {
        public async Task interactionCreated(SocketInteraction arg)
        {
            switch(arg.Type)
            {
                case InteractionType.MessageComponent:
                    await messageComponent(arg);
                    break;
                case InteractionType.ApplicationCommand:
                    //아직 슬래시 커맨드 미지원
                    break;
                case InteractionType.Ping:
                    break;
                default:
                    LoggingService.Log($"Unkown interaction {arg.Type}", LogSeverity.Warning);
                    break;
            }
        }
        private async Task messageComponent(SocketInteraction arg)
        {
            var parsedArg = (SocketMessageComponent)arg;
            var id = parsedArg.Data.CustomId;
            switch(id)
            {
                case "helpComponent":
                    await Help.sendHelp(parsedArg);
                    break;
                default:
                    LoggingService.Log($"Unkown component {id}", LogSeverity.Warning);
                    break;
            }
        }
    }
}