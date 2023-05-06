using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    public class Node
    {
        public int FValue { get; set; }
        public List<string> Directions { get; set; }
        public Position CurrentPosition { get; set; }

        public Node(int fValue, List<string> directions, Position currentPosition)
        {
            FValue = fValue;
            Directions = directions;
            CurrentPosition = currentPosition;
        }
    }
}
