using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    class Waypoint
    {
        public MapNode node;
        public Waypoint parent;
        public double cost;
        public double g;
        public double h;
        public double f;

        public Waypoint(MapNode _node, Waypoint _parent)
        {
            g = 0;
            h = 0;
            f = 0;

            parent = _parent;
            node = _node;
            if (!node.ground && !node.atmos)
            {
                cost = 1;
            }
            else
            {
                if (node.atmos)
                {
                    cost = 1000;
                }
                if (node.ground)
                {
                    cost = 1000000;
                }
            }
        }

        public Waypoint(MapNode _node)
        {
            g = 0;
            h = 0;
            f = 0;

            node = _node;
            if (!node.ground && !node.atmos)
            {
                cost = 1;
            }
            else
            {
                if (node.atmos)
                {
                    cost = 1000;
                }
                if (node.ground)
                {
                    cost = 1000000;
                }
            }
        }
    }
}
