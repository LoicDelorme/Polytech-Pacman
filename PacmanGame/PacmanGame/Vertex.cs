using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame
{
    class Vertex
    {
        public static int INFINITY = int.MaxValue;
        private int potential;
        private bool isMark;
        private Coordinate previous;
        private Coordinate next;

        public Vertex()
        {
            potential = INFINITY;
            isMark = false;
            previous = null;
            next = null;
        }

        public int Potential
        {
            get
            {
                return potential;
            }

            set
            {
                potential = value;
            }
        }

        public bool IsMark
        {
            get
            {
                return isMark;
            }

            set
            {
                isMark = value;
            }
        }

        public Coordinate Previous
        {
            get
            {
                return previous;
            }

            set
            {
                previous = value;
            }
        }

        public Coordinate Next
        {
            get
            {
                return next;
            }

            set
            {
                next = value;
            }
        }
    }
}
