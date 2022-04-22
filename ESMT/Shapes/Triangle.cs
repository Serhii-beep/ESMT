using System;
using System.Collections.Generic;
using System.Linq;

namespace ESMT.Shapes
{
    public class Triangle
    {
        public Point[] Vertices { get; } = new Point[3];

        public Circle Circumcircle { get; set; }

        public Triangle(Point p1, Point p2, Point p3)
        {
            Vertices[0] = p1;
            if(!Point.Is3PointsClockwise(p1, p2, p3))
            {
                Vertices[1] = p3;
                Vertices[2] = p2;
            }
            else
            {
                Vertices[1] = p2;
                Vertices[2] = p3;
            }
            Vertices[0].Triangles.Add(this);
            Vertices[1].Triangles.Add(this);
            Vertices[2].Triangles.Add(this);
            Circumcircle = new Circle();
            Circumcircle.FitTriangle(this);
        }

        public bool SharesEdgeWith(Triangle triangle)
        {
            return Vertices.Count(v => triangle.Vertices.Contains(v)) == 2;
        }
    }
}
