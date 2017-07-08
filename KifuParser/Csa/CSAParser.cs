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
            // 駒
            var pieceParser =
                from piece in Parse.Regex("[(FU)(KY)(KE)(GI)(KI)(KA)(HI)(OU)(TO)(NY)(NK)(NG)(UM)(RY)]")
                select piece;

            // 位置付き駒
            var pieceWithPositionParser =
                from postion in Parse.Regex(@"\d{2}")
                from piece in pieceParser
                select $"{piece}{postion}";

            // 対局者名
            var playerParser =
                from n in Parse.Char('N').Token()
                from bw in Parse.Regex("[+-]")
                from player in Parse.Regex(".+")
                select player;

            // 各種棋譜情報
            // 棋戦名
            var gameNameParser =
                from key in Parse.String(@"$EVENT:").Text()
                from gameName in Parse.Regex(".+")
                select gameName;

            // 対局場所
            var locationParser =
                from key in Parse.String(@"$SITE:").Text()
                from location in Parse.Regex(".+")
                select location;

            // 対局開始日時
            var gameStartTimeParser =
                from key in Parse.String(@"$START_TIME:").Text()
                from datetime in Parse.Regex("[0-9/: ]+")
                select DateTime.Parse(datetime).ToString();

            // 対局終了日時
            var gameEndTimeParser =
                from key in Parse.String(@"$END_TIME:").Text()
                from datetime in Parse.Regex("[0-9/: ]+")
                select DateTime.Parse(datetime).ToString();

            // 対局終了日時
            var remainTimeParser =
                from key in Parse.String(@"$TIME_LIMIT:").Text()
                from remainTime in Parse.Regex(@"\d\d:\d\d\+\d\d")
                select remainTime;

            // 開始局面
            var openingParser =
                from key in Parse.String("PI").Text()
                from pieces in pieceWithPositionParser.Many()
                select key;

            // 消費時間
            var timeParser =
                from t in Parse.Char('T')
                from time in Parse.Number
                select time;

            var nullParser =
                from value in Parse.Regex(".*").Or(Parse.Return(string.Empty)).Token()
                select string.Empty;

            var oneStatementParser = playerParser
                                        .Or(gameNameParser)
                                        .Or(locationParser)
                                        .Or(gameStartTimeParser)
                                        .Or(gameEndTimeParser)
                                        .Or(remainTimeParser)
                                        .Or(openingParser)
                                        .Or(timeParser)
                                        .Or(nullParser);

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
                    foreach(var statement in statements.Where(x => x.Length != 0))
                    {
                        Console.WriteLine(statement);
                    }
                }
            }
        }
    }
}
