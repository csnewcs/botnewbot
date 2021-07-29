using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace botnewbot.BotData
{
    public class BotConfig
    {
        public static string BotToken { get; set; }
        public static string Prefix { get; set; }
        public static ulong AuthorID { get; } = 453554012353069090;
        public static void Init()
        {
            if (File.Exists("config.json"))
            {
                var json = JObject.Parse(File.ReadAllText("config.json"));
                BotToken = json["Token"].ToString();
                Prefix = json["Prefix"].ToString();
                return;
            }
            makeNewConfig();
        }
        private static void makeNewConfig()
        {
            Console.WriteLine("봇의 설정을 시작할게요.\n봇의 토큰을 입력해 주세요");
            string token = Console.ReadLine();
            Console.WriteLine("봇의 접두사를 입력해 주세요");
            string prefix = Console.ReadLine();
            BotToken = token;
            Prefix = prefix;
            JObject json = new JObject();
            json.Add("Token", BotToken);
            json.Add("Prefix", Prefix);
            File.WriteAllText("config.json", json.ToString());
        }
    }
}