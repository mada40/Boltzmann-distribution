using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Boltzmann_distribution
{
    abstract public class PhysicalObject
    {
        public PointF Position { get; set; }

        public virtual RectangleF Bounds { get; private set; }
        abstract public void draw(ref Graphics g, Pen pen, double deltatime);

        abstract public void move(MyVector v);

    }
}
