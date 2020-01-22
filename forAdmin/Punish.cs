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
    [Group("처벌")]
    public class Punish : ModuleBase<SocketCommandContext>
    {
        [Command("뮤트")]
        public async Task mute()
        {

        }

        [Command("킥")]
        public async Task kick()
        {

        }

        [Command("밴")]
        public async Task ban()
        {

        }
        
        private bool isAdmin()
        {
            return false;
        }
    }
}