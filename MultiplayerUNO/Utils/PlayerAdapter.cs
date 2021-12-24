using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// 前端与后端交互的接口 
    /// 提供RecvQueue属性，供前端提取服务器信息 
    /// 提供SendMsg2Server方法，供前端向服务器发送数据
    /// </summary>
    public abstract class PlayerAdapter
    {
        protected BlockingCollection<string> sendQueue;
        protected BlockingCollection<string> recvQueue;
        
        public BlockingCollection<string> RecvQueue { get
            {
                return recvQueue;
            } } // 收到来自Server的消息将存于此队列

        protected Thread sendThread;
        protected Thread recvThread;

        public abstract void Initialize();
        public abstract void SendMsg2Server(string msg);
    }
}
