using System;

namespace Search
{
    class Program
    {
        static string[] fileLines;
        static string[,] world;

        static void Main(string[] args)
        {
            //Check that args has 2 objects
            if (args.Length != 2)
            {
                Console.WriteLine("Please Enter Command as |Search <filepath> <algorithm>" );
                return;
            }

            //Checks for the files existence 
            if (Validation.IsValid(args[0]))
            {
                Console.WriteLine("File exists!");
                fileLines = Validation.CreateArray();
                Validation.ParseText(fileLines);
                world = WorldGrid.GetMap();
                WorldGrid.PrintGrid();
            }
            else
            {
                Console.WriteLine("File doesn't exist!");
                return;
            }

            //Send args[1] to validation
            Validation.ParseSearch(args[1], world);
        }
    }
}
