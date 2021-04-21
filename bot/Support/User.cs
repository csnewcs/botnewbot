using System;
using Discord.WebSocket;
using csnewcs.Sql;
using Discord;

namespace botnewbot.Support
{
    class User
    {
        private SqlHelper _helper;

        public void setSqlHelper(SqlHelper helper)
        {
            _helper = helper;
        }
        public string getNickName(SocketGuildUser user)
        {
            return string.IsNullOrEmpty(user.Nickname) ? user.Username : user.Nickname;
        }

        public bool exist(SocketGuildUser user)
        {
            return  _helper.dataExits("guild_" + user.Guild.Id.ToString(), "id", user.Id);
        }
        public void add(SocketGuildUser user)
        {
            string[] columns = new string[] {"id", "money"};
            object[] datas = new object[] {user.Id, 100};
            _helper.addData("guild_" + user.Guild.Id.ToString(), columns, datas);
        }

        public static void add(SocketGuildUser user, SqlHelper _helper)
        {
            string[] columns = new string[] {"id", "money"};
            object[] datas = new object[] {user.Id, 100};
            _helper.addData("guild_" + user.Guild.Id.ToString(), columns, datas);
        }
    }
}