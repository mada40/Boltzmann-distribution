using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    internal class ArrayLines
    {
        private List<PointF> points = new List<PointF>();

        public PointF posMouse;
        
        public int MinSquareLen { get; set; }
        public int getCount() => points.Count - 1;

        public Line this[int i]
        {
            get => new Line(points[i], points[i + 1]);
            set
            {
                points[i] = value.Position;
                points[i + 1] = value.Point2;
            }
        }

        public void addPoint(int x, int y)
        {
            if (points.Count() > 0)
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

        public void draw(ref Graphics g, Pen pen, double deltatime)
        {
            if(points.Count() >= 1)
                g.DrawLine(pen, points.Last(), posMouse);

            if (points.Count() < 2) return;

            g.DrawLines(pen, points.ToArray());
        }

        public ArrayLines(int minSqLen = 0)
        {
            points = new List<PointF>();
            MinSquareLen = minSqLen;
        }

        public void clear()
        {
            points.Clear();
        }

    }
}
