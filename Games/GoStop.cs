using System;

namespace csnewcs.Game.GoStop
{
    class GoStop
    {
        int hwatuCardCount = 50;
        private readonly int playerCount;
        public GoStop(ulong[] ids)
        {
            if (ids.Length > 3)
            {

            }
            Hwatu[] allHwatus = new Hwatu[50] {
                new Hwatu(Month.Jan, HwatuType.Light), new Hwatu(Month.Jan, HwatuType.Pea), new Hwatu(Month.Jan, HwatuType.Pea), new Hwatu(Month.Jan, HwatuType.RedBelt),
                new Hwatu(Month.Feb, HwatuType.Bird), new Hwatu(Month.Feb, HwatuType.Pea), new Hwatu(Month.Feb, HwatuType.Pea), new Hwatu(Month.Feb, HwatuType.RedBelt),
                new Hwatu(Month.Mar, HwatuType.Light), new Hwatu(Month.Mar, HwatuType.Pea), new Hwatu(Month.Mar, HwatuType.Pea), new Hwatu(Month.Mar, HwatuType.RedBelt),
                new Hwatu(Month.Apr, HwatuType.Bird), new Hwatu(Month.Apr, HwatuType.Pea), new Hwatu(Month.Apr, HwatuType.Pea), new Hwatu(Month.Apr, HwatuType.GrassBelt),
                new Hwatu(Month.May, HwatuType.Etc), new Hwatu(Month.May, HwatuType.Pea), new Hwatu(Month.May, HwatuType.Pea), new Hwatu(Month.May, HwatuType.GrassBelt),
                new Hwatu(Month.Jun, HwatuType.Etc), new Hwatu(Month.Jun, HwatuType.Pea), new Hwatu(Month.Jun, HwatuType.Pea), new Hwatu(Month.Jun, HwatuType.BlueBelt),
                new Hwatu(Month.Jul, HwatuType.Etc), new Hwatu(Month.Jul, HwatuType.Pea), new Hwatu(Month.Jul, HwatuType.Pea), new Hwatu(Month.Jul, HwatuType.GrassBelt),
                new Hwatu(Month.Aug, HwatuType.Light), new Hwatu(Month.Aug, HwatuType.Pea), new Hwatu(Month.Aug, HwatuType.Pea), new Hwatu(Month.Aug, HwatuType.Bird),
                new Hwatu(Month.Sep, HwatuType.SSangPea), new Hwatu(Month.Sep, HwatuType.Pea), new Hwatu(Month.Sep, HwatuType.Pea), new Hwatu(Month.Sep, HwatuType.BlueBelt),
                new Hwatu(Month.Oct, HwatuType.Etc), new Hwatu(Month.Oct, HwatuType.Pea), new Hwatu(Month.Oct, HwatuType.Pea), new Hwatu(Month.Oct, HwatuType.BlueBelt),
                new Hwatu(Month.Nov, HwatuType.SSangPea), new Hwatu(Month.Nov, HwatuType.Pea), new Hwatu(Month.Nov, HwatuType.Pea), new Hwatu(Month.Nov, HwatuType.Light),
                new Hwatu(Month.Dec, HwatuType.BeaLight), new Hwatu(Month.Dec, HwatuType.SSangPea), new Hwatu(Month.Dec, HwatuType.EtcBelt), new Hwatu(Month.Dec, HwatuType.BeaBird),
                new Hwatu(Month.Joker, HwatuType.SSangPea), new Hwatu(Month.Joker, HwatuType.SSangPea)
            };
        }
    }
    enum Month //13월은 조커
    {
        Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, Joker
    }
    enum HwatuType
    {
        Pea, SSangPea, BlueBelt, RedBelt, GrassBelt, EtcBelt, Bird, BeaBird, Light, BeaLight, Etc //피, 쌍피, 청단, 홍단, 초단, 띠 있는 나머지, 새, 비새, 광, 비광, 기타
    }
    struct Hwatu
    {
        Month month;
        HwatuType hwatuType;
        public Hwatu(Month _month, HwatuType _hwatuType)
        {
            month = _month;
            hwatuType = _hwatuType;
        }
    }
    struct Player
    {
        Hwatu[] hwatus;
        Hwatu[] getHwatus;
        int order;
        public Player(Hwatu[] _hwatus, Hwatu[] _getHwatus, int _order)
        {
            hwatus = _hwatus;
            getHwatus = _getHwatus;
            order = _order;
        }
    }
    struct Field
    {

    }
}