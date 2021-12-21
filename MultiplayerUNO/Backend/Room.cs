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
using LitJson;

namespace MultiplayerUNO.Backend
{
    public partial class Room
    {
        public static readonly int MinPlayerNumber = 2;
        public static readonly int MaxPlayerNumber = 6;

        public static readonly int MaxSeatIDCounter = 4096;

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
            PlayerLeave,
            PlayerTimeout,
            RobotResponse
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

            currentStatus = GameStatus.Waiting;
        }

        protected Socket listenSocket;
        protected LinkedList<Player.Player> ingamePlayers;
        protected int assignedSeatID = 0;

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


        protected enum GameStatus
        {
            Waiting, 
            Common, 
            QueryPlayer, 
            Plus2Loop,
            CardsDrawing,
            Plus4Loop,
            Questioning,
            GameEnd
        };


        protected GameStatus currentStatus;
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

                JsonData cameJson = null;
                try{ cameJson = JsonMapper.ToObject(msgArgs.msg); }
                catch (JsonException) { continue; }

                string info = string.Format("[{0}] {1}", msgArgs.player.Name, msgArgs.msg);
                Console.WriteLine(info);

                try
                {
                    switch (currentStatus)
                    {
                        case GameStatus.Waiting:
                            ProcWaiting(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Common:
                            ProcCommon(cameJson, msgArgs.player);
                            break;
                        case GameStatus.QueryPlayer:
                            ProcQuery(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Plus2Loop:
                            ProcPlus2Loop(cameJson, msgArgs.player);
                            break;
                        case GameStatus.Plus4Loop:
                            ProcPlus4Loop(cameJson, msgArgs.player);
                            break;
                    }
                }
                catch (TieExceptions)
                {
                    JsonData json = new JsonData
                    {
                        ["turnID"] = 0
                    };
                    GameEndProcess(json);
                }
                catch (PlayerFinishException e)
                {
                    JsonData json = new JsonData
                    {
                        ["turnID"] = e.player.ingameID,
                        ["lastCard"] = lastCard.CardId,
                        ["intInfo"] = lastCardInfo
                    };
                    GameEndProcess(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                

                // foreach (Player.Player p in ingamePlayers)
                //     p.SendMessage(info);
            }
        }

        protected void GameEndProcess(JsonData json)
        {
            gameTimer.Dispose(); //终止计时器
            currentStatus = GameStatus.GameEnd;

            json["state"] = (int)currentStatus;
            json["playerCards"] = new JsonData();
            json["playerCards"].SetJsonType(JsonType.Array);

            foreach (Player.Player p in ingamePlayers)
                json["playerCards"].Add(p.BuildPlayerMapJson(true));
            string sendJson = json.ToJson();

            // 以下：剔除所有被AI接管的玩家
            LinkedList<Player.Player> tempPlayers = new LinkedList<Player.Player>();
            foreach (Player.Player p in ingamePlayers)
            {
                if (p.isRobot == 0)
                {
                    tempPlayers.AddLast(p);
                    p.SendMessage(sendJson);
                }
            }
            ingamePlayers = tempPlayers;

            currentStatus = GameStatus.Waiting;

        }

        public void PlayerJoin(Player.Player player)
        {
            if(ingamePlayers.Count >= MaxPlayerNumber || currentStatus != GameStatus.Waiting
                || assignedSeatID >= MaxSeatIDCounter)
            {
                // 不再接受玩家加入
                RemotePlayer remote = player as RemotePlayer;
                remote?.Leave(); //直接把玩家踢了
                return;
            }

            // 玩家加入房间
            ingamePlayers.AddLast(player);
            
            assignedSeatID++;
            player.seatID = assignedSeatID;

            JsonData json = BuildPlayerWaitingJson();
            player.SendMessage(json.ToJson());

            string joinJson = new JsonData
            {
                ["type"] = 3,
                ["player"] = player.GetPlayerJson()
            }.ToJson();

            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(joinJson);
        }

        public bool PlayerLeave(Player.Player player)
        {
            if(currentStatus == GameStatus.Waiting)
            {
                // 等待状态下有玩家退出：直接退
                bool res = ingamePlayers.Remove(player);
                if (!res) return false;

                string leaveJson = new JsonData
                {
                    ["type"] = 2,
                    ["player"] = player.GetPlayerJson()
                }.ToJson();

                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(leaveJson);
            }
            else
            {
                // 游戏过程中玩家退出：AI接管
                player.isRobot = 1;
                string leaveJson = new JsonData
                {
                    ["state"] = -1,
                    ["playerID"] = player.ingameID
                }.ToJson();

                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(leaveJson);
            }

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

        public JsonData BuildPlayerWaitingJson()
        {

            JsonData json = new JsonData();
            json["type"] = 0;
            json["player"] = new JsonData();
            json["player"].SetJsonType(JsonType.Array);

            foreach (Player.Player player in ingamePlayers)
            {
                json["player"].Add(player.GetPlayerJson());
            }

            return json;
        }
    }
}
