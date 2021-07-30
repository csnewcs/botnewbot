using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

using botnewbot.Services;

namespace botnewbot.Handlers
{
    public class SqlHandler
    {
        private readonly MySqlConnection connection;
        public SqlHandler()
        {
            connection = new MySqlConnection("server=localhost;Database=botnewbot;Uid=botnewbot;SslMode=none;");
        }
        private void createGuildMoneyTableIfNotExist(ulong guildId)
        {
            connection.Open();
            string cmd = $"CREATE TABLE IF NOT EXISTS money_{guildId} (id BIGINT UNSIGNED NOT NULL, money BIGINT UNSIGNED NOT NULL);";
            using (var command = new MySqlCommand(cmd, connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        private bool checkGuildMoneyTableExist(ulong guildId)
        {
            connection.Open();
            string hasTableCommand = $"SELECT 1 FROM Information_schema.tables WHERE table_schema='botnewbot' AND table_name='money_{guildId}';";
            MySqlCommand cmd = new MySqlCommand(hasTableCommand, connection);
            var result = cmd.ExecuteReader();
            bool exists = false;
            while (result.Read())
            {
                if ((int)result["1"] == 1) exists = true;
                break;
            }
            result.Close();
            connection.Close();
            return exists;
        }
        // public void createGuildSettingTableIfNotExist(ulong guildId)
        // {
            
        // }
        public bool getAllMoneyFromOneGuild(ulong guildId, out Dictionary<ulong, ulong> money)
        {
            money = new Dictionary<ulong, ulong>();
            /*
              멤버ID : 돈
            */
            bool result = false;
            if(checkGuildMoneyTableExist(guildId))
            {
                connection.Open();
                using (var command = new MySqlCommand($"SELECT * FROM money_{guildId}", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            money.Add((ulong)reader["id"], (ulong)reader["money"]);
                        }
                    }
                    result = true;
                }
                connection.Close();
            }
            return result;
        }
        
        public ulong getUserMoney(ulong userId, ulong guildId)
        {
            ulong money = 100;
            if(!checkGuildMoneyTableExist(guildId)) createGuildMoneyTableIfNotExist(guildId);            
            if(userExist(userId, guildId))
            {
                connection.Open();
                using (var command = new MySqlCommand($"SELECT money FROM money_{guildId} WHERE id = {userId}", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            money = Convert.ToUInt64(reader["money"]);
                        }
                    }
                }
            }
            else
            {
                addUser(userId, guildId);
            }
            return money;
        }
        public void setUserMoney(ulong userId, ulong guildId, ulong money)
        {
            if(!checkGuildMoneyTableExist(guildId)) createGuildMoneyTableIfNotExist(guildId);
            connection.Open();
            using (var command = new MySqlCommand($"UPDATE money_{guildId} SET money = {money} WHERE id = {userId};", connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        private bool userExist(ulong userId, ulong guildId)
        {
            connection.Open();
            object result;
            using (var command = new MySqlCommand($"SELECT 1 FROM money_{guildId} WHERE id = {userId};", connection))
            {
                result = command.ExecuteScalar();
            }
            connection.Close();
            return result != null;
        }
        private void addUser(ulong userId, ulong guildId)
        {
            Console.WriteLine("C");
            connection.Open();
            using (var command = new MySqlCommand($"INSERT INTO money_{guildId} (id, money) VALUES ({userId}, 100);", connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}