using System;
using System.Collections.Generic;
using System.Text;

namespace ESMT.Shapes
{
    public class Circle
    {
        public Point Center { get; set; }

        public double RadiusSquared { get; set; }

        public Circle() { }

        public Circle(Point center, double radiusSquared)
        {
            Center = center;
            RadiusSquared = radiusSquared;
        }

        public bool IsPointInsideCircle(Point p)
        {
            return (p.X - Center.X) * (p.X - Center.X) + (p.Y - Center.Y) * (p.Y - Center.Y) < RadiusSquared;
        }

        public void FitTriangle(Triangle triangle)
        {
            Point v1 = triangle.Vertices[0];
            Point v2 = triangle.Vertices[1];
            Point v3 = triangle.Vertices[2];
            double dA = v1.X * v1.X + v1.Y * v1.Y;
            double dB = v2.X * v2.X + v2.Y * v2.Y;
            double dC = v3.X * v3.X + v3.Y * v3.Y;

            double aux1 = dA * (v3.Y - v2.Y) + dB * (v1.Y - v3.Y) + dC * (v2.Y - v1.Y);
            double aux2 = -(dA * (v3.X - v2.X) + dB * (v1.X - v3.X) + dC * (v2.X - v1.X));
            double div = 2 * (v1.X * (v3.Y - v2.Y) + v2.X * (v1.Y - v3.Y) + v3.X * (v2.Y - v1.Y));

            Center = new Point(aux1 / div, aux2 / div);
            RadiusSquared = (Center.X - v1.X) * (Center.X - v1.X) + (Center.Y - v1.Y) * (Center.Y - v1.Y);
        }
    }
}
