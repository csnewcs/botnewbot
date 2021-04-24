using System;
using Discord;
using Discord.WebSocket;

namespace botnewbot.Support
{
    public class Permission
    {
        public bool canSendMessage(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.SendMessages || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canSendFile(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.AttachFiles|| role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canManageMessage(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.ManageMessages || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canManageChannel(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.ManageChannels || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canManageRole(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.ManageRoles || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canKickMember(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.KickMembers || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool canBanMember(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.BanMembers || role.Permissions.Administrator) has = true;
            }
            return has;
        }

        public bool canMuteMember(SocketGuildUser user)
        {
            if(user.Guild.OwnerId == user.Id) return true;
            bool has = false;
            foreach(var role in user.Roles)
            {
                if(role.Permissions.MuteMembers || role.Permissions.Administrator) has = true;
            }
            return has;
        }
        public bool compareUserPosition(SocketGuildUser user, SocketGuildUser compare)
        {
            if(user.Id == user.Guild.OwnerId) return true;                      //본인이 관리자면 무조건 확인
            else if(compare.Id == user.Guild.OwnerId) return false; //비교하는 대상이 관리자면 무조건 확인
            int compareTop = 0;
            foreach(var role in compare.Roles)
            {
                if(role.Position > compareTop) compareTop = role.Position;
            }
            foreach(var role in user.Roles)
            {
                if(role.Position > compareTop) return true;
            }
            return false;
        }
        public bool compareRolePosition(SocketGuildUser user, SocketRole role)
        {
            int userRolePositionTop = 0;
            foreach (var userRole in user.Roles)
            {
                if(userRolePositionTop < userRole.Position) userRolePositionTop = userRole.Position;
            }
            Console.WriteLine(role.Position + " : " + userRolePositionTop);
            return role.Position < userRolePositionTop;
        }
    }
}