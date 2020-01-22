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
    [Group("순위")]
    public class Rank : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task help()
        {

        }

        [Command("나")]
        public async Task me()
        {

        }

        [Command("모두")]
        public async Task all()
        {

        }

        [Command("상위권")]
        public async Task top()
        {
            
        }
    }
}