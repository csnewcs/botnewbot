using System;
using System.Collections.Generic;
using System.Net;
using Discord;
using Discord.WebSocket;
using csnewcs.Game.GoStop;

namespace botnewbot.Support
{
    public class Game
    {
        Dictionary<SocketGuildChannel, GoStop> _gostop = new Dictionary<SocketGuildChannel, GoStop>();
        Dictionary<ulong, SocketGuildChannel> _turnPlayer = new Dictionary<ulong, SocketGuildChannel>();
        Dictionary<SocketGuildChannel, List<ulong>> _tempUsers = new Dictionary<SocketGuildChannel, List<ulong>>(); 
        Dictionary<ulong, KeyValuePair<Hwatu, Hwatu[]>> _selectGet = new Dictionary<ulong, KeyValuePair<Hwatu, Hwatu[]>>();
        Dictionary<ulong, Hwatu[]> _selectFieldGet = new Dictionary<ulong, Hwatu[]>();
        
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
        public Dictionary<ulong, SocketGuildChannel> turnPlayer
        {
            get
            {
                return _turnPlayer;
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
        public Dictionary<ulong, Hwatu[]> selectFieldGet
        {
            get {return _selectFieldGet;}
        }
        public Dictionary<ulong, KeyValuePair<Hwatu, Hwatu[]>> selectGet
        {
            get {return _selectGet;}
        }
    }
}