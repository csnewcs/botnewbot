using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace botnewbot.Commands
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("핑")]
        [Alias("ping")]
        public async Task ping()
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now - Context.Message.Timestamp;
            EmbedBuilder builder = new EmbedBuilder()
            .AddField("API 서버와의 지연 시간", Context.Client.Latency + "ms")
            .AddField("메세지와의 지연 시간", span.TotalMilliseconds + "ms");
            await ReplyAsync("", false, builder.Build());
        }
    }
}