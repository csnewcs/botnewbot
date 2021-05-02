using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using botnewbot.Support;

namespace bot
{
    /////////////////////////////////
    // 여기는 사람들의 역할을 관리하는 곳 //
    /////////////////////////////////
    [Group("역할")]
    public class Role : ModuleBase<SocketCommandContext>
    {
        Permission permission = new Permission();
        private User _user = new User();
        [Command]
        public async Task helpRole()
        {
            await ReplyAsync($"이 명령어는 이전되었습니다. '{Program.prefix}명령어'를 사용해 주세요");
            /*EmbedBuilder build = new EmbedBuilder()
            .WithTitle("역할 명령어 도움말")
            .WithColor(new Color(0xbe33ff))
            .AddField("부여", "사람 혹은 사람들에게 역할을 부여합니다.\n(사용법: $역할 부여 [역할 부여할 사람 멘션 (여러명 가능)] [@부여할 역할 멘션 (여러개 가능)])")
            .AddField("부여 모두", "모든 사람에게 역할을 부여합니다.\n(사용법: $역할 부여 모두 [부여할 역할 멘션 (여러개 가능)])")
            .AddField("제거", "사람 혹은 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 제거 [역할을 없앨 사람 멘션 (여러명 가능)] [@빼앗을 역할 멘션 (여러개 가능)])")
            .AddField("제거 모두", "모든 사람들에게서 역할을 뺏습니다.\n(사용법: $역할 제거 모두 [빼앗을 역할 멘션 (여러개 가능)])");
            await Context.User.SendMessageAsync("", embed:build.Build());
            await ReplyAsync("DM으로 결과를 전송했습니다.");*/
        }
        [Command("부여", true)]
        public async Task giveRole()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string instead = "";
            SocketGuildUser moder = Context.Guild.GetUser(Context.User.Id);
            SocketGuildUser bot = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            var mentionedUsers = Context.Message.MentionedUsers;
            var mentionedRoles = Context.Message.MentionedRoles;
            
            if (!permission.canManageRole(moder)) //만약 명령어를 실행한 사람이 권한이 없을 때
            {
                builder.AddField("실패!", "당신은 역할을 관리할 권한이 없어요.");
                instead += "실패!\n당신은 역할을 관리할 권한이 없어요.";
            }
            else if (!permission.canManageRole(bot)) //실행한 사람은 권한이 있으나 봇에겐 권한이 없을 때
            {
                builder.AddField("실패!", "저는 역할을 관리할 권한이 없어요. 권한을 확인해 주세요.");
                instead += "실패!\n저는 역할을 관리할 권한이 없어요. 권한을 확인해 주세요.";
            }
            else if (mentionedRoles.Count > 20)
            {
                builder.AddField("실패!", "역할 부여는 한 번에 최대 20개의 역할만 가능해요.");
                instead += "실패!\n역할 부여는 한 번에 최대 20개의 역할만 가능해요.";
            }
            else if (mentionedRoles.Count == 0 || mentionedUsers.Count == 0)
            {
                builder.AddField("실패!", "아무도 멘션하지 않았거나 아무 역할도 멘션하지 않았어요.");
                instead += "실패!\n아무도 멘션하지 않았거나 아무 역할도 멘션하지 않았어요.";
            }
            else //모두 통과를 했을 때
            {
                foreach (var role in mentionedRoles)
                {
                    if (!permission.compareRolePosition(moder, role))
                    {
                        builder.AddField("작업 실패!", $"```{role.Name}을 부여하는데 실패했어요. (이유: 부여하고자 하는 역할이 사용자보다 위에 있거나 같은 위치에요.)```");
                        instead += $"\n{role.Name}을 부여하는데 실패했어요. (이유: 부여하고자 하는 역할이 사용자보다 위에 있거나 같은 위치에요.)";
                    }
                    else if (permission.compareRolePosition(bot, role)) 
                    {
                        foreach (var user in mentionedUsers)
                        {
                            SocketGuildUser guildUser = user as SocketGuildUser;
                            if(!guildUser.Roles.Contains(role)) await guildUser.AddRoleAsync(role);
                        }
                        builder.AddField("작업 성공", $"```{_user.getNickName(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1}명 에게 {role.Name} 역할을 부여했어요.```");
                        instead += $"\n작업 성공\n{_user.getNickName(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1}명 에게 {role.Name} 역할을 부여했어요.";    
                    }
                    else //만약 내가 넣고자 하는 역할이 나보다 더 위라면(=관리할 수 없다면)
                    {
                        builder.AddField("작업 실패", $"```{role.Name}을 부여하는데 실패했어요. (이유: 부여하고자 하는 역할이 봇보다 위에 있거나 같은 위치에요.)```");
                        instead += $"작업 실패\n{role.Name}을 부여하는데 실패하였습니다. (이유: 부여하고자 하는 역할이 봇보다 위에 있거나 같은 위치에요.)\n";
                    }
                    
                    
                }
            }

            try
            {
                await ReplyAsync("", false, builder.Build());
            }
            catch
            {
                await ReplyAsync($"{instead}\n\nEmbed를 보낼 권한이 없어 일반 텍스트로 대체되었어요. 권한을 확인해 주세요.");
            }
            // try
            // {
            //     SocketGuildUser user = Context.User as SocketGuildUser;
            //     SocketMessage message = Context.Message;
            //     if (!(permission.compareRolePosition(user, message.MentionedRoles) && support.hasPermission(user, Support.Permission.ManageRole)))
            //     {
            //         await ReplyAsync("봇의 권한이 없거나 명령어를 사용하는 사람의 권한이 없어요.");
            //         return;
            //     }
            //     var mentionedUsers = message.MentionedUsers;
            //     var mentionedRoles = message.MentionedRoles;
            //     if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0)
            //     {
            //         await ReplyAsync("유저와 역할을 멘션했는지 확인하세요");
            //         return;
            //     } 
            //     foreach (SocketGuildUser a in mentionedUsers)
            //     {
            //         await a.AddRolesAsync(mentionedRoles);
            //     }
            //     if (mentionedUsers.Count == 1)
            //     {
            //         if (mentionedRoles.Count == 1)
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //         else
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //     }
            //     else
            //     {
            //         if (mentionedRoles.Count == 1)
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}'역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //         else
            //         {
            //             Random rd = new Random();
            //             EmbedBuilder embedBuilder = new EmbedBuilder()
            //             .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //             .AddField("역할 부여 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들께 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할 부여가 완료되었습니다.");
            //             await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //         }
            //     }
            // }
            // catch (Exception e)
            // {
            //     await ReplyAsync("```" + e.ToString() + "```");
            // }
        }
        [Command("제거", true)]
        public async Task ridRole()
        {
            EmbedBuilder builder = new EmbedBuilder();
            string instead = "";
            SocketGuildUser moder = Context.Guild.GetUser(Context.User.Id);
            SocketGuildUser bot = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
            var mentionedUsers = Context.Message.MentionedUsers;
            var mentionedRoles = Context.Message.MentionedRoles;
            
            if (!permission.canManageRole(moder)) //만약 명령어를 실행한 사람이 권한이 없을 때
            {
                builder.AddField("실패!", "당신은 역할을 관리할 권한이 없어요.");
                instead += "실패!\n당신은 역할을 관리할 권한이 없어요.";
            }
            else if (!permission.canManageRole(bot)) //실행한 사람은 권한이 있으나 봇에겐 권한이 없을 때
            {
                builder.AddField("실패!", "저는 역할을 관리할 권한이 없어요. 권한을 확인해 주세요.");
                instead += "실패!\n저는 역할을 관리할 권한이 없어요. 권한을 확인해 주세요.";
            }
            else if (mentionedRoles.Count > 20)
            {
                builder.AddField("실패!", "역할 부여는 한 번에 최대 20개의 역할만 가능해요.");
                instead += "실패!\n역할 제거은 한 번에 최대 20개의 역할만 가능해요.";
            }
            else if (mentionedRoles.Count == 0 || mentionedUsers.Count == 0)
            {
                builder.AddField("실패!", "아무도 멘션하지 않았거나 아무 역할도 멘션하지 않았어요.");
                instead += "실패!\n아무도 멘션하지 않았거나 아무 역할도 멘션하지 않았어요.";
            }
            else //모두 통과를 했을 때
            {
                foreach (var role in mentionedRoles)
                {
                    if (!permission.compareRolePosition(moder, role))
                    {
                        builder.AddField("작업 실패!", $"```{role.Name}을 제거하는데 실패했어요. (이유: 부여하고자 하는 역할이 사용자보다 위에 있거나 같은 위치에요.)```");
                        instead += $"\n{role.Name}을 제거하는데 실패했어요. (이유: 부여하고자 하는 역할이 사용자보다 위에 있거나 같은 위치에요.)";
                    }
                    else if (permission.compareRolePosition(bot, role)) 
                    {
                        foreach (var user in mentionedUsers)
                        {
                            SocketGuildUser guildUser = user as SocketGuildUser;
                            if(guildUser.Roles.Contains(role)) await guildUser.RemoveRoleAsync(role);
                        }
                        builder.AddField("작업 성공", $"```{_user.getNickName(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1}명 에게서 {role.Name} 역할을 제거했어요.```");
                        instead += $"\n작업 성공\n{_user.getNickName(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1}명 에게서 {role.Name} 역할을 제거했어요.";    
                    }
                    else //만약 내가 넣고자 하는 역할이 나보다 더 위라면(=관리할 수 없다면)
                    {
                        builder.AddField("작업 실패", $"```{role.Name}을 제거하는데 실패했어요. (이유: 부여하고자 하는 역할이 봇보다 위에 있거나 같은 위치에요.)```");
                        instead += $"작업 실패\n{role.Name}을 제거하는데 실패했어요. (이유: 부여하고자 하는 역할이 봇보다 위에 있거나 같은 위치에요.)\n";
                    }
                    
                    
                }
            }

            try
            {
                await ReplyAsync("", false, builder.Build());
            }
            catch
            {
                await ReplyAsync($"{instead}\n\nEmbed를 보낼 권한이 없어 일반 텍스트로 대체되었어요. 권한을 확인해 주세요.");
            }

            // if (!(support.hasPermission(user, Support.Permission.ManageRole) && support.isOver(user, mentionedRoles)))
            // {
            //     return;
            // }
            // if (mentionedUsers.Count == 0 || mentionedRoles.Count == 0) return;

            // foreach (SocketGuildUser a in mentionedUsers)
            // {
            //     await a.RemoveRolesAsync(mentionedRoles);
            // }
            // if (mentionedUsers.Count == 1)
            // {
            //     if (mentionedRoles.Count == 1)
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 제거 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            //     else
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 제거 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들의 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            // }
            // else
            // {
            //     if (mentionedRoles.Count == 1)
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 제거 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}'역할이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            //     else
            //     {
            //         Random rd = new Random();
            //         EmbedBuilder embedBuilder = new EmbedBuilder()
            //         .WithColor(rd.Next(0, 256),rd.Next(0, 256),rd.Next(0, 256))
            //         .AddField("역할 제거 완료", $"{support.getNickname(mentionedUsers.First() as SocketGuildUser)}님 외 {mentionedUsers.Count - 1} 분들의 '{mentionedRoles.First()}' 외 {mentionedRoles.Count - 1}개의 역할들이 사라졌습니다.");
            //         await message.Channel.SendMessageAsync("", embed:embedBuilder.Build());
            //     }
            // }
        }
    }
}