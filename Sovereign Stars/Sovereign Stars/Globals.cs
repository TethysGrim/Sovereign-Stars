using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    public class Globals
    {
        public static Bitmap pic;
        public static int gridSize;

        public static Random randint;

        /// <summary>
        /// I have no idea what this does.
        /// </summary>
        public Globals()
        {
            pic = new Bitmap(600, 600);
            randint = new Random();
            gridSize = 5;
        }

        /// <summary>
        /// Converts a number in Degrees to Radians.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double DegToRad(double a)
        {
            return ((a / 180) * Math.PI);
        }

        /// <summary>
        /// Converts a number in Radians to Degrees.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double RadToDeg(double a)
        {
            return ((a * 180) / Math.PI);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        }

        public static double Distance(double xdist, double ydist)
        {
            return Math.Sqrt((xdist * xdist) + (ydist * ydist));
        }

        public static Color colorFromTemp(int _temp)
        {
            int r = 255;
            int g = 255;
            int b = 255;

            int temperature = _temp;

            return Color.FromArgb(255, r, g, b);
        }

        public static double dispToX(double _x, double _xc, double _zL)
        {
            double x = ((_x - _xc) / _zL);
            return x;
        }

        public static double dispToY(double _y, double _yc, double _zL)
        {
            double y = ((_y - _yc) / _zL);
            return y;
        }

        public static double xToDisp(double _x, double _xc, double _zL)
        {
            double x = _x * _zL + _xc;
            return x;
        }

        public static double yToDisp(double _y, double _yc, double _zL)
        {
            double y = _y * _zL + _yc;
            return y;
        }

        public static double wellradius(int size)
        {
            double r = (size * gridSize * 0.8) - (gridSize * 0.3);
            return r;
        }
    }
}
