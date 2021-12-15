using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend
{
    public class TieExceptions : ApplicationException
    {

    }

    public class PlayerFinishException : ApplicationException
    {
        public Player.Player player;
        public PlayerFinishException(Player.Player p)
        {
            player = p;
        }
    }
}
