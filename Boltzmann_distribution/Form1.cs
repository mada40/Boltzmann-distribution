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
        const float ERASER_R = 30f;
        const int N = 10;
        const string COUNT_STR_RU = "Количество";
        const string COUNT_STR_EN = "Count";

        const string SPEED_STR_RU = "Скорость расчета";
        const string SPEED_STR_EN = "Сalculation speed";

        Graphics g;
        Bitmap bmp;

        Pen penForArrLines = new Pen(Color.Gray, 1);
        bool isMouse = false;
        bool isEraser = false; 
        bool isPause = true;
        bool isRussian = true;

        PointF posEraser;

        public static World world;
        ArrayLines vp;
        static DateTime lastTime;
        double coeffSpeed = 1.0;
        public MainForm()
        {
            InitializeComponent();
            
            bmp = new Bitmap(500, 500);
            g = Graphics.FromImage(bmp);

            Size size = pictureBox1.Size;
            RectangleF boundsWorld = new RectangleF(new PointF(0, 0), size);

            world = new World(boundsWorld, 75);
            world.add(new SourceField(new PointF(75, 75), 50, 0));
            world.CountActMol = 45;

            lastTime = DateTime.Now;

            vp = new ArrayLines(MIN_SQ_LEN);

            trackBarCount.Maximum = world.MaxCountMolecules;
            trackBarCount.Value = world.CountActMol;
            trackBarRadius.Value = (int)Molecule.R_DEF;



            initializationAllCharts();
            translateALL(true);
            labelCount.Text += ": " + trackBarCount.Value.ToString();

        }

        public void translateALL(bool isRu)
        {
            if(isRu)
            {
                pageAuthors.Text = "АВТОРЫ";
                pageMain.Text = "ГЛАВНАЯ";
                pageModel.Text = "МОДЕЛЬ";

                buttonExit.Text = "ВЫХОД";
                buttonTheory.Text = "ТЕОРИЯ";
                buttonChangeLanguage.Text = "Язык: Русский";

                labelMSU.Text = "МГУ им. М. В. Ломоносова";
                labelName.Text = "Распределение Больцмана в сосудах различной формы";
                labelRadius.Text = "Радиус";
                labelTemperature.Text = "Температура";
                labelCount.Text = COUNT_STR_RU;
                labelSpeed.Text = SPEED_STR_RU;

                chart2.Titles[0].Text = "Плотность распределения по координатам";
                chart2.ChartAreas[0].AxisY.Title = "ЧИСЛО\nМОЛЕКУЛ";
                chart2.ChartAreas[1].AxisY.Title = "ЧИСЛО\nМОЛЕКУЛ";

                labelPolina.Text = "Кривуля Полина";
                labelIvan.Text = "Листопадов Иван";
                labelNikita.Text = "Панин Никита";

                labelOlga.Text = "Научный руководитель: Чичигина Ольга Александровна";

            }
            else
            {
                pageAuthors.Text = "AUTHORS";
                pageMain.Text = "MAIN";
                pageModel.Text = "MODEL";

                buttonExit.Text = "EXIT";
                buttonTheory.Text = "THEORY";
                buttonChangeLanguage.Text = "Language: English";

                labelMSU.Text = "Lomonosov Moscow State University";
                labelName.Text = "Boltzmann distribution in vessels of various shapes";

                labelRadius.Text = "Radius";
                labelTemperature.Text = "Temperature";
                labelCount.Text = COUNT_STR_EN;
                labelSpeed.Text = SPEED_STR_EN;

                chart2.Titles[0].Text = "Distribution density by coordinates";
                chart2.ChartAreas[0].AxisY.Title = "NUMBER OF \nMOLECULES";
                chart2.ChartAreas[1].AxisY.Title = "NUMBER OF \nMOLECULES";

                labelPolina.Text = "Krivulya Polina";
                labelIvan.Text = "Listopadov Ivan";
                labelNikita.Text = "Panin Nikita";

                labelOlga.Text = "Scientific supervisor: Olga Alexandrovna Chichigina";
            }
        }

        public void initializationAllCharts()
        {
            for (int i = 0; i < N; i++)
            {
                chart2.Series[0].Points.Add(0);
                chart2.Series[1].Points.Add(0);
            }

            FontFamily fontFamily = chart2.ChartAreas[0].AxisX.TitleFont.FontFamily;

            chart2.ChartAreas[0].AxisX.Title = "X";
            chart2.ChartAreas[1].AxisX.Title = "Y";
            for (int i = 0; i < 2; i++)
            {
                
                chart2.ChartAreas[i].AxisY.TitleFont = new Font(fontFamily, 14f);
              
                chart2.ChartAreas[i].AxisX.TitleFont = new Font(fontFamily, 17f);
                chart2.ChartAreas[i].AxisX.LabelStyle.Font = new Font(fontFamily, 12f);
                chart2.ChartAreas[i].AxisY.LabelStyle.Font = new Font(fontFamily, 14f);
            }

            int kX = (pictureBox1.Width + N - 1) / N;
            int kY = (pictureBox1.Height + N - 1) / N;

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = pictureBox1.Width;
            chart2.ChartAreas[0].AxisX.Interval = kX;

            chart2.ChartAreas[1].AxisX.Minimum = 0;
            chart2.ChartAreas[1].AxisX.Maximum = pictureBox1.Height;
            chart2.ChartAreas[1].AxisX.Interval = kY;

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
           
            vp.draw(ref g, penForArrLines, deltatime);

            if(isEraser)
            {
                float x = posEraser.X;
                float y = posEraser.Y;
                g.DrawEllipse(penForArrLines, x - ERASER_R, y - ERASER_R, 2 * ERASER_R, 2 * ERASER_R);
            }
            pictureBox1.Image = bmp;
            //pageAuthors.Text = (1000.0 / deltatime / coeffSpeed).ToString();
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
            double lastMaxX = chart2.ChartAreas[0].AxisY.Maximum;
            double lastMaxY = chart2.ChartAreas[1].AxisY.Maximum;


            if (maxX > lastMaxX || maxX * 2 < lastMaxX)
                chart2.ChartAreas[0].AxisY.Maximum = maxX;

            if (maxY > lastMaxY || maxY * 2 < lastMaxY)
                chart2.ChartAreas[1].AxisY.Maximum = maxY;


            for (int i = 0; i < N; i++)
            {
                chart2.Series[0].Points[i] = new DataPoint(i * kX, tmpX[i]);
                chart2.Series[1].Points[i] = new DataPoint(i * kY, tmpY[i]);
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
            if(e.Button == MouseButtons.Left)
            {
                isMouse = true;
                vp.posMouse = e.Location;
                vp.addPoint(e.Location);
            }
            else
            {
                isEraser = true;
                posEraser = e.Location;
                world.remove(e.Location, ERASER_R);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMouse)
            {
                vp.addPoint(e.Location, true);
                for (int i = 0; i < vp.getCount(); i++)
                {
                    world.add(vp[i]);
                }
                vp.clear();
            }
            isMouse = false;
            isEraser = false;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //world[0].Position = e.Location;

            if (isMouse)
            {
                vp.posMouse = e.Location;
                vp.addPoint(e.Location);
            }

            if(isEraser)
            {
                posEraser = e.Location;
                world.remove(e.Location, ERASER_R);
            }

            
        }

        private void pageModel_Enter(object sender, EventArgs e)
        {
            
            lastTime = DateTime.Now;
            trackBarSpeed.Value = 8 / 2;
            labelSpeed.Text =  (isRussian? SPEED_STR_RU: SPEED_STR_EN) + ": " + coeffSpeed.ToString() + "X";
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
            isMouse = false;
            isEraser = false;
        }
        private void claerButton_MouseClick(object sender, MouseEventArgs e)
        {
            allClear();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            coeffSpeed = 2.0 * trackBarSpeed.Value / 8;

            labelSpeed.Text = (isRussian ? SPEED_STR_RU : SPEED_STR_EN) + ": " + coeffSpeed.ToString() + "X";

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (world != null)
            {
                world.Bounds = pictureBox1.Bounds;

                for(int i = 0; i < 150; i++)
                    world.update(10.0, 20);
            }

            initializationAllCharts();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);


        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            world.CountActMol = trackBarCount.Value;
            labelCount.Text = (isRussian ? COUNT_STR_RU : COUNT_STR_EN) + ": " +  trackBarCount.Value.ToString();
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
             Form2 f2 = new Form2(isRussian);
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
            isMouse = false;
            isEraser = false;
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

        private void buttonChangeLanguage_MouseClick(object sender, MouseEventArgs e)
        {
            isRussian = !isRussian;
            translateALL(isRussian);
        }
    }
}
