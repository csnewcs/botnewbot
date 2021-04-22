using System;
using csnewcs.Sql;
using Discord.WebSocket;

namespace botnewbot.Support
{
    public class Money
    {
        private SqlHelper _helper;
        public Money(SqlHelper helper = null)
        {
            _helper = helper;
        }

        public void setSqlHelper(SqlHelper helper)
        {
            _helper = helper;
        }
        public long getUserMoney(SocketGuildUser user)
        {
            long money = (long) _helper.getData($"guild_{user.Guild.Id}", "id", user.Id, "money");
            return money;
        }
        public long getUserMoney(ulong guildId, ulong id)
        {
            long money = (long) _helper.getData($"guild_{guildId}", "id", id, "money");
            return money;
        }

        public void setMoney(SocketGuildUser user, long money)
        {
            _helper.setData($"guild_{user.Guild.Id}", "id", user.Id, "money", money);
        }
        public void setMoney(ulong guildId, ulong id, long money)
        {
            _helper.setData($"guild_{guildId}", "id", id, "money", money);
        }

        public bool addMoney(SocketGuildUser user, long addMoney)
        {
            long money = getUserMoney(user);
            money += addMoney;
            if (money < 0) return false;
            setMoney(user, money);
            return true;
        }
        public bool addMoney(ulong guildId, ulong id, long addMoney)
        {
            long money = getUserMoney(guildId, id);
            money += addMoney;
            if (money < 0) return false;
            setMoney(guildId, id, money);
            return true;
        }
        
        public string unit(long money)
        {
            if (money == 0) return "0";
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
    }
}