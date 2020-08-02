using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    public class Skybox
    {
        public Image backgroundStars;
        public NodeMap system1;

        public List<Solar> solars;
        public int systemRad;

        public int x;
        public int y;

        public Skybox(string name, int _x, int _y)
        {
            backgroundStars = new Bitmap(600, 600);
            starMap();

            system1 = new NodeMap(name);
            system1.hexGrid(199, 0, 0);

            x = _x;
            y = _y;

            genPlanets();
        }

        public void genPlanets()
        {
            solars = new List<Solar>();

            List<int> sizes = new List<int>();

            int size = 9 + Globals.randint.Next(3);
            int sizeInc = Globals.randint.Next(3);
            if (sizeInc == 0)
            {
                sizeInc--;
            }
            while (Globals.randint.Next(2) == 0 && size > 0 && size < 15)
            {
                size += sizeInc;
            }
            sizes.Add(size);

            int numPlanets = 5 + Globals.randint.Next(6) + Globals.randint.Next(6) + Globals.randint.Next(6);
            for (int j = 0; j < numPlanets; j++)
            {
                size = 1;
                while (Globals.randint.Next(2) == 0 && size > 0 && size < 15)
                {
                    size += 1;
                }
                sizes.Add(size);
            }

            sizes.Sort();
            sizes.Reverse();

            systemRad = 100;

            if (sizes[0] > (sizes[1] * 1.2))
            {
                createSingle(sizes);
            }
            else
            {
                createBinary(sizes);
            }

            system1.cullDistance(systemRad);
            system1.unsup = new List<MapNode>();
            for (int j = 0; j < system1.nodes.Count(); j++)
            {
                if (system1.nodes[j].suppressionCode == 0)
                {
                    system1.unsup.Add(system1.nodes[j]);
                }
                system1.nodes[j].checkUnsup();
            }
        }

        public void createSingle(List<int> sizes)
        {
            Solar newPlanet;

            double mass = (4.0 / 3.0) * Math.PI * Math.Pow(sizes[0], 3);
            systemRad = (int)(Math.Sqrt(mass) + (sizes[0] * Globals.gridSize) + Globals.wellradius(sizes[0]));
            for (int j = 0; j < sizes.Count(); j++)
            {
                newPlanet = randomPlanet(sizes[j]);
                if (j == 0)
                {
                    newPlanet.x = 0;
                    newPlanet.y = 0;
                    solars.Add(newPlanet);
                    system1.gravityWell(newPlanet);
                }
                else
                {
                    if (Globals.Distance(newPlanet.x, newPlanet.y) + newPlanet.gravityRad + Globals.gridSize < systemRad)
                    {
                        solars.Add(newPlanet);
                        system1.gravityWell(newPlanet);
                    }
                }
            }
        }

        public void createBinary(List<int> sizes)
        {
            Solar newPlanet;

            double distance = 1;
            double wellradii = Globals.wellradius(sizes[0]) + Globals.wellradius(sizes[1]);
            double angle = Globals.randint.Next(360);
            double mass1 = ((double)4 / (double)3) * Math.PI * Math.Pow(sizes[0], 2);
            double mass2 = ((double)4 / (double)3) * Math.PI * Math.Pow(sizes[1], 2);
            double ratio1 = mass2 / mass1;
            double ratio2 = mass1 / mass2;

            systemRad = (int)(wellradii * ratio2 * Math.Sqrt(mass1 + mass2) / 15);

            double xdist1 = (distance * ratio1 * (Math.Cos(Globals.DegToRad(angle)))) +
                (wellradii * ratio1 * Math.Cos(Globals.DegToRad(angle)));
            double ydist1 = (distance * ratio1 * (Math.Sin(Globals.DegToRad(angle)))) +
                (wellradii * ratio1 * Math.Sin(Globals.DegToRad(angle)));
            double xdist2 = (distance * ratio2 * (Math.Cos(Globals.DegToRad(angle - 180)))) +
                (wellradii * ratio2 * Math.Cos(Globals.DegToRad(angle - 180)));
            double ydist2 = (distance * ratio2 * (Math.Sin(Globals.DegToRad(angle - 180)))) +
                (wellradii * ratio2 * Math.Sin(Globals.DegToRad(angle - 180)));
            //double barycenter distunit = Globals.randint.Next(360)
            for (int j = 0; j < sizes.Count(); j++)
            {
                newPlanet = randomPlanet(sizes[j]);
                if (j == 0)
                {
                    newPlanet.x = xdist1;
                    newPlanet.y = ydist1;
                    solars.Add(newPlanet);
                    system1.gravityWell(newPlanet);
                }
                else
                {
                    if (j == 1)
                    {
                        newPlanet.x = xdist2;
                        newPlanet.y = ydist2;
                        solars.Add(newPlanet);
                        system1.gravityWell(newPlanet);
                    }
                    else
                    {
                        if (Globals.Distance(newPlanet.x, newPlanet.y) + newPlanet.gravityRad + Globals.gridSize < systemRad)
                        {
                            solars.Add(newPlanet);
                            system1.gravityWell(newPlanet);
                        }
                    }
                }
            }
        }

        public Solar randomPlanet(int _size)
        {
            bool planetplaced = false;
            bool valid = true;
            Solar planet;
            int size = _size;

            double x = 0;
            double y = 0;

            int tries = 0;
            while (!planetplaced && tries < 100)
            {
                tries++;
                valid = true;

                double dist = Globals.randint.Next(250);
                double angle = Globals.randint.Next(360);

                x = dist * Math.Sin(Globals.DegToRad(angle));
                y = dist * -Math.Cos(Globals.DegToRad(angle));

                for (int j = 0; j < solars.Count(); j++)
                {
                    double gravityRad = Globals.wellradius(size);
                    double curdist = Globals.Distance(x, y, solars[j].x, solars[j].y);
                    double mindist = gravityRad + solars[j].gravityRad + Globals.gridSize;

                    if (curdist < mindist)
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    planetplaced = true;
                }
            }

            planet = new Solar(x, y, "New ERROR", solars.Count() + 2, size);
            if (size == 1)
            {
                planet = new Solar(x, y, "New Asteroid", solars.Count() + 2, size);
            }
            if (1 < size && size <= 7)
            {
                planet = new Solar(x, y, "New Planet", solars.Count() + 2, size);
            }
            if (7 < size && size <= 8)
            {
                planet = new Solar(x, y, "New Dwarf Star", solars.Count() + 2, size);
            }
            if (8 < size && size <= 12)
            {
                planet = new Solar(x, y, "New Star", solars.Count() + 2, size);
            }
            if (12 < size && size <= 14)
            {
                planet = new Solar(x, y, "New Giant Star", solars.Count() + 2, size);
            }
            if (14 < size)
            {
                planet = new Solar(x, y, "New Supergiant", solars.Count() + 2, size);
            }
            return planet;
        }

        public void starMap()
        {
            Graphics tool = Graphics.FromImage(backgroundStars);

            List<Brush> primary = new List<Brush>();
            List<Brush> secondary = new List<Brush>();
            primary.Add(Brushes.Orange);
            primary.Add(Brushes.Blue);
            primary.Add(Brushes.Red);
            primary.Add(Brushes.Yellow);
            secondary.Add(Brushes.Brown);
            secondary.Add(Brushes.DarkBlue);
            secondary.Add(Brushes.DarkRed);
            secondary.Add(Brushes.DarkOrange);

            int numStars = Globals.randint.Next(100) + 50;
            List<Int32> stars = new List<Int32>();
            for (int j = 0; j < numStars; j++)
            {
                int r = 1;
                while (Globals.randint.Next(2) == 0)
                {
                    r++;
                }
                stars.Add(r);
            }
            stars.Sort();

            for (int j = 0; j < numStars; j++)
            {
                double r = (double)stars[j] / 2;
                int x = Globals.randint.Next(600);
                int y = Globals.randint.Next(600);

                int k = Globals.randint.Next(4);
                tool.FillEllipse(secondary[k], x - (int)(r * 4) - 1, y - (int)(r * 0.5) - 1, (int)(r * 8), (int)(r * 1));
                tool.FillEllipse(secondary[k], x - (int)(r * 0.5) - 1, y - (int)(r * 4) - 1, (int)(r * 1), (int)(r * 8));
                tool.FillEllipse(secondary[k], x - (int)(r * 1.5) - 1, y - (int)(r * 1.5) - 1, (int)(r * 3), (int)(r * 3));
                tool.FillEllipse(primary[k], x - (int)r - 1, (int)(y - r) - 1, (int)(r * 2), (int)(r * 2));
                tool.FillEllipse(primary[k], x - (int)(r * 0.25) - 1, y - (int)(r * 2) - 1, (int)(r * 0.5), (int)(r * 4));
                tool.FillEllipse(primary[k], x - (int)(r * 2) - 1, y - (int)(r * 0.25) - 1, (int)(r * 4), (int)(r * 0.5));
                tool.FillEllipse(Brushes.White, x - (int)((r + 1) * 0.5) - 1, y - (int)((r + 1) * 0.5) - 1,
                    (int)((r + 1) * 1), (int)((r + 1) * 1));

                if (r > 2)
                {
                    for (int l = 0; l < 360; l += 5)
                    {
                        double angle = l;
                        double xdist = r * 3 * (Math.Cos(Globals.DegToRad((double)angle)));
                        double ydist = r * 3 * (Math.Sin(Globals.DegToRad((double)angle)));
                        double xdist2 = r * 3 * (Math.Cos(Globals.DegToRad((double)angle - 5)));
                        double ydist2 = r * 3 * (Math.Sin(Globals.DegToRad((double)angle - 5)));

                        tool.DrawLine(new Pen(secondary[k], (float)(r / 2)),
                            (int)(x + xdist), (int)(y + ydist),
                            (int)(x + xdist2), (int)(y + ydist2));
                    }
                }
            }
        }
    }
}
