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
                from player in Parse.Regex(".+").Text()
                select player;

            var timeParser =
                from t in Parse.Char('T').Token()
                from time in Parse.Number.Token()
                select time;

            var oneStatementParser = timeParser.Or(playerParser);

            var moreStateParser =
                from comma in Parse.Char(',').Token()
                from st in oneStatementParser
                select st;

            var statementParser =
                from st in oneStatementParser.Once()
                from mst in moreStateParser.Many()
                select st.Concat(mst);

            var oneRecordParser = statementParser.Many();

            var moreRecordParser = from separtor in Parse.Regex(@"/\r\n")
                             from rec in oneRecordParser
                                   select rec;

            var documentParser =
                (from one in oneRecordParser.Once()
                 from more in moreRecordParser.Many()
                 select one.Concat(more)).End();      

            foreach (var redords in documentParser.Parse(content))
            {
                foreach(var statements in redords)
                {
                    foreach(var statement in statements)
                    {
                        Console.WriteLine(statement);
                    }
                }
            }
        }
    }
}
