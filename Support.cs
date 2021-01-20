using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

using Newtonsoft.Json.Linq;
using csnewcs.Sql;
using csnewcs.Game.GoStop;

namespace bot
{
    public class Support
    {
        SqlHelper sqlHelper;
        Dictionary<ulong, ulong> setting = new Dictionary<ulong, ulong>(); //현재 설정중인 것들 저장
        Dictionary<ulong, Server> server = new Dictionary<ulong, Server>(); //서버 객체 리스트
        Dictionary<ulong, ulong>  _helpMessages = new Dictionary<ulong, ulong>(); //메세지 ID, 사용자 ID
        Dictionary<SocketGuildChannel, GoStop> _gostop = new Dictionary<SocketGuildChannel, GoStop>();
        Dictionary<SocketGuildChannel, List<ulong>> _tempUsers = new Dictionary<SocketGuildChannel, List<ulong>>(); 
        public Dictionary<ulong, ulong> helpMessages
        {
            get
            {
                return _helpMessages;
            }
            set
            {
                _helpMessages = value;
            }
        }
        public Dictionary<SocketGuildChannel, GoStop> goStopGame
        {
            get
            {
                return _gostop;
            }
            set
            {
                _gostop = value;
            }
        }

        public Dictionary<SocketGuildChannel, List<ulong>> tempUsers
        {
            get
            {
                return _tempUsers;
            }
            set
            {
                _tempUsers = value;
            }
        }



        public Support(SqlHelper helper = null)
        {
            sqlHelper = helper;
        }
      
        public void changeSqlHelper(SqlHelper helper)
        {
            sqlHelper = helper;
        }
        public JObject getGuildConfig(SocketGuild guild)
        {
            return JObject.Parse(sqlHelper.getData("guild_" + guild.Id.ToString(), "id", "serverinfo", "serverinfo").ToString());
        }
        public long getMoney(SocketGuildUser user)
        {
            long money = (long)sqlHelper.getData("guild_" + user.Guild.Id, "id", user.Id, "money");
            // Console.WriteLine(money);
            return money;
        }
        public void setMoney(SocketGuildUser user, long money)
        {
            sqlHelper.setData("guild_" + user.Guild.Id.ToString(), "id", user.Id,  "money", money);
        }
        public void addUser(SocketGuildUser user)
        {
            string[] columns = new string[] {"id", "money"};
            object[] datas = new object[] {user.Id, 100};
            sqlHelper.addData("guild_" + user.Guild.Id.ToString(), columns, datas);
        }
        public bool userExists(SocketGuildUser user)
        {
            return  sqlHelper.dataExits("guild_" + user.Guild.Id.ToString(), "id", user.Id);
        }
        public bool guildExists(SocketGuild guild)
        {
            return sqlHelper.tableExits("botnewbot", "guild_" + guild.Id.ToString());
        }
        public void addGuild(SocketGuild guild)
        {
            sqlHelper.addTable("guild_" + guild.Id.ToString(), new string[] {
                    "id:varchar(20) NOT NULL",
                    "money:BIGINT",
                    "serverinfo:TEXT"
                });
            sqlHelper.addData("guild_" + guild.Id.ToString(), new string[] {"id", "serverinfo"}, new object[] {"serverinfo", @"{""editMessage"": 0, ""deleteMessage"": 0, ""noticeBot"": 0}"});
            foreach (var user in guild.Users)
            {
                if (user.IsBot) continue;
                addUser(user);
            }
        }
        public void delUser(SocketGuildUser user)
        {
            // sqlHelper.remove
        }
        public void delGuild(SocketGuild guild)
        {

        }

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
        public string unit(long money)
        {
            string result = "";
            long[] units = new long[5] {10000000000000000, 1000000000000, 100000000, 10000, 1};
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