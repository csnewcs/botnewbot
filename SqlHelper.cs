using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace csnewcs.Sql
{
    public class SqlHelper
    {
        readonly MySqlConnection conn;
        public SqlHelper(string server, string dbName, Dictionary<string, string[]> checkTables = null)
        {
            /*
            인자로 받는 것
            server: 서버 주소
            dbName: 데이터베이스 이름
            checkTables: 체크할 테이블 {
                    "테이블 이름": {"<Columm 이름>:<데이터 타입>", "<Columm 이름>:<데이터 타입>", "<Columm 이름>:<데이터 타입>"},
                    "테이블 이름": {"<Columm 이름>:<데이터 타입>", "<Columm 이름>:<데이터 타입>", "<Columm 이름>:<데이터 타입>"}
                }
            */
            string connString = $"server={server};Database={dbName};Uid={dbName};";
            conn = new MySqlConnection(connString);
            foreach (var table in checkTables)
            {
                conn.Open();
                string hasTableCommand = $"SELECT 1 FROM Information_schema.tables WHERE table_schema='{dbName}' AND table_name='{table.Key}';";
                MySqlCommand cmd = new MySqlCommand(hasTableCommand, conn);
                var result = cmd.ExecuteReader();
                bool notExists = true;
                while (result.Read())
                {
                    if ((int)result["1"] == 1) notExists = false;
                }
                result.Close();
                if (notExists) 
                {
                    hasTableCommand = $"CREATE TABLE {table.Key} (EMPTYCOLUMN TINYINT);";
                    cmd.CommandText = hasTableCommand;
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"{table.Key} 테이블 생성 완료");
                }
                foreach (var a in table.Value)
                {
                    string[] column = a.Split(':');
                    string hasColumnCommand = $"SELECT 1 FROM information_schema.columns WHERE table_schema='{dbName}' AND table_name='{table.Key}' AND column_name='{column[0]}';";
                    cmd.CommandText = hasColumnCommand;
                    notExists = true;
                    result = cmd.ExecuteReader();
                    while (result.Read())
                    {
                        if ((int)result["1"] == 1) notExists = false;
                    }
                    result.Close();

                    if (notExists)
                    {
                        hasTableCommand = $"ALTER TABLE {table.Key} ADD {column[0]} {column[1]};";
                        cmd.CommandText = hasTableCommand;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"{table.Key} 테이블의 {column[0]} 생성 완료");
                    }
                }
                conn.Close();
            }
        }
        public  void addDB()
        {
        }
        public  void addTable(string tableName, string[] columns)
        {
            string cmd = $"CREATE TABLE {tableName} (EMPTYCOLUMN TINYINT);";
            MySqlCommand command = new MySqlCommand(cmd, conn);
            conn.Open();
            command.ExecuteNonQuery();
            
            foreach (string aColumn in columns)
            {
                string[] column = aColumn.Split(':');
                cmd = $"ALTER TABLE {tableName} ADD {column[0]} {column[1]};";
                command.CommandText = cmd;
                command.ExecuteNonQuery();
            }
            conn.Close();
        }
        public void addColumn()
        {
        }
        public  void removeDB()
        {
        }
        public void removeTable()
        {
        }
        public void removeColumn()
        {
        }
        public object getData(string table, string whereColumn, object whereData, string getColumn)
        {
            string cmd = $"select * from {table} where {whereColumn} = '{whereData}';";
            // Console.WriteLine(cmd);
            conn.Open();
            MySqlCommand command = new MySqlCommand(cmd, conn);
            var reader = command.ExecuteReader();
            object turn = null;
            while (reader.Read())
            {
                turn =  reader[getColumn];
            }
            conn.Close();
            return turn;
        }
        public void addData(string table, string[] column, object[] data)
        {
            conn.Open();
            // string cmd = $"DESC {table};";
            // var reader = new MySqlCommand(cmd, conn).ExecuteReader();
            // Console.WriteLine(reader.GetSByte
            int i = 0;
            // while (reader.Read())
            // {
            //     Console.WriteLine(reader["1"]);
            // }

            string cmd = $"INSERT INTO {table} (";
            foreach (var a in column) cmd += a + ",";
            cmd = cmd.Substring(0, cmd.Length - 1);
            cmd += ") VALUES (";
            foreach (var a in data) 
            {                
                cmd += $"'{a}',";
            }
            cmd = cmd.Substring(0,cmd.Length - 1);
            cmd += ");";
            Console.WriteLine(cmd);

            conn.Open();
            MySqlCommand command = new MySqlCommand(cmd, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public void removeData(string table, string where, object whereData)
        {

        }
        public void setData(string table, string whereColumn ,  object whereData,string column,  object data)
        {
            string cmd = $"UPDATE {table} SET {column} = '{data}' WHERE {whereColumn} = '{whereData}';";
            conn.Open();
            MySqlCommand command = new MySqlCommand(cmd, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public bool tableExits(string dbName, string tableName)
        {
            conn.Open();
            string hasTableCommand = $"SELECT 1 FROM Information_schema.tables WHERE table_schema='{dbName}' AND table_name='{tableName}';";
            MySqlCommand cmd = new MySqlCommand(hasTableCommand, conn);
            var result = cmd.ExecuteReader();
            bool exists = false;
            while (result.Read())
            {
                if ((int)result["1"] == 1) exists = true;
                break;
            }
            result.Close();
            return exists;
        }
        public bool columnExits(string column)
        {
            return false;
        }
        public bool dataExits(string table, string whereColumn, object whereData)
        {
            string cmd = $"select 1 from {table} where {whereColumn} = '{whereData}';";
            conn.Open();
            MySqlCommand command = new MySqlCommand(cmd, conn);
            var result = command.ExecuteReader();
            bool success = false;
            while(result.Read())
            {
                if ((int)result["1"] == 1)
                {
                    success = true;
                    break;
                }
            }
            conn.Close();
            return success;
        }
        public bool[] tableExits(string[] tables)
        {
            return new bool[0];
        }  
        public bool[] columnExits(string[] columns)
        {
            return new bool[0];
        }
    }
}
