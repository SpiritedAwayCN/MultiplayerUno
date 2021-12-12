using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    public abstract class PlayerAdapter
    {
        protected BlockingCollection<string> sendQueue;
        protected BlockingCollection<string> recvQueue;

        /* public BlockingCollection<string> SendQueue { get
            {
                return sendQueue;
            } } */
        
        public BlockingCollection<string> RecvQueue { get
            {
                return recvQueue;
            } }

        protected Thread sendThread;
        protected Thread recvThread;

        public abstract void Initialize();
        public abstract void SendMsg2Server(string msg);
    }
}
