using System;
using System.Collections.Generic;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace csnewcs.Game.GoStop
{
    public class GoStop
    {
        int hwatuCardCount = 50;
        int playerCount;
        const string hwatuPath = "./HwatuImages";
        Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();

        Hwatu[] allHwatus = new Hwatu[50] {
            new Hwatu(Month.Jan, HwatuType.Light, Image.Load($"{hwatuPath}/1Light.png")), new Hwatu(Month.Jan, HwatuType.Pea, Image.Load($"{hwatuPath}/1Pea1.png")), new Hwatu(Month.Jan, HwatuType.Pea, Image.Load($"{hwatuPath}/1Pea2.png")), new Hwatu(Month.Jan, HwatuType.RedBelt, Image.Load($"{hwatuPath}/1RedBelt.png")),
            new Hwatu(Month.Feb, HwatuType.Bird, Image.Load($"{hwatuPath}/2Bird.png")), new Hwatu(Month.Feb, HwatuType.Pea, Image.Load($"{hwatuPath}/2Pea1.png")), new Hwatu(Month.Feb, HwatuType.Pea, Image.Load($"{hwatuPath}/2Pea2.png")), new Hwatu(Month.Feb, HwatuType.RedBelt, Image.Load($"{hwatuPath}/2RedBelt.png")),
            new Hwatu(Month.Mar, HwatuType.Light, Image.Load($"{hwatuPath}/3Light.png")), new Hwatu(Month.Mar, HwatuType.Pea, Image.Load($"{hwatuPath}/3Pea1.png")), new Hwatu(Month.Mar, HwatuType.Pea, Image.Load($"{hwatuPath}/3Pea2.png")), new Hwatu(Month.Mar, HwatuType.RedBelt, Image.Load($"{hwatuPath}/3RedBelt.png")),
            new Hwatu(Month.Apr, HwatuType.Bird, Image.Load($"{hwatuPath}/4Bird.png")), new Hwatu(Month.Apr, HwatuType.Pea, Image.Load($"{hwatuPath}/4Pea1.png")), new Hwatu(Month.Apr, HwatuType.Pea, Image.Load($"{hwatuPath}/4Pea2.png")), new Hwatu(Month.Apr, HwatuType.GrassBelt, Image.Load($"{hwatuPath}/4GrassBelt.png")),
            new Hwatu(Month.May, HwatuType.Etc, Image.Load($"{hwatuPath}/5Etc.png")), new Hwatu(Month.May, HwatuType.Pea, Image.Load($"{hwatuPath}/5Pea1.png")), new Hwatu(Month.May, HwatuType.Pea, Image.Load($"{hwatuPath}/5Pea2.png")), new Hwatu(Month.May, HwatuType.GrassBelt, Image.Load($"{hwatuPath}/5GrassBelt.png")),
            new Hwatu(Month.Jun, HwatuType.Etc, Image.Load($"{hwatuPath}/6Etc.png")), new Hwatu(Month.Jun, HwatuType.Pea, Image.Load($"{hwatuPath}/6Pea1.png")), new Hwatu(Month.Jun, HwatuType.Pea, Image.Load($"{hwatuPath}/6Pea2.png")), new Hwatu(Month.Jun, HwatuType.BlueBelt, Image.Load($"{hwatuPath}/6BlueBelt.png")),
            new Hwatu(Month.Jul, HwatuType.Etc, Image.Load($"{hwatuPath}/7Etc.png")), new Hwatu(Month.Jul, HwatuType.Pea, Image.Load($"{hwatuPath}/7Pea1.png")), new Hwatu(Month.Jul, HwatuType.Pea, Image.Load($"{hwatuPath}/7Pea2.png")), new Hwatu(Month.Jul, HwatuType.GrassBelt, Image.Load($"{hwatuPath}/7GrassBelt.png")),
            new Hwatu(Month.Aug, HwatuType.Light, Image.Load($"{hwatuPath}/8Light.png")), new Hwatu(Month.Aug, HwatuType.Pea, Image.Load($"{hwatuPath}/8Pea1.png")), new Hwatu(Month.Aug, HwatuType.Pea, Image.Load($"{hwatuPath}/8Pea2.png")), new Hwatu(Month.Aug, HwatuType.Bird, Image.Load($"{hwatuPath}/8Bird.png")),
            new Hwatu(Month.Sep, HwatuType.SSangPea, Image.Load($"{hwatuPath}/9SSangPea.png")), new Hwatu(Month.Sep, HwatuType.Pea, Image.Load($"{hwatuPath}/9Pea1.png")), new Hwatu(Month.Sep, HwatuType.Pea, Image.Load($"{hwatuPath}/9Pea2.png")), new Hwatu(Month.Sep, HwatuType.BlueBelt, Image.Load($"{hwatuPath}/9BlueBelt.png")),
            new Hwatu(Month.Oct, HwatuType.Etc, Image.Load($"{hwatuPath}/10Etc.png")), new Hwatu(Month.Oct, HwatuType.Pea, Image.Load($"{hwatuPath}/10Pea1.png")), new Hwatu(Month.Oct, HwatuType.Pea, Image.Load($"{hwatuPath}/1Pea2.png")), new Hwatu(Month.Oct, HwatuType.BlueBelt, Image.Load($"{hwatuPath}/10BlueBelt.png")),
            new Hwatu(Month.Nov, HwatuType.SSangPea, Image.Load($"{hwatuPath}/11SSangPea.png")), new Hwatu(Month.Nov, HwatuType.Pea, Image.Load($"{hwatuPath}/11Pea1.png")), new Hwatu(Month.Nov, HwatuType.Pea, Image.Load($"{hwatuPath}/11Pea2.png")), new Hwatu(Month.Nov, HwatuType.Light, Image.Load($"{hwatuPath}/11Light.png")),
            new Hwatu(Month.Dec, HwatuType.BeaLight, Image.Load($"{hwatuPath}/12BeaLight.png")), new Hwatu(Month.Dec, HwatuType.SSangPea, Image.Load($"{hwatuPath}/12SSangPea.png")), new Hwatu(Month.Dec, HwatuType.EtcBelt, Image.Load($"{hwatuPath}/12EtcBelt.png")), new Hwatu(Month.Dec, HwatuType.BeaBird, Image.Load($"{hwatuPath}/12BeaBird.png")),
            new Hwatu(Month.Joker, HwatuType.SSangPea, Image.Load($"{hwatuPath}/Joker.png")), new Hwatu(Month.Joker, HwatuType.SSangPea, Image.Load($"{hwatuPath}/Joker.png"))
        };
        
        public GoStop(ulong[] ids) //
        {
            // if (ids.Length > 4)
            // {
            //     throw new Exception("ToManyPlayer");
            // }
            // else if(ids.Length < 2)
            // {
            //     throw new Exception("ToLessPlayer");
            // }
            int playerHwatuCount = ids.Length > 3 ? 7 : 10;
            int fieldHwatuCount = ids.Length > 3 ? 6 : 8;
            Random rd = new Random();
            for(int i = 0; i < allHwatus.Length; i++) //화투 패 섞기
            {
                swap(ref allHwatus, i, rd.Next(0, allHwatus.Length));
            }
            for(int i = 0; i < ids.Length; i++)
            {
                int random = rd.Next(0, ids.Length);
                var temp = ids[i];
                ids[i] = ids[random];
                ids[random] = ids[i];
            }


            for(int i = 0; i < ids.Length; i++)
            {
                List<Hwatu> give = new List<Hwatu>();
                for(int j = 0; j < playerHwatuCount; j++)
                {
                    give.Add(allHwatus[i * playerHwatuCount + j]);
                }
                Console.WriteLine(ids[i]);
                players.Add(ids[i], new Player(give, i));
            }

        }
        void swap(ref Hwatu[] hwatus, int indexa, int indexb)
        {
            var temp = hwatus[indexa];
            hwatus[indexa] = hwatus[indexb];
            hwatus[indexb] = temp;
        }
        public List<Hwatu> getHwatus(ulong id)
        {
            return players[id].getHwatus();
        }
    }
    public enum Month //13월은 조커
    {
        Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, Joker
    }
    public enum HwatuType
    {
        Pea, SSangPea, BlueBelt, RedBelt, GrassBelt, EtcBelt, Bird, BeaBird, Light, BeaLight, Etc //피, 쌍피, 청단, 홍단, 초단, 띠 있는 나머지, 새, 비새, 광, 비광, 기타
    }
    public struct Hwatu
    {
        Month month;
        HwatuType hwatuType;
        Image _hwatuImage;
        public Image hwatuImage
        {
            get
            {
                return _hwatuImage;
            }
        }
        public Hwatu(Month _month, HwatuType _hwatuType, Image _hwatuImage)
        {
            month = _month;
            hwatuType = _hwatuType;
            this._hwatuImage = _hwatuImage;
        }
        public override string ToString()
        {
            return $"{month}, {hwatuType}";
        }
    }
    public struct Player
    {
        List<Hwatu> hwatus;
        List<Hwatu> scoreHwatus;
        public List<Hwatu> getHwatus()
        {
            return hwatus;
        }
        // Hwatu[] getHwatus;
        int order;
        public Player(List<Hwatu> _hwatus, int _order)
        {
            hwatus = _hwatus;
            scoreHwatus = new List<Hwatu>();
            order = _order;
        }
    }
    public struct Field
    {
        List<Hwatu> hwatus;
        public Field(List<Hwatu> _hwatus) => hwatus = _hwatus;
    }
}