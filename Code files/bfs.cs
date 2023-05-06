
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class bfs
    {
        static string[,] map;
        static int nodes = 0;
        static bool Initialize_Valid = false;    //Check if both Start and Goal positions exist
        static bool goalReached = false;        //Check if Goal has been reached
        static List<string> directions = new List<string>();

        //<x, y> data. Goal and Current position also contains list of existing directions
        //static Position sPosition;
        static GoalPosition gPosition;
        //static List<GoalPosition> gPositions = new List<GoalPosition>();
        static Node currentPosition;

        //List of adjacent positions, unvisited and visited positions
        static List<Node> adjacents = new();
        static List<Node> notVisited = new();
        static List<Position> visited = new();

        public static string Search(string[,] pWorld)
        {
            List<string> outcome = new List<string>();
            map = pWorld;

            Find_Positions();
            while (notVisited.Count != 0 && !goalReached && Initialize_Valid)
            {
                nodes++;                //Increment node count
                VisitPosition();        //Take first notVisited item, put into visited
                FindValidAdjacents();   //Create list of adjacent positions.
                AddAdjacentsToList();   //Add positions that are not in the visited list to the end of notVisited
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

        //Find the start and end positions
        static void Find_Positions()
        {
            //Console.WriteLine($"\n\tFunction '{System.Reflection.MethodBase.GetCurrentMethod().Name}'");
            bool foundStart = false;
            bool foundGoal = false;

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == "S")
                    {
                        //Console.WriteLine($"Start found at '{x}, {y}'");
                        //sPosition = new Position(x, y);
                        notVisited.Add(new Node(0, directions, new Position(x, y)));
                        foundStart = true;
                    }
                    else if (map[y, x] == "G")
                    {
                        //Console.WriteLine($"End found at '{x}, {y}'");
                        //gPositions.Add(new GoalPosition(directions, new Position(x, y)));
                        foundGoal = true;
                    }
                }
            }

            Initialize_Valid = foundStart && foundGoal;
        }

        //Visit the first position in the notVisited list, remove it and set as cPosition.
        //Place position into visited list. Denote 'visited' with a '-' in the world grid.
        static void VisitPosition()
        {
            //Console.WriteLine($"\n\tFunction '{System.Reflection.MethodBase.GetCurrentMethod().Name}'");
            currentPosition = notVisited[0];
            notVisited.RemoveAt(0);
            visited.Add(currentPosition.CurrentPosition);
            map[currentPosition.CurrentPosition.Y, currentPosition.CurrentPosition.X] = "-";
        }

        //Check the four directions around cPosition to check that it is within the world grid.
        static void FindValidAdjacents()
        {
            //Console.WriteLine($"\n\tFunction '{System.Reflection.MethodBase.GetCurrentMethod().Name}'");
            //Console.WriteLine($"Checking adjacents around {currentPosition.CurrentPosition}");
            adjacents.Clear();
            //Up = y-1
            if (currentPosition.CurrentPosition.Y - 1 >= 0)
            {
                //Console.Write($"Up position '{currentPosition.CurrentPosition.X}, {currentPosition.CurrentPosition.Y - 1}'");
                CheckAdjacentValue("up", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y - 1);
            }
            //Left = x-1
            if (currentPosition.CurrentPosition.X - 1 >= 0)
            {
                //Console.Write($"Left position '{currentPosition.CurrentPosition.X - 1}, {currentPosition.CurrentPosition.Y}'");
                CheckAdjacentValue("left", currentPosition.CurrentPosition.X - 1, currentPosition.CurrentPosition.Y);
            }
            //Down = y+1
            if (currentPosition.CurrentPosition.Y + 1 < map.GetLength(0))
            {
                //Console.Write($"Down position '{currentPosition.CurrentPosition.X}, {currentPosition.CurrentPosition.Y + 1}'");
                CheckAdjacentValue("down", currentPosition.CurrentPosition.X, currentPosition.CurrentPosition.Y + 1);
            }
            //Right = x+1
            if (currentPosition.CurrentPosition.X + 1 < map.GetLength(1))
            {
                //Console.Write($"Right position '{currentPosition.CurrentPosition.X + 1}, {currentPosition.CurrentPosition.Y}'");
                CheckAdjacentValue("right", currentPosition.CurrentPosition.X + 1, currentPosition.CurrentPosition.Y);
            }
        }

        //Create new list of directions from existing cPosition.
        //Check current value in given world space. If goal, flag goalFound boolean.
        //Add next direction to the existing list of directions
        static void CheckAdjacentValue(string nextDir, int x, int y)
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

            if (map[y, x] == "G")
            {
                //Console.WriteLine(" - Goal found!");
                gPosition = new GoalPosition(directions, new Position(x, y));
                goalReached = true;
            }
            else if (map[y, x] == " ")
            {
                //Console.WriteLine(" - Empty");
                adjacents.Add(new Node(0, directions, new Position(x, y)));
            }
        }

        //Check that the position hasn't already been visited by another node.
        //Ensure no duplicates exist in the notVisited list. Add position to the notVisited list.
        static void AddAdjacentsToList()
        {
            //Console.WriteLine($"\n\tFunction '{System.Reflection.MethodBase.GetCurrentMethod().Name}'");
            foreach (var adj in adjacents)
            {
                bool exists = false;
                foreach (var vis in visited)
                {
                    if (adj.CurrentPosition.X == vis.X && adj.CurrentPosition.Y == vis.Y)
                    {
                        //Console.WriteLine($"'{adj.CurrentPosition}' has already been visited.");
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
                    notVisited.Add(adj);
                    Console.WriteLine($"'{adj.CurrentPosition}' has been added to list.");
                    map[adj.CurrentPosition.Y, adj.CurrentPosition.X] = "N";
                }
            }
            WorldGrid.UpdateGrid(map);
        }
    }
}
