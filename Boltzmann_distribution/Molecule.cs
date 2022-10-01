using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    internal class Molecule : PhysicalObject
    {
        public float R { get; set; }
        public override float Weight { get => R * R * R; }

        public override RectangleF Bounds
        {
            get
            {
                return new RectangleF(Position.X - R, Position.Y - R, 2*R, 2*R);
            }
            

        }
        public MyVector getOffset(double deltatime)
        {
            float dx = (float)(Vector.X * deltatime);
            float dy = (float)(Vector.Y * deltatime);
            return new MyVector(dx, dy);
        }
        public Molecule(int seed, RectangleF rect, double speed)
        {
            R = 50f;
            Random rnd = new Random(seed);
            setRandomPos(rnd.Next(), rect);
            setSpeed(rnd.Next(), speed);
        }

        public Molecule(PointF pos, float r, MyVector v)
        {
            R = r;
            Position = pos;
            Vector = v;
        }

        public override void draw(ref Graphics g, Pen pen) 
        {
            g.DrawEllipse(pen, Bounds);
            float dx = (float)(Vector.X * 70);
            float dy = (float)(Vector.Y * 70);
            PointF beginRay = this.Position;
            PointF endRay = new PointF(beginRay.X + dx, beginRay.Y + dy);
            g.DrawLine(pen, beginRay, endRay);

        }

        public void setRandomPos(int seed, RectangleF rect)
        {
            Random rnd = new Random(seed);
            float x = rect.X + rnd.Next((int)R, (int)(rect.Width - R));
            float y = rect.Y + rnd.Next((int)R, (int)(rect.Height - R));

            Position = new PointF(x, y);
        }
        public void setSpeed(int seed, double speed)
        {
            Random rnd = new Random(seed);
            double arc = rnd.NextDouble() * 2.0 * Math.PI;
            float vx = (float)(Math.Cos(arc) * speed);
            float vy = (float)(Math.Sin(arc) * speed);
            Vector = new MyVector(vx, vy);
        }

        public MyVector GetMaxOffset(Molecule m, double deltatime)
        {
            MyVector v = getOffset(deltatime);
            return MyMath.GetMaxOffsetOfCircleToCircle(Position, R, v, m.Position, m.R);
        }

        public MyVector GetMaxOffset(Line line, double deltatime)
        {
            MyVector v = getOffset(deltatime);
            const float epsilon = 0.01f;
            return MyMath.GetMaxOffsetOfCircleToLineSegment(Position, R, v, line.Position, line.Point2, epsilon);
        }




    }
}
