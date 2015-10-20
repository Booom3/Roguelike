using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    class BeamObject : GameObject
    {
        public Position Offset;
        public Position worldPos;
        public BeamObject()
        {
            sign.sign = '#';
            sign.color = ConsoleColor.Cyan;
            enterable = true;
        }
    }
}
