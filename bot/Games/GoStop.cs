using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace csnewcs.Game.GoStop
{
    public class GoStop
    {
        int hwatuCardCount = 50;
        const string hwatuPath = "./HwatuImages";
        List<Player> _players = new List<Player>();

        public List<Player> players
        {
            get {return _players;}
        }

        Player _turn;
        public Player turn
        {
            get
            {
                return _turn;
            }
        }

        
        Field field;

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
            new Hwatu(Month.Oct, HwatuType.Etc, Image.Load($"{hwatuPath}/10Etc.png")), new Hwatu(Month.Oct, HwatuType.Pea, Image.Load($"{hwatuPath}/10Pea1.png")), new Hwatu(Month.Oct, HwatuType.Pea, Image.Load($"{hwatuPath}/10Pea2.png")), new Hwatu(Month.Oct, HwatuType.BlueBelt, Image.Load($"{hwatuPath}/10BlueBelt.png")),
            new Hwatu(Month.Nov, HwatuType.SSangPea, Image.Load($"{hwatuPath}/11SSangPea.png")), new Hwatu(Month.Nov, HwatuType.Pea, Image.Load($"{hwatuPath}/11Pea1.png")), new Hwatu(Month.Nov, HwatuType.Pea, Image.Load($"{hwatuPath}/11Pea2.png")), new Hwatu(Month.Nov, HwatuType.Light, Image.Load($"{hwatuPath}/11Light.png")),
            new Hwatu(Month.Dec, HwatuType.BeaLight, Image.Load($"{hwatuPath}/12BeaLight.png")), new Hwatu(Month.Dec, HwatuType.SSangPea, Image.Load($"{hwatuPath}/12SSangPea.png")), new Hwatu(Month.Dec, HwatuType.EtcBelt, Image.Load($"{hwatuPath}/12EtcBelt.png")), new Hwatu(Month.Dec, HwatuType.BeaBird, Image.Load($"{hwatuPath}/12BeaBird.png")),
            new Hwatu(Month.Joker, HwatuType.SSangPea, Image.Load($"{hwatuPath}/Joker.png")), new Hwatu(Month.Joker, HwatuType.SSangPea, Image.Load($"{hwatuPath}/Joker.png"))
        };
        
        public GoStop(ulong[] ids) //
        {
            if (ids.Length > 4)
            {
                throw new Exception("ToManyPlayer");
            }
            else if(ids.Length < 2)
            {
                throw new Exception("ToLessPlayer");
            }
            int playerHwatuCount = ids.Length > 2 ? 7 : 10;
            int fieldHwatuCount = ids.Length > 2 ? 6 : 8;
            Random rd = new Random();

            for(int i = 0; i < allHwatus.Length; i++) //화투 패 섞기
            {
                swap(ref allHwatus, i, rd.Next(0, allHwatus.Length));
            }
            // for(int i = 0; i < ids.Length; i++)
            // {
            //     int random = rd.Next(0, ids.Length);
                
            //     var temp = ids[i];
            //     ids[i] = ids[random];
            //     Console.WriteLine($"{temp} <-> {ids[random]}");
            //     ids[random] = ids[i];
            // }

            int index = 0;
            // foreach(var one in allHwatus)
            // {
                List<Hwatu> give = new List<Hwatu>();
                
                for(int i = 0; i < ids.Length; i++)
                {
                    give = new List<Hwatu>();
                    for(int j = 0; j < playerHwatuCount; j++)
                    {
                        give.Add(allHwatus[index]);
                        index++;
                    }
                    _players.Add(new Player(give, i, ids[i]));
                }
                
                give = new List<Hwatu>();
                for(int i = 0; i < fieldHwatuCount; i++)
                {
                    if(allHwatus[i].month == Month.Joker)
                    {
                        _players[0].scoreHwatus.Add(allHwatus[i]);
                        index++;
                        continue;
                    }
                    give.Add(allHwatus[index]);
                    index++;
                }
                List<Hwatu> etc = new List<Hwatu>();
                for(; index < allHwatus.Length; index++)
                {
                    etc.Add(allHwatus[index]);
                }
                field = new Field(give, etc);
            _turn =  _players[0];
            }
        // }
        public Player getPlayer(ulong id)
        {
            Player turn = new Player(new List<Hwatu>(), 0, 0);
            foreach(var a in _players)
            {
                if (a.id == id) turn = a;
            }
            if(turn.id == 0)
            {
                throw new Exception("NonePlayer");
            }
            return turn;
        }
        public Player[] getAllPlayers()
        {
            Player[] returnPlayers = _players.ToArray();
            return returnPlayers;
        }
        public Field Field
        {
            get
            {
                return field;
            }
        }
        void swap(ref Hwatu[] hwatus, int indexa, int indexb)
        {
            var temp = hwatus[indexa];
            hwatus[indexa] = hwatus[indexb];
            hwatus[indexb] = temp;
        }
        public void turnPlayerPutHwatu(Hwatu hwatu, int getwhat = 0)
        {
            int index = _players.IndexOf(turn);
            Player temp = turn;
            putHwatu(ref temp, ref field, hwatu, getwhat);
            _players[index] = temp;
        }
        public void putHwatu(ref Player player, ref Field field, Hwatu hwatu, int getwhat = 0)
        {
            if (!player.hwatus.Contains(hwatu))
            {
                throw new Exception("PlayerDoesNotHave");
            }
            if(hwatu.month == Month.Joker)
            {
                player.scoreHwatus.Add(hwatu);
                player.hwatus.Remove(hwatu);
                player.hwatus.Add(field.reverseHwatus[0]);
                field.reverseHwatus.RemoveAt(0);
            }

            Hwatu[] getHwatu = field.canGet(hwatu);
            player.hwatus.Remove(hwatu);
            if(getHwatu.Length == 0)
            {
                field.hwatus.Add(hwatu);
            }
            else
            {
                field.hwatus.Remove(getHwatu[getwhat]);
                player.scoreHwatus.Add(hwatu);
                player.scoreHwatus.Add(getHwatu[getwhat]);
            }

            Hwatu plusalpha = field.reverseHwatus[0];
            getHwatu = field.canGet(plusalpha);
            
            if(getHwatu.Length == 0)
            {
                field.reverseHwatus.RemoveAt(0);
                field.hwatus.Add(plusalpha);
            }
            else if(getHwatu.Length == 1)
            {
                field.reverseHwatus.RemoveAt(0);
                field.hwatus.Remove(getHwatu[0]);
                player.scoreHwatus.Add(getHwatu[0]);
                player.scoreHwatus.Add(plusalpha);
            }
            else
            {
                throw new Exception("Please Select Hwatu");
            }

            

            changeTurn();
        }
        public void selectHwatu(Hwatu hwatu)
        {
            int index = _players.IndexOf(turn);
            Player temp = turn;
            
            temp.scoreHwatus.Add(hwatu);
            temp.scoreHwatus.Add(field.reverseHwatus[0]);
            field.reverseHwatus.RemoveAt(0);    
            field.hwatus.Remove(hwatu);

            _players[index] = temp;
            changeTurn();
        }
        void changeTurn()
        {
            int index = _players.IndexOf(turn);
            index = _players.Count - 1 == index ? 0 : index + 1;
            _turn = players[index];
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
        Month _month;
        HwatuType _hwatuType;
        public HwatuType hwatuType
        {
            get
            {
                return _hwatuType;
            }
        }
        public Month month
        {
            get
            {
                return _month;
            }
        }
        Image _hwatuImage;
        public Image hwatuImage
        {
            get
            {
                return _hwatuImage;
            }
        }
        public string toKR()
        {
            if (Month.Joker == month) return "조커";
            string type = "";
            switch(_hwatuType)
            {
                case HwatuType.Pea: type = "피"; break;
                case HwatuType.SSangPea: type = "쌍피"; break;
                case HwatuType.Bird: type = "새"; break;
                case HwatuType.BeaBird: type = "비새"; break;
                case HwatuType.BlueBelt  :  type = "청단"; break;
                case HwatuType.RedBelt  :  type = "홍단"; break;
                case HwatuType.GrassBelt  :  type = "초단"; break;
                case HwatuType.EtcBelt  :  type = "기타 단"; break;
                case HwatuType.Etc  :  type = "껍데기"; break;
                case HwatuType.Light  :  type = "광"; break;
                case HwatuType.BeaLight  :  type = "비광"; break;
            }
            return $"{(int)_month + 1} {type}";
        }
        
        public Hwatu(Month month, HwatuType _hwatuType, Image _hwatuImage)
        {
            _month = month;
            this._hwatuImage = _hwatuImage;
            this._hwatuType = _hwatuType;
        }
        public override string ToString()
        {
            return $"{month}, {hwatuType}";
        }
    }
    public struct Player
    {
        List<Hwatu> _hwatus;
        List<Hwatu> _scoreHwatus;
        ulong _id;
        readonly Size _cardSize;
        int _go;


        public int go
        {
            get {return _go;}
            set {_go = value;}
        }
        public ulong id
        {
            get {return _id;}
        }
        public List<Hwatu> hwatus
        {
            get {return _hwatus;}
        }
        public List<Hwatu> scoreHwatus
        {
            get {return _scoreHwatus;}
        }
        // Hwatu[] getHwatus;
        public Image getHwatusImage()
        {
            int width = hwatus[0].hwatuImage.Width + 20;
            int height = hwatus[0].hwatuImage.Height;

                var image = new Image<Rgba32>(width * hwatus.Count, height);
                int index=0;
                
                foreach(var a in hwatus)
                {
                    Point point = new Point(index * width, 0);
                    image.Mutate(image => image.DrawImage(a.hwatuImage, point, 1));
                    index++;
                }
                return image;
        }
        public Image getScoreHwatusImage()
        {
            // scoreHwatus = fortest; //일단 테스트


            int width = scoreHwatus[0].hwatuImage.Width - 10;
            int height = scoreHwatus[0].hwatuImage.Height - 10;

            List<List<Image>>[] images = new List<List<Image>>[4] {
                new List<List<Image>>(), new List<List<Image>>(), new List<List<Image>>(), new List<List<Image>>()
            };
            int[,] rowcolumn = new int[4,2] {
                {
                    0, 0
                },
                {
                    0, 0
                },
                {
                    0, 0
                },
                {
                    0, 0
                }
            };
            
            for(int i = 0; i < scoreHwatus.Count; i++)
            {
                switch(scoreHwatus[i].hwatuType)
                {
                    case HwatuType.Pea or HwatuType.SSangPea:
                        if(images[0].Count <= rowcolumn[0,1]) images[0].Add(new List<Image>());

                        images[0][rowcolumn[0,1]].Add(scoreHwatus[i].hwatuImage);
                        rowcolumn[0,0] += scoreHwatus[i].hwatuType == HwatuType.SSangPea ? 2:1;
                        if(rowcolumn[0,0] >= 5) 
                        {
                            rowcolumn[0, 1]++;
                            rowcolumn[0, 0] = 0;
                        }
                        break;
                    case HwatuType.BlueBelt or HwatuType.RedBelt or HwatuType.GrassBelt or HwatuType.EtcBelt:
                        if(images[1].Count <= rowcolumn[1,1]) images[1].Add(new List<Image>());

                        images[1][rowcolumn[1, 1]].Add(scoreHwatus[i].hwatuImage);
                        rowcolumn[1, 0]++;
                        if(rowcolumn[1, 0] >= 5)
                        {
                            rowcolumn[1, 1]++;
                            rowcolumn[1, 0] = 0;
                        }
                        break;
                    case HwatuType.Bird or HwatuType.BeaBird or HwatuType.Etc:
                        if(images[2].Count <= rowcolumn[2,1]) images[2].Add(new List<Image>());

                        images[2][rowcolumn[2, 1]].Add(scoreHwatus[i].hwatuImage);
                        rowcolumn[2, 0]++;
                        if(rowcolumn[2, 0] >= 5)
                        {
                            rowcolumn[2, 1]++;
                            rowcolumn[2, 0] = 0;
                        }
                        break;
                    case HwatuType.Light or HwatuType.BeaLight:
                        if(images[3].Count <= rowcolumn[3,1]) images[3].Add(new List<Image>());

                        images[3][rowcolumn[3, 1]].Add(scoreHwatus[i].hwatuImage);
                        rowcolumn[3, 0]++;
                        if(rowcolumn[3, 0] >= 5)
                        {
                            rowcolumn[3, 1]++;
                            rowcolumn[3, 0] = 0;
                        }
                        break;
                }
            }
            
            Image[] partImages = new Image[4];

            Size cardSize = _cardSize;

            Size returnImageSize = new Size(18 * cardSize.Width, 0);
            int index = 0;
            foreach(var part in images)
            {
                Point pt = new Point(0, 0);
                partImages[index] = new Image<Rgba32>(cardSize.Width * 5 + 26, cardSize.Height * (part.Count + 0)+41);

                foreach(var line in part)
                {
                    foreach (var one in line) 
                    {
                        partImages[index].Mutate(x => x.DrawImage(one, new Point(cardSize.Width * pt.X + 1, partImages[index].Size().Height - (cardSize.Height * (pt.Y+1))  + 1), 1));
                        pt.X++;
                        // Console.WriteLine($"필요: {(cardSize.Width * pt.X + 1) + one.Size().Width}x{(partImages[index].Size().Width - cardSize.Height * pt.Y + 1) + one.Size().Height}, 현재: {partImages[index].Size().Width}x{partImages[index].Size().Height}");
                    }
                    pt.Y++;
                    pt.X = 0;
                }
                if (partImages[index].Size().Height > returnImageSize.Height) returnImageSize.Height = partImages[index].Size().Height;
                index++;
            }
            returnImageSize.Height += cardSize.Height * 2 + 45;
            returnImageSize.Width += 26;
            
            Image returnImage = new Image<Rgba32>(returnImageSize.Width, returnImageSize.Height);
            returnImage.Mutate(m => {
                m.DrawImage(partImages[3], new Point(1,  returnImageSize.Height / 2), 1);
                m.DrawImage(partImages[2], new Point((returnImageSize.Width - partImages[2].Size().Width) / 2, (returnImageSize.Height / 2 - partImages[2].Size().Height) / 2 + 1), 1);
                m.DrawImage(partImages[1], new Point((returnImageSize.Width - partImages[1].Size().Width) / 2, (returnImageSize.Height - partImages[1].Size().Height ) + 1), 1);
                m.DrawImage(partImages[0], new Point(returnImageSize.Width - (cardSize.Width * 5), returnImageSize.Height - partImages[0].Height), 1);
            });
            return returnImage;

            // for (int i = 0; i < 4; i++)
            // {
            //     cardImages[i] = new Image<Rgba32>();
            // }
        }
        int order;
        public Player(List<Hwatu> hwatus, int _order, ulong id)
        {
            _cardSize = hwatus.Count == 0? new Size(0, 0): hwatus.FirstOrDefault().hwatuImage.Size();
            _hwatus = hwatus;
            _scoreHwatus = new List<Hwatu>();
            order = _order;
            _id = id;
            _go = 0;
        }
        public static bool operator ==(Player one, Player two)
        {
            if(one.id == two.id && one.hwatus == two.hwatus && one.scoreHwatus == two.scoreHwatus) return true;
            else return false;
        }
        public static bool operator !=(Player one, Player two)
        {
            if(one.id == two.id && one.hwatus == two.hwatus && one.scoreHwatus == two.scoreHwatus) return false;
            else return true;
        }
        
        public int getScore()
        {
            int score = 0;
            int pea = 0, blueBelt = 0, redBelt = 0, grassBelt = 0, etcBelt = 0, bird = 0, beaBird = 0, light = 0, beaLight = 0, etc = 0;
            
            foreach(var hwatu in _scoreHwatus)
            {
                var type = hwatu.hwatuType;
                switch(type)
                {
                    case  HwatuType.Pea: pea++; break;
                    case HwatuType.SSangPea: pea += 2; break;
                    case HwatuType.RedBelt: redBelt++; break;
                    case HwatuType.BlueBelt: blueBelt++; break;
                    case HwatuType.GrassBelt: grassBelt++; break;
                    case HwatuType.EtcBelt: etcBelt++; break;
                    case HwatuType.Bird: bird++; break;
                    case HwatuType.BeaBird: beaBird++; break;
                    case HwatuType.Light: light++; break;
                    case HwatuType.BeaLight: beaLight++; break;
                    case HwatuType.Etc: etc++; break;
                }
            }
            int totalBelt = redBelt+ grassBelt + blueBelt + etcBelt;
            int totalAnimal = bird + beaBird + etc;

            Console.WriteLine("피: " + pea);
            score += pea > 18 ?  pea - 19 : 0;
            score += redBelt == 3 ? 3 : 0;
            score += blueBelt == 3 ? 3 : 0;
            score += grassBelt == 3 ? 3 : 0;
            score += (redBelt + grassBelt + blueBelt + etcBelt) > 4 ? (redBelt + grassBelt + blueBelt + etcBelt) - 4 : 0;
            if(bird == 3)
            {
                score += 5;
                if(beaBird == 1) score++;
            }
            score += (bird + beaBird + etc) > 4 ? (bird + beaBird + etc) - 4 : 0;
            if(light > 1)
            {
                if (light + beaLight == 3)
                {
                    if(beaLight == 0) score += 3;
                    else score += 2;
                } 
                else if(light + beaLight == 5) score += 10;
                else score += light + beaLight;
            }

            return score;
        }
    }
    public struct Field
    {
        List<Hwatu> _hwatus;
        List<Hwatu> _reverseHwatus;
        public List<Hwatu> hwatus
        {
            get
            {
                return _hwatus;
            }
        }
        public List<Hwatu> reverseHwatus
        {
            get
            {
                return _reverseHwatus;
            }
        }
        public Image getFieldImage()
        {            
            Size size = _hwatus[0].hwatuImage.Size();
            Image returnImage = new Image<Rgba32>(size.Width * _hwatus.Count, size.Height * _hwatus.Count);
            returnImage.Mutate(m => m.BackgroundColor(Rgba32.ParseHex("#39977E")));

            Point max = new Point(size.Width * (_hwatus.Count - 1), size.Height * (_hwatus.Count - 1));

            Random rd = new Random();
            foreach(var one in _hwatus)
            {
                Image image = new Image<Rgba32>(one.hwatuImage.Width, one.hwatuImage.Height);
                image.Mutate(m => {
                    m.DrawImage(one.hwatuImage, 1); m.Rotate(rd.Next(-100, 100));});
                returnImage.Mutate(m => m.DrawImage(image, new Point(rd.Next(0, max.X), rd.Next(0, max.Y)), 1));
            }
            return returnImage;
        }
        public Field(List<Hwatu> hwatus, List<Hwatu> reverse)
        {
            _hwatus = hwatus;
            _reverseHwatus = reverse;
        }
        public Hwatu[] canGet(Hwatu hwatu)
        {
            List<Hwatu> hwatus = new List<Hwatu>();
            foreach(var fieldHwatu in _hwatus)
            {
                if(fieldHwatu.month == hwatu.month)
                {
                    hwatus.Add(fieldHwatu);
                }
            }
            return hwatus.ToArray();
        }
        
    }
}