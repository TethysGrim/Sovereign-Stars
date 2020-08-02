using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sovereign_Stars
{
    public class NodeMap
    {
        public List<MapNode> nodes;
        public List<MapNode> unsup;
        public List<Solar> solars;
        public string name;
        public bool gridIni;

        public NodeMap(string _name)
        {
            name = _name;
            nodes = new List<MapNode>();
            solars = new List<Solar>();
            gridIni = false;
        }

        public void addNode(MapNode newNode)
        {
            nodes.Add(newNode);
        }

        public void hexGrid(int size, int x, int y)
        {
            int midpoint = size / 2;
            double offset;
            List<List<MapNode>> grid = new List<List<MapNode>>();

            for (int j = 0; j < size; j++)
            {
                grid.Add(new List<MapNode>());
                for (int k = 0; k < size - Math.Abs(j - midpoint); k++)
                {
                    offset = Math.Abs((int)(j - midpoint) * 0.5);
                    MapNode node1 = new MapNode((y + ((Globals.gridSize * (Math.Cos(Globals.DegToRad((double)30)))) * (j - midpoint))),
                        (x + (Globals.gridSize * (k + offset - midpoint))));
                    grid[j].Add(node1);
                    nodes.Add(node1);
                }
            }

            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size - Math.Abs(j - midpoint); k++)
                {
                    MapNode node1 = grid[j][k];
                    if (k < grid[j].Count - 1)
                    {
                        node1.connect(grid[j][k + 1]);
                    }
                    if (j < grid.Count / 2)
                    {
                        if (k < grid[j + 1].Count - 1)
                        {
                            node1.connect(grid[j + 1][k + 1]);
                        }
                        if ((j < grid.Count - 1) && (k < grid[j + 1].Count - 1))
                        {
                            node1.connect(grid[j + 1][k]);
                        }
                    }
                    if (j >= grid.Count / 2)
                    {
                        if ((k > 0) && (j < grid.Count - 1))
                        {
                            node1.connect(grid[j + 1][k - 1]);
                        }
                        if ((j < grid.Count - 1) && (k < grid[j + 1].Count))
                        {
                            node1.connect(grid[j + 1][k]);
                        }
                    }
                }
            }

            
            gridIni = true;
        }

        /// <summary>
        /// Creates a polar region of map nodes centered around a solar object.
        /// </summary>
        /// <param name="obj"></param>
        public void gravityWell(Solar obj)
        {
            for (int j = 0; j < nodes.Count(); j++)
            {
                MapNode node = nodes[j];
                if (Globals.Distance(obj.x, obj.y, node.x, node.y) <= obj.gravityRad)
                {
                    node.suppressionCode = obj.suppressionCode;
                }
            }

            List<List<MapNode>> grid = new List<List<MapNode>>();
            double anglePer = 360.0 / (double)obj.size;
            double angleOff;
            for (int j = 0; j < obj.size; j++)
            {
                List<MapNode> ring = new List<MapNode>();
                grid.Add(ring);
                for (int k = 0; k < obj.size; k++)
                {
                    angleOff = anglePer * (k % 2) / 2;

                    double nodex = obj.x + ((obj.radius - (Globals.gridSize * 0.2) + (k * Globals.gridSize * 0.6)) *
                        Math.Sin(Globals.DegToRad((double)(anglePer * j) + obj.angle - angleOff)));
                    double nodey = obj.y - ((obj.radius - (Globals.gridSize * 0.2) + (k * Globals.gridSize * 0.6)) *
                        Math.Cos(Globals.DegToRad((double)(anglePer * j) + obj.angle - angleOff)));
                    if (k == 0)
                    {
                        if (obj.atmosphere > 0)
                        {
                            MapNode l = new MapNode(nodex, nodey, true, true);
                            ring.Add(l);
                        }
                        else
                        {
                            MapNode l = new MapNode(nodex, nodey, true, false);
                            ring.Add(l);
                        }
                    }
                    else
                    {
                        if (k < obj.atmosphere + 1)
                        {
                            MapNode l = new MapNode(nodex, nodey, false, true);
                            ring.Add(l);
                        }
                        else
                        {
                            MapNode l = new MapNode(nodex, nodey, false, false);
                            ring.Add(l);
                        }
                    }

                }
            }

            obj.edge = new List<MapNode>();
            if (obj.size != 1)
            {
                for (int j = 0; j < grid.Count(); j++)
                {
                    List<MapNode> ring = grid[j];
                    for (int k = 0; k < ring.Count(); k++)
                    {
                        MapNode node = ring[k];
                        addNode(node);

                        int offset = (k % 2);

                        if (k != 0)
                        {
                            if (j != 0)
                            {
                                if (j != grid.Count() - 1)
                                {
                                    if (offset == 0)
                                    {
                                        node.connect(grid[j + 1 - offset][k - 1]);
                                    }
                                    if (offset == 1)
                                    {
                                        node.connect(grid[j + 1 - offset][k - 1]);
                                    }
                                }
                                else
                                {
                                    if (offset == 0)
                                    {
                                        node.connect(grid[0][k - 1]);
                                    }
                                    if (offset == 1)
                                    {
                                        node.connect(grid[j][k - 1]);
                                    }
                                }
                                node.connect(grid[j - offset][k - 1]);
                            }
                            if (j == 0)
                            {
                                node.connect(grid[j + 1 - offset][k - 1]);
                                if (offset == 1)
                                {
                                    node.connect(grid[grid.Count() - 1][k - 1]);
                                }
                                if (offset == 0)
                                {
                                    node.connect(grid[j][k - 1]);
                                }
                            }
                        }
                        
                        if (k == obj.size - 1)
                        {
                            obj.edge.Add(node);
                        }

                        if (j == 0)
                        {
                            node.connect(grid[ring.Count() - 1][k]);
                        }
                        else
                        {
                            node.connect(grid[j - 1][k]);
                        }
                    }
                }
            }
            else
            {
                addNode(grid[0][0]);
                obj.edge.Add(grid[0][0]);
            }

            for (int j = 0; j < nodes.Count(); j++)
            {
                MapNode node = nodes[j];
                for (int k = 0; k < node.neighbors.Count(); k++)
                {
                    MapNode sub = node.neighbors[k];
                    if (sub.suppressionCode == obj.suppressionCode)
                    {
                        MapNode closest = obj.edge[0];
                        MapNode secondClosest = obj.edge[0];
                        MapNode close = obj.edge[0];
                        double closestDist = 9999;
                        double secondDist = 9999;
                        for (int l = 0; l < obj.edge.Count(); l++)
                        {
                            close = obj.edge[l];
                            if (Globals.Distance(node.x, node.y, close.x, close.y) < closestDist)
                            {
                                secondClosest = closest;
                                secondDist = closestDist;
                                closest = close;
                                closestDist = Globals.Distance(node.x, node.y, close.x, close.y);
                            }
                            else
                            {
                                if (Globals.Distance(node.x, node.y, close.x, close.y) < secondDist)
                                {
                                    secondClosest = close;
                                    secondDist = Globals.Distance(node.x, node.y, close.x, close.y);
                                }
                            }
                        }

                        node.connect(closest);

                        if (secondDist - closestDist < 2)
                        {
                            node.connect(secondClosest);
                        }
                    }
                }
            }
        }

        public void cullDistance(int dist)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                MapNode node = nodes[j];
                if (Globals.Distance(node.x, node.y) > dist)
                {
                    node.suppressionCode = 1;
                }
            }
        }

        public List<MapNode> findPath(MapNode _ORIG, MapNode _DEST)
        {
            Waypoint ORIG = new Waypoint(_ORIG);
            Waypoint DEST = new Waypoint(_DEST, ORIG);
            List<Waypoint> path = new List<Waypoint>();
            List<Waypoint> frontier = new List<Waypoint>();
            List<Waypoint> searched = new List<Waypoint>();

            if (_ORIG != _DEST)
            {

                bool giveUp = false;

                //Step One
                frontier.Add(ORIG);

                //Step Two
                for (int j = 0; j < ORIG.node.unsupNeighbors.Count(); j++)
                {
                    frontier.Add(new Waypoint(ORIG.node.unsupNeighbors[j], ORIG));
                }
                //Step Three
                searched.Add(frontier[0]);
                frontier.RemoveAt(0);

                bool contained = false;

                while (!contained && !giveUp)
                {
                    for (int j = 0; j < frontier.Count(); j++)
                    {
                        //Step Four
                        Waypoint current = frontier[j];
                        current.g = current.parent.g + current.cost;

                        //Step Five
                        current.h = Globals.Distance(current.node.x, current.node.y,
                            DEST.node.x, DEST.node.y) * Globals.gridSize;

                        //Step Six
                        current.f = current.g + current.h;

                        if (current.node == _DEST)
                        {
                            contained = true;
                        }
                    }
                    //Step Seven
                    Waypoint lowest = frontier[0];
                    for (int j = 1; j < frontier.Count(); j++)
                    {
                        if (frontier[j].f < lowest.f)
                        {
                            lowest = frontier[j];
                        }
                    }
                    searched.Add(lowest);
                    frontier.Remove(lowest);

                    //Step Eight
                    for (int j = 0; j < lowest.node.unsupNeighbors.Count(); j++)
                    {
                        MapNode current = lowest.node.unsupNeighbors[j];
                        bool existsInSearched = false;
                        for (int k = 0; k < searched.Count(); k++)
                        {
                            if (searched[k].node == current)
                            {
                                existsInSearched = true;
                                break;
                            }
                        }

                        bool existsInFrontier = false;
                        for (int k = 0; k < frontier.Count(); k++)
                        {
                            if (frontier[k].node == current)
                            {
                                existsInFrontier = true;
                                break;
                            }
                        }

                        if (!existsInSearched && !existsInFrontier)
                        {
                            frontier.Add(new Waypoint(current, lowest));
                        }
                    }

                    //Step Nine

                    for (int j = 1; j < frontier.Count(); j++)
                    {
                        Waypoint current = frontier[j];
                        if (current.node == _DEST)
                        {
                            DEST = current;
                            searched.Add(current);
                            frontier.Remove(current);
                            contained = true;
                        }
                    }
                }
                //Step Ten
            }
            else
            {
                searched.Add(ORIG);
                DEST = ORIG;
            }

            bool origReached = false;
            path.Add(DEST);
            while (!origReached)
            {
                Waypoint current = path[path.Count() - 1];
                if (current.parent != null)
                {
                    path.Add(current.parent);
                }
                else
                {
                    origReached = true;
                }
            }

            //Okay we're done now.
            List<MapNode> finalPath = new List<MapNode>();
            for (int j = 0; j < path.Count(); j++)
            {
                finalPath.Add(path[j].node);
            }
            return finalPath;
        }

        public List<MapNode> findPath(MapNode _ORIG, MapNode _DEST, int _urgency)
        {
            int urgency = _urgency;
            Waypoint ORIG = new Waypoint(_ORIG);
            Waypoint DEST = new Waypoint(_DEST, ORIG);
            List<Waypoint> path = new List<Waypoint>();
            List<Waypoint> frontier = new List<Waypoint>();
            List<Waypoint> searched = new List<Waypoint>();

            if (_ORIG != _DEST)
            {

                bool giveUp = false;

                //Step One
                frontier.Add(ORIG);

                //Step Two
                for (int j = 0; j < ORIG.node.unsupNeighbors.Count(); j++)
                {
                    frontier.Add(new Waypoint(ORIG.node.unsupNeighbors[j], ORIG));
                }
                //Step Three
                searched.Add(frontier[0]);
                frontier.RemoveAt(0);

                bool contained = false;


                while (!contained && !giveUp)
                {
                    for (int j = 0; j < frontier.Count(); j++)
                    {
                        //Step Four
                        Waypoint current = frontier[j];
                        current.g = current.parent.g + current.cost;

                        //Step Five
                        current.h = Globals.Distance(current.node.x, current.node.y,
                            DEST.node.x, DEST.node.y) * Globals.gridSize;

                        //Step Six
                        current.f = current.g + current.h;

                        if (current.node == _DEST)
                        {
                            contained = true;
                        }
                    }
                    //Step Seven
                    Waypoint lowest = frontier[0];
                    for (int j = 1; j < frontier.Count(); j++)
                    {
                        if (frontier[j].f < lowest.f)
                        {
                            lowest = frontier[j];
                        }
                    }
                    searched.Add(lowest);
                    frontier.Remove(lowest);

                    //Step Eight
                    for (int j = 0; j < lowest.node.unsupNeighbors.Count(); j++)
                    {
                        MapNode current = lowest.node.unsupNeighbors[j];
                        bool existsInSearched = false;
                        for (int k = 0; k < searched.Count(); k++)
                        {
                            if (searched[k].node == current)
                            {
                                existsInSearched = true;
                                break;
                            }
                        }

                        bool existsInFrontier = false;
                        for (int k = 0; k < frontier.Count(); k++)
                        {
                            if (frontier[k].node == current)
                            {
                                existsInFrontier = true;
                                break;
                            }
                        }

                        if (!existsInSearched && !existsInFrontier)
                        {
                            frontier.Add(new Waypoint(current, lowest));
                        }
                    }

                    //Step Nine

                    for (int j = 1; j < frontier.Count(); j++)
                    {
                        Waypoint current = frontier[j];
                        if (current.node == _DEST)
                        {
                            DEST = current;
                            searched.Add(current);
                            frontier.Remove(current);
                            contained = true;
                        }
                    }
                }
                //Step Ten
            }
            else
            {
                searched.Add(ORIG);
                DEST = ORIG;
            }

            bool origReached = false;
            path.Add(DEST);
            while (!origReached)
            {
                Waypoint current = path[path.Count() - 1];
                if (current.parent != null)
                {
                    path.Add(current.parent);
                }
                else
                {
                    origReached = true;
                }
            }

            //Okay we're done now.
            List<MapNode> finalPath = new List<MapNode>();
            for (int j = 0; j < path.Count(); j++)
            {
                finalPath.Add(path[j].node);
            }
            return finalPath;
        }
    }
}
