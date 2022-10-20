using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    internal class World
    {
        private  Molecule[] molecules;

        private List<PhysicalObject> passiveObject = new List<PhysicalObject>();
        private List<PhysicalObject> bufferPassObj = new List<PhysicalObject>();
        private Pen penForBuffer = new Pen(Color.Red, 4f);
        private Pen penForPassObj = new Pen(Color.Black, 2f);
        private Pen penForMolecule = new Pen(Color.Orange, 1f);
        public int CountActMol { get; set; }
        public int MaxCountMolecules { get; private set; }
        public int CountLines { get; private set; }
        public int Count => passiveObject.Count;
        private const float EPS_LEN_RAY = 0.0000000001f;
        private const float EPS = 0.1f;
        private const int MAX_COUNT_ITER = 8;

        private RectangleF _bounds;
        public RectangleF Bounds 
        { 
            get => _bounds; 
            set
            {
                _bounds = value;
                createBounds();

                Random rnd = new Random();
                for (int i = 0; i < MaxCountMolecules; i++)
                {
                    molecules[i].setRandomPos(rnd.Next(), _bounds);
                }
            }
        }

        private void createBounds()
        {
            PointF LT = _bounds.Location;
            PointF RT = new PointF(LT.X + _bounds.Width, LT.Y);

            PointF LB = new PointF(LT.X, LT.Y + _bounds.Height);
            PointF RB = new PointF(RT.X, RT.Y + _bounds.Height);

            if (CountLines == 0)
            {
                add(new Line());
                add(new Line());
                add(new Line());
                add(new Line());
            }
            passiveObject[0] = (new Line(LT, RT));
            passiveObject[1] = (new Line(RT, RB));
            passiveObject[2] = (new Line(RB, LB));
            passiveObject[3] = (new Line(LB, LT));
        }

        public World(RectangleF bounds, int cntMol) 
        {
            molecules = new Molecule[cntMol];
            for(int i = 0; i <cntMol; i++)
            {
                molecules[i] = new Molecule();
            }
            CountLines = 0;
            MaxCountMolecules = cntMol;
            CountActMol = 0;
            Bounds = bounds;

            Random rnd = new Random(5);//2
            for (int i = 0; i < cntMol; i++)
            {
                double speed = 0.02;
                Molecule tmp = new Molecule(rnd.Next(), bounds, speed);

                add(tmp);
            }
        }

        private void addMaxNumberItemsFromBuffer()
        {
            //return;
            List<PhysicalObject> newBuff = new List<PhysicalObject>();
            foreach (var objFromBuff in bufferPassObj)
            {
                bool isPossible = true;
                if (objFromBuff is Line l1)
                {
                    for(int i = 0; i < CountActMol; i++)
                    {
                        Molecule mol = molecules[i];
                        if (MyMath.isInsercted(mol.Position, mol.R, l1.Position, l1.Point2))
                            isPossible = false;
                    }
                }

                if (isPossible)
                    passiveObject.Add(objFromBuff);
                else
                    newBuff.Add(objFromBuff);  
            }

            bufferPassObj.Clear();
            bufferPassObj.AddRange(newBuff);

        }

        private int cntAddedMol = 0;
        public void add(PhysicalObject phObject)
        {
            if (phObject is Molecule mol)
            {
                molecules[cntAddedMol++] = mol;
            }
            else
            {
                if (phObject is Line line)
                {
                    CountLines++;
                }
                bufferPassObj.Add(phObject);
            }



            addMaxNumberItemsFromBuffer();
        }

        public void update(double deltatime, int k)
        {
            for (int i = 0; i < k; i++)
            {
                upp(deltatime / k);
            }
        }

        private void upp(double deltatime)
        {
            addMaxNumberItemsFromBuffer();

            for (int i = 0; i < CountActMol; i++)
            {
                Molecule actMol = molecules[i];
                updateOneMol(ref actMol, deltatime);
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
                foreach (var passiveObj in passiveObject)
                {
                    PhysicalObject ph = passiveObj;
                    gg(ref actMol, ref ph, ref offset, ref maxK, ref nearestObject);
                }


                for (int i = 0; i < CountActMol; i++)
                {
                    PhysicalObject tmp = molecules[i];
                    gg(ref actMol, ref tmp, ref offset, ref maxK, ref nearestObject);
                    molecules[i] = (Molecule)tmp;
                }


                actMol.move(offset * maxK);
                offset = offset * (1.0 - maxK);//осталось пройти
                Physics.CollisionBetweenMoleculeAndObject(ref nearestObject, ref actMol, ref offset, EPS);


            }
        }

        private void gg(ref Molecule actMol, ref PhysicalObject passiveObj, ref MyVector offset, ref double maxK, ref PhysicalObject nearestObject)
        {
            if (actMol == passiveObj)
                return;

            Physics.PushOut(ref actMol, ref passiveObj);

            if (passiveObj is SourceField s)
            {
                MyVector grav = Physics.CoulombInteraction(s, actMol.Position);
                offset += grav;
                actMol.Vector += grav;
            }
            double k = actMol.GetPossibleMaxOffset(passiveObj, offset);

            if (k < maxK && k >= 0.0)
            {
                maxK = k;
                nearestObject = passiveObj;
            }
            
        }



        public Molecule this [int i]
        {
            get => molecules[i];
            set => molecules[i] = value;
        }

        public void pushOutAllMolecules()
        {
            for (int k = 0; k < 5; k++)
            {
                for (int i = 0; i < CountActMol; i++)
                {
                    Molecule m1 = molecules[i];
                    for (int j = 0; j < CountActMol; j++)
                    {
                        if (i == j)
                            continue;

                        Molecule m2 = molecules[j];
                        Physics.PushOut(ref m1, ref m2);
                    }

                    foreach (var item in passiveObject)
                    {
                        PhysicalObject tmp = item;
                        Physics.PushOut(ref m1, ref tmp);
                    }
                }
            }
        }
            
        public void draw(ref Graphics g, double deltatime)
        {
            foreach (var phObject in passiveObject)
            {
                phObject.draw(ref g, penForPassObj, deltatime);
            }

            for(int i = 0; i < CountActMol; ++i)
            {
                molecules[i].draw(ref g, penForMolecule, deltatime);
            }


            foreach (var passObj in bufferPassObj)
            {
                passObj.draw(ref g, penForBuffer, deltatime);
            }


        }

        public void clear()
        {
            bufferPassObj.Clear();
            passiveObject.Clear();
            cntAddedMol = 0;
            CountActMol = 0;
            CountLines = 0;
            Random rnd = new Random();
            foreach (var mol in molecules)
            {
                mol.setRandomPos(rnd.Next(), Bounds);
                mol.setSpeed(rnd.Next(), 0.02);
            }
            createBounds();
        }
    }
}
