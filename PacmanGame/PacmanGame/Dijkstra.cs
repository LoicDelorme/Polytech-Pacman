using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame
{
    class Dijkstra
    {
        public static Directions.Direction getDirection(Coordinate departure, Coordinate end, int width, int height, byte[,] map)
        {
            Vertex[,] vertexes = new Vertex[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j] != PacmanGame.WALL)
                    {
                        vertexes[i, j] = new Vertex();
                    }
                }
            }

            vertexes[end.X, end.Y].Potential = 0;
            Coordinate currentCoordinate = end;

            while (!currentCoordinate.Equals(departure))
            {
                Vertex z = vertexes[currentCoordinate.X, currentCoordinate.Y];
                z.IsMark = true;

                if (currentCoordinate.Y > 0)
                {
                    if (vertexes[currentCoordinate.X, currentCoordinate.Y - 1] != null)
                    {
                        Vertex s = vertexes[currentCoordinate.X, currentCoordinate.Y - 1];
                        if (s.Potential > z.Potential + 1)
                        {
                            s.Potential = z.Potential + 1;
                            s.Previous = currentCoordinate;
                        }
                    }
                }

                if (currentCoordinate.Y + 1 < height)
                {
                    if (vertexes[currentCoordinate.X, currentCoordinate.Y + 1] != null)
                    {
                        Vertex s = vertexes[currentCoordinate.X, currentCoordinate.Y + 1];
                        if (s.Potential > z.Potential + 1)
                        {
                            s.Potential = z.Potential + 1;
                            s.Previous = currentCoordinate;
                        }
                    }
                }

                if (currentCoordinate.X > 0)
                {
                    if (vertexes[currentCoordinate.X - 1, currentCoordinate.Y] != null)
                    {
                        Vertex s = vertexes[currentCoordinate.X - 1, currentCoordinate.Y];
                        if (s.Potential > z.Potential + 1)
                        {
                            s.Potential = z.Potential + 1;
                            s.Previous = currentCoordinate;
                        }
                    }
                }

                if (currentCoordinate.X + 1 < width)
                {
                    if (vertexes[currentCoordinate.X + 1, currentCoordinate.Y] != null)
                    {
                        Vertex s = vertexes[currentCoordinate.X + 1, currentCoordinate.Y];
                        if (s.Potential > z.Potential + 1)
                        {
                            s.Potential = z.Potential + 1;
                            s.Previous = currentCoordinate;
                        }
                    }
                }

                int min = Vertex.INFINITY;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (vertexes[i, j] != null)
                        {
                            if (!vertexes[i, j].IsMark && vertexes[i, j].Potential < min)
                            {
                                min = vertexes[i, j].Potential;
                                currentCoordinate = new Coordinate(i, j);
                            }
                        }
                    }
                }
            }

            Coordinate next = vertexes[currentCoordinate.X, currentCoordinate.Y].Previous;
            Directions.Direction res = Directions.Direction.up;

            if (next == null)
            {
                return res;
            }
            
            if (next.X != departure.X)
            {
                if (next.X > departure.X)
                {
                    res = Directions.Direction.right;
                }
                else
                {
                    res = Directions.Direction.left;
                }
            }
            else
            {
                if (next.Y > departure.Y)
                {
                    res = Directions.Direction.down;
                }
                else
                {
                    res = Directions.Direction.up;
                }    
            }

            return res;
        }
    }
}
