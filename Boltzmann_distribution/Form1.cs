using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Boltzmann_distribution
{
    public partial class Form1 : Form
    {
        const int MIN_SQ_LEN = 50*50;
        Graphics g;
        Bitmap bmp;

        bool isMouse = false;
        bool isPause = true;

        
        World world;
        ArrayLines vp;
        static DateTime lastTime;
        double coeffSpeed = 1.0;
        public Form1()
        {
            InitializeComponent();
            
            bmp = new Bitmap(500, 500);
            g = Graphics.FromImage(bmp);

            Size size = pictureBox1.Size;
            RectangleF boundsWorld = new RectangleF(new PointF(0, 0), size);

            world = new World(boundsWorld, 20);
            //world.add(new SourceField(new PointF(300, 700), -100f, 100f, 32f));

            lastTime = DateTime.Now;

            vp = new ArrayLines(MIN_SQ_LEN);

            trackBarCount.Maximum = world.MaxCountMolecules;
            trackBarRadius.Value = (int)Molecule.R_DEF;
        }




        public static double DeltaMS()
        {
            var now = DateTime.Now;
            double a = (now - lastTime).TotalMilliseconds;
            lastTime = now;
            return a;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g.Clear(Color.White);

            double deltatime = DeltaMS() * coeffSpeed;
            pauseButton.Text = (isPause? "⏸︎" : "⏯︎");
            if (!isPause)
                world.update(deltatime, 16);

            world.draw(ref g, deltatime);



            //pageAuthors.Text = (1000 / deltatime * coeffSpeed).ToString();
            Pen penForArrLines = new Pen(Color.Gray, 1);
            vp.draw(ref g, penForArrLines, deltatime);
            pictureBox1.Image = bmp;

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                    case Keys.Space:
                    if (tabControl1.SelectedTab == tabControl1.TabPages[1])
                        isPause = !isPause;
                    break;
                default:
                    break;
            }

            if (!isPause)
                lastTime = DateTime.Now;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
            vp.posMouse = e.Location;
            vp.addPoint(e.Location);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
            vp.addPoint(e.Location, true);
            for (int i = 0; i < vp.getCount(); i++)
            {
                world.add(vp[i]);
            }
            vp.clear();
        }

        

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //world[0].Position = e.Location;

            if (!isMouse) 
                return;
            vp.posMouse = e.Location;
            vp.addPoint(e.Location);
            
        }

        private void pageModel_Enter(object sender, EventArgs e)
        {
            
            lastTime = DateTime.Now;
            trackBarSpeed.Value = 8 / 2;
            label4.Text = coeffSpeed.ToString() + "X";
        }

        private void pauseButton_MouseClick(object sender, MouseEventArgs e)
        {
            isPause = !isPause;
        }

        private void claerButton_MouseClick(object sender, MouseEventArgs e)
        {
            world.clear();
            trackBarCount.Value = 0;
            isPause = true;
            
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            coeffSpeed = 2.0 * trackBarSpeed.Value / 8;
          
            label4.Text = coeffSpeed.ToString() + "X";
            
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (world != null)
            {
                world.Bounds = pictureBox1.Bounds;
            }


            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            world.CountActMol = trackBarCount.Value;
            world.pushOutAllMolecules();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            for (int i = 0; i < world.MaxCountMolecules; i++)
            {
                world[i].R = trackBarRadius.Value;
            }

            world.pushOutAllMolecules();
            
            //world.update(0.1);
        }

        private void trackBarTemperature_Scroll(object sender, EventArgs e)
        {
            for (int i = 0; i < world.MaxCountMolecules; i++)
            {
                world[i].changeSpeed(trackBarTemperature.Value / 350.0);
            }

            world.pushOutAllMolecules();

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            float sign = e.Button == MouseButtons.Right ? -1 : 1;
            world.add(new SourceField(e.Location, 30 * sign, 70));
        }
    }
}
