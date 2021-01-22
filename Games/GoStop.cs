using System;
using System.Linq;
using System.Collections.Generic;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

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
                players.Add(ids[i], new Player(give, i, allHwatus));
            }

        }
        public Player getPlayer(ulong id)
        {
            return players[id];
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
        HwatuType _hwatuType;
        public HwatuType hwatuType
        {
            get
            {
                return _hwatuType;
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
        public Hwatu(Month _month, HwatuType _hwatuType, Image _hwatuImage)
        {
            month = _month;
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
        List<Hwatu> hwatus;
        List<Hwatu> scoreHwatus;
        List<Hwatu> fortest;

        public List<Hwatu> getHwatus()
        {
            return hwatus;
        }
        // Hwatu[] getHwatus;
        public Image getHwatusImage()
        {
            int width = hwatus[0].hwatuImage.Width + 20;
            int height = hwatus[0].hwatuImage.Height;

            using (var image = new Image<Rgba32>(width * hwatus.Count, height))
            {
                int index=0;
                foreach(var a in hwatus)
                {
                    Point point = new Point(index * width, 0);
                    image.Mutate(image => image.DrawImage(a.hwatuImage, point, 1));
                    index++;
                }
                return image;
            }
        }
        public Image getScoreHwatusImage()
        {
            scoreHwatus = fortest; //일단 테스트


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
                Console.WriteLine(scoreHwatus[i]);
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
            Size cardSize = new Size(images[0][0][0].Width - 25 , images[0][0][0].Height - 40);
            Size returnImageSize = new Size(18 * cardSize.Width, 0);
            int index = 0;
            foreach(var part in images)
            {
                Point pt = new Point(0, 0);
                partImages[index] = new Image<Rgba32>(cardSize.Width * 5 + 26, cardSize.Height * (part.Count + 0)+41);
                foreach(var line in part)
                {
                    foreach(var one in line)
                    {
                        // Console.WriteLine($"필요: {(cardSize.Width * pt.X + 1) + one.Size().Width}x{(partImages[index].Size().Width - cardSize.Height * pt.Y + 1) + one.Size().Height}, 현재: {partImages[index].Size().Width}x{partImages[index].Size().Height}");
                        partImages[index].Mutate(x => x.DrawImage(one, new Point(cardSize.Width * pt.X + 1, partImages[index].Size().Height - (cardSize.Height * (pt.Y+1))  + 1), 1));
                        pt.X++;
                    }
                    pt.Y++;
                    pt.X = 0;
                }
                if (partImages[index].Size().Height > returnImageSize.Height) returnImageSize.Height = partImages[index].Size().Height;
                index++;
            }
            returnImageSize.Height += cardSize.Height * 2 + 4;
            
            Image returnImage = new Image<Rgba32>(returnImageSize.Width + 26, returnImageSize.Height + 41);
            returnImage.Mutate(m => {
                m.DrawImage(partImages[3], new Point(1,  returnImageSize.Height / 2), 1);
                m.DrawImage(partImages[1], new Point((returnImageSize.Width - partImages[1].Size().Width) / 2, (returnImageSize.Height - partImages[1].Size().Height / 2) + 1), 1);
                m.DrawImage(partImages[2], new Point((returnImageSize.Width - partImages[2].Size().Width) / 2, (returnImageSize.Height - cardSize.Height) / 2 + 1), 1);
                m.DrawImage(partImages[0], new Point(returnImageSize.Width - (cardSize.Width * 5), returnImageSize.Height - partImages[0].Height), 1);
            });
            return returnImage;

            // for (int i = 0; i < 4; i++)
            // {
            //     cardImages[i] = new Image<Rgba32>();
            // }
        }
        int order;
        public Player(List<Hwatu> _hwatus, int _order, Hwatu[] test)
        {
            hwatus = _hwatus;
            scoreHwatus = new List<Hwatu>();
            order = _order;
            fortest = test.ToList();
        }

    }
    public struct Field
    {
        List<Hwatu> hwatus;
        public Field(List<Hwatu> _hwatus) => hwatus = _hwatus;
    }
}