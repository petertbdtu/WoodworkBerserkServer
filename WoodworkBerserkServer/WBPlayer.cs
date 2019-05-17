using System;
using System.Collections.Generic;
using System.Text;

namespace WoodworkBerserkServer
{
    class WBPlayer
    {
        public int Id { get; set; }
        public int Map { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public WBPlayer(int id, int map, int x, int y)
        {
            Id = id;
            Map = map;
            X = x;
            Y = y;
        }
        
    }
    
}
