using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    public class MapNode
    {
        public double x;
        public double y;
        public List<MapNode> neighbors;
        public List<MapNode> unsupNeighbors;
        public int suppressionCode;
        public bool ground;
        public bool atmos;
        public bool distort;

        /// <summary>
        /// Creates a new MapNode object.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_isGround"></param>
        public MapNode(double _x, double _y)
        {
            x = _x;
            y = _y;
            neighbors = new List<MapNode>();
            unsupNeighbors = new List<MapNode>();
            suppressionCode = 0;

            ground = false;
            atmos = false;
            distort = false;
        }

        public MapNode(double _x, double _y, bool _ground, bool _atmos)
        {
            x = _x;
            y = _y;
            neighbors = new List<MapNode>();
            unsupNeighbors = new List<MapNode>();
            suppressionCode = 0;

            ground = _ground;
            atmos = _atmos;
            distort = true;
        }

        /// <summary>
        /// Connects two MapNodes together.
        /// </summary>
        /// <param name="target"></param>
        public void connect(MapNode target)
        {
            neighbors.Add(target);
            target.neighbors.Add(this);
        }

        public void checkUnsup()
        {
            List<MapNode> tested = new List<MapNode>();
            unsupNeighbors = new List<MapNode>();
            MapNode neighbor;

            if (suppressionCode == 0)
            {
                for (int j = 0; j < neighbors.Count(); j++)
                {
                    neighbor = neighbors[j];
                    if (neighbor.suppressionCode == 0)
                    {
                        unsupNeighbors.Add(neighbor);
                    }
                    //if (tested.Contains(neighbor))
                    //{
                    //    //neighbors.Remove(neighbor);
                    //    //j -= 1;
                    //}
                    //else
                    //{
                    //    if (neighbor.suppressionCode == 0)
                    //    {
                    //        unsupNeighbors.Add(neighbor);
                    //    }
                    //}
                    tested.Add(neighbor);
                }
            }
            else
            {
                for (int j = 0; j < unsupNeighbors.Count(); j++)
                {
                    unsupNeighbors.RemoveAt(j);
                }
            }
        }
    }
}
