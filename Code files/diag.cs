using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    //This is similar to the A* method in every way, except instead of just using the four horizontal and vertical directions, we will also be using diagonal directions.
    //This increases our moveset from four to eight, giving a more natural 2D movement set. This will lower the amount of moves in all situations, as well as provide access between gaps
    //that would otherwise block an agent that can only move in the original four directions.
    class diag
    {
        static string[,] world;
        static int nodes = 0;
        static bool StartGoalExists = false;    //Check if both Start and Goal positions exist
        static bool goalReached = false;        //Check if goal has been reached
        static List<string> directions = new List<string>();

        static GoalPosition gPosition;
        static List<GoalPosition> gPositions = new List<GoalPosition>();

        static Node cPosition;
        static List<Node> successors = new List<Node>();
        static List<Node> openSet = new List<Node>();
        static List<Node> closedSet = new List<Node>();

        public static string Search(string[,] pWorld)
        {
            List<string> outcome = new List<string>();
            world = pWorld;

            FindStartEnd();
            while (openSet.Count != 0 && !goalReached && StartGoalExists) //While openSet is not empty, and a Goal has not been found, and both Start and Goal positions exist
            {
                nodes++;                //Increment node count
                FindLowestF();          //Visit first node in openSet with lowest distance to goal
                FindValidSuccessors();  //Generate surrounding successors
                SuccessorHeuristics();  //Create heuristics for successors
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

            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    if (world[y, x] == "S")
                    {
                        openSet.Add(new Node(0, directions, new Position(x, y)));
                        foundStart = true;
                    }
                    else if (world[y, x] == "G")
                    {
                        gPositions.Add(new GoalPosition(directions, new Position(x, y)));
                        foundGoal = true;
                    }
                }
            }
            StartGoalExists = foundStart && foundGoal;
        }

        static void FindLowestF()
        {
            int lowestH = openSet[0].FValue;
            int lowestOpenSet = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FValue < lowestH)
                {
                    lowestH = openSet[i].FValue;
                    lowestOpenSet = i;
                }
            }
            cPosition = openSet[lowestOpenSet];
            openSet.Remove(openSet[lowestOpenSet]);
            if (world[cPosition.CurrentPosition.Y, cPosition.CurrentPosition.X] != "S") //Keep 'Start' position for visual reference
            {
                world[cPosition.CurrentPosition.Y, cPosition.CurrentPosition.X] = "-";
            }
        }

        static void FindValidSuccessors()
        {
            successors.Clear(); //Reset successors list
                                //Up =          y-1
            if (cPosition.CurrentPosition.Y - 1 >= 0)
            {
                GenerateSuccessor("up", cPosition.CurrentPosition.X, cPosition.CurrentPosition.Y - 1);
            }
            //UpLeft =      x-1, y-1
            if (cPosition.CurrentPosition.X - 1 >= 0 && cPosition.CurrentPosition.Y - 1 >= 0)
            {
                GenerateSuccessor("upleft", cPosition.CurrentPosition.X - 1, cPosition.CurrentPosition.Y - 1);
            }
            //Left =        x-1
            if (cPosition.CurrentPosition.X - 1 >= 0)
            {
                GenerateSuccessor("left", cPosition.CurrentPosition.X - 1, cPosition.CurrentPosition.Y);
            }
            //DownLeft =    x-1, y+1
            if (cPosition.CurrentPosition.X - 1 >= 0 && cPosition.CurrentPosition.Y + 1 < world.GetLength(0))
            {
                GenerateSuccessor("downleft", cPosition.CurrentPosition.X - 1, cPosition.CurrentPosition.Y + 1);
            }
            //Down =        y+1
            if (cPosition.CurrentPosition.Y + 1 < world.GetLength(0))
            {
                GenerateSuccessor("down", cPosition.CurrentPosition.X, cPosition.CurrentPosition.Y + 1);
            }
            //DownRight =   x+1, y+1
            if (cPosition.CurrentPosition.X + 1 < world.GetLength(1) && cPosition.CurrentPosition.Y + 1 < world.GetLength(0))
            {
                GenerateSuccessor("downright", cPosition.CurrentPosition.X + 1, cPosition.CurrentPosition.Y + 1);
            }
            //Right =       x+1
            if (cPosition.CurrentPosition.X + 1 < world.GetLength(1))
            {
                GenerateSuccessor("right", cPosition.CurrentPosition.X + 1, cPosition.CurrentPosition.Y);
            }
            //UpRight=      x+1, y-1
            if (cPosition.CurrentPosition.X + 1 < world.GetLength(1) && cPosition.CurrentPosition.Y - 1 >= 0)
            {
                GenerateSuccessor("upleft", cPosition.CurrentPosition.X + 1, cPosition.CurrentPosition.Y - 1);
            }
        }

        static void GenerateSuccessor(string nextDir, int xCheck, int yCheck)
        {
            directions = new List<string>(cPosition.Directions.Count);
            if (cPosition.Directions.Count > 0)
            {
                foreach (var pos in cPosition.Directions)
                {
                    directions.Add(pos);
                }
            }

            directions.Add(nextDir);

            if (world[yCheck, xCheck] == " ")
            {
                successors.Add(new Node(cPosition.FValue, directions, new Position(xCheck, yCheck)));
            }
            else if (world[yCheck, xCheck] == "G")
            {   //Goal found in this position
                successors.Add(new Node(cPosition.FValue, directions, new Position(xCheck, yCheck)));
                gPosition = new GoalPosition(directions, new Position(xCheck, yCheck));
                goalReached = true;
            }
        }

        static void SuccessorHeuristics()
        {
            for (int i = 0; i < successors.Count; i++)
            {
                if (world[successors[i].CurrentPosition.Y, successors[i].CurrentPosition.X] != "G")
                {
                    int f, g, h;
                    f = successors[i].FValue + 1;
                    g = ManhattanDist(successors[i].CurrentPosition);
                    h = f + g;

                    if (ExistsInSet(openSet, successors[i], h) || ExistsInSet(closedSet, successors[i], h))
                    {
                        //Already exists and has lowest possible heuristic value, do nothing
                    }
                    else
                    {   //Add to openSet
                        openSet.Add(new Node(h, successors[i].Directions, new Position(successors[i].CurrentPosition.X, successors[i].CurrentPosition.Y)));
                        world[successors[i].CurrentPosition.Y, successors[i].CurrentPosition.X] = "N";
                    }
                }
            }
            closedSet.Add(cPosition);
            WorldGrid.UpdateGrid(world);
        }

        static int ManhattanDist(Position successor)
        {
            int manDist = Math.Abs(successor.X - gPositions[0].Coordinates.X) + Math.Abs(successor.Y - gPositions[0].Coordinates.Y);
            foreach (GoalPosition goal in gPositions)
            {   //Check the Manhattan Distance against all goal positions, take the lowest one
                int currDist = Math.Abs(successor.X - goal.Coordinates.X) + Math.Abs(successor.Y - goal.Coordinates.Y);
                if (currDist < manDist)
                {
                    manDist = currDist;
                }
            }
            return manDist;
        }

        static bool ExistsInSet(List<Node> setCheck, Node successor, int h)
        {
            bool exists = false;
            for (int i = 0; i < setCheck.Count; i++)
            {
                if (setCheck[i].CurrentPosition.X == successor.CurrentPosition.X && setCheck[i].CurrentPosition.Y == successor.CurrentPosition.Y)
                {
                    if (setCheck[i].FValue <= h)
                    {
                        exists = true;
                    }
                }
            }
            return exists;
        }
    }
}
