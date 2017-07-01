using KifuParser.Kif;
using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var posParseSame =
                from pos in Parse.String("同")
                select new { X = "同", Y = "同" };

            var posParse =
                from destX in Parse.Regex("[１２３４５６７８９]")
                from destY in Parse.Regex("[一二三四五六七八九]")
                select new { X = destX, Y = destY };

            var srcPost =
                from a in Parse.Char('(')
                from srcX in Parse.Regex("[1-9]")
                from srcY in Parse.Regex("[1-9]")
                from b in Parse.Char(')')
                select new { X = int.Parse(srcX), Y = int.Parse(srcY) };

            var timeParse =
                from sp in Parse.WhiteSpace.Many()
                from st in Parse.Char('(')
                from useTime in Parse.Regex(@"[\s\d]\d:\d\d")
                from slash in Parse.Char('/')
                from allTime in Parse.Regex(@"\d\d:\d\d:\d\d")
                from ed in Parse.Char(')')
                select TimeSpan.ParseExact(useTime.Trim(), @"m\:ss", null);

            var gameInfoParse =
                from key in Parse.Regex("[^：]+")
                from c in Parse.String("：")
                from value in Parse.Regex(".+").Or(Parse.Return(string.Empty))
                select (ICommand)new GameInfoCommand(key, value);

            var move =
                from line in Parse.Number.Token()
                from dest in posParseSame.Or(posParse).Token()
                from promoted in Parse.String("成").Optional().Select(x => x.IsDefined ? x.Get().ToString() : string.Empty)
                from piece in Parse.Regex("[玉飛角金銀桂香歩龍馬と]")
                from leftright in Parse.Regex("[右左]").Optional().Select(x => x.IsDefined ? x.Get().ToString() : string.Empty)
                from updown in Parse.Regex("[上直寄引]").Optional().Select(x => x.IsDefined ? x.Get().ToString() : string.Empty)
                from action in Parse.Regex("不?成|打").Optional().Select(x => x.IsDefined ? x.Get().ToString() : string.Empty)
                from src in srcPost.Optional().Select(x => x.IsDefined ? new { X = x.Get().X, Y = x.Get().Y } : new { X = 0, Y = 0 })
                from time in timeParse
                select (ICommand)new MoveCommand() { Line = line, DestX = dest.X, DestY = dest.Y, SrcX = src.X, SrcY = src.Y, Piece = piece, LeftRight = leftright, UpDown = updown, Action = action, Time = time};

            var resign =
                from line in Parse.Number.Token()
                from r in Parse.String("投了").Token()
                from time in timeParse
                select (ICommand)new MoveCommand() { Line = line, Resign = true, Time = time };

            var nullparse =
                from v in Parse.Regex(".*").Or(Parse.Return(string.Empty)).Token()
                select (ICommand)new NullCommand();

            var moves = gameInfoParse.Or(resign).Or(move).Or(nullparse).Many().End();

            using (var sr = new System.IO.StreamReader(@"TextFile1.txt", Encoding.Default))
            {
                // ファイルの最後まで読み込む
                var content = sr.ReadToEnd();

                foreach (var m in moves.Parse(content))
                {
                    Console.WriteLine(m);
                }
            }
        }
    }
}
