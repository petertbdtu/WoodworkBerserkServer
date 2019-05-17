using System;
using System.Collections.Generic;
using System.Text;

namespace WoodworkBerserkServer
{
    class WBGame
    {
        private List<WBPlayer> players;
        // maybe store list of players here only? not also in wbservice

        // Area data
        //  Entity data
        //  Terrain data (same as entity?)

        public WBGame(List<WBPlayer> players)
        {
            this.players = players;
        }

        public void DoTick()
        {

        }
    }
}
