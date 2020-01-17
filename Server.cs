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
        public bool addServer(SocketGuild guild, string msg = "", SocketUser owner = null) //작업 받는 곳
        {
            switch (now)
            {
                case 0:
                    informEditedMessage(guild, owner);
                    now = 1;
                    break;
                case 1:
                    if (answerEditedMessage(guild, msg)) now = 2;
                    else informEditedMessage(guild, owner);
                    break;
            }

            return false;
        }
        private void informEditedMessage(SocketGuild guild, SocketUser owner)
        {
            int index = 1;
            string send = "메세지가 수정되었을 때 알릴 채널을 선택해 주세요. (알리지 않을 거라면 0번을, 새로고침은 #을 눌러주세요)\n================================================================\n";
            foreach (var channel in guild.Channels) //텍스트 채널만 리스트에 추가해 보내기
            {
                if (channel is SocketTextChannel) 
                {
                    saveChannel.Add(index, new string[2] {channel.Id.ToString(), channel.Name});
                    send += $"{index}: {channel.Name}\n";
                    index++;
                }
            }
            send += "================================================================";
            Console.WriteLine(send);
            owner.SendMessageAsync(send);
        }
        private bool answerEditedMessage(SocketGuild guild, string message)
        {
            if (message == "#") return false;
            else if (message == "0") {return true;}
            else
            {
                save.Add("editMessage", saveChannel[int.Parse(message)][0]);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                return true;
            }
        }
    }
}