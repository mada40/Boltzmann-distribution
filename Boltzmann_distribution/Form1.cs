using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boltzmann_distribution
{
    public partial class Form1 : Form
    {
        Graphics g;
        Bitmap bmp;

        Pen pen;
        bool isMouse = false;
        Point mouse;
        
        World world;
        List<VectorPoints> vp = new List<VectorPoints>();
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(1000, 1000);
            g = Graphics.FromImage(bmp);
            pen = new Pen(Color.Black, 2f);
            world = new World();
            Molecule molecule = new Molecule(230, new RectangleF(0, 0, 700, 700), 5);
            Molecule molecule1 = new Molecule(23, new RectangleF(0, 0, 700, 700), 5);
            Line line1 = new Line(new PointF(500, 250), new Point(500, 550));

            world.add(molecule);
            world.add(molecule1);
            world.add(line1);

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g.Clear(Color.White);
            world.draw(ref g, pen);

            foreach (var v in vp)
            {
                v.draw(ref g, pen);
            }
            if (vp.Count > 0 && isMouse)
            {
                float rad = (float)Math.Sqrt(vp.Last().MinSquareLen);
                g.DrawEllipse(pen, mouse.X - rad, mouse.Y - rad, rad * 2f, rad * 2f);
            }
            pictureBox1.Image = bmp;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
            mouse = e.Location;
            vp.Add(new VectorPoints(4900));
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
        }

        

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouse) return;
            mouse = e.Location;
            vp.Last().addPoint(e.Location);
            
        }
    }
}
