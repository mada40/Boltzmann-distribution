using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    internal class SourceField : PhysicalObject
    {
        public float Charge { get; set; }
        public float RaduisWell { get; set; }
        public SourceField(PointF pos, float charge, float R_well)
        {
            Position = pos;
            Charge = charge;
            RaduisWell = R_well;
        }

        public double F(double x)
        {
            double ans = (1.0 / (x * x) - RaduisWell / (x * x * x));  
            return ans;
        }

        double time = 0;
        public override void draw(ref Graphics g, Pen pen, double deltatime)
        {
            float R = 5f;
            var rect = new RectangleF(Position.X - R, Position.Y - R, 2 * R, 2 * R);
            g.DrawEllipse(pen, rect);

            time += deltatime;

            if (Charge == 0.0)
                return;

            R = RaduisWell + 2f * Math.Abs(Charge);
            rect = new RectangleF(Position.X - R, Position.Y - R, 2 * R, 2 * R);
            g.FillEllipse(new SolidBrush(Color.FromArgb(64,255,255,255)), rect);
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    double norm = time / 1600.0 + Math.PI / 4.0 * i;
                    float x = (float)(Math.Cos(norm) + 1f) / 2f;
                    if (Math.Sin(norm) * Charge * (1 - 2 * j) < 0)
                        x = 0f;

                    Color color;
                    
                    if(j == 0)
                    {
                        R = RaduisWell + 2f * Math.Abs(Charge) * x;
                    }
                    else
                    {
                        R = RaduisWell * x;
                    }

                    if(j == 0 && Charge > 0 || j == 1 && Charge < 0)
                    {
                        color = Color.FromArgb((int)(255f * (1f - x)), Color.BlueViolet);
                    }
                    else
                    {
                        color = Color.FromArgb((int)(255f * x), Color.Red);
                    }
                    Pen tmp = new Pen(color, 1f);
                    rect = new RectangleF(Position.X - R, Position.Y - R, 2 * R, 2 * R);
                    g.DrawEllipse(tmp, rect);
                }
            }
            
        }




        public override void move(MyVector v)
        {
            Position += v;
        }
    }
}
