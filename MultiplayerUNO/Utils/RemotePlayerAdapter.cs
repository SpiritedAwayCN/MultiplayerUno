using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// 远程连接他人服务器 前端接口
    /// </summary>
    class RemotePlayerAdapter : PlayerAdapter
    {
        protected string _hostname;
        protected int _port;
        protected Socket serverSocket;

        public static int BUFFERSIZE = 8192;

        /// <summary>
        /// 远程连接，需指定主机与端口
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public RemotePlayerAdapter(string hostName, int port)
        {
            _hostname = hostName;
            _port = port;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 远程玩家的初始化操作，需要建立tcp连接，启用接收与发送线程
        /// </summary>
        public override void Initialize()
        {
            serverSocket.Connect(_hostname, _port); // 连接服务器

            sendQueue = new BlockingCollection<string>();
            recvQueue = new BlockingCollection<string>();

            // 客户端发送线程
            sendThread = new Thread(() =>
            {
                while (true)
                {
                    string msg = null;
                    try
                    {
                        msg = sendQueue.Take(); // 取出将要发送的信息
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }

                    if (msg == null) continue;

                    try
                    {
                        byte[] contents = Encoding.UTF8.GetBytes(msg + "$");
                        serverSocket.Send(contents); // 发送至服务器
                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                }
            });
            sendThread.IsBackground = true; // 设为后台线程

            // 客户端接收线程
            recvThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] content = new byte[BUFFERSIZE];
                    string msg = null;
                    try
                    {
                        int n = serverSocket.Receive(content); // 从服务器接收数据
                        msg = Encoding.UTF8.GetString(content, 0, n);
                        
                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        recvQueue.CompleteAdding();
                        Close();
                        break;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        recvQueue.CompleteAdding();
                        Close();
                        break;
                    }

                    if (msg != null)
                    {
                        foreach(string sw in msg.Split('$')){
                            if (sw.Length <= 0) continue;
                            recvQueue.Add(sw); // 分割后，存入收取队列，供前端Take
                        }
                    }

                }
            });
            recvThread.IsBackground = true; // 设置为后台线程

            sendThread.Start();
            recvThread.Start();
        }

        /// <summary>
        /// 内部调用，关闭连接
        /// </summary>
        protected void Close()
        {
            serverSocket.Close();
            sendQueue.CompleteAdding();
        }

        /// <summary>
        /// 供前端调用，关闭连接
        /// </summary>
        public void Disconnect()
        {
            serverSocket.Close();
            recvQueue.CompleteAdding();
            sendQueue.CompleteAdding();
        }

        /// <summary>
        /// 远程玩家发送消息，将消息存入发送队列供发送线程取出
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        public override void SendMsg2Server(string msg)
        {
            sendQueue.Add(msg);
        }
    }
}
