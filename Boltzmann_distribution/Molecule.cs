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
        const float R_DEF = 15;
        public float R { get; set; }
        public MyVector Vector { get; set; }
        private const float EPS = 0.0001F;


        public override RectangleF Bounds
        {
            get => new RectangleF(Position.X - R, Position.Y - R, 2 * R, 2 * R);
        }
        public MyVector getOffset(double deltatime) => Vector * deltatime;

        public Molecule(int seed, RectangleF rect, double speed)
        {
            R = R_DEF;
            Random rnd = new Random(seed);
            setRandomPos(rnd.Next(), rect);
            setSpeed(rnd.Next(), speed);
        }

        public Molecule(PointF pos,  MyVector vec, float r = R_DEF)
        {
            Position = pos;
            R = r;
            Vector = vec;
        }

        public Molecule()
        {
            Position = new PointF();
            Vector = new MyVector();
            R = R_DEF;
        }

        public override void move(MyVector v)
        {
            Position += v;
        }

        public override void draw(ref Graphics g, Pen pen, double deltatime) 
        {
            g.DrawEllipse(pen, Bounds);
            PointF beginRay = Position;
            PointF endRay = beginRay + getOffset(deltatime);
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

        private double GetPossibleMaxOffset(Molecule m, MyVector maxOffset)
        {
            return MyMath.GetMaxOffsetOfCircleToCircle(Position, R, maxOffset, m.Position, m.R);
        }

        private double GetPossibleMaxOffset(Line line, MyVector maxOffset)
        {
            return MyMath.GetMaxOffsetOfCircleToLineSegment(Position, R, maxOffset, line.Position, line.Point2, EPS);
        }



        public double GetPossibleMaxOffset(PhysicalObject ph, MyVector maxOffset)
        {

            if(ph is Molecule mol)
            {
                return GetPossibleMaxOffset(mol, maxOffset);
            }
            else if(ph is Line line)
            {
                return GetPossibleMaxOffset(line, maxOffset);
            }

            return 1.0;
            

        }




    }
}
