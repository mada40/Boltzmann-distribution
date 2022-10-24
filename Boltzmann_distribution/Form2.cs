using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boltzmann_distribution
{
    public partial class Form2 : Form
    {
        SourceField tmp;

        public Form2()
        {
            InitializeComponent();
            label1.Text = trackBar1.Value.ToString();
            label2.Text = trackBar2.Value.ToString();
        }
        public PointF pos;
        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            float charge = trackBar1.Value;
            float r = trackBar2.Value;
            tmp = new SourceField(pos, charge, r);
            MainForm.world.add(tmp);
            tmp = null;
            Close();
        }


        

        private void Form2_Leave(object sender, EventArgs e)
        {
            tmp = null;
            Close();
        }

        private void Form2_Deactivate(object sender, EventArgs e)
        {
            tmp = null;
            Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar2.Value.ToString();
        }
    }
}
