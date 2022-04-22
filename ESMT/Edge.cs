using ESMT.Shapes;
using System;

namespace ESMT
{
    public class Edge
    {
        public Point Point1 { get; }

        public Point Point2 { get; }

        public double Length { get; private set; }

        public Edge(Point p1, Point p2)
        {
            Point1 = p1;
            Point2 = p2;
            Length = Math.Sqrt(Math.Pow(Point2.X - Point1.X, 2) + Math.Pow(Point2.Y - Point1.Y, 2));
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;
            if(obj.GetType() != GetType())
                return false;
            var edge = obj as Edge;
            return (Point1 == edge.Point1 && Point2 == edge.Point2) || (Point1 == edge.Point2 && Point2 == edge.Point1);
        }

        public override int GetHashCode()
        {
            int code = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
            return code.GetHashCode();
        }
    }
}
