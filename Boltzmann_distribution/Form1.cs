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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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
        List<ArrayLines> vp = new List<ArrayLines>();
        static DateTime lastTime;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(1000, 1000);
            g = Graphics.FromImage(bmp);
            pen = new Pen(Color.Black, 2f);
            world = new World();
            Random rnd = new Random(2);//2
            for (int i = 0; i < 30; i++)
            {
                double speed = 0.3;
 
                world.add(new Molecule(rnd.Next(), new RectangleF(50, 50, 750, 700), speed));
            }
            //world.add(new Molecule(230, new RectangleF(0, 100, 700, 650), 0.2));
            //world.add(new Molecule(99, new RectangleF(0, 0, 600, 700), 0.2));
            //world.add(new Molecule(24, new RectangleF(0, 250, 100, 700), 0.2));
            //world.add(new Molecule(25, new RectangleF(0, 250, 100, 700), 0.2));
            //world.add(new Line(new PointF(50, 620), new Point(850, 620)));

            world.add(new Line(new PointF(50, 50), new Point(800, 50)));
            world.add(new Line(new PointF(800, 750), new Point(800, 50)));
            world.add(new Line(new PointF(800, 750), new Point(50, 750)));
            world.add(new Line(new PointF(50, 50), new Point(50, 750)));

            //world.add(new Line(new PointF(253, 501), new Point(599, 272)));
            //world.add(new Line(new PointF(120, 120), new Point(190, 190)));
            //world.add(new Line(new PointF(490, 520), new Point(620, 625)));
            lastTime = DateTime.Now;

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

            double deltatime = 2.0*DeltaMS();
            deltatime = 8;
            world.update(deltatime);
            world.draw(ref g, pen, deltatime);

            Molecule m1 = (Molecule)world[0];
            Molecule m2 = (Molecule)world[1];
            if (MyMath.isInsercted(m1.Position, m1.R, m2.Position, m2.R))
            {
                deltatime = 16;
            }

            foreach (var item in vp)
            {
                item.draw(ref g, pen, deltatime);
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
            vp.Add(new ArrayLines(100));
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;
            for (int i = 0; i < vp.Last().getCount(); i++)
            {
                world.add(vp.Last()[i]);
            }
        }

        

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //world[0].Position = e.Location;

            if (!isMouse) 
                return;
            mouse = e.Location;
            vp.Last().addPoint(e.Location);
            
        }

        private void pageModel_Enter(object sender, EventArgs e)
        {
            lastTime = DateTime.Now;
        }
    }
}
