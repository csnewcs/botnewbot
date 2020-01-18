using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace bot
{
    class Server
    {
        Dictionary<int, string[]> saveChannel = new Dictionary<int, string[]>();
        JObject save = new JObject();
        int now = 0;
        public bool addServer(SocketGuild guild, SocketUser owner, string msg = "") //작업 받는 곳
        {
            switch (now)
            {
                case 0:
                    informEditedMessage(guild, owner);
                    now = 1;
                    break;
                case 1:
                    if (answerEditedMessage(guild, msg, owner)) now = 2;
                    else informEditedMessage(guild, owner);
                    break;
                case 2:
                    if (answerDeleteMessage(guild, msg, owner)) now = 3;
                    else {Embed embed = selChannel(guild, "누군가가 메세지를 삭제"); owner.SendMessageAsync("", embed:embed);}
                    break;
            }

            return false;
        }
        private void informEditedMessage(SocketGuild guild, SocketUser owner)
        {
            Embed send = selChannel(guild, "누군가가 메세지를 수정");
            owner.SendMessageAsync("", embed:send);
        }
        private bool answerEditedMessage(SocketGuild guild, string message, SocketUser owner)
        {
            Embed send = selChannel(guild, "누군가가 메세지를 삭제");
            if (message == "#") return false;
            else if (message == "0") 
            {
                 EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xff0000))
                .AddField("설정 완료", "누군가가 메세지를 수정할 때 알리지 않기로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("",embed:embed);
                File.WriteAllText($"servers/{guild.Id}/config.json", "{}");
                owner.SendMessageAsync("", embed:send);
                return true;
            }
            else
            {
                save.Add("editMessage", saveChannel[int.Parse(message)][0]);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0x00881d))
                .AddField("설정 완료", $"누군가가 메세지를 수정할 때 알릴 채널을 {saveChannel[int.Parse(message)][1]}로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("", embed:embed);
                System.Threading.Thread.Sleep(50);
                owner.SendMessageAsync("", embed:send);
                return true;
            }
        }
        private bool answerDeleteMessage(SocketGuild guild, string message, SocketUser owner)
        {
            Embed send = selChannel(guild, "봇에 관한 공지를");
            if (message == "#") return false;
            else if (message == "0") 
            {
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xff0000))
                .AddField("설정 완료", "누군가가 메세지를 삭제할 때 알리지 않기로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("", embed:embed);
                File.WriteAllText($"servers/{guild.Id}/config.json", "{}");
                owner.SendMessageAsync("", embed:send);
                return true;
            }
            else
            {
                save.Add("deleteMessage", saveChannel[int.Parse(message)][0]);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0x00881d))
                .AddField("설정 완료", $"누군가가 메세지를 삭제할 때 알릴 채널을 {saveChannel[int.Parse(message)][1]}로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("", embed:embed);
                System.Threading.Thread.Sleep(50);
                owner.SendMessageAsync("", embed:send);
                return true;
            }
        }
        private Embed selChannel(SocketGuild guild, string what)
        {
            int index = 1;
            saveChannel.Clear();
            EmbedBuilder builder = new EmbedBuilder()
            .WithTitle($"{what}할 때 알릴 채널을 선택해 주세요. (알리지 않을 거라면 0번을, 새로고침은 #을 눌러주세요)");
            string send = "";
            foreach (var channel in guild.Channels) //텍스트 채널만 리스트에 추가해 보내기
            {
                if (channel is SocketTextChannel) 
                {
                    saveChannel.Add(index, new string[2] {channel.Id.ToString(), channel.Name});
                    send += $"{index}: {channel.Name}\n";
                    index++;
                }
            }
            builder.AddField("채널 목록", send);
            Embed embed = builder.Build();
            return embed;
        }
    }
}