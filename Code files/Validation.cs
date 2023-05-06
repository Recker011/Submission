using System;
using System.IO;
using System.Text.RegularExpressions;   //So Regex can work
using System.Collections.Generic;
using Search;
using System.Net;
using System.Diagnostics;

namespace Search
{
    partial class Validation
    {
        static string? filepath; 
        static string[]? strings;
        static List<int>? numbers;
        

        /* Checks if the FIle taken in from console is valid and if the file is valid stores it for use */
        public static bool IsValid(string directory)
        {
            Console.Write($"Checking for file: {directory} ");
            Thread.Sleep( 1000 ); // Some flair can be commented out if it interferes with use

            if (File.Exists(directory))
                filepath = directory;


            return File.Exists(directory);
        }

/* Reads the lines into an array */
        public static string[] CreateArray() => File.ReadAllLines(filepath);

/* Parse the text into individual lines */
        public static void ParseText(string[] lines)
        {
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                numbers = new List<int>();

                //Regex works by splitting off the non numerical parts of the parsed string leaving only the numbers. 
                strings = MyRegex().Split(lines[lineNumber]);
                foreach (string value in strings)
                {
                    if (!string.IsNullOrEmpty(value))
                        numbers.Add(int.Parse(value));
                }

                //World Creation
                WorldGrid.GridBounds(lineNumber, numbers);
            }
        }

        public static void ParseSearch(string searchType, string[,] world)
        {
           // Console.Write("Search type: ");
            string? result;
            string? search_alg;
            int time;
            Stopwatch stopwatch = new Stopwatch();
            // matches console input argument for search type 
            switch (searchType)
            {
                //Depth first search
                case "dfs":
                    WorldGrid.searchType = "Depth-First Search";
                    stopwatch.Start();
                    result = dfs.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "Depth-First Search";

                    break;
                //Breadth First search
                case "bfs":
                    WorldGrid.searchType = "Breadth-First Search";
                    stopwatch.Start();
                    result = bfs.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "Breadth-First Search";
                    break;

                //Greedy Best First Search
                case "gbfs":
                    WorldGrid.searchType = "Greedy Best-First Search";
                    stopwatch.Start();
                    result = gbfs.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "Greedy Best-First Search";
                    break;
                //A* 
                case "as":
                    WorldGrid.searchType = "A* Search";
                    stopwatch.Start();
                    result = astar.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "A Star Search";
                    break;
                //CUSTOM 1 Diagonal A* (basically A* but it moves in 8 directions instead of just 4 )
                case "cus1":
                    WorldGrid.searchType = "Diagonal A* Search";
                    stopwatch.Start();
                    result = diag.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "Diagonal A* Search";
                    break;
                //CUSTOM 2 UNiform COst Seaerch
                case "cus2":
                    WorldGrid.searchType = "Uniform Cost Search";
                    stopwatch.Start();
                    result = ucs.Search(world);
                    stopwatch.Stop();
                    time = (int)stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    search_alg = "Uniform Cost Search";
                    break;

                //Anything else
                default:
                    Console.WriteLine($"'{searchType}' does not match any available options. Please enter 'dfs', 'bfs', 'gbfs', 'as', cus1, cus2");
                    return;
            }

            Console.WriteLine($"\nFile Tested:\n{filepath} \n\nSearch Type:\n{search_alg}\n\nTime Taken: {time} milliseconds\n\nNo of Nodes: {result}");
        }

        [GeneratedRegex("\\D+")] // Regex to split non numerals from the numerals
        private static partial Regex MyRegex();
    }
}