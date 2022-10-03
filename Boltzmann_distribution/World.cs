using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boltzmann_distribution
{
    internal class World
    {
        private List<PhysicalObject> physicalObjects;
        public int CountMolecules { get; private set; }
        public int CountLines { get; private set; }
        private const float EPS_LEN_RAY = 0.00001f;
        private const float EPS = 0.1f;
        public World()
        {
            CountLines = 0;
            CountMolecules = 0;
            physicalObjects = new List<PhysicalObject>();
        }

        public void add(PhysicalObject phObject)
        {
            physicalObjects.Add(phObject);

            if (phObject is Molecule)
                CountMolecules++;

            if (phObject is Line)
                CountLines++;

        }

        public void update(double deltatime)
        {
            foreach (var activeObj in physicalObjects)
            {
                if(activeObj is Molecule actMol)
                {
                    updateOneMol(ref actMol, deltatime);
                    
                }
            }
        }

        private void updateOneMol(ref Molecule actMol, double deltatime)
        {
            MyVector offset = actMol.getOffset(deltatime);
            PhysicalObject ignor = actMol;
            while (offset.LengthqSuared() > EPS_LEN_RAY)
            {
                MyVector maxOffset = offset;
                PhysicalObject nearestObject = null;
                foreach (var passiveObj in physicalObjects)
                {
                    if (passiveObj == actMol || passiveObj == ignor)
                        continue;

                    MyVector v = actMol.GetPossibleMaxOffset(passiveObj, offset);

                    if (v.Length() < maxOffset.Length())
                    {
                        maxOffset = v;
                        nearestObject = passiveObj;
                    }

                }

                ignor = nearestObject;

                actMol.move(maxOffset);
                offset = offset - maxOffset;//осталось пройти

                if (nearestObject is Line nLine)
                    offset = Physics.CollisionBetweenLineAndMolecule(ref nLine, ref actMol, offset, EPS);
                
                if (nearestObject is Molecule nMol)
                    offset = Physics.CollisionBetweenMoleculeAndMolecule(ref nMol, ref actMol, offset);

            }
        }

        public PhysicalObject this [int i]
        {
            get => physicalObjects[i];
            set => physicalObjects[i] = value;
        }
            
        public void draw(ref Graphics g, Pen pen, double deltatime)
        {
            foreach (var phObject in physicalObjects)
            {
                phObject.draw(ref g, pen);
            }
        }
    }
}
