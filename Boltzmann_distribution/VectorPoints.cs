using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Boltzmann_distribution
{
    internal class VectorPoints
    {
        private List<PointF> points = new List<PointF>();

        public int MinSquareLen { get; set; }
        public int getCount() => points.Count;

        public void addPoint(int x, int y)
        {
            if(points.Count() > 0)
            {
                float dx = points.Last().X - x;
                float dy = points.Last().Y - y;
                if (dx * dx + dy * dy < MinSquareLen)
                    return;
            }

            points.Add(new Point(x, y));
        }

        public void addPoint(Point x)
        {
            addPoint(x.X, x.Y);
        }

        public void draw(ref Graphics g, Pen pen)
        {
            if (points.Count() < 2) return;
            g.DrawLines(pen, points.ToArray());
        }

        public VectorPoints(int minSqLen = 0)
        {
            points = new List<PointF>();
            MinSquareLen = minSqLen;
        }

    }
}
