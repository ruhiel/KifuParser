using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var move =
                from line in Parse.Number.Token()
                from srcX in Parse.Regex("[１２３４５６７８９]")
                from srcY in Parse.Regex("[１２３４５６７８９]")
                    //from piece in Parse.Regex("玉飛角金銀桂香歩龍馬と")
                select new Move() { Line = line, SrcX = srcX, SrcY = srcY };

            var nullparse =
                from v in Parse.Regex(".+").Token()
                select new Move();

            Parser<IEnumerable<Move>> moves = move.Or(nullparse).Many().End();

            // StreamReader の新しいインスタンスを生成する
            var cReader = (
                new System.IO.StreamReader(@"TextFile1.txt", Encoding.Default)
            );

            // ファイルの最後まで読み込む
            var stBuffer = cReader.ReadToEnd();

            // cReader を閉じる (正しくは オブジェクトの破棄を保証する を参照)
            cReader.Close();

            var list = moves.Parse(stBuffer);
            foreach(var m in list)
            {
                Console.WriteLine(m.Line);
            }
        }
    }
}
