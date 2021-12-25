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
    /// <summary>
    /// 游戏房间类
    /// </summary>
    public partial class Room
    {
        public static readonly int MinPlayerNumber = 2;
        public static readonly int MaxPlayerNumber = 6;

        public static readonly int MaxSeatIDCounter = 4096;

        /// <summary>
        /// InfoQueue存储的数据格式
        /// </summary>
        public class MsgArgs
        {
            public Player.Player player;    // 发送玩家
            public string msg;  // 字符串数据
            public MsgType type = MsgType.Remote; // 信息类型
        }


        public enum MsgType
        {
            Remote,     // 由前端发送
            PlayerJoin, // 玩家进入房间
            PlayerLeave, // 玩家离开房间
            PlayerTimeout, // 玩家超时
            RobotResponse // AI接管自动响应
        }


        public BlockingCollection<MsgArgs> InfoQueue { get; } // 信息处理专用Queue

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
        protected int assignedSeatID = 0; // 已分配的UID，永远不减

        /// <summary>
        /// 初始化房间
        /// </summary>
        public void InitializeRoom()
        {
            // 开启游戏处理线程
            processThread = new Thread(ProcessThreadFunc);
            processThread.IsBackground = true;
            processThread.Start();

            // 绑定socket
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(10);

            // 监听线程
            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Socket rSocket = listenSocket.Accept(); // 建立连接
                        RemotePlayer remotePlayer = new RemotePlayer(rSocket, this);
                        new Thread(() =>
                        {
                            if (remotePlayer.OpenStream())
                            {
                                // 成功进入房间，通知处理线程
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

            listenThread.IsBackground = true; //后台线程
            listenThread.Start();
        }

        /// <summary>
        /// 游戏状态
        /// </summary>
        protected enum GameStatus
        {
            Waiting, // 等待 
            Common,  // 普通
            QueryPlayer, // 不出摸牌询问
            Plus2Loop, // 叠+2
            CardsDrawing, // 摸牌
            Plus4Loop, // +4质疑
            Questioning, // 已弃用
            GameEnd // 游戏结束
        };


        protected GameStatus currentStatus; // 当前状态

        /// <summary>
        /// 核心：游戏处理线程
        /// </summary>
        public void ProcessThreadFunc()
        {
            while (true)
            {
                MsgArgs msgArgs = null;
                try
                {
                    msgArgs = InfoQueue.Take(); // 不断尝试从消息中取出信息
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
                    // Server自己发出的玩家加入信息
                    PlayerJoin(msgArgs.player);
                    continue;
                }
                if(msgArgs.type == MsgType.PlayerLeave)
                {
                    // Server发出的玩家离开信息
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
                    // 根据当前状态调用对应的处理函数
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
                    // 平局异常，游戏结束
                    JsonData json = new JsonData
                    {
                        ["turnID"] = 0
                    };
                    GameEndProcess(json);
                }
                catch (PlayerFinishException e)
                {
                    // 有玩家打完了牌，游戏结束
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

        /// <summary>
        /// 游戏结束后的操作
        /// </summary>
        protected void GameEndProcess(JsonData json)
        {
            gameTimer.Dispose(); //终止计时器
            currentStatus = GameStatus.GameEnd;

            json["state"] = (int)currentStatus;
            json["playerCards"] = new JsonData();
            json["playerCards"].SetJsonType(JsonType.Array);

            foreach (Player.Player p in ingamePlayers)
                json["playerCards"].Add(p.BuildPlayerMapJson(true)); // 此时显示所有玩家的手牌
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

            currentStatus = GameStatus.Waiting; //切换回等待状态

        }

        /// <summary>
        /// 玩家加入房间
        /// </summary>
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
            
            assignedSeatID++; // 玩家UID递增
            player.seatID = assignedSeatID; // UID复制

            JsonData json = BuildPlayerWaitingJson(); // 构造等待状态下的房间信息json
            player.SendMessage(json.ToJson()); // 发送给加入的player

            string joinJson = new JsonData
            {
                ["type"] = 3,
                ["player"] = player.GetPlayerJson()
            }.ToJson();

            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(joinJson); // 向房间内所有玩家广播进入信息
        }

        /// <summary>
        /// 玩家离开
        /// </summary>
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
                    p.SendMessage(leaveJson); // 广播玩家离开
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
                    p.SendMessage(leaveJson); // 广播
            }

            return true;
        }


        /// <summary>
        /// 供前端调用，关闭房间
        /// </summary>
        public void CloseRoom()
        {
            listenSocket.Close(); // 关套接字

            foreach(Player.Player player in ingamePlayers)
            {
                RemotePlayer remotePlayer = player as RemotePlayer;
                remotePlayer?.Leave(); // 强制远程玩家离开
            }
        }

        /// <summary>
        /// 构造等待状态下房间内玩家json
        /// </summary>
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
