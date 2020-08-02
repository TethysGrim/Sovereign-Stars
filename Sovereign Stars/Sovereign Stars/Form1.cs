using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sovereign_Stars
{
    public partial class Form1 : Form
    {
        #region vars
        Globals global;
        CursorDisp mousePos;

        Image display;
        //Image layers;
        Image bkg;
        Image map;
        Image path;
        //Image uilayer;
        //Image cursor;

        int lastTick;

        Graphics drawTools;

        Font systemHeader;
        Font planetHeader;

        List<Skybox> galaxy;
        int currentSky;

        MapNode start;
        MapNode end;

        public int zoomLevel = 1;
        public int zoomDefault = 1;
        public int xcenter = 300;
        public int ycenter = 300;
        public static int center;
        public static int distunit;

        public List<double> circlex;
        public List<double> circley;
        public List<double> hexx;
        public List<double> hexy;

        #endregion

        public Form1()
        {
            lastTick = Environment.TickCount;
            //ticks = 0;

            InitializeComponent();
            global = new Globals();
            mousePos = new CursorDisp();

            systemHeader = new Font("Consolas", 14);
            planetHeader = new Font("Consolas", 10);

            circlex = new List<double>();
            circley = new List<double>();
            for (int k = 0; k < 360; k += 5)
            {
                circlex.Add(Math.Cos(Globals.DegToRad(k)));
                circley.Add(Math.Sin(Globals.DegToRad(k)));
            }

            hexx = new List<double>();
            hexy = new List<double>();
            for (int k = 0; k < 360; k += 60)
            {
                hexx.Add(Math.Cos(Globals.DegToRad(k)));
                hexy.Add(Math.Sin(Globals.DegToRad(k)));
            }

            int numSys = Globals.randint.Next(10) + 10;
            galaxy = new List<Skybox>();
            for (int j = 0; j < numSys; j++)
            {
                bool placed = false;

                while (!placed)
                {
                    bool valid = true;
                    int skyx = Globals.randint.Next(100);
                    int skyy = Globals.randint.Next(100);

                    for (int k = 0; k < galaxy.Count(); k++)
                    {
                        if (Globals.Distance(skyx, skyy, galaxy[k].x, galaxy[k].y) < 10)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        galaxy.Add(new Skybox("System" + Convert.ToString(j), skyx, skyy));
                        placed = true;
                    }
                }
            }

            initializeBitmap();

            start = findClosestNode(0, 0);
        }

        public void drawBackground(Graphics tool)//Draws the black background
        {
            tool.FillRectangle(Brushes.Black, 0, 0, 600, 600);
        }

        public void initializeBitmap()
        {
            Globals.pic.MakeTransparent(Globals.pic.GetPixel(10, 10));

            //display = Globals.pic.Clone() as Image;
            //layers = Globals.pic.Clone() as Image;

            bkg = Globals.pic.Clone() as Image;
            drawTools = Graphics.FromImage(bkg);
            drawBackground(drawTools);

            map = bkg.Clone() as Image;
            path = Globals.pic.Clone() as Image;
            //uilayer = Globals.pic.Clone() as Image;
            //cursor = Globals.pic.Clone() as Image;

            refreshBitmap();
        }

        public void drawGrid(Graphics tool)//Draws unsuppressed hex nodes
        {
            for (int j = 0; j < galaxy[currentSky].system1.unsup.Count; j++)
            {
                MapNode node1 = galaxy[currentSky].system1.unsup[j];
                drawNode(node1, tool);
                //if (node1.suppressionCode == 0)
                //{
                //    for (int k = 0; k < node1.neighbors.Count; k++)
                //    {
                //        if (node1.neighbors[k].suppressionCode == 0)
                //        {
                //            MapNode node2 = node1.neighbors[k];
                //            tool.DrawLine(new Pen(Brushes.Purple, 1),
                //                            (int)xToDisp(node1.x), (int)yToDisp(node1.y),
                //                            (int)xToDisp(node2.x), (int)yToDisp(node2.y));
                //        }
                //    }
                //    for (int k = 0; k < node1.unsupNeighbors.Count; k++)
                //    {
                //        if (node1.neighbors[k].suppressionCode == 0)
                //        {
                //            MapNode node2 = node1.neighbors[k];
                //            tool.DrawLine(new Pen(Brushes.White, 1),
                //                (int)xToDisp(node1.x), (int)yToDisp(node1.y),
                //                (int)xToDisp(node2.x), (int)yToDisp(node2.y));
                //        }
                //    }
                //}
            }
        }

        public void drawNode(MapNode obj, Graphics tool)//Actually draws the hex nodes
        {
            //Brush color = Brushes.DarkBlue;
            //if (obj.atmos)
            //{
            //    color = Brushes.Cyan;
            //}
            //if (obj.ground)
            //{
            //    color = Brushes.Brown;
            //}

            //int r = (int)(1 * zoomLevel);
            //tool.FillEllipse(color, (int)Globals.xToDisp(obj.x, xcenter, zoomLevel) - r,
            //                   (int)Globals.yToDisp(obj.y, ycenter, zoomLevel) - r, r * 2, r * 2);

            if (!obj.distort)
            {
                for (int k = 0; k < hexx.Count(); k++)
                {
                    double x = Globals.gridSize * 1.5;
                    double y = Globals.gridSize * Math.Cos(Globals.DegToRad((double)30));
                    double hypotenuse = Globals.Distance(x, y);

                    double xdist = hypotenuse / 3.0 * hexx[k];
                    double ydist = hypotenuse / 3.0 * hexy[k];
                    double xdist2 = 0;
                    double ydist2 = 0;
                    if (k != 0)
                    {
                        xdist2 = hypotenuse / 3.0 * hexx[k - 1];
                        ydist2 = hypotenuse / 3.0 * hexy[k - 1];
                    }
                    if (k == 0)
                    {
                        xdist2 = hypotenuse / 3.0 * hexx[hexx.Count() - 1];
                        ydist2 = hypotenuse / 3.0 * hexy[hexy.Count() - 1];
                    }

                    Pen newpen = new Pen((Color.FromArgb(64, 64, 64)), 1);
                    tool.DrawLine(newpen,
                        (int)Globals.xToDisp(obj.x + xdist, xcenter, zoomLevel),
                        (int)Globals.yToDisp(obj.y + ydist, ycenter, zoomLevel),
                        (int)Globals.xToDisp(obj.x + xdist2, xcenter, zoomLevel),
                        (int)Globals.yToDisp(obj.y + ydist2, ycenter, zoomLevel));
                    newpen.Dispose();
                }
            }
        }

        public void drawSolar(Solar obj, Graphics tool)//Draws solars and their nodes
        {
            int r = (int)(obj.radius * zoomLevel);
            int ra = (int)((obj.radius + (Globals.gridSize * obj.atmosphere * 0.6)) * zoomLevel);

            if (obj.atmosphere != 0)
            {
                if (obj.fusion)
                {
                    tool.FillEllipse(Brushes.OrangeRed, (float)Globals.xToDisp(obj.x, xcenter, zoomLevel) - ra,
                        (float)Globals.yToDisp(obj.y, ycenter, zoomLevel) - ra, ra * 2, ra * 2);
                }
                else
                {
                    tool.FillEllipse(Brushes.DarkCyan, (float)Globals.xToDisp(obj.x, xcenter, zoomLevel) - ra,
                        (float)Globals.yToDisp(obj.y, ycenter, zoomLevel) - ra, ra * 2, ra * 2);
                }
            }

            if (obj.fusion)
            {

                tool.FillEllipse(Brushes.Yellow, (float)Globals.xToDisp(obj.x, xcenter, zoomLevel) - r,
                                   (float)Globals.yToDisp(obj.y, ycenter, zoomLevel) - r, r * 2, r * 2);
            }
            else
            {
                tool.FillEllipse(Brushes.PaleGreen, (float)Globals.xToDisp(obj.x, xcenter, zoomLevel) - r,
                                   (float)Globals.yToDisp(obj.y, ycenter, zoomLevel) - r, r * 2, r * 2);
            }

            for (int k = 0; k < circlex.Count(); k++)
            {
                double xdist = obj.gravityRad * circlex[k];
                double ydist = obj.gravityRad * circley[k];
                double xdist2 = 0;
                double ydist2 = 0;
                if (k != 0)
                {
                    xdist2 = obj.gravityRad * circlex[k - 1];
                    ydist2 = obj.gravityRad * circley[k - 1];
                }
                if (k == 0)
                {
                    xdist2 = obj.gravityRad * circlex[circlex.Count() - 1];
                    ydist2 = obj.gravityRad * circley[circley.Count() - 1];
                }

                Pen newpen = new Pen((Brushes.Blue), 1);
                tool.DrawLine(newpen,
                    (int)Globals.xToDisp(obj.x + xdist, xcenter, zoomLevel), (int)Globals.yToDisp(obj.y + ydist, ycenter, zoomLevel),
                    (int)Globals.xToDisp(obj.x + xdist2, xcenter, zoomLevel), (int)Globals.yToDisp(obj.y + ydist2, ycenter, zoomLevel));
                newpen.Dispose();
            }

            for (int j = 1; j < obj.size; j++)
            {
                for (int k = 0; k < circlex.Count(); k++)
                {
                    double xdist = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circlex[k];
                    double ydist = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circley[k];
                    double xdist2 = 0;
                    double ydist2 = 0;

                    if (k != 0)
                    {
                        xdist2 = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circlex[k - 1];
                        ydist2 = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circley[k - 1];
                    }
                    if (k == 0)
                    {
                        xdist2 = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circlex[circlex.Count() - 1];
                        ydist2 = ((0.6 * Globals.gridSize * j) + obj.radius - (0.5 * Globals.gridSize)) * circley[circley.Count() - 1];
                    }

                    Pen newpen = new Pen(Color.FromArgb(64, 64, 64), 1);
                    tool.DrawLine(newpen,
                        (int)Globals.xToDisp(obj.x + xdist, xcenter, zoomLevel),
                        (int)Globals.yToDisp(obj.y + ydist, ycenter, zoomLevel),
                        (int)Globals.xToDisp(obj.x + xdist2, xcenter, zoomLevel),
                        (int)Globals.yToDisp(obj.y + ydist2, ycenter, zoomLevel));
                    newpen.Dispose();
                }
            }

            for (int j = 0; j < obj.size; j++)
            {
                if (obj.size > 1)
                {
                    double anglePer = 360.0 / ((double)obj.size);
                    for (double k = 0; k < obj.size; k++)
                    {
                        //((obj.radius - 2 + (k * 6)) * Math.Sin(Globals.DegToRad((double)(anglePer * j) + obj.angle - angleOff)))
                        double angleOff = anglePer * (k % 2) / 2;

                        double angle = (j * anglePer) + (anglePer / 2) + obj.angle - angleOff;
                        double xdist1 = ((obj.radius + (0.1 * Globals.gridSize) + (k * 0.6 * Globals.gridSize)) * Math.Sin(Globals.DegToRad(angle)));
                        double ydist1 = ((obj.radius + (0.1 * Globals.gridSize) + (k * 0.6 * Globals.gridSize)) * Math.Cos(Globals.DegToRad(angle)));
                        double xdist2 = ((obj.radius - (0.5 * Globals.gridSize) + (k * 0.6 * Globals.gridSize)) * Math.Sin(Globals.DegToRad(angle)));
                        double ydist2 = ((obj.radius - (0.5 * Globals.gridSize) + (k * 0.6 * Globals.gridSize)) * Math.Cos(Globals.DegToRad(angle)));

                        Pen newpen = new Pen(Color.FromArgb(64, 64, 64), 1);
                        tool.DrawLine(newpen,
                            (int)Globals.xToDisp(obj.x + xdist1, xcenter, zoomLevel), (int)Globals.yToDisp(obj.y - ydist1, ycenter, zoomLevel),
                            (int)Globals.xToDisp(obj.x + xdist2, xcenter, zoomLevel), (int)Globals.yToDisp(obj.y - ydist2, ycenter, zoomLevel));
                        newpen.Dispose();
                    }
                }
            }

            tool.DrawString(obj.name, planetHeader, Brushes.Green,
                (float)Globals.xToDisp(obj.x, xcenter, zoomLevel) + 5, (float)Globals.yToDisp(obj.y, ycenter, zoomLevel) + 5);
        }

        public void drawSystem()//manages drawing of solars and hex grid
        {
            map = bkg.Clone() as Image;
            drawTools = Graphics.FromImage(map);
            drawTools.DrawImage(galaxy[currentSky].backgroundStars, 0, 0);
            for (int j = 0; j < galaxy[currentSky].solars.Count(); j++)
            {
                drawSolar(galaxy[currentSky].solars[j], drawTools);
            }

            for (int k = 0; k < circlex.Count(); k++)
            {
                //double xdist = galaxy[currentSky].systemRad * (Math.Cos(Globals.DegToRad((double)angle)));
                //double ydist = galaxy[currentSky].systemRad * (Math.Sin(Globals.DegToRad((double)angle)));
                //double xdist2 = galaxy[currentSky].systemRad * (Math.Cos(Globals.DegToRad((double)angle - 5)));
                //double ydist2 = galaxy[currentSky].systemRad * (Math.Sin(Globals.DegToRad((double)angle - 5)));

                double xdist = galaxy[currentSky].systemRad * circlex[k];
                double ydist = galaxy[currentSky].systemRad * circley[k];
                double xdist2 = 0;
                double ydist2 = 0;

                if (k != 0)
                {
                    xdist2 = galaxy[currentSky].systemRad * circlex[k - 1];
                    ydist2 = galaxy[currentSky].systemRad * circley[k - 1];
                }
                if (k == 0)
                {
                    xdist2 = galaxy[currentSky].systemRad * circlex[circlex.Count() - 1];
                    ydist2 = galaxy[currentSky].systemRad * circley[circley.Count() - 1];
                }

                Pen newpen = new Pen((Brushes.Blue), 1);
                drawTools.DrawLine(newpen,
                    (int)Globals.xToDisp(xdist, xcenter, zoomLevel), (int)Globals.yToDisp(ydist, ycenter, zoomLevel),
                    (int)Globals.xToDisp(xdist2, xcenter, zoomLevel), (int)Globals.yToDisp(ydist2, ycenter, zoomLevel));
                newpen.Dispose();
            }

            drawGrid(drawTools);
        }

        public void drawCursor(Graphics drawTools)
        {
            //cursor.Dispose();
            //cursor = Globals.pic.Clone() as Image;
            //drawTools = Graphics.FromImage(cursor);
            Point loc = pictureBox1.PointToClient(Cursor.Position);
            mousePos.x = loc.X;
            mousePos.y = loc.Y;

            if (mousePos.Active)
            {
                //tool.FillEllipse(Brushes.Red, mousePos.x - 13, mousePos.y - 13, 27, 27);
                drawTools.FillEllipse(Brushes.Red, (float)Globals.xToDisp(start.x, xcenter, zoomLevel) - 6,
                    (float)Globals.yToDisp(start.y, ycenter, zoomLevel) - 6, 11, 11);
                end = findClosestNode(mousePos.x, mousePos.y);
                drawTools.FillEllipse(Brushes.Red, (float)Globals.xToDisp(end.x, xcenter, zoomLevel) - 6,
                    (float)Globals.yToDisp(end.y, ycenter, zoomLevel) - 6, 11, 11);

                //for (int j = 0; j < end.unsupNeighbors.Count(); j++)
                //{
                //    drawTools.DrawLine(new Pen((Brushes.DarkRed), 2),
                //                            (int)xToDisp(end.x), (int)yToDisp(end.y),
                //                            (int)xToDisp(end.unsupNeighbors[j].x), (int)yToDisp(end.unsupNeighbors[j].y));
                //}

                //drawTools.DrawString("X: " + Convert.ToString((int)end.x), planetHeader, Brushes.Red,
                //    (float)xToDisp(end.x) + 20, (float)yToDisp(end.y) - 30);
                //drawTools.DrawString("Y: " + Convert.ToString((int)end.y), planetHeader, Brushes.Red,
                //    (float)xToDisp(end.x) + 20, (float)yToDisp(end.y) - 20);
                //drawTools.DrawString("N: " + Convert.ToString((int)end.unsupNeighbors.Count()), planetHeader, Brushes.Red,
                //    (float)xToDisp(end.x) + 20, (float)yToDisp(end.y) - 10);

                Pen newpen = new Pen((Brushes.Red), 2);
                drawTools.DrawLine(newpen,
                    (int)Globals.xToDisp(start.x, xcenter, zoomLevel), (int)Globals.yToDisp(start.y, ycenter, zoomLevel),
                    (int)Globals.xToDisp(end.x, xcenter, zoomLevel), (int)Globals.yToDisp(end.y, ycenter, zoomLevel));
                newpen.Dispose();

                double dist = Globals.Distance(start.x, start.y, end.x, end.y);
                drawTools.DrawString(Convert.ToString(dist), planetHeader, Brushes.Red,
                    (float)(Globals.xToDisp(end.x, xcenter, zoomLevel) + 5), (float)(Globals.yToDisp(end.y, ycenter, zoomLevel) - 10));
            }
            if (!mousePos.Active)
            {
                drawTools.FillEllipse(Brushes.Red, mousePos.x - 5, mousePos.y - 5, 11, 11);
            }
        }

        public void drawUILayer(Graphics drawTools)
        {
            //uilayer = Globals.pic.Clone() as Image;
            //drawTools = Graphics.FromImage(uilayer);
            for (int j = 0; j < galaxy[currentSky].solars.Count(); j++)
            {
                drawTools.DrawString(galaxy[currentSky].solars[j].name, planetHeader, Brushes.LightGreen,
                    (float)Globals.xToDisp(galaxy[currentSky].solars[j].x, xcenter, zoomLevel) + 5,
                    (float)Globals.yToDisp(galaxy[currentSky].solars[j].y, ycenter, zoomLevel) + 5);
            }

            int now = Environment.TickCount;
            int fps = Convert.ToInt32(1000 / (now - lastTick));
            drawTools.DrawString("FPS: " + Convert.ToString(fps), systemHeader, Brushes.Magenta, 5, 5);
            lastTick = now;

            drawTools.DrawString(galaxy[currentSky].system1.name, systemHeader, Brushes.Magenta, 260, 5);

            drawTools.FillRectangle(Brushes.Black, 500, 500, 100, 100);
            int r = 2;
            drawTools.FillEllipse(Brushes.Blue, 500 - (r * 2) + galaxy[currentSky].x, 500 - (r * 2) + galaxy[currentSky].y, r * 4, r * 4);
            for (int j = 0; j < galaxy.Count(); j++)
            {
                drawTools.FillEllipse(Brushes.White, 500 - r + galaxy[j].x, 500 - r + galaxy[j].y, r * 2, r * 2);
            }

            drawTools.DrawString("Arrow Keys to Scroll", planetHeader, Brushes.Magenta, 5, 555);
            drawTools.DrawString("/ or * to Switch Systems", planetHeader, Brushes.Magenta, 5, 565);
            drawTools.DrawString("PgUp or PgDn to Zoom", planetHeader, Brushes.Magenta, 5, 575);
            drawTools.DrawString("Press Q while dragging!", planetHeader, Brushes.Magenta, 5, 585);
        }

        public void drawPath()
        {
            if (mousePos.Active)
            {
                drawTools = Graphics.FromImage(path);
                Point loc = pictureBox1.PointToClient(Cursor.Position);
                mousePos.x = loc.X;
                mousePos.y = loc.Y;

                List<MapNode> foundpath = galaxy[currentSky].system1.findPath(start, end);
                MapNode prev = foundpath[0];
                for (int j = 1; j < foundpath.Count(); j++)
                {
                    Pen newpen = new Pen((Brushes.Yellow), 2);
                    drawTools.DrawLine(newpen,
                        (int)Globals.xToDisp(prev.x, xcenter, zoomLevel), (int)Globals.yToDisp(prev.y, ycenter, zoomLevel),
                        (int)Globals.xToDisp(foundpath[j].x, xcenter, zoomLevel), (int)Globals.yToDisp(foundpath[j].y, ycenter, zoomLevel));
                    newpen.Dispose();

                    prev = foundpath[j];
                }
            }
        }

        public MapNode findClosestNode(int _x, int _y)
        {
            double x = Globals.dispToX(_x, xcenter, zoomLevel);
            double y = Globals.dispToY(_y, ycenter, zoomLevel);

            Skybox sky = galaxy[currentSky];
            MapNode closest = sky.system1.nodes[0];
            for (int j = 0; j < sky.system1.nodes.Count(); j++)
            {
                if (sky.system1.nodes[j].suppressionCode == 0)
                {
                    if (Globals.Distance(x, y, closest.x, closest.y) > Globals.Distance(x, y,
                        sky.system1.nodes[j].x, sky.system1.nodes[j].y))
                    {
                        closest = sky.system1.nodes[j];
                    }
                }
            }

            return closest;
        }

        private void Form1_MouseHandle(object sender, MouseEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Home:
                    {
                        zoomLevel = zoomDefault;
                        xcenter = 300;
                        ycenter = 300;

                        refreshBitmap();
                        break;
                    }
                case Keys.Q:
                    {
                        refreshBitmap();

                        drawPath();
                        break;
                    }
                case Keys.Divide:
                    {
                        if (currentSky > 0)
                        {
                            zoomLevel = zoomDefault;
                            xcenter = 300;
                            ycenter = 300;

                            currentSky--;
                            refreshBitmap();
                        }

                        break;
                    }
                case Keys.Multiply:
                    {
                        if (currentSky < galaxy.Count() - 1)
                        {
                            zoomLevel = zoomDefault;
                            xcenter = 300;
                            ycenter = 300;

                            currentSky++;
                            refreshBitmap();
                        }

                        break;
                    }
                case Keys.PageDown:
                    {
                        if (zoomLevel > 1)
                        {
                            zoomLevel /= 2;
                        }
                        xcenter = (int)(300 - (300 - xcenter) * 0.5);
                        ycenter = (int)(300 - (300 - ycenter) * 0.5);

                        refreshBitmap();
                        break;
                    }
                case Keys.PageUp:
                    {
                        zoomLevel *= 2;
                        xcenter = 300 - (300 - xcenter) * 2;
                        ycenter = 300 - (300 - ycenter) * 2;

                        refreshBitmap();
                        break;
                    }
                case Keys.Up:
                    {
                        ycenter += 50;

                        refreshBitmap();
                        break;
                    }
                case Keys.Down:
                    {
                        ycenter -= 50;

                        refreshBitmap();
                        break;
                    }
                case Keys.Left:
                    {
                        xcenter += 50;

                        refreshBitmap();
                        break;
                    }
                case Keys.Right:
                    {
                        xcenter -= 50;

                        refreshBitmap();
                        break;
                    }

            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePos.Active = true;
            start = findClosestNode(mousePos.x, mousePos.y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mousePos.Active = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        public void refreshBitmap()
        {
            //bkg = Globals.pic.Clone() as Image;
            //drawTools = Graphics.FromImage(bkg);
            //drawBackground(drawTools);
            //map.Dispose();
            map = Globals.pic.Clone() as Image;
            //path.Dispose();
            path = Globals.pic.Clone() as Image;
            drawSystem();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //tool.DrawImage(Image.FromFile("Test.jpg"), 0, 0, 600, 600);
            //layers.Dispose();
            display = map.Clone() as Image;
            drawTools = Graphics.FromImage(display);

            //uilayer.Dispose();
            //cursor.Dispose();

            //drawTools = Graphics.FromImage(layers);

            //drawTools.DrawImage(bkg, 0, 0, 600, 600);
            //drawTools.DrawImage(galaxy[currentSky].backgroundStars, 0, 0, 600, 600);
            //drawTools.DrawImage(map, 0, 0, 600, 600);
            drawTools.DrawImage(path, 0, 0, 600, 600);
            drawUILayer(drawTools);
            //drawTools.DrawImage(uilayer, 0, 0, 600, 600);
            drawCursor(drawTools);
            //drawTools.DrawImage(cursor, 0, 0, 600, 600);
            //drawTools.DrawImage(layers, 0, 0, 600, 600);

            pictureBox1.BackgroundImage = display;
            timer1.Enabled = true;
        }
    }
}