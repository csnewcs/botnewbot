using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace bot
{
    [Group("도박")]
    public class Gamble : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {

        }
    }
}