using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overloadingtut
{
    struct Position
    {
        public int x;
        public int y;
        public Position(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public static bool operator==(Position lhs, Position rhs)
        {
            return (lhs.x == rhs.x && lhs.y == rhs.y);
        }

        public static bool operator!=(Position lhs, Position rhs)
        {
            return (lhs.x != rhs.x || lhs.y != rhs.y);
        }

        public override bool Equals(object obj)
        {
            if (! (obj is Position))
            {
                return false;
            }
            return this == (Position) obj;
        }
    }
}
