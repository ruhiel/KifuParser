using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser
{
    class Move
    {
        public string Line { get; set; }
        public string SrcX { get; set; }
        public string SrcY { get; set; }

        public string Piece { get; set; }
        public string Action { get; set; }
        public string DestY { get; internal set; }
        public string DestX { get; internal set; }
    }
}
