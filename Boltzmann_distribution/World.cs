using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Boltzmann_distribution
{
    internal class World
    {
        private List<PhysicalObject> physicalObjects;
        private List<PhysicalObject> buffer = new List<PhysicalObject>();
        private Pen penForBuffer = new Pen(Color.Red, 4f);
        public int CountMolecules { get; private set; }
        public int CountLines { get; private set; }
        public int Count => physicalObjects.Count;
        private const float EPS_LEN_RAY = 0.0001f;
        private const float EPS = 0.1f;
        private const int MAX_COUNT_ITER = 32;
        public World()
        {
            CountLines = 0;
            CountMolecules = 0;
            physicalObjects = new List<PhysicalObject>();
        }

        public World(int cntMol, float molR, double minSpeed, double maxSpeed)
        {

        }

        private void addMaxNumberItemsFromBuffer()
        {
            //return;
            List<PhysicalObject> newBuff = new List<PhysicalObject>();
            foreach (var objFromBuff in buffer)
            {
                bool isPossible = true;
                foreach (var phObject in physicalObjects)
                    if (phObject is Molecule mol)
                    {
                        if(objFromBuff is Line lb)
                        {
                            if (MyMath.isInsercted(mol.Position, mol.R, lb.Position, lb.Point2))
                                isPossible = false;
                        }
                        
                        if(objFromBuff is Molecule mb)
                        {
                            if(MyMath.isInsercted(mol.Position, mol.R, mb.Position, mb.R))
                                isPossible = false;
                        }
                    }


                if (isPossible)
                    physicalObjects.Add(objFromBuff);
                else
                    newBuff.Add(objFromBuff);
      
            }

            buffer.Clear();
            buffer.AddRange(newBuff);
        }


        public void add(PhysicalObject phObject)
        {
            if (phObject is Line)
                CountLines++;

            if (phObject is Molecule)
                CountMolecules++;


            buffer.Add(phObject);
            addMaxNumberItemsFromBuffer();



        }

        public void update(double deltatime)
        {
            addMaxNumberItemsFromBuffer();
            updatePh();
            foreach (var activeObj in physicalObjects)
            {
                if(activeObj is Molecule actMol)
                {
                    updateOneMol(ref actMol, deltatime);
                }
            }

        }

        private void updatePh()
        {
            foreach (var item in physicalObjects)
            {
                if (item is SourceField s)
                {
                    foreach (var item1 in physicalObjects)
                    {
                        if (item1 is Molecule m)
                            Physics.CoulombInteraction(s, ref m);
                    }
                }
            }
            
        }

        private void updateOneMol(ref Molecule actMol, double deltatime)
        {
            MyVector offset = actMol.getOffset(deltatime);

            int countIter = 0;
            while (offset.LengthSquared() > EPS_LEN_RAY && countIter++ < MAX_COUNT_ITER)
            {
                double maxK = 1.0;
                PhysicalObject nearestObject = null;
                foreach (var passiveObj in physicalObjects)
                {
                    if (passiveObj == actMol)
                        continue;
                
                    double k = actMol.GetPossibleMaxOffset(passiveObj, offset);

                    if (k < maxK && k >= 0)
                    {
                        maxK = k;
                        nearestObject = passiveObj;
                    }
                
                }


                actMol.move(offset * maxK);
                offset = offset * (1.0 - maxK); ;//осталось пройти
                Physics.CollisionBetweenMoleculeAndObject(ref nearestObject, ref actMol, ref offset, EPS);


            }
            //check(actMol);
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
                phObject.draw(ref g, pen, deltatime);
            }

            foreach (var line in buffer)
            {
                line.draw(ref g, penForBuffer, deltatime);
            }
        }
    }
}
