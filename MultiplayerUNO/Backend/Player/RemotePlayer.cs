using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;
 
namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    /// 远程玩家，通过Socke连接到房间
    /// </summary>
    public class RemotePlayer : Player
    {
        public static string ProtocolVersion = "0.0.1";

        protected Thread sendThread;    // 发送线程
        protected Thread recvThread;    // 接受线程

        protected Socket clientSocket;  // 套接字

        public Room GameRoom { get; }   // 所在房间


        public RemotePlayer(Socket socket, Room gameRoom)
        {
            sendQueue = new BlockingCollection<string>();

            clientSocket = socket;
            GameRoom = gameRoom;

            sendThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string msg = sendQueue.Take(); //阻塞于此
                        socket.Send(Encoding.UTF8.GetBytes(msg + "$")); // 末尾添加$便于分割
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
                    catch (InvalidOperationException e) {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
            sendThread.IsBackground = true; // 设为后台线程

            recvThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] content = new byte[BUFFERSIZE];
                        int n = clientSocket.Receive(content); //阻塞于此
                        string word = Encoding.UTF8.GetString(content, 0, n);

                        foreach(string sw in word.Split('$'))   // 根据$分割
                        {
                            if (sw.Length <= 0) continue;
                            GameRoom.InfoQueue.Add(new Room.MsgArgs
                            {
                                player = this,
                                msg = sw
                            }); // 信息放入InfoQueue，供游戏处理线程处理
                        }
                        
                    }
                    catch(ObjectDisposedException se)
                    {
                        Console.WriteLine(se.Message);
                        break;

                    }catch(SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Console.WriteLine(ioe.Message);
                        break;
                    }catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

                GameRoom.InfoQueue.Add(new Room.MsgArgs
                {
                    player = this,
                    type = Room.MsgType.PlayerLeave
                }); // 告知游戏处理线程玩家已离开
            });

            recvThread.IsBackground = true; // 设置为后台线程
        }


        public static int BUFFERSIZE = 8192;
        /// <summary>
        /// Socket连接建立后的操作，判断发过来的首条消息是否格式正确，协议版本是否一致
        /// </summary>
        /// <returns>对方发送的信息是否合法</returns>
        public bool OpenStream()
        {
            try
            {
                byte[] content = new byte[BUFFERSIZE];
                int n = clientSocket.Receive(content);
                string word = Encoding.UTF8.GetString(content, 0, n);
                JsonData json = JsonMapper.ToObject(word.Split('$')[0]);
                if (!((string)json["version"] == ProtocolVersion))  // 版本应一致
                    throw new ArgumentException("Inconsistent Version", "version");

                name = (string)json["name"];    // 获取玩家名称

                Console.WriteLine(word);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                clientSocket.Close();
                return false;   // 任何不合法，直接断掉
            }


            sendThread.Start(); // 开启发送线程
            recvThread.Start(); // 开启接受线程
            return true;
        }

        /// <summary>
        /// 后端调用，将玩家踢出房间
        /// </summary>
        public void Leave()
        {
            sendQueue.CompleteAdding();
            clientSocket.Close();
        }
    }
}
