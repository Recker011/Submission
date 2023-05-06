using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
/*Initializes the goal position*/
    class GoalPosition
    {
        public List<string> Directions { get; set; }
        public Position Coordinates { get; set; }

        public GoalPosition(List<string> directions, Position coordinates)
        {
            Directions = directions;
            Coordinates = coordinates;
        }
    }
}
