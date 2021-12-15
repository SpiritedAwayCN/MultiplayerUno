using MultiplayerUNO.Backend;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace MultiplayerUNO.Utils
{
    public class LocalPlayerAdapter : PlayerAdapter
    {
        protected Room gameRoom;
        public IPEndPoint EndPoint { get; }
        public string PlayerName { get; }

        public LocalPlayerAdapter(int port, string initJson)
        {
            PlayerName = (string)JsonMapper.ToObject(initJson)["name"];

            recvQueue = new BlockingCollection<string>();
            EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            gameRoom = new Room(this);
        }

        public override void Initialize()
        {
            gameRoom.InitializeRoom();
        }

        public override void SendMsg2Server(string msg)
        {
            gameRoom.InfoQueue.Add(new Room.MsgArgs()
            {
                msg = msg,
                player = gameRoom.LocalPlayer
            });
        }
    }
}
