using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiplayerUNO.Backend.Player
{
    public class RemotePlayer : Player
    {

        protected Thread sendThread;
        protected Thread recvThread;

        protected Socket clientSocket;

        public Room GameRoom { get; }


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
                        socket.Send(Encoding.UTF8.GetBytes(msg));
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
            sendThread.IsBackground = true;

            recvThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] content = new byte[BUFFERSIZE];
                        int n = clientSocket.Receive(content); //阻塞于此
                        string word = Encoding.UTF8.GetString(content, 0, n);

                        GameRoom.InfoQueue.Add(new Room.MsgArgs
                        {
                            player = this,
                            msg = word
                        });
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
                });
            });

            recvThread.IsBackground = true;
        }


        public static int BUFFERSIZE = 8192;
        public bool OpenStream()
        {
            try
            {
                byte[] content = new byte[BUFFERSIZE];
                int n = clientSocket.Receive(content);
                string word = Encoding.UTF8.GetString(content, 0, n);
                name = word;

                Console.WriteLine(word);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                clientSocket.Close();
                return false;
            }


            sendThread.Start();
            recvThread.Start();
            return true;
        }

        public void Leave()
        {
            sendQueue.CompleteAdding();
            clientSocket.Close();
        }
    }
}
