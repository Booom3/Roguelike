using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    struct Tile
    {
        public bool enterable;
        public Sign sign;
        public Tile(char Sign, bool Enterable, ConsoleColor Color)
        {
            sign = new Sign();
            sign.sign = Sign;
            sign.color = Color;
            enterable = Enterable;
        }
    }
}
