using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    static internal class Physics
    {
        static public MyVector CollisionBetweenLineAndMolecule(ref Line passLine, ref Molecule actMol, MyVector restOffset, float epsilon)
        {
            PointF b1 = passLine.Position;
            PointF b2 = passLine.Point2;
            PointF molPos = actMol.Position;
            MyVector normal = new MyVector(b1, b2).GetNormal();

            if(Math.Abs(MyVector.distance(b1, molPos) - actMol.R) < epsilon)
                normal = new MyVector(b1, molPos);

            if (Math.Abs(MyVector.distance(b2, molPos) - actMol.R) < epsilon)
                normal = new MyVector(b2, molPos);

            actMol.Vector = MyMath.Reflect(actMol.Vector, normal);
            return MyMath.Reflect(restOffset, normal);
        }


        static public MyVector CollisionBetweenMoleculeAndMolecule(ref Molecule passMol, ref Molecule actMol, MyVector restOffset)
        {
            //MyVector normal = new MyVector(passMol.Position, actMol.Position);
            //float actSpeed = actMol.Vector.Length();
            //float pasSpeed = passMol.Vector.Length();
            //
            //float k = (pasSpeed / actSpeed);
            //actMol.Vector = MyMath.Reflect(actMol.Vector, normal) * k;
            //passMol.Vector = MyMath.Reflect(passMol.Vector, normal) * (actSpeed / pasSpeed);

           

            Molecule m1 = passMol;
            Molecule m2 = actMol;
            float Dx = m1.Position.X - m2.Position.X;
            float Dy = m1.Position.Y - m2.Position.Y;
            double d = MyVector.distance(m1.Position, m2.Position); // расстояние между центрами частиц

            double sin = Dx / d;
            double cos = Dy / d;

            // формула из интернета 
            double Vn1 = m2.Vector.X * sin + m2.Vector.Y * cos;
            double Vn2 = m1.Vector.X * sin + m1.Vector.Y * cos;

            double Vt1 = -m2.Vector.X * cos + m2.Vector.Y * sin;
            double Vt2 = -m1.Vector.X * cos + m1.Vector.Y * sin;

            //устанавливем нMyовые скорости для частиц
            m1.Vector = new MyVector(Vn1 * sin - Vt2 * cos, Vn1 * cos + Vt2 * sin);
            m2.Vector = new MyVector(Vn2 * sin - Vt1 * cos, Vn2 * cos + Vt1 * sin);

            return m2.Vector * (m2.Vector.Length() * restOffset.Length());
        }
    }
}
