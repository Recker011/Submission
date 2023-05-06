
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class dfs
    {
        static string[,] map;
        static int nodes = 0;
        static bool StartGoalExists = false;    //Check if both Start and Goal positions exist
        static bool goalReached = false;          //Check if goal has been reached
        static List<string> directions = new List<string>();

        //Position data. Goal and Current position also contains list of existing directions
        //static Position sPosition;
        static GoalPosition gPosition;
        static Node currentPosition;

        //List of adjacent positions, unvisited and visited positions
        static List<Node> adjacents = new List<Node>();
        static List<Node> notVisited = new List<Node>();
        static List<Position> visited = new List<Position>();

        public static string Search(string[,] pWorld)
        {
            List<string> outcome = new List<string>();
            map = pWorld;

            Find_Positions();
            if (StartGoalExists)
            {
                while (notVisited.Count != 0 && !goalReached)
                {
                    nodes++;                //Increment node count
                    VisitPosition();        //Take first notVisited item, put into visited
                    FindValidAdjacents();   //Create list of adjacent positions.
                    AddAdjacentsToList();   //Add positions that are not in the visited list to the start of notVisited
                }
            }

            outcome.Add(nodes.ToString());
            outcome.Add("\n");

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

        static void Find_Positions()
        {
            bool foundStart = false;
            bool foundGoal = false;

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == "S")
                    {
                        notVisited.Add(new Node(0, directions, new Position(x, y)));
                        foundStart = true;
                    }
                    else if (map[y, x] == "G")
                    {
                        gPosition = new GoalPosition(directions, new Position(x, y));
                        foundGoal = true;
                    }
                }
            }

            StartGoalExists = foundStart && foundGoal;
        }

        static void VisitPosition()
        {
            currentPosition = notVisited[0];
            notVisited.RemoveAt(0);
            visited.Add(currentPosition.CurrentPosition);
            if (map[currentPosition.CurrentPosition.Y, currentPosition.CurrentPosition.X] != "S") //Keep 'Start' position for visual reference
            {
                map[currentPosition.CurrentPosition.Y, currentPosition.CurrentPosition.X] = "-";
            }
        }

        //This is checked in reverse due to the backwards nature of insertion into 
        static void FindValidAdjacents()
        {
            adjacents.Clear();
            //Right = x+1
            if (currentPosition.CurrentPosition.X + 1 < map.GetLength(1))
            {
                CheckAdjacentValue("right", currentPosition.CurrentPosition.X + 1, currentPosition.CurrentPosition.Y);
            }
            //Down = y+1
            if (currentPosition.CurrentPosition.Y + 1 < map.GetLength(0))
            {
                CheckAdjacentValue("down", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y + 1);
            }
            //Left = x-1
            if (currentPosition.CurrentPosition.X - 1 >= 0)
            {
                CheckAdjacentValue("left", currentPosition.CurrentPosition.X - 1, currentPosition.CurrentPosition.Y);
            }
            //Up = y-1
            if (currentPosition.CurrentPosition.Y - 1 >= 0)
            {
                CheckAdjacentValue("up", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y - 1);
            }
        }

        static void CheckAdjacentValue(string nextDir, int xCheck, int yCheck)
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
                gPosition = new GoalPosition(directions, new Position(xCheck, yCheck));
                goalReached = true;
            }
            else if (map[yCheck, xCheck] == " " || map[yCheck, xCheck] == "N")
            {
                adjacents.Add(new Node(0, directions, new Position(xCheck, yCheck)));
            }
        }

        static void AddAdjacentsToList()
        {
            foreach (var adj in adjacents)
            {
                bool exists = false;
                foreach (var vis in visited)
                {
                    if (adj.CurrentPosition.X == vis.X && adj.CurrentPosition.Y == vis.Y)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    //If the position already exists in the notVisited list, it needs to be removed first
                    for (int i = 0; i < notVisited.Count(); i++)
                    {
                        if (notVisited[i].CurrentPosition.X == adj.CurrentPosition.X && notVisited[i].CurrentPosition.Y == adj.CurrentPosition.Y)
                        {
                            notVisited.RemoveAt(i);
                        }
                    }
                    notVisited.Insert(0, adj);
                    map[adj.CurrentPosition.Y, adj.CurrentPosition.X] = "N";
                }
            }
            WorldGrid.UpdateGrid(map);
        }
    }
}
