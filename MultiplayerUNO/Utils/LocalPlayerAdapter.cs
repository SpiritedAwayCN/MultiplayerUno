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
    /// <summary>
    /// 本地玩家开服用的前端接口
    /// </summary>
    public class LocalPlayerAdapter : PlayerAdapter
    {
        protected Room gameRoom;
        public IPEndPoint EndPoint { get; }
        public string PlayerName { get; }

        /// <summary>
        /// 本地开服，创建房间的接口
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="initJson">玩家打招呼用的json</param>
        public LocalPlayerAdapter(int port, string initJson)
        {
            PlayerName = (string)JsonMapper.ToObject(initJson)["name"];

            recvQueue = new BlockingCollection<string>();
            EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            gameRoom = new Room(this);  // 创建房间
        }

        /// <summary>
        /// 初始化，对于本地开服者，需要初始化房间
        /// </summary>
        public override void Initialize()
        {
            gameRoom.InitializeRoom();
        }

        /// <summary>
        /// 对于本地开服玩家，只需将消息存入房间的信息处理队列
        /// </summary>
        /// <param name="msg">消息json</param>
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
