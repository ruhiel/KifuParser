using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser.Csa
{
    public class CSAParser
    {
        public static void ParseContent(string content)
        {
            var playerParser =
                from n in Parse.Char('N').Token()
                from bw in Parse.Regex("[+-]")
                from player in Parse.Letter.AtLeastOnce().Text()
                select player;

            var timeParser =
                from t in Parse.Char('T').Token()
                from time in Parse.Number.Token()
                select time;

            var stateParser = timeParser.Or(playerParser);

            var multiStateParser =
                from comma in Parse.Char(',').Token()
                from st in stateParser
                select st;

            var lineParser =
                from st in stateParser.Once()
                from mst in multiStateParser.Many()
                select st.Concat(mst);

            var p = lineParser.Many().End();

            foreach (var a in p.Parse(content))
            {
                foreach(var c in a)
                {
                    Console.WriteLine(c);
                }
            }
        }
    }
}
