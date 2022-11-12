using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace Boltzmann_distribution
{
    public partial class MainForm : Form
    {
        const int MIN_SQ_LEN = 30*30;
        Graphics g;
        Bitmap bmp;

        bool isMouse = false;
        bool isPause = true;

        
        public static World world;
        ArrayLines vp;
        static DateTime lastTime;
        double coeffSpeed = 1.0;
        const int N = 10;
        public MainForm()
        {
            InitializeComponent();
            
            bmp = new Bitmap(500, 500);
            g = Graphics.FromImage(bmp);

            Size size = pictureBox1.Size;
            RectangleF boundsWorld = new RectangleF(new PointF(0, 0), size);

            world = new World(boundsWorld, 75);

            lastTime = DateTime.Now;

            vp = new ArrayLines(MIN_SQ_LEN);

            trackBarCount.Maximum = world.MaxCountMolecules;
            trackBarRadius.Value = (int)Molecule.R_DEF;



            initializationAllCharts();
            

        }

        public void initializationAllCharts()
        {
            for (int i = 0; i < N; i++)
            {
                chart1.Series[0].Points.Add(0);
                chart2.Series[0].Points.Add(0);
            }

            chart1.ChartAreas[0].AxisX.Title = "КООРДИНАТА X";
            chart1.ChartAreas[0].AxisY.Title = "КОЛИЧЕСТВО\nМОЛЕКУЛ";

            chart2.ChartAreas[0].AxisX.Title = "КООРДИНАТА Y";
            chart2.ChartAreas[0].AxisY.Title = "КОЛИЧЕСТВО\nМОЛЕКУЛ";
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

            try
            {
                if (!isPause)
                    world.update(deltatime, 20);

                world.draw(ref g, deltatime);
            }
            catch (Exception)
            {
                allClear();
            }

            double midSpeed = 0.0;
            for (int i = 0; i < world.CountActMol; i++)
            {
                midSpeed += world[i].Vector.Length();
            }
            midSpeed /= world.CountActMol;
            //pageAuthors.Text = (1000 / deltatime * coeffSpeed).ToString();
            //pageAuthors.Text = midSpeed.ToString();
            Pen penForArrLines = new Pen(Color.Gray, 1);
            vp.draw(ref g, penForArrLines, deltatime);
            pictureBox1.Image = bmp;

            updateCharts(deltatime);
        }

        private void updateCharts(double deltatime)
        {
            int[] tmpX = new int[N];
            int[] tmpY = new int[N];
            int kX = (pictureBox1.Width + N - 1) / N;
            int kY = (pictureBox1.Height + N - 1) / N;

            int maxX = 1;
            int maxY = 1;
            for (int i = 0; i < world.CountActMol; i++)
            {
                int x = (int)(world[i].Position.X / kX);
                int y = (int)(world[i].Position.Y / kY);
                x = Math.Max(0, x);
                y = Math.Max(0, y);
                tmpX[x % N]++;
                tmpY[y % N]++;
                maxX = Math.Max(maxX, tmpX[x]);
                maxY = Math.Max(maxY, tmpY[y]);
            }
            double lastMaxX = chart1.ChartAreas[0].AxisY.Maximum;
            double lastMaxY = chart2.ChartAreas[0].AxisY.Maximum;

            chart1.ChartAreas[0].AxisX.Maximum = pictureBox1.Width;
            chart1.ChartAreas[0].AxisX.Interval = kX;

            chart2.ChartAreas[0].AxisX.Maximum = pictureBox1.Height;
            chart2.ChartAreas[0].AxisX.Interval = kY;

            if (maxX > lastMaxX || maxX * 2 < lastMaxX)
                chart1.ChartAreas[0].AxisY.Maximum = maxX;

            if (maxY > lastMaxY || maxY * 2 < lastMaxY)
                chart2.ChartAreas[0].AxisY.Maximum = maxY;

            splitContainer3.SplitterDistance = splitContainer3.Height / 2;

            for (int i = 0; i < N; i++)
            {
                chart1.Series[0].Points[i] = new DataPoint(i * kX, tmpX[i]);
                chart2.Series[0].Points[i] = new DataPoint(i * kY, tmpY[i]);
            }
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
            label4.Text =  "Скорость расчета: " + coeffSpeed.ToString() + "X";
        }

        private void pauseButton_MouseClick(object sender, MouseEventArgs e)
        {
            isPause = !isPause;
        }

        private void allClear()
        {
            world.clear();
            vp.clear();
            trackBarCount.Value = 0;
            isPause = true;
        }
        private void claerButton_MouseClick(object sender, MouseEventArgs e)
        {
            allClear();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            coeffSpeed = 2.0 * trackBarSpeed.Value / 8;

            label4.Text = "Скорость расчета: " + coeffSpeed.ToString() + "X";

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
            label1.Text = "Количество: " +  trackBarCount.Value.ToString();
            world.pushOutAllMolecules();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            for (int i = 0; i < world.MaxCountMolecules; i++)
            {
                world[i].R = trackBarRadius.Value;
            }

            world.pushOutAllMolecules();
          
        }

        private void trackBarTemperature_Scroll(object sender, EventArgs e)
        {
            for (int i = 0; i < world.MaxCountMolecules; i++)
            {
                world[i].changeSpeed(trackBarTemperature.Value / 800.0);
            }

            world.pushOutAllMolecules();

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
             Form2 f2 = new Form2();
             f2.pos = e.Location;
             f2.Show();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            Close();
            Dispose();
        }

        private void pageModel_Leave(object sender, EventArgs e)
        {
            isPause = true;
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var proc = Process.Start(@"https://docs.google.com/document/d/1NdwCArv2xffdeXgb7NO7TtBSbuGNnlMFkNmZk9t8lkE/edit?pli=1");
                proc.Close();
                proc.Dispose();
            }
            catch
            {
                
            }
        }
    }
}
