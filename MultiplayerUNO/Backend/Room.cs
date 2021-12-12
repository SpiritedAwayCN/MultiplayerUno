using MultiplayerUNO.Backend.Player;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiplayerUNO.Backend
{
    public class Room
    {
        public class MsgArgs
        {
            public Player.Player player;
            public string msg;
            public MsgType type = MsgType.Remote;
        }

        public enum MsgType
        {
            Remote,
            PlayerJoin,
            PlayerLeave
        }


        public BlockingCollection<MsgArgs> InfoQueue { get; }

        public const string Version = "0.1.0";

        protected IPEndPoint iPEndPoint;
        protected Thread listenThread;

        protected Thread processThread;
        public LocalPlayer LocalPlayer { get; }

        public Room(LocalPlayerAdapter localPlayerAdapter)
        {
            iPEndPoint = localPlayerAdapter.EndPoint;
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InfoQueue = new BlockingCollection<MsgArgs>();
            
            
            ingamePlayers =  new LinkedList<Player.Player>(); // 这该死的c#竟然不给线程安全的LinkedList
            // 规定：除初始化外，所有对此容器的操作，均在ProcessThread一个线程中执行
            // 或者也能暴力加锁，但还是算了
            LocalPlayer = new LocalPlayer(localPlayerAdapter);
            ingamePlayers.AddLast(LocalPlayer);

        }

        protected Socket listenSocket;
        protected LinkedList<Player.Player> ingamePlayers;

        public void InitializeRoom()
        {
            processThread = new Thread(ProcessThreadFunc);
            processThread.IsBackground = true;
            processThread.Start();

            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(10);
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Socket rSocket = listenSocket.Accept();
                        RemotePlayer remotePlayer = new RemotePlayer(rSocket, this);
                        new Thread(() =>
                        {
                            if (remotePlayer.OpenStream())
                            {
                                remotePlayer.SendMessage("假装这条是包含房间内全部玩家的信息JSON");
                                InfoQueue.Add(new MsgArgs()
                                {
                                    player = remotePlayer,
                                    type = MsgType.PlayerJoin
                                });
                            }
                        }).Start();

                    }catch(ObjectDisposedException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }catch(SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void ProcessThreadFunc()
        {
            while (true)
            {
                MsgArgs msgArgs = null;
                try
                {
                    msgArgs = InfoQueue.Take();
                }catch(ObjectDisposedException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                
                if(msgArgs.type == MsgType.PlayerJoin)
                {
                    PlayerJoin(msgArgs.player);
                    continue;
                }
                if(msgArgs.type == MsgType.PlayerLeave)
                {
                    PlayerLeave(msgArgs.player);
                    continue;
                }

                string info = string.Format("[{0}] {1}", msgArgs.player.Name, msgArgs.msg);
                Console.WriteLine(info);
                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(info);
            }
        }

        public void PlayerJoin(Player.Player player)
        {
            ingamePlayers.AddLast(player);

            foreach (Player.Player p in ingamePlayers)
                p.SendMessage("Player (" + player.Name + ") has joined the room");
        }

        public bool PlayerLeave(Player.Player player)
        {
            bool res = ingamePlayers.Remove(player);
            if (!res) return false;

            foreach (Player.Player p in ingamePlayers)
                p.SendMessage("Player (" + player.Name + ") has left the room");
            return true;
        }


        public void CloseRoom()
        {
            listenSocket.Close();

            foreach(Player.Player player in ingamePlayers)
            {
                RemotePlayer remotePlayer = player as RemotePlayer;
                remotePlayer?.Leave();
            }
        }


    }
}
