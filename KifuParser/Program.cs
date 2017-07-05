using KifuParser.Csa;
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
            using (var sr = new System.IO.StreamReader(@"TextFile1.csa", Encoding.Default))
            {
                // ファイルの最後まで読み込む
                var content = sr.ReadToEnd();

                CSAParser.ParseContent(content);
            }
        }
    }
}
