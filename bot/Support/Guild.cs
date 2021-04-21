using System;
using Discord.WebSocket;

using csnewcs.Sql;

namespace botnewbot.Support
{
    public class Guild
    {
        private SqlHelper _helper;
        public void setSqlHelper(SqlHelper helper) => _helper = helper;
        public bool exists(SocketGuild guild)
        {
            return _helper.tableExits("botnewbot", "guild_" + guild.Id.ToString());
        }
        public void add(SocketGuild guild)
        {
            _helper.addTable("guild_" + guild.Id.ToString(), new string[] {
                "id:varchar(20) NOT NULL",
                "money:BIGINT",
                "serverinfo:TEXT"
            });
            _helper.addData("guild_" + guild.Id.ToString(), new string[] {"id", "serverinfo"}, new object[] {"serverinfo", @"{""editMessage"": 0, ""deleteMessage"": 0, ""noticeBot"": 0}"});
            foreach (var user in guild.Users)
            {
                if (user.IsBot) continue;
                User.add(user, _helper);
            }
        }
    }
    
}