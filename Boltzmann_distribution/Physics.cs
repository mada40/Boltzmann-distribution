using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    static internal class Physics
    {
        static public void CollisionBetweenLineAndMolecule(ref Line passLine, ref Molecule actMol, ref MyVector restOffset, float epsilon)
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
            restOffset =  MyMath.Reflect(restOffset, normal);
        }

        static public void CollisionBetweenMoleculeAndMolecule(ref Molecule passMol, ref Molecule actMol, ref MyVector restOffset)
        {
            float Dx = passMol.Position.X - actMol.Position.X;
            float Dy = passMol.Position.Y - actMol.Position.Y;
            double d = MyVector.distance(passMol.Position, actMol.Position); // расстояние между центрами частиц

            double sin = Dx / d;
            double cos = Dy / d;

            // формула из интернета 
            double Vn1 = actMol.Vector.X * sin + actMol.Vector.Y * cos;
            double Vn2 = passMol.Vector.X * sin + passMol.Vector.Y * cos;

            double Vt1 = -actMol.Vector.X * cos + actMol.Vector.Y * sin;
            double Vt2 = -passMol.Vector.X * cos + passMol.Vector.Y * sin;


            double restDeltaTime = restOffset.Length() / actMol.Vector.Length();

            //устанавливем новые скорости для частиц
            passMol.Vector = new MyVector(Vn1 * sin - Vt2 * cos, Vn1 * cos + Vt2 * sin);
            actMol.Vector = new MyVector(Vn2 * sin - Vt1 * cos, Vn2 * cos + Vt1 * sin);

            restOffset =  actMol.Vector * restDeltaTime;
        }

        static public void CollisionBetweenMoleculeAndObject(ref PhysicalObject passPhObj, ref Molecule actMol, ref MyVector restOffset, float epsilon)
        {
            if (passPhObj == null)
                return;

            if (passPhObj is Line line)
            {
                CollisionBetweenLineAndMolecule(ref line, ref actMol, ref restOffset, epsilon);
                return;
            }

            if (passPhObj is Molecule mol)
            {
                CollisionBetweenMoleculeAndMolecule(ref mol, ref actMol, ref restOffset);
                return;
            }
            throw new Exception();
        }

        //CoulombInteraction

        static public void CoulombInteraction(SourceField s, ref Molecule m)
        {
            MyVector v = new MyVector(s.Position, m.Position);
            float dist = (float)v.Length();
            MyVector.normalize(ref v);
            MyVector gravVec = v * s.Charge * s.F(dist);
            m.Vector -= gravVec;

        }
    }
}
