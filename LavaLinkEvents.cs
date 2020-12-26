using System;
using System.Threading.Tasks;
using System.Linq;
using Victoria;
using Victoria.EventArgs;
using Victoria.Enums;
using Victoria.Responses.Rest;
using Discord;
using Discord.WebSocket;

namespace bot
{
    class LavaLinkEvents
    {
        public async Task TrackEnded(TrackEndedEventArgs e)
        {
            e.Player.Queue.Remove(e.Player.Queue.FirstOrDefault());
            if (e.Player.Queue.Count != 0)
            {
                await e.Player.PlayAsync(e.Player.Queue.FirstOrDefault());
            }
        }
    }
}