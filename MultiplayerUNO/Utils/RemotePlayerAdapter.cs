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
    class RemotePlayerAdapter : PlayerAdapter
    {
        protected string _hostname;
        protected int _port;
        protected Socket serverSocket;

        public static int BUFFERSIZE = 8192;

        public RemotePlayerAdapter(string hostName, int port)
        {
            _hostname = hostName;
            _port = port;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        }

        public override void Initialize()
        {
            serverSocket.Connect(_hostname, _port);

            sendQueue = new BlockingCollection<string>();
            recvQueue = new BlockingCollection<string>();

            sendThread = new Thread(() =>
            {
                while (true)
                {
                    string msg = null;
                    try
                    {
                        msg = sendQueue.Take();
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }

                    if (msg == null) continue;

                    try
                    {
                        byte[] contents = Encoding.UTF8.GetBytes(msg);
                        serverSocket.Send(contents);
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
            sendThread.IsBackground = true;

            recvThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] content = new byte[BUFFERSIZE];
                    string msg = null;
                    try
                    {
                        int n = serverSocket.Receive(content);
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

                    if (msg != null) recvQueue.Add(msg);

                }
            });
            recvThread.IsBackground = true;

            sendThread.Start();
            recvThread.Start();
        }

        protected void Close()
        {
            serverSocket.Close();
            sendQueue.CompleteAdding();
        }

        public void Disconnect()
        {
            serverSocket.Close();
            recvQueue.CompleteAdding();
            sendQueue.CompleteAdding();
        }

        public override void SendMsg2Server(string msg)
        {
            sendQueue.Add(msg);
        }
    }
}
