using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KifuParser.Kif
{
    public class GameInfoCommand : ICommand
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public GameInfoCommand(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString() => $"{Key}:{Value}";
    }
}
