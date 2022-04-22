using ESMT.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESMT
{
    public class SpanningTreeEngine
    {
        public List<Edge> GetSpanningTree(IEnumerable<Edge> edges)
        {
            List<Point> vertices = GetVertices(edges);
            List<Edge> result = new List<Edge>();
            var sortedEdges = edges.OrderBy(e => e.Length).ToList();
            DisjointSet set = new DisjointSet(vertices.Count());
            foreach(var vertice in vertices)
            {
                set.MakeSet(vertice.Num);
            }
            foreach(var edge in sortedEdges)
            {
                if(set.FindSet(edge.Point1.Num) != set.FindSet(edge.Point2.Num))
                {
                    result.Add(edge);
                    set.Union(edge.Point1.Num, edge.Point2.Num);
                }
            }
            return result;
        }

        private List<Point> GetVertices(IEnumerable<Edge> edges)
        {
            List<Point> result = new List<Point>();
            int counter = 0;
            foreach(var edge in edges)
            {
                if(!result.Contains(edge.Point1))
                {
                    edge.Point1.Num = counter;
                    result.Add(edge.Point1);
                    ++counter;
                }
                if(!result.Contains(edge.Point2))
                {
                    edge.Point2.Num = counter;
                    result.Add(edge.Point2);
                    ++counter;
                }
            }
            return result;
        }
    }
}
