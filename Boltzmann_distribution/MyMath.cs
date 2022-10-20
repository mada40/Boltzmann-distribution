using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    static internal class MyMath
    {

        public static MyVector Reflect(MyVector v, MyVector normal)
        {
            return v - normal * (2f * MyVector.dot(v, normal) / normal.LengthSquared()) ;      
        }
        public static float getLengthNormal(PointF posC, PointF p1, PointF p2)
        {
            MyVector v1 = new MyVector(posC, p1);
            MyVector v2 = new MyVector(posC, p2);

            float doubleS = MyVector.mult_coorZ(v1, v2);//векторное произведение или площадь параллелограмма
            return Math.Abs(doubleS / MyVector.distance(p1, p2));

        }

        public static float distance(PointF posC, PointF p1, PointF p2)
        {
            MyVector v1 = new MyVector(p1, posC);
            MyVector v2 = new MyVector(p2, posC);
            MyVector v3 = new MyVector(p1, p2);

            if (MyVector.signCos(v1, v3) >= 0 && MyVector.signCos(-v2, v3) >= 0)
                return getLengthNormal(posC, p1, p2);

            return (float)Math.Min(v1.Length(), v2.Length());
        }
        public static bool isInsercted(PointF posC, float R, PointF p1, PointF p2) => distance(posC, p1, p2) < R;

        public static bool isInsercted(PointF a1, PointF a2, PointF b1, PointF b2)
        {
            MyVector v1 = new MyVector(a1, b1);
            MyVector v2 = new MyVector(a1, b2);
            MyVector ray = new MyVector(a1, a2);
            float k1 = MyVector.mult_coorZ(ray, v1);
            float k2 = MyVector.mult_coorZ(ray, v2);

            v1 = new MyVector(b1, a1);
            v2 = new MyVector(b1, a2);
            MyVector barrier = new MyVector(b1, b2);
            float k3 = MyVector.mult_coorZ(barrier, v1);
            float k4 = MyVector.mult_coorZ(barrier, v2);
            return (k1 * k2 <= 0 && k3 * k4 <= 0);
        }

        public static bool isInsercted(PointF pos1, float R1, PointF pos2, float R2)
        {
            float newR = R1 + R2;
            return MyVector.distanceSquared(pos1, pos2) < newR * newR;
        }

        public static double GetMaxOffsetOfCircleToCircle(PointF pos1, float R1, MyVector v, PointF pos2, float R2)
        {
            float newR = R1 + R2;
            if (!isInsercted(pos2, newR, pos1, pos1 + v))
                return 1.0;

            float a = v.LengthSquared();

            MyVector beginRayV = new MyVector(pos1);
            MyVector posCircleV = new MyVector(pos2);
            float b = 2 * (MyVector.dot(v, beginRayV) - MyVector.dot(posCircleV, v));
            float c = MyVector.distanceSquared(pos1, pos2) - newR * newR;

            //ax^2 + bx + c = 0;
            //x = (-b +-sqrt(b^2-4ac))/2a
            double d = Math.Sqrt(b * b - 4f * a * c);

            if (a == 0.0f)
                a = 0.0001f;
            double x1 = (-b + d) / (2f * a);
            double x2 = (-b - d) / (2f * a);

            if(x1 < 0.0 && x2 < 0.0)
                return Math.Max(x1, x2);

            return Math.Min(x1, x2);
        }

        public static double GetMaxOffsetOfCircleToLineSegment(PointF pos, float R, MyVector v, PointF ls1, PointF ls2, float epsilon)
        {
            PointF endRay = pos + v;
            float d1 = distance(ls1, pos, endRay);
            float d2 = distance(ls2, pos, endRay);
            float d3 = distance(endRay, ls1, ls2);
            float min = Math.Min(d1, Math.Min(d2, d3));

            if (min >= R && !isInsercted(ls1, ls2, pos, endRay))
                return 1.0;

            
            //если круг сталкивается с концом отрезка
            double t1 = GetMaxOffsetOfCircleToCircle(pos, R, v, ls1, 0f);
            float x1 = distance(pos + v *t1, ls1, ls2);
            if (d1 < R && Math.Abs(x1 - R) < epsilon)
                return t1;

            //если круг сталкивается с другим концом отрезка
            double t2 = GetMaxOffsetOfCircleToCircle(pos, R, v, ls2, 0f);
            float x2 = distance(pos + v*t2, ls1, ls2);
            if (d2 < R && Math.Abs(x2 - R) < epsilon)
                return t2;

            MyVector barrierV = new MyVector(ls1, ls2);
            float p = MyVector.mult_coorZ(new MyVector(ls2), new MyVector(ls1));
            float u = MyVector.mult_coorZ(new MyVector(pos), barrierV);

            float del = MyVector.mult_coorZ(barrierV, v);
            if (del == 0.0f)
                del = 0.0001f;
            x1 = (float)(-R * barrierV.Length() + p + u) / del;
            x2 = (float)(R * barrierV.Length() + p + u) / del;
            return Math.Min(x1 ,x2);
        }
    }
}
