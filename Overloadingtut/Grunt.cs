using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class Grunt : GameObject
    {
        public float size = 10f;
        public Grunt()
        {
            sign.sign = 'G';
            sign.color = ConsoleColor.DarkRed;
        }
    }
}
