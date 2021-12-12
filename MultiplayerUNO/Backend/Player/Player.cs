using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend.Player
{
    public abstract class Player
    {
        protected string name;

        public string Name { get { return name; } }
        public int Id { get; }

        public BlockingCollection<string> sendQueue;

        public void SendMessage(string msg)
        {
            sendQueue.Add(msg);
        }
    }
}
