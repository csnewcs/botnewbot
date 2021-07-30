using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace botnewbot.Commands
{
    using System.Threading.Tasks;
    public class Information : ModuleBase<SocketCommandContext>
    {
        [Command("정보")]
        [Alias("info")]
        public async Task info()
        {
            var guilds = Context.Client.Guilds;
            int guildCount = guilds.Count;
            int userCount = 0;
            int channelCount = 0;
            List<ulong> userList = new List<ulong>();
            foreach(var guild in guilds)
            {
                foreach(var channel in guild.Channels)
                {
                    if(channel is SocketTextChannel) channelCount++;
                }
                foreach(var user in guild.Users)
                {
                    if(!(user.IsBot || user.IsWebhook))
                    {
                        userCount++;
                        if(!userList.Contains(user.Id)) userList.Add(user.Id);
                    } 
                }
            }
            EmbedBuilder builder = new EmbedBuilder()
            .AddField("이 봇이 들어간 서버 수", $"`{guildCount}개의 서버`")
            .AddField("이 봇이 들어간 채널 수", $"`{channelCount}개의 채팅 채널`")
            .AddField("이 봇의 사용자 수(중복 포함)", $"`{userCount}`명")
            .AddField("이 봇의 사용자 수(중복 제외)", $"`{userList.Count}`명"); //SQL 작업 완료시 그걸 기준으로
            await ReplyAsync("", embed: builder.Build());
        }
    }
}