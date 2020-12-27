using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace bot
{
    class Support
    {
        public Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        public Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        public string getNickname(SocketGuildUser guild)
        {
            if (string.IsNullOrEmpty(guild.Nickname))
            {
                return guild.Username;
            }
            else
            {
                return guild.Nickname;
            }
        }
        public string unit(ulong money)
        {
            string result = "";
            ulong[] units = new ulong[5] {10000000000000000, 1000000000000, 100000000, 10000, 1};
            char[] unitChars = new char[5] {'경', '조', '억', '만', '\0'};
            int index = 0;
            foreach (var number in units)
            {
                if (number < money)
                {
                    result += $"{money/number}{unitChars[index]} ";
                    money %= number;
                }
                index++;
            }
            return result;
        }
        public async Task reset(SocketGuildUser user)
        {
            if (!hasPermission(user, Permission.Admin))
            {
                return;
            }
            SocketGuild guild = user.Guild;
            File.Delete($"servers/{guild.Id}/config.json");
            setting.Add(user.Id, guild.Id);
            server.Add(user.Id, new Server());
            await user.SendMessageAsync("초기 설정을 시작합니다.");
            server[user.Id].addServer(guild, user);
        }
        public bool isOver(SocketGuildUser first, SocketGuildUser second) //위에 있는 역할일수록 수가 큼
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles, two = second.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in two)
            {
                if (a.Position > twoTop) twoTop = a.Position;
            }
            return oneTop > twoTop;
        }
        public bool isOver(SocketGuildUser first, IReadOnlyCollection<SocketUser> second)
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in second)
            {
                foreach (var b in (a as SocketGuildUser).Roles)
                {
                    if (b.Position > twoTop) twoTop = b.Position;
                }
            }
            return oneTop > twoTop;
        }
        public bool isOver(SocketGuildUser first, IReadOnlyCollection<SocketRole> second)
        {
            if (first.Id == first.Guild.OwnerId)
            {
                return true;
            }
            IReadOnlyCollection<SocketRole> one = first.Roles;
            int oneTop = 0;
            int twoTop = 0;
            foreach (var a in one)
            {
                if (a.Position > oneTop) oneTop = a.Position;
            }
            foreach (var a in second)
            {
                if (a.Position > twoTop) twoTop = a.Position;
            }
            return oneTop > twoTop;
        }
        public bool hasPermission(SocketGuildUser user, Permission p)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return true;
            }
            bool has = false;
            foreach (var a in user.Roles) //더 빠른 알고리즘이 생각났지만 코드가 너무 더러워 질 것 같ㄷ...
            {
                if (a.Permissions.Administrator)
                {
                    has = true;
                }
               switch (p)
               {
                    case Permission.BanUser:
                        if (a.Permissions.BanMembers) has = true;
                        break;
                    case Permission.KickUser:
                        if (a.Permissions.KickMembers) has = true;
                        break;
                    case Permission.DeleteMessage:
                        if (a.Permissions.ManageMessages) has = true;
                        break;
                    case Permission.ManageRole:
                        if (a.Permissions.ManageRoles) has = true;
                        break;
                    case Permission.MuteUser:
                        if (a.Permissions.MuteMembers) has = true;
                        break;
               }
               if (has) break;
            }
            return has;
        }
        public enum Permission //며칠 전에 이거 책에서 봐서 다행이네
        {
            DeleteMessage,
            BanUser,
            KickUser,
            MuteUser,
            ManageRole,
            Admin
        }
    }
}