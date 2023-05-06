using System;
using System.Collections.Generic;
using System.Threading;

namespace Search
{

/* This class is responsible for Printing of the World Map onto the COnsole. It handles the presentation of the output from each class */
    class WorldGrid
    {
        static string[,] map;
        public static string searchType = "";

        public static void GridBounds(int line_number, List<int> int_number)
        {
/* calculates the Boundaries of the world from the first digits in the array of provided text. */
            if (line_number == 0)        
            {
                map = new string[int_number[0], int_number[1]];
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        map[x, y] = " ";
                    }
                }
                Console.WriteLine($"Size: [{map.GetLength(0)},{map.GetLength(1)}]");
            } 
 /* Creates the starting position from the second line of parsed text from the provided data file */
            else if (line_number == 1)  
            {
                if (int_number.Count == 2)
                {
                    map[int_number[1], int_number[0]] = "S"; // marks the starting position with S, maybe the Use of "I" would be better
                }
            }
/* Creates the goal position(s) from the third line of parsed text from the provided data file */
            else if (line_number == 2)   
            {
                int counter = 0;
                int first_value = 0;
                foreach (int i in int_number)
                {
                    counter++;
                    if (counter != 2)
                    {
                        first_value = i;
                    }
                    else
                    {
                        // goal position is marked on map
                        Console.WriteLine($"{first_value}, {i}");
                        map[i, first_value] = "G"; // May be I can add a Unicode character
                        counter = 0;
                    }
                }
            }
/* Creates Walls Based in the range of values given in text file */
            else    
            {
                for (int y = int_number[1]; y < int_number[1] + int_number[3]; y++)
                {
                    for (int x = int_number[0]; x < int_number[0] + int_number[2]; x++)
                    {
                        map[y, x] = "W"; // Another UNicode?
                    }
                }
            }
        }
/* Returns the Map */
        public static string[,] GetMap()
        {
            return map;
        }

/* Updates the grid after one frame has been printed */
        public static void UpdateGrid(string[,] map_to_be_printed)
        {
            map = map_to_be_printed;

            PrintGrid();
        }
/* Prints the world map per coordinate based on the parsed information */
        public static void PrintGrid()
        {
            Console.Clear();
            Console.WriteLine(searchType);
            for (int x = 0; x < map.GetLength(0); x++)
            {
                Console.Write("| ");
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    switch (map[x, y])
                    {
                        case "S":
                            ColorOutput(map[x, y], ConsoleColor.Red, true);
                            break;
                        case "G":
                            ColorOutput(map[x, y], ConsoleColor.Green, true);
                            break;
                        case "-":
                            ColorOutput(map[x, y], ConsoleColor.DarkGray, true);
                            break;
                        case "N":
                            ColorOutput(map[x, y], ConsoleColor.Yellow, true);
                            break;
                        default:
                            Console.Write(map[x, y]);
                            break;
                    }
                    Console.Write(" | ");
                }
                Console.Write("\n");
            }
            Thread.Sleep(25);
        }
/* Custom function to Print the Output to the console in a color */
        static void ColorOutput(string value, ConsoleColor colour, bool reset)
        {
            Console.ForegroundColor = colour;
            Console.Write(value);

            if (reset)
            {
                Console.ResetColor();
            }
            
        }


    }
}
