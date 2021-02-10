using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Ping : ModuleBase<SocketCommandContext>
{
    [Command("핑")]
    public async Task ping()
    {
        DateTime now = DateTime.Now;
        double messagePing = (now - Context.Message.CreatedAt).TotalMilliseconds;
        Random rd = new Random();
        uint color = (uint)rd.Next(0, 0xffffff);
        EmbedBuilder builder = new EmbedBuilder()
            .AddField("웹소켓 핑", $"{Context.Client.Latency}ms")
            .AddField("메세지 핑", $"{messagePing}ms")
            .WithColor(color);
        await ReplyAsync("", false, builder.Build());
    }
}