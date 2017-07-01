using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser.Kif
{
    public class MoveCommand : ICommand
    {
        public string Line { get; set; }
        public int? SrcX { get; set; }
        public int? SrcY { get; set; }
        public string Piece { get; set; }
        public string LeftRight { get; set; }
        public string DestY { get; internal set; }
        public string DestX { get; internal set; }
        public TimeSpan Time { get; internal set; }
        public bool Resign { get; set; }
        public string UpDown { get; internal set; }
        public string Action { get; internal set; }
        public string Dest => DestX == "同" ? DestX : $"{DestX}{DestY}";

        override public string ToString() => Resign ? $"投了{Time}" : $"{Dest}{Piece}{LeftRight}{UpDown}{Action}{Time}";
    }
}
