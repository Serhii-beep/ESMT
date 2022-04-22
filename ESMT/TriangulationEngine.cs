using ESMT.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESMT
{
    public class TriangulationEngine
    {
        public IEnumerable<Triangle> GetTriangulation(List<Point> points, double width, double height)
        {
            points.ForEach(p => p.Triangles.Clear());
            Point topLeft = new Point(10, 10);
            Point bottomLeft = new Point(10, height - 10);
            Point bottomRight = new Point(width - 10, height - 10);
            Point topRight = new Point(width - 10, 10);
            if(!Contains(points, topLeft))
                points.Add(topLeft);
            if(!Contains(points, bottomLeft))
                points.Add(bottomLeft);
            if(!Contains(points, bottomRight))
                points.Add(bottomRight);
            if(!Contains(points, topRight))
                points.Add(topRight);
            HashSet<Triangle> result = new HashSet<Triangle>();
            result.Add(new Triangle(topLeft, bottomLeft, bottomRight));
            result.Add(new Triangle(topLeft, bottomRight, topRight));
            foreach(Point point in points)
            {
                HashSet<Triangle> badTriangles = new HashSet<Triangle>();
                badTriangles = result.Where(t => t.Circumcircle.IsPointInsideCircle(point)).ToHashSet();
                List<Edge> polygon = new List<Edge>();
                List<Edge> edges = new List<Edge>();
                foreach(Triangle triangle in badTriangles)
                {
                    edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                    edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                    edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
                }
                polygon = edges.GroupBy(e => e).Where(e => e.Count() == 1).Select(e => e.First()).ToList();
                foreach(Triangle triangle in badTriangles)
                {
                    foreach(Point v in triangle.Vertices)
                    {
                        v.Triangles.Remove(triangle);
                    }
                    result.Remove(triangle);
                }
                foreach(Edge edge in polygon.Where(e => e.Point1 != point && e.Point2 != point))
                {
                    Triangle t = new Triangle(point, edge.Point1, edge.Point2);
                    result.Add(t);
                }
            }
            HashSet<Triangle> toRemove = new HashSet<Triangle>();
            for(int i = 0; i < result.Count; ++i)
            {
                var t = result.ElementAt(i);
                var vert = t.Vertices;
                if(Contains(vert, topLeft) || Contains(vert, bottomLeft) || Contains(vert, bottomRight) ||
                    Contains(vert, topRight))
                {
                    toRemove.Add(t);
                }
            }
            foreach(var t in toRemove)
            {
                result.Remove(t);
                foreach(var v in t.Vertices)
                {
                    v.Triangles.Remove(t);
                }
            }
            points.Remove(topLeft);
            points.Remove(bottomLeft);
            points.Remove(bottomRight);
            points.Remove(topRight);
            return result;
        }

        private bool Contains(IEnumerable<Point> points, Point p)
        {
            foreach (var point in points)
            {
                if(point.X == p.X && point.Y == p.Y)
                    return true;
            }
            return false;
        }
    }
}
