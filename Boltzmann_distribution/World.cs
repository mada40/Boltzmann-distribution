using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boltzmann_distribution
{
    internal class World
    {
        private List<PhysicalObject> physicalObjects;
        public int CountMolecules { get; private set; }
        public int CountLines { get; private set; }
        public World()
        {
            CountLines = 0;
            CountMolecules = 0;
            physicalObjects = new List<PhysicalObject>();
        }

        public void add(PhysicalObject phObject)
        {
            physicalObjects.Add(phObject);

            if (phObject is Molecule molecule)
                CountMolecules++;

            if (phObject is Line line)
                CountLines++;

        }

        public PhysicalObject this [int i]
        {
            get => physicalObjects[i];
            set => physicalObjects[i] = value;
        }
            
        public void draw(ref Graphics g, Pen pen)
        {
            foreach (var phObject in physicalObjects)
            {
                phObject.draw(ref g, pen);
            }
        }
    }
}
