using System;
using System.Collections.Generic;

namespace ESMT.Shapes
{
    public class Point
    {
        public double X { get; }

        public double Y { get; }

        public int Num { get; set; }

        public HashSet<Triangle> Triangles { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Triangles = new HashSet<Triangle>();
        }
        /*
             | p1.x  p1.y  1 |
             | p2.x  p2.y  1 |   >   0
             | p3.x  p3.y  1 |
        */
        public static bool Is3PointsClockwise(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y) > 0;
        }

        public static IEnumerable<Point> GenerateRandomPoints(int amount, double width, double height)
        {
            List<Point> points = new List<Point>();
            var random = new Random();
            for(int i = 0; i < amount; ++i)
            {
                double x = random.NextDouble() * (width - 20) + 10;
                double y = random.NextDouble() * (height - 20) + 10;
                Point p = new Point(x, y);
                if(points.Contains(p))
                {
                    --i;
                    continue;
                }
                points.Add(p);
            }
            return points;
        }
    }
}
