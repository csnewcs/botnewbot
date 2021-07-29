using System;
using botnewbot.Services;

namespace botnewbot
{
    class Program
    {
        static void Main(string[] args) => new DiscordService().Init().GetAwaiter().GetResult();
    }
}
