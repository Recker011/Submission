using Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class astar
    {
        static string[,] map;
        static int nodes = 0;
        static bool initialize_valid = false;
        static bool goalReached = false;
        static List<string> directions = new();

        static GoalPosition? goal_position;
        static List<GoalPosition> goal_positions = new();

        static Node? current_position;

        static List<Node> neighbor_nodes = new();
        static List<Node> unexpanded_nodes = new();
        static List<Node> expanded_nodes = new();

        public static string Search(string[,] pWorld)
        {
            List<string> outcome = new List<string>();
            map = pWorld;

            Find_Positions();
            while (unexpanded_nodes.Count != 0 && !goalReached && initialize_valid)
            {
                nodes++;
                Find_lowest_node();
                Valid_next_node();
                Node_hn();
            }
            outcome.Add(nodes.ToString());
            outcome.Add("\n");

            if (!goalReached)
            {
                outcome.Add("No solution found.");
            }
            else
            {
                outcome.AddRange(goal_position.Directions);
            }
            return string.Join(" ", outcome);
        }

        static void Find_Positions()
        {
            bool start_realized = false;
            bool goal_realized = false;
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == "S")
                    {
                        unexpanded_nodes.Add(new Node(0, directions, new Position(x, y)));
                        start_realized = true;
                    }
                    else if (map[y, x] == "G")
                    {
                        goal_positions.Add(new GoalPosition(directions, new Position(x, y)));
                        goal_realized = true;
                    }
                }
            }
            initialize_valid = start_realized && goal_realized;
        }

        //Find vacant node position with lowest 'f' value
        static void Find_lowest_node()
        {
            int lowestH = unexpanded_nodes[0].FValue;
            int lowestOpenSet = 0;
            for (int i = 0; i < unexpanded_nodes.Count; i++)
            {
                if (unexpanded_nodes[i].FValue < lowestH)
                {
                    lowestH = unexpanded_nodes[i].FValue;
                    lowestOpenSet = i;
                }
            }
            current_position = unexpanded_nodes[lowestOpenSet];
            unexpanded_nodes.RemoveAt(lowestOpenSet);
            if (map[current_position.CurrentPosition.Y, current_position.CurrentPosition.X] != "S")
            {
                map[current_position.CurrentPosition.Y, current_position.CurrentPosition.X] = "-";
            }
        }

        static void Valid_next_node()
        {
            neighbor_nodes.Clear();
            if (current_position.CurrentPosition.Y - 1 >= 0)
            {
                Generate_next_position("up", current_position.CurrentPosition.X, current_position.CurrentPosition.Y - 1);
            }
            if (current_position.CurrentPosition.X - 1 >= 0)
            {
                Generate_next_position("left", current_position.CurrentPosition.X - 1, current_position.CurrentPosition.Y);
            }
            if (current_position.CurrentPosition.Y + 1 < map.GetLength(0))
            {
                Generate_next_position("down", current_position.CurrentPosition.X, current_position.CurrentPosition.Y + 1);
            }
            if (current_position.CurrentPosition.X + 1 < map.GetLength(1))
            {
                Generate_next_position("right", current_position.CurrentPosition.X + 1, current_position.CurrentPosition.Y);
            }
        }


        //Create successor
        static void Generate_next_position(string nextDir, int xCheck, int yCheck)
        {
            directions = new List<string>(current_position.Directions.Count);
            if (current_position.Directions.Count > 0)
            {
                foreach (var pos in current_position.Directions)
                {
                    directions.Add(pos);
                }
            }
            directions.Add(nextDir);

            if (map[yCheck, xCheck] == " ")
            {
                neighbor_nodes.Add(new Node(current_position.FValue, directions, new Position(xCheck, yCheck)));
            }
            else if (map[yCheck, xCheck] == "G")
            {
                neighbor_nodes.Add(new Node(current_position.FValue, directions, new Position(xCheck, yCheck)));
                goal_position = new GoalPosition(directions, new Position(xCheck, yCheck));
                goalReached = true;
            }
        }

        //Create heuristics for node
        static void Node_hn()
        {
            for (int i = 0; i < neighbor_nodes.Count; i++)
            {
                if (map[neighbor_nodes[i].CurrentPosition.Y, neighbor_nodes[i].CurrentPosition.X] != "G")
                {
                    int f = neighbor_nodes[i].FValue + 1;
                    int g = Manhattan(neighbor_nodes[i].CurrentPosition);
                    int h = f + g;

                    if (Exists_in_list(unexpanded_nodes, neighbor_nodes[i], h) || Exists_in_list(expanded_nodes, neighbor_nodes[i], h))
                    {
                        // Already exists and has lowest possible heuristic value, do nothing
                    }
                    else
                    {
                        unexpanded_nodes.Add(new Node(f, neighbor_nodes[i].Directions, new Position(neighbor_nodes[i].CurrentPosition.X, neighbor_nodes[i].CurrentPosition.Y)));
                        map[neighbor_nodes[i].CurrentPosition.Y, neighbor_nodes[i].CurrentPosition.X] = "N";
                    }
                }
            }
            expanded_nodes.Add(current_position);
            WorldGrid.UpdateGrid(map);
        }


        // This function calculates the Manhattan distance between the successor and the closest goal position(s)
        static int Manhattan(Position successor)
        {
            int manDist = Math.Abs(successor.X - goal_positions[0].Coordinates.X) + Math.Abs(successor.Y - goal_positions[0].Coordinates.Y);

            foreach (GoalPosition goal in goal_positions)
            {
                int currDist = Math.Abs(successor.X - goal.Coordinates.X) + Math.Abs(successor.Y - goal.Coordinates.Y);
                if (currDist < manDist)
                {
                    manDist = currDist;
                }
            }

            return manDist;
        }


        static bool Exists_in_list(List<Node> listcheck, Node successor, int h)
        {
            bool exists = false;

            for (int i = 0; i < listcheck.Count; i++)
            {
                if (listcheck[i].CurrentPosition.X == successor.CurrentPosition.X && listcheck[i].CurrentPosition.Y == successor.CurrentPosition.Y)
                {
                    if (listcheck[i].FValue <= h)
                    {
                        exists = true;
                    }
                }
            }

            return exists;
        }

    }
}

/*
 Convert the astar class to a non-static class to allow for better code organization and improved testability.
Create separate methods for each major step of the A* algorithm for better code readability and easier testing.
Replace the use of Tuples with custom classes for better code readability and type safety.
Convert some of the static lists to instance lists to prevent conflicts between multiple instances of the astar class
 */
