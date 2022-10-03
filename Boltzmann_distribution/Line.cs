using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Boltzmann_distribution
{
    internal class Line: PhysicalObject
    {
        public Line(PointF p1, PointF p2)
        {
            Position = p1;
            Point2 = p2;
        }
        public float Length() => MyVector.distance(Position, Point2);
        public PointF Point2 { get; set; }
        public override void draw(ref Graphics g, Pen pen)
        {
            g.DrawLine(pen, Position, Point2);
        }
        public override void move(MyVector v)
        {
            Position = Position + v;
            Point2 = Point2 + v;
        }
        public override RectangleF Bounds
        {
            get
            {
                float minX = Math.Min(Position.X, Point2.X);
                float minY = Math.Min(Position.Y, Point2.Y);
                float maxX = Math.Max(Position.X, Point2.X);
                float maxY = Math.Max(Position.Y, Point2.Y);
                return new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
        }

    }
}
