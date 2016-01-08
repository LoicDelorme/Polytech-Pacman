using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PacmanGame
{
    class Coordinate
    {
        private int x;
        private int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate(Vector2 position)
        {
            x = (int) position.X;
            y = (int) position.Y;
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public static Boolean operator==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return ((coordinate1.x == coordinate2.x) && (coordinate1.y == coordinate2.y));
        }

        public static Boolean operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return ((coordinate1.x != coordinate2.x) && (coordinate1.y != coordinate2.y));
        }
    }
}
