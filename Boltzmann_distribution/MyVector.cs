using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace Boltzmann_distribution
{
    internal class MyVector 
    {
        public float X { get; set; }
        public float Y { get; set; }

        public MyVector(float x = 0f, float y = 0f)
        {
            X = x;
            Y = y;
        }

        public MyVector(double x, double y)
        {
            X = (float)x;
            Y = (float)y;
        }

        public MyVector(PointF p)
        {
            X = p.X;
            Y = p.Y;
        }

        public MyVector(PointF begin, PointF end)
        {
            X = end.X - begin.X;
            Y = end.Y - begin.Y;
        }



        public MyVector GetNormal() => new MyVector(-Y, X) / Length(); 

        public float LengthSquared() => ((X * X) + (Y * Y));
        public double Length() => Math.Sqrt(LengthSquared());
        public static void normalize(ref MyVector v) { v /= v.Length(); }

        public static float dot(MyVector v1, MyVector v2) => v1.X * v2.X + v1.Y * v2.Y;
        public static float mult_coorZ(MyVector v1, MyVector v2) => v1.X * v2.Y - v1.Y * v2.X;
        public static int signCos(MyVector v1, MyVector v2)
        {
            float tmp = dot(v1, v2);
            if (tmp > 0)
                return 1;
            else if (tmp < 0)
                return -1;

            return 0;

        }
        public static int signSin(MyVector v1, MyVector v2) => Math.Sign(mult_coorZ(v1, v2));
        public static float Cos(MyVector v1, MyVector v2) => dot(v1, v2) / (float)(v1.Length() * v2.Length());
        public static float Sin(MyVector v1, MyVector v2) => mult_coorZ(v1, v2) / (float)(v1.Length() * v2.Length());
        public static float distanceSquared(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return dx * dx + dy * dy;
        }
        public static float distance(PointF p1, PointF p2) => (float)Math.Sqrt(distanceSquared(p1, p2));


        public static MyVector operator -(MyVector v) => new MyVector(-v.X, -v.Y);
        public static MyVector operator *(MyVector v, float x) => new MyVector(v.X * x, v.Y * x);
        public static MyVector operator *(MyVector v, double x) => new MyVector(v.X * x, v.Y * x);
        public static MyVector operator /(MyVector v, float x) => new MyVector(v.X / x, v.Y / x);
        public static MyVector operator /(MyVector v, double x) => new MyVector(v.X / x, v.Y / x);
        public static PointF operator +(PointF p, MyVector v) => new PointF(p.X + v.X, p.Y + v.Y);
        public static PointF operator -(PointF p, MyVector v) => new PointF(p.X - v.X, p.Y - v.Y);
        public static MyVector operator +(MyVector v1, MyVector v2) => new MyVector(v1.X + v2.X, v1.Y + v2.Y);
        public static MyVector operator -(MyVector v1, MyVector v2) => new MyVector(v1.X - v2.X, v1.Y - v2.Y);
    }
}
