using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json.Linq;

using botnewbot.Support;

namespace bot
{
    ////////////////////////////////////
    // 여기는 처벌 관련된 명령어 사용하는 곳 //
    ///////////////////////////////////
    
    [Group("처벌")]
    public class Punish : ModuleBase<SocketCommandContext>
    {
        Support support = new Support();
        Permission permission = new Permission();
        [Command]
        public async Task help() //명령어: $처벌
        {
            await ReplyAsync($"이 명령어는 이전되었습니다. '{Program.prefix}명령어'를 사용해 주세요");
            /*EmbedBuilder builder = new EmbedBuilder()
            .WithTitle("처벌 명령어 사용법")
            .WithColor(new Color(0xbe33ff))
            .AddField("뮤트", "지정한 사람의 마이크를 없앱니다.\n음성채팅방에서 다른 누구도 그 사람의 목소리를 들을 수 없습니다.\n(지정한 사람이 음성채팅방에 있어야 사용 가능)\n(사용법: $처벌 뮤트 [뮤트 시킬 사용자 언급(여러 명 가능)])")
            .AddField("킥", "지정한 사람을 서버에서 쫓아냅니다.\n단 다시 들어올 수 있습니다.\n(사용법: $처벌 킥 [킥 시킬 사용자 언급(여러 명 가능)])")
            .AddField("밴", "지정한 사람을 서버에서 쫓아냅니다.\n단 밴이 풀릴 때 까지 다시 들어올 수 없습니다.\n(사용법: $처벌 밴 [밴 시킬 사용자 언급(여러 명 가능)])");
            await Context.User.SendMessageAsync("", embed:builder.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");*/
        }
        [Command("뮤트", true)]
        public async Task mute() //명령어: $처벌 뮤트 <누군가를 멘션>
        {
            EmbedBuilder toSendEmbed = new EmbedBuilder();
            string toSendMessage = ""; //Embed를 보낼 수 없는 상황에 보낼 메세지
            SocketGuildUser user = Context.User as SocketGuildUser;
            SocketGuildUser bot = Context.Guild.GetUser(Context.Client.CurrentUser.Id);

            SocketMessage msg = Context.Message;
            var toMuteUsers = msg.MentionedUsers;
            if(toMuteUsers.Count == 0)
            {
                toSendEmbed.AddField("실패!", "이유: 언급된 사람이 없습니다.");
                toSendMessage = "실패!\n언급된 사람이 없습니다.";
            }
            else if(!permission.canManageChannel(user))
            {
                toSendEmbed.AddField("실패!", $"이유: {user.Nickname}님은 채널을 관리할 권한이 없습니다.");
                toSendMessage = $"실패!\n이유: {user.Nickname}님은 채널을 관리할 권한이 없습니다.";
            }
            else if(!permission.canManageChannel(bot))
            {
                toSendEmbed.AddField("실패!", "이유: 이 봇은 채널을 관리할 권한이 없습니다.");
                toSendMessage = "실패!\n이유: 이 봇은 채널을 관리할 권한이 없습니다.";
            }
            else
            {
                int muteUserCount = toMuteUsers.Count;
                var allChannels = Context.Guild.Channels;
                List<SocketGuildChannel> textChannels = new List<SocketGuildChannel>();
                foreach(var channel in  allChannels)
                {
                    if(channel is SocketTextChannel) textChannels.Add(channel);
                }
                foreach(var muteUser in toMuteUsers)
                {
                    SocketGuildUser muteGuildUser = muteUser as SocketGuildUser;
                    OverwritePermissions overwrite = new OverwritePermissions().Modify(sendMessages: PermValue.Deny);
                    foreach(var channel in textChannels)
                    {
                        if(user.Id == bot.Id)
                        {
                            toSendEmbed.AddField("뉴봇이는 슬퍼요", "저를 뮤트시키려 하시다니 너무 슬펴요ㅠㅠ");
                            toSendMessage = "뉴봇이는 슬퍼요\n저를 뮤트시키려 하시다니 너무 슬퍼요ㅠㅠ";
                            muteUserCount--;
                            continue;
                        }
                        await channel.AddPermissionOverwriteAsync(user, overwrite);
                    }
                }
                string addMessage = muteUserCount == 1 ? $"{toMuteUsers.First().Username}님" : $"{muteUserCount}분 의 뮤트 처리가 완료되었습니다.";

                toSendEmbed.AddField("성공!", addMessage);
                toSendMessage = addMessage;
            }
            try
            {
                await ReplyAsync("", false, toSendEmbed.Build());
            }
            catch
            {
                await ReplyAsync(toSendMessage + "\n\n봇이 Embed를 보낼 수 없어서 일반 메세지로 대체되었습니다. 봇의 권한을 확인해 주세요.");
            }
        }
        [Command("킥", true)]
        public async Task kick()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string instead = "";
            
            User user = new User();
            Permission permission = new Permission();
            
            SocketGuildUser guildUser = Context.User as SocketGuildUser;
            if(permission.canKickMember(Context.Guild.GetUser(Context.Client.CurrentUser.Id)))
            {
                builder.AddField("작업 실패", "이 봇에겐 멤버를 킥할 권한이 없어요. 권한을 확인해 주세요.");
                instead += "작업 실패\n이 봇에겐 멤버를 킥할 권한이 없어요. 권한을 확인해 주세요.";
            }
            else if (permission.canKickMember(guildUser))
            {
                SocketMessage msg = Context.Message;
                var kickUsers = msg.MentionedUsers;
                if (kickUsers.Count == 0)
                {
                    builder.AddField("작업 실패", "킥 할 분들을 멘션해 주세요.");
                    instead += "작업 실패\n킥 할 분들을 멘션해 주세요.";
                }
                else if (kickUsers.Count != 1)
                {
                    builder.AddField("작업 완료", $"{user.getNickName(kickUsers.First() as SocketGuildUser)}외 {kickUsers.Count}분의 킥 처리가 완료되었습니다.");
                    instead += $"작업 완료\n{user.getNickName(kickUsers.First() as SocketGuildUser)}외 {kickUsers.Count}분의 킥 처리가 완료되었습니다.";
                }
                else 
                {
                    builder.AddField("작업 완료", $"{user.getNickName(kickUsers.First() as SocketGuildUser)}님의 킥 처리가 완료되었습니다.");
                    instead += $"작업 완료\n{user.getNickName(kickUsers.First() as SocketGuildUser)}님의 킥 처리가 완료되었습니다.";
                }
            }
            else
            {
                builder.AddField("작업 실패", "당신은 이 명령어를 사용할 권한이 없어요.");
                instead += "작업 실패\n당신은 이 명령어를 사용할 권한이 없어요.";
            }

            try
            {
                await ReplyAsync("", false, builder.Build());
            }
            catch
            {
                await ReplyAsync(instead + "\n\n봇이 Embed를 보낼 수 없어서 일반 메세지로 대체되었습니다. 봇의 권한을 확인해 주세요.");
            }
        }
        // [Command("밴", true)]
        // public async Task ban()
        // {
        //     SocketGuildUser user = Context.User as SocketGuildUser;
        //     SocketMessage msg = Context.Message;
        //     var banUsers = msg.MentionedUsers;
        //     if (!(support.hasPermission(user, Support.Permission.BanUser) && support.isOver(user, banUsers)) || banUsers.Count == 0)
        //     {
        //         return;
        //     }
        //     foreach (var a in banUsers)
        //     {
        //         await (a as SocketGuildUser).BanAsync();
        //     }
        //     Random rd = new Random();
        //     if (banUsers.Count != 1)
        //     {
        //         EmbedBuilder builder = new EmbedBuilder()
        //         .WithColor((uint)rd.Next(0x000000, 0xffffff))
        //         .AddField("작업 완료", $"{support.getNickname(banUsers.First() as SocketGuildUser)}외 {banUsers.Count}분의 밴 처리가 완료되었습니다.");

        //         await msg.Channel.SendMessageAsync("", embed:builder.Build());
        //     }
        //     else 
        //     {
        //         EmbedBuilder builder = new EmbedBuilder()
        //         .WithColor((uint)rd.Next(0x000000, 0xffffff))
        //         .AddField("작업 완료", $"{support.getNickname(banUsers.First() as SocketGuildUser)}님의 밴 처리가 완료되었습니다.");
        //         await msg.Channel.SendMessageAsync("", embed:builder.Build());
        //     }
        // }
    }
}