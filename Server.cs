using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Discord.WebSocket;
using Discord;

namespace bot
{
    class Server
    {
        Dictionary<int, string[]> saveChannel = new Dictionary<int, string[]>(); //채널들 저장할 곳
        JObject save = new JObject(); //저장할 json
        int now = 0;
        public bool addServer(SocketGuild guild, SocketUser owner, string msg = "") //작업 받는 곳
        {
            switch (now)
            {
                case 0:
                    informEditedMessage(guild, owner); //메세지 수정시 어디에 알릴건지 보여주기
                    now = 1;
                    break;
                case 1:
                    if (answerEditedMessage(guild, msg, owner)) now = 2; //답변을 받고 메세지 삭제시 어디에 알릴건지 보여주기
                    else informEditedMessage(guild, owner); //새로고침
                    break;
                case 2:
                    if (answerDeleteMessage(guild, msg, owner)) now = 3; //답변을 받고 봇 관련 공지시 어디에 알릴건지 보여주기
                    else {Embed embed = selChannel(guild, "누군가가 메세지를 삭제"); owner.SendMessageAsync("", embed:embed);} //새로고침
                    break;
                case 3:
                    if (answerNoticeBot(guild, msg, owner)) //설정 완료되었는지 확인
                    {
                        foreach (SocketGuildUser user in guild.Users) //유저 추가
                        {
                            if (!user.IsBot) File.WriteAllText($"servers/{guild.Id}/{user.Id}","{\"power\":0, \"money\":100}");
                        }
                        File.Delete($"servers/{guild.Id}/{owner.Id}"); //소유자는 최대 권한 (0(블랙리스트): 명령어 사용 불가, 1(유저): 명령어 사용 가능, 2(관리자): 관리자 명령어 사용 가능, 3(소유자): 소유자 명령어 사용 가능, 유저 이하로 강등되지 않음)
                        File.WriteAllText($"servers/{guild.Id}/{owner.Id}","{\"power\":3, \"money\":100}");
                        return true;
                    }
                    else {Embed embed = selChannel(guild, "봇에 관한 공지를"); owner.SendMessageAsync("", embed:embed);} //새로고침
                    break;
            }
            return false; //위 switch에서 return이 되지 않았다면 여기서 return
        }
        private void informEditedMessage(SocketGuild guild, SocketUser owner)
        {
            Embed send = selChannel(guild, "누군가가 메세지를 수정");
            owner.SendMessageAsync("", embed:send);
        }
        private bool answerEditedMessage(SocketGuild guild, string message, SocketUser owner)
        {
            Embed send = selChannel(guild, "누군가가 메세지를 삭제"); //다음 질문 미리 준비
            if (message == "#") return false; //새로고침 요청
            else if (message == "0") //등록 거부
            {
                save.Add("editMessage",0);
                 EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xff0000))
                .AddField("설정 완료", "누군가가 메세지를 수정할 때 알리지 않기로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("",embed:embed);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                owner.SendMessageAsync("", embed:send);
                return true;
            }
            else //등록
            {
                try //제대로 된 값을 인식했을 때 수행
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
                catch //잘못된 값을 인식했을 때 수행
                {
                    owner.SendMessageAsync("저런, 그런 채널은 없어요. 채널 앞에 붙은 숫자로만 답해주세요.");
                    return false;
                }
            }
        }
        private bool answerDeleteMessage(SocketGuild guild, string message, SocketUser owner)
        {
            Embed send = selChannel(guild, "봇에 관한 공지를");
            if (message == "#") return false;
            else if (message == "0") 
            {
                save.Add("deleteMessage",0);
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xff0000))
                .AddField("설정 완료", "누군가가 메세지를 삭제할 때 알리지 않기로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("", embed:embed);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                owner.SendMessageAsync("", embed:send);
                return true;
            }
            else
            {
                try
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
                catch
                {
                    owner.SendMessageAsync("저런, 그런 채널은 없어요. 채널 앞에 붙은 숫자로만 답해주세요.");
                    return false;
                }
            }
        }
        private bool answerNoticeBot(SocketGuild guild, string message, SocketUser owner)
        {
            if (message == "#") return false;
            else if (message == "0")
            {
                save.Add("noticeBot",0);
                EmbedBuilder builder = new EmbedBuilder()
                .WithColor(new Color(0xff0000))
                .AddField("설정 완료", "이 봇에 관한 공지를 알리지 않기로 설정했습니다.");
                Embed embed = builder.Build();
                owner.SendMessageAsync("", embed:embed);
                File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                owner.SendMessageAsync("마지막 설정 정리를 하고 있습니다. 잠시만 기다려주세요");
                return true;
            }
            else
            {
                try
                {
                    save.Add("noticeBot", saveChannel[int.Parse(message)][0]);
                    File.WriteAllText($"servers/{guild.Id}/config.json", save.ToString());
                    EmbedBuilder builder = new EmbedBuilder()
                    .WithColor(new Color(0x00881d))
                    .AddField("설정 완료", $"이 봇에 관한 공지를 알릴 채널을 {saveChannel[int.Parse(message)][1]}로 설정했습니다.");
                    Embed embed = builder.Build();
                    owner.SendMessageAsync("", embed:embed);
                    owner.SendMessageAsync("마지막 설정 정리를 하고 있습니다. 잠시만 기다려주세요");
                    return true;
                }
                catch
                {
                    owner.SendMessageAsync("저런, 그런 채널은 없어요. 채널 앞에 붙은 숫자로만 답해주세요.");
                    return false;
                }
            }
        }
        private Embed selChannel(SocketGuild guild, string what) //질문 준비하는 곳(+ 새로고침 기능 겸)
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