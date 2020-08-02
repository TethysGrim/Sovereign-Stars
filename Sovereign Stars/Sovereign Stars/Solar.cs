using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    public class Solar
    {
        public int size;
        public double x;
        public double y;
        public string name;
        public int atmosphere;
        public bool fusion;
        public double radius;
        public double gravityRad;
        public int suppressionCode;
        public List<MapNode> edge;
        public double angle;

        public Solar(double _x, double _y, string _name, int _suppressionCode)
        {
            x = _x;
            y = _y;
            name = _name;
            angle = Globals.randint.Next(360);

            int size = 1;
            while (Globals.randint.Next(2) == 1)
            {
                size += 1;
            }

            radius = size * Globals.gridSize / 5;
            gravityRad = Globals.wellradius(size);

            suppressionCode = _suppressionCode;

            configure();
        }

        public Solar(double _x, double _y, string _name, int _suppressionCode, int _size)
        {
            x = _x;
            y = _y;
            name = _name;
            angle = Globals.randint.Next(360);
            size = _size;

            radius = size * Globals.gridSize / 5;
            gravityRad = Globals.wellradius(size);

            suppressionCode = _suppressionCode;

            configure();
        }

        public void configure()
        {
            if (size <= 2)
            {
                atmosphere = 0;
                fusion = false;
            }

            if (2 < size && size <= 4)
            {
                atmosphere = 1;
                if (Globals.randint.Next(size) == 0)
                {
                    atmosphere = 0;
                }
                fusion = false;
            }

            if (4 < size && size <= 7)
            {
                atmosphere = Globals.randint.Next((int)(size / 4)) + (size / 4);
                fusion = false;
            }

            if (7 < size)
            {
                atmosphere = Globals.randint.Next((int)(size / 4)) + (size / 4);
                fusion = true;
            }
        }


    }
}
