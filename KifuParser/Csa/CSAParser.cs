using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser.Csa
{
    /// <summary>
    /// http://www.computer-shogi.org/protocol/record_v22.html
    /// </summary>
    public class CSAParser
    {
        public static void ParseContent(string content)
        {
            // 先後手番
            var blackWhiteParser =
                from bw in Parse.Regex(@"\+|\-")
                select bw;

            // 駒
            var pieceParser =
                from piece in Parse.Regex("(FU)|(KY)|(KE)|(GI)|(KI)|(KA)|(HI)|(OU)|(TO)|(NY)|(NK)|(NG)|(UM)|(RY)")
                select piece;

            // 位置付き駒
            var pieceWithPositionParser =
                from postion in Parse.Regex(@"\d{2}")
                from piece in pieceParser
                select $"{postion}{piece}";

            // 先後付き駒
            var pieceWithBlackWhite =
                from bw in blackWhiteParser
                from piece in pieceParser
                select $"{bw}{piece}";

            // 先後位置付き駒
            var pieceFullParser =
                from bw in blackWhiteParser
                from postion in Parse.Regex(@"\d{2}")
                from piece in pieceParser
                select $"{bw}{postion}{piece}";

            // バージョン
            var versionParser =
                from v in Parse.Char('V')
                from version in Parse.Regex("[0-9.]+")
                select version;

            // 対局者名
            var playerParser =
                from n in Parse.Char('N').Token()
                from bw in blackWhiteParser
                from player in Parse.Regex(".+").Token()
                select player;

            // 各種棋譜情報
            // 棋戦名
            var gameNameParser =
                from key in Parse.String(@"$EVENT:").Token()
                from gameName in Parse.Regex(".+").Token()
                select gameName;

             // 対局場所
             var locationParser =
                from key in Parse.String(@"$SITE:").Token()
                from location in Parse.Regex(".+").Token()
                select location;

            // 対局開始日時
            var gameStartTimeParser =
                from key in Parse.String(@"$START_TIME:").Token()
                from datetime in Parse.Regex("[0-9/: ]+").Token()
                select DateTime.Parse(datetime).ToString();

            // 対局終了日時
            var gameEndTimeParser =
                from key in Parse.String(@"$END_TIME:").Token()
                from datetime in Parse.Regex("[0-9/: ]+").Token()
                select DateTime.Parse(datetime).ToString();

            // 対局終了日時
            var remainTimeParser =
                from key in Parse.String(@"$TIME_LIMIT:").Token()
                from remainTime in Parse.Regex(@"\d\d:\d\d\+\d\d").Token()
                select remainTime;

            // 戦型
            var openningNameParser =
                from key in Parse.String(@"$OPENING:").Token()
                from openingName in Parse.Regex(".+").Token()
                select openingName;

            // 開始局面
            var startingPositionParser =
                from key in Parse.String("PI").Text()
                from pieces in pieceWithPositionParser.Many()
                select key;

            // 開始局面(一括)
            var startingPositionBulkParser =
                from key in Parse.Regex("P[1-9]")
                from a1 in (pieceWithBlackWhite.Or(Parse.Regex("..."))).Repeat(9)
                from ret in Parse.String("\r\n")
                select key + a1.Aggregate((x, y) => x + y);

            // 消費時間
            var timeParser =
                from t in Parse.Char('T')
                from time in Parse.Number
                select time;

            // 先後手番
            var blackWhiteTurnParser =
                from bw in blackWhiteParser
                from ret in Parse.String("\r\n")
                select bw == "+" ? "先手" : "後手";

            // コメント
            var commentParser =
                from value in Parse.Regex("^'.*").Token()
                select "COMMENT";

            // 指し手
            var moveParser =
                from bw in blackWhiteParser.Token()
                from prevPosition in Parse.Regex(@"\d\d").Token()
                from nextPosition in Parse.Regex(@"\d\d").Token()
                from piece in pieceParser.Token()
                select $"{bw}{prevPosition}{nextPosition}{piece}";

            // 特殊な指し手、終局状況
            var specialMoveParser =
                from p in Parse.Char('%').Token()
                from key in Parse.Regex(".+").Token()
                select key;

            var nullParser =
                from value in Parse.Regex(".*").Or(Parse.Return(string.Empty)).Token()
                select "NULL";

            var oneStatementParser = versionParser
                                        .Or(playerParser)
                                        .Or(gameNameParser)
                                        .Or(locationParser)
                                        .Or(gameStartTimeParser)
                                        .Or(gameEndTimeParser)
                                        .Or(remainTimeParser)
                                        .Or(openningNameParser)
                                        .Or(startingPositionBulkParser)
                                        .Or(startingPositionParser)
                                        .Or(timeParser)
                                        .Or(commentParser)
                                        .Or(blackWhiteTurnParser)
                                        .Or(moveParser)
                                        .Or(specialMoveParser)
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
                    foreach(var statement in statements.Where(x => x != "COMMENT" && x != "NULL"))
                    {
                        Console.WriteLine(statement);
                    }
                }
            }
        }
    }
}
