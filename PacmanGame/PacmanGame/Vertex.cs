using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame
{
    class Vertex
    {
        private int potential;
        private bool isMark;
        private Coordinate next;

        public Vertex()
        {
            potential = int.MaxValue;
            isMark = false;
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
