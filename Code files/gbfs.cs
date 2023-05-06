
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class gbfs
    {
        static string[,] map;
        static int nodes = 0;
        static bool StartGoalExists = false;    //Check if both Start and Goal positions exist
        static bool goalReached = false;        //Check if goal has been reached
        static List<string> directions = new List<string>();

        static GoalPosition gPosition;                                                      //Used as final found goal
        static List<GoalPosition> gPositions = new List<GoalPosition>();  //List of all goal positions

        static Node currentPosition;

        //List of Nodes
        static List<Node> successors = new List<Node>();
        static List<Node> openSet = new List<Node>();
        static List<Node> closedSet = new List<Node>();

        public static string Search(string[,] pWorld)
        {
            List<string> outcome = new List<string>();
            map = pWorld;

            FindStartEnd();
            while (openSet.Count != 0 && !goalReached && StartGoalExists)
            {
                nodes++;                //Increment node count
                FindLowestDistance();          //Visit first node in openSet with lowest distance to goal
                FindValidNeighbours();  //Generate neighbours of node
                NeighbourHeuristics();  //Generate Manhattan distance of each neighbour
            }

            outcome.Add(nodes.ToString());

            if (!goalReached)
            {
                outcome.Add("No solution found.");
            }
            else
            {
                outcome.AddRange(gPosition.Directions);
            }

            return string.Join(" ", outcome);
        }

        static void FindStartEnd()
        {
            bool foundStart = false;
            bool foundGoal = false;

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == "S")
                    {
                        openSet.Add(new Node(0, directions, new Position(x, y)));
                        foundStart = true;
                    }
                    else if (map[y, x] == "G")
                    {
                        gPositions.Add(new GoalPosition(directions, new Position(x, y)));
                        foundGoal = true;
                    }
                }
            }

            StartGoalExists = foundStart && foundGoal;
        }

        static void FindLowestDistance()
        {
            int lowestG = openSet[0].FValue;
            int lowestOpenSet = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FValue < lowestG)
                {
                    lowestG = openSet[i].FValue;
                    lowestOpenSet = i;
                }
            }
            currentPosition = openSet[lowestOpenSet];
            openSet.Remove(openSet[lowestOpenSet]);
            if (map[currentPosition.CurrentPosition.Y, currentPosition.CurrentPosition.X] != "S") //Keep 'Start' position for visual reference
            {
                map[currentPosition.CurrentPosition.Y, currentPosition.CurrentPosition.X] = "-";
            }
        }

        static void FindValidNeighbours()
        {
            successors.Clear();
            //Up =      y-1
            if (currentPosition.CurrentPosition.Y - 1 >= 0)
            {
                GenerateNeighbour("up", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y - 1);
            }
            //Left =    x-1
            if (currentPosition.CurrentPosition.X - 1 >= 0)
            {
                GenerateNeighbour("left", currentPosition.CurrentPosition.X - 1, currentPosition.CurrentPosition.Y);
            }
            //Down =    y+1
            if (currentPosition.CurrentPosition.Y + 1 < map.GetLength(0))
            {
                GenerateNeighbour("down", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y + 1);
            }
            //Right =   x+1
            if (currentPosition.CurrentPosition.X + 1 < map.GetLength(1))
            {
                GenerateNeighbour("right", currentPosition.CurrentPosition.X + 1, currentPosition.CurrentPosition.Y);
            }
        }

        static void GenerateNeighbour(string nextDir, int xCheck, int yCheck)
        {
            directions = new List<string>(currentPosition.Directions.Count);
            if (currentPosition.Directions.Count > 0)
            {
                foreach (var pos in currentPosition.Directions)
                {
                    directions.Add(pos);
                }
            }

            directions.Add(nextDir);

            if (map[yCheck, xCheck] == "G")
            {
                successors.Add(new Node(0, directions, new Position(xCheck, yCheck)));
                gPosition = new GoalPosition(directions, new Position(xCheck, yCheck));
                goalReached = true;
            }
            else if (map[yCheck, xCheck] == " ")
            {
                successors.Add(new Node(0, directions, new Position(xCheck, yCheck)));
            }
        }

        static void NeighbourHeuristics()
        {
            foreach (var neighbour in successors)
            {
                if (map[neighbour.CurrentPosition.Y, neighbour.CurrentPosition.X] != "G")
                {
                    //Manhattan distance
                    int g = Manhattan(neighbour.CurrentPosition);

                    //Check exists
                    if (ExistsInSet(openSet, neighbour.CurrentPosition) || ExistsInSet(closedSet, neighbour.CurrentPosition))
                    {
                    }
                    else
                    {
                        openSet.Add(new Node(g, neighbour.Directions, neighbour.CurrentPosition));
                        map[neighbour.CurrentPosition.Y, neighbour.CurrentPosition.X] = "N";
                    }
                }
            }
            closedSet.Add(currentPosition);
            WorldGrid.UpdateGrid(map);
        }

        static int Manhattan(Position posCheck)
        {
            int manDist = Math.Abs(posCheck.X - gPositions[0].Coordinates.X) + Math.Abs(posCheck.Y - gPositions[0].Coordinates.Y);
            foreach (GoalPosition goal in gPositions)
            {   //Check the Manhattan Distance against all goal positions, take the lowest one
                int currDist = Math.Abs(posCheck.X - goal.Coordinates.X) + Math.Abs(posCheck.Y - goal.Coordinates.Y);
                if (currDist < manDist)
                {
                    manDist = currDist;
                }
            }
            return manDist;
        }

        static bool ExistsInSet(List<Node> setCheck, Position posCheck)
        {
            bool exists = false;
            for (int i = 0; i < setCheck.Count; i++)
            {
                if (setCheck[i].CurrentPosition.X == posCheck.X && setCheck[i].CurrentPosition.Y == posCheck.Y)
                {
                    exists = true;
                   // break;
                }
            }
            return exists;
        }
    }
}
