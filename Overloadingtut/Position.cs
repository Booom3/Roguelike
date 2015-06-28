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




        public static int Distance(Position Pos1, Position Pos2)
        {
            return Math.Abs(Pos1.x - Pos2.x) + Math.Abs(Pos1.y - Pos2.y);
        }

        public static Position operator+(Position lhs, Position rhs)
        {
            return new Position(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Position operator-(Position lhs, Position rhs)
        {
            return new Position(lhs.x - rhs.x, lhs.y - rhs.y);
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

        public override int GetHashCode()
        {
            return this.x.GetHashCode() * 17 + this.y.GetHashCode();
        }
    }
}
