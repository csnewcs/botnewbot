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
    public class ReSetting : ModuleBase<SocketCommandContext>
    {
        [Command("초기 설정")]
        public async Task reset()
        {

        }
    }
}