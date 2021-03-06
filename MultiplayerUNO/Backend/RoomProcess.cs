using System;
using System.Collections.Generic;
using System.Threading;
using LitJson;
using MultiplayerUNO.Utils;

namespace MultiplayerUNO.Backend
{
    public partial class Room
    {
        protected const int NormalWaitingTime = 45000;  // 默认等待时间
        protected const int ShortWaitingTime = 10000; // 摸牌后是否出牌等待时间

        /// <summary>
        /// 无效，向对应玩家重发游戏格局json
        /// </summary>
        protected void SendInvalidIInfo(Player.Player sendPlayer)
        {
            sendPlayer.SendMessage(BuildGamePatternJson().ToJson());
        }

        /// <summary>
        /// +4质疑中 处理
        /// </summary>
        protected void ProcPlus4Loop(JsonData jsonData, Player.Player sendPlayer)
        {
            // 如果请求游戏状态json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }

            // 判断发送玩家、请求ID是否对应
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID)
            {
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if(state == 4)
            { // 不质疑.准备摸牌
                DrawCardBack2Common(4, sendPlayer);
            }
            else
            {
                if (plus4Player.CanHandoutPlus4(plus4ResponseCard, plus4ColorID & 3))
                    DrawCardBack2Common(6, sendPlayer); // 质疑失败
                else
                    DrawCardBack2Common(4, plus4Player, false); // 质疑成功

            }
        }

        /// <summary>
        /// 叠+2处理
        /// </summary>
        protected void ProcPlus2Loop(JsonData jsonData, Player.Player sendPlayer)
        {
            // 如果请求游戏状态json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }
            // 判断请求id与发送者是否合法
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID)
            {
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if (state == 3)
            {   // 叠+2出牌
                int cardId = (int)jsonData["card"];
                Card responseCard = FindCardInPlayerHandcards(cardId, sendPlayer);

                if (responseCard == null || responseCard.Number != 11)
                {
                    // 玩家没有这张牌，或这张牌不是+2
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                // 以上，合法性判断完成，开始出牌
                PlayerHandCard(sendPlayer, responseCard, -1);
            }
            else
            {
                DrawCardBack2Common(drawingCardCounter, sendPlayer); // 摸牌
                drawingCardCounter = 0; //计数器归0
            }

        }

        /// <summary>
        /// 摸牌后询问
        /// </summary>
        protected void ProcQuery(JsonData jsonData, Player.Player sendPlayer)
        {
            // 如果请求游戏状态json
            int state = (int)jsonData["state"];
            if(state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }


            int action = (int)jsonData["action"];
            int responseID = (int)jsonData["queryID"];
            int colorId = -1;

            if(gainCard.Color == Card.CardColor.Invalid)
            {
                colorId = (int)jsonData["color"];
                if(colorId >> 2 != 0) // 万能/+4牌，需要指定颜色
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
            }

            if(responseID != queryID)
            {
                // 请求编号需要一致
                SendInvalidIInfo(sendPlayer);
                return;
            }

            if(action == 1)
            {
                // 出的情况
                if (lastCard != null && !gainCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)))
                {
                    // 需要可以响应
                    SendInvalidIInfo(sendPlayer);
                    return;
                }

                // 经检验合法
                PlayerHandCard(sendPlayer, gainCard, colorId); // 出牌

            }
            else
            {
                // 不出选择过的情况
                gameTimer.Dispose(); // 关闭计时器
                Change2NextTurnPlayerNode(); // 更新当前玩家为将要操作的玩家
                queryID++; // 即将发起客户端询问请求

                TimerStart(new MsgArgs
                {
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                    player = currentPlayerNode.Value,
                    type = MsgType.PlayerTimeout
                }); // 计时开始

                currentStatus = GameStatus.Common; // 游戏状态设置为1

                if(currentPlayerNode.Value.isRobot == 1) // AI接管时
                {
                    // 能出则出
                    InfoQueue.Add(new MsgArgs
                    {
                        msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                        player = currentPlayerNode.Value,
                        type = MsgType.PlayerTimeout
                    });
                }

                string json = BuildGamePatternJson().ToJson();
                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(json); // 向每名玩家发送游戏格局Json
            }


        }

        protected void ProcCommon(JsonData jsonData, Player.Player sendPlayer)
        {
            // 如果请求游戏状态json
            int state = (int)jsonData["state"];
            if (state == 0)
            {
                sendPlayer.SendMessage(BuildGameStateJson(sendPlayer).ToJson());
                return;
            }
            // 需要请求id一致
            int responseID = (int)jsonData["queryID"];
            if (responseID != queryID || sendPlayer.ingameID != currentPlayerNode.Value.ingameID) {
                SendInvalidIInfo(sendPlayer);
                return;
            }
            
            if(state == 1)
            {
                // 出了牌的情形
                int cardId = (int)jsonData["card"];
                int colorId = (int)jsonData["color"];
                Card responseCard = FindCardInPlayerHandcards(cardId, sendPlayer);

                // 必须是可响应的牌
                if(responseCard == null || (responseCard.Color == Card.CardColor.Invalid && colorId >> 2 != 0))
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                bool condition2 = responseCard.IsPlus4() && sendPlayer.handCards.Count <= 1; //最后一张手牌不能打+4
                bool condition3 = lastCard != null && !responseCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)); //这个牌不能响应前一张排

                if(condition2 || condition3)
                {
                    SendInvalidIInfo(sendPlayer);
                    return;
                }
                // 以上，合法性判断完成；以下，操作成功的处理

                PlayerHandCard(sendPlayer, responseCard, colorId);
            }
            else
            { // 不出牌的情形
                gameTimer.Dispose(); // 停止计时器

                gainCard = cardPile.DrawOneCard();
                sendPlayer.GainCard(gainCard); // 摸一张牌
                queryID++; // 即将发起客户端询问请求

                TimerStart(new MsgArgs
                {
                    msg = AutoResponseOrNot().ToJson(), //默认不出
                    player = currentPlayerNode.Value,
                    type = MsgType.PlayerTimeout
                }, ShortWaitingTime); // 计时开始

                if(sendPlayer.isRobot == 1) // AI接管时
                {
                    InfoQueue.Add(new MsgArgs
                    {
                        msg = AutoResponseOrNot(true).ToJson(), //能出就出
                        player = currentPlayerNode.Value,
                        type = MsgType.RobotResponse
                    });
                }

                currentStatus = GameStatus.QueryPlayer; // 进入2号状态
                foreach (Player.Player p in ingamePlayers)
                    p.SendMessage(BuildGamePatternJson(p).ToJson()); // 向每名玩家发送游戏格局Json

            }
        }

        /// <summary>
        /// 房间处于等待状态的处理
        /// </summary>
        protected void ProcWaiting(JsonData jsonData, Player.Player sendPlayer)
        {
            int type = (int)jsonData["type"];

            switch (type)
            {
                case 0:
                    // 请求状态json
                    sendPlayer.SendMessage(BuildPlayerWaitingJson().ToJson());
                    return;
                case 1:
                    // 玩家准备
                    if(sendPlayer.IsReady == false)
                    {
                        sendPlayer.IsReady = true;
                        string readyJson = new JsonData
                        {
                            ["type"] = 1,
                            ["player"] = sendPlayer.GetPlayerJson()
                        }.ToJson();

                        foreach (Player.Player p in ingamePlayers)
                        {
                            p.SendMessage(readyJson);
                        }
                    }
                    return;
                case 2:
                    // 玩家取消准备
                    if (sendPlayer.IsReady)
                    {
                        sendPlayer.IsReady = false;
                        string readyJson = new JsonData
                        {
                            ["type"] = 4,
                            ["player"] = sendPlayer.GetPlayerJson()
                        }.ToJson();

                        foreach (Player.Player p in ingamePlayers)
                        {
                            p.SendMessage(readyJson);
                        }
                    }
                    return;
            }

            if (type != 3) return;

            // 玩家开始游戏
            bool canStart = true;
            foreach (Player.Player p in ingamePlayers)
                if(p.IsReady == false)
                {
                    canStart = false;
                    break;
                }
            if (!canStart || ingamePlayers.Count < MinPlayerNumber)
            {
                // 有人未准备 或 未到达最小游戏人数，开始失败
                sendPlayer.SendMessage("{\"type\":6}");
                return;
            }

            // 游戏开始
            ShuffleIngamePlayers();  // 重排玩家
            cardPile = new GameCardPile();  // 新牌堆
            cardPile.ShuffleCards();    // 洗牌
            direction = 1;  // 正常顺序转
            lastCard = null;  // 开局不存在需要响应的上一张牌
            lastCardInfo = -1; // 仅lastCard为+4/万能时有意义
            queryID = 1; // 重置请求编号计数器
            drawingCardCounter = 0; // +2的摸牌计数器
            plus4ColorID = -1; // 被+4响应牌的颜色
            plus4Player = null; // 打出+4的玩家
            plus4ResponseCard = null; //被+4响应牌

            currentStatus = GameStatus.Common;  // 进入1号状态

            foreach (Player.Player p in ingamePlayers)
            {
                p.StartGameReset();
                // TODO (UI DEUBG): 定制每个人的手牌张数
                // UI 调试 START
                //if (p.Name.IndexOf("Server") != -1) {
                //    p.GainCard(cardPile.DrawCards(1));
                //} else {
                //    p.GainCard(cardPile.DrawCards(10));
                //}
                // UI 调试 END
                p.GainCard(cardPile.DrawCards(7)); // 每个玩家发初始7张牌
            }
            currentPlayerNode = ingamePlayers.First;    // 第一个玩家首个行动

            GC.Collect(); // 垃圾清理

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            });

            foreach (Player.Player p in ingamePlayers)
            {
                p.SendMessage(BuildGameStateJson(p).ToJson());  // 向每个玩家发出游戏状态json
            }

        }

        /// <summary>
        /// 随机打乱房间内的玩家，并分配playerID
        /// </summary>
        protected void ShuffleIngamePlayers()
        {
            Player.Player[] tempPlayers = new Player.Player[ingamePlayers.Count];
            int[] indexes = new int[ingamePlayers.Count];
            Random random = new Random();

            for (int i = 0; i < indexes.Length; i++)
                indexes[i] = i;
            for (int i = indexes.Length - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int t = indexes[k];
                indexes[k] = indexes[i];
                indexes[i] = t;
            }

            int counter = 0;
            foreach(Player.Player p in ingamePlayers)
            {
                tempPlayers[indexes[counter]] = p;
                counter++;
            }

            ingamePlayers.Clear();

            for (int i = 0; i < indexes.Length; i++)
            {
                tempPlayers[i].ingameID = i + 1;
                ingamePlayers.AddLast(tempPlayers[i]);
            }
        }

        /// <summary>
        /// 查找玩家手牌中某编号的牌，没有则返回null
        /// </summary>
        protected Card FindCardInPlayerHandcards(int cardID, Player.Player player)
        {
            foreach(Card card in player.handCards)
            {
                if (card.CardId == cardID) return card;
            }
            return null;
        }

        /// <summary>
        /// 4号状态的处理，摸完牌后立即转移至正常状态
        /// </summary>
        protected void DrawCardBack2Common(int cardCount, Player.Player drawnPlayer, bool toNext = true)
        {
            Card[] drawnCards = cardPile.DrawCards(cardCount);
            drawnPlayer.GainCard(drawnCards); // 摸牌

            // 以下，构造json
            JsonData json = new JsonData
            {
                ["state"] = 4,
                ["lastCard"] = cardCount, // 摸上的牌数目
                ["turnID"] = drawnPlayer.ingameID
            };
            string basicJson = json.ToJson(); // 无手牌信息的json

            json["playerCards"] = new JsonData();
            json["playerCards"].SetJsonType(JsonType.Array);
            foreach (Card card in drawnCards)
            {
                json["playerCards"].Add(card.CardId);
            }
            string personalJson = json.ToJson(); // 有手牌信息的json

            foreach (Player.Player p in ingamePlayers)
            {   // 4号状态
                p.SendMessage(p.ingameID == drawnPlayer.ingameID ? personalJson : basicJson);
            }
            // 摸牌结束，回到1号状态

            if(toNext) Change2NextTurnPlayerNode(); // 跳到下一名玩家，lastCard不变
            currentStatus = GameStatus.Common; // 切换至一号状态，lastCard等属性不变

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            }); // 计时开始

            if (currentPlayerNode.Value.isRobot == 1) // 如果已被AI接管
            {
                InfoQueue.Add(new MsgArgs
                {
                    player = currentPlayerNode.Value,
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                    type = MsgType.RobotResponse
                });
            }

            string patternJson = BuildGamePatternJson().ToJson();
            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(patternJson); // 向每名玩家发送游戏格局Json

        }

        /// <summary>
        /// 玩家摸牌
        /// </summary>
        protected void PlayerHandCard(Player.Player sendPlayer, Card responseCard, int colorId)
        {
            if (colorId < 0) colorId = 0;

            gameTimer.Dispose(); // 关闭计时器
            if (responseCard.Number == 12) direction *= -1; //翻转
                                                            // TODO  +2/+4的情况，还没写

            sendPlayer.RemoveCard(responseCard); //玩家失去牌
            cardPile.Move2DiscardPile(responseCard); //牌进入弃牌堆

            Card tempCard = lastCard;
            int tempinfo = lastCardInfo;

            lastCard = responseCard; //更新需要响应的牌为当前打出的牌
            lastCardInfo = 4 * sendPlayer.ingameID +
                (responseCard.Color == Card.CardColor.Invalid ? colorId : 0); // 协议规定格式
            Change2NextTurnPlayerNode(responseCard.Number == 10); // 更新当前玩家为将要操作的玩家

            // 出完牌的判断
            if (sendPlayer.handCards.Count <= 0)
                throw new PlayerFinishException(sendPlayer);

            queryID++; // 即将发起客户端询问请求
            currentStatus = GameStatus.Common; 

            if (responseCard.Number == 11) // +2牌
            {
                currentStatus = GameStatus.Plus2Loop; // +2牌则开始叠加操作
                drawingCardCounter += 2; // 计数器叠加
            }else if(responseCard.CardId >= 104)
            {
                plus4ResponseCard = tempCard; // 更新+4响应的牌
                plus4Player = sendPlayer; // 更新打出+4的玩家
                plus4ColorID = tempinfo;
                currentStatus = GameStatus.Plus4Loop; // +4牌，询问质疑
            }

            TimerStart(new MsgArgs
            {
                msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value).ToJson(),
                player = currentPlayerNode.Value,
                type = MsgType.PlayerTimeout
            }); // 计时开始

            if (currentPlayerNode.Value.isRobot == 1) // 如果已被AI接管
            {
                InfoQueue.Add(new MsgArgs
                {
                    player = currentPlayerNode.Value,
                    msg = AutoPseudoActPlayer(lastCard, currentPlayerNode.Value, true).ToJson(),
                    type = MsgType.RobotResponse
                });
            }

            string json = BuildGamePatternJson().ToJson();
            foreach (Player.Player p in ingamePlayers)
                p.SendMessage(json); // 向每名玩家发送游戏格局Json
        }


        protected GameCardPile cardPile;
        protected LinkedListNode<Player.Player> currentPlayerNode;
        protected Card lastCard, gainCard, plus4ResponseCard;
        protected int lastCardInfo, plus4ColorID;
        protected int direction;
        protected Timer gameTimer;
        protected long lastTimerStartMs;
        protected int lastTimerWaitSec;
        protected int drawingCardCounter;
        protected Player.Player plus4Player;

        protected int queryID; // 请求编号

        /// <summary>
        /// 开始计时器
        /// </summary>
        protected void TimerStart(MsgArgs msg, int waitingTime = NormalWaitingTime)
        {
            // gameTimer?.Dispose();

            gameTimer = new Timer((state) =>
            {
                InfoQueue.Add(msg);
            }, msg, waitingTime + 4000, Timeout.Infinite);
            lastTimerStartMs = DateTime.Now.Ticks;
            lastTimerWaitSec = waitingTime;
        }

        /// <summary>
        /// 构造游戏格局json
        /// </summary>
        protected JsonData BuildGamePatternJson(Player.Player player = null)
        {
            JsonData json = new JsonData
            {
                ["state"] = (int)currentStatus,
                ["queryID"] = queryID
            };

            // 不同的状态不同的构造方式，详见定义文档
            switch (currentStatus)
            {
                case GameStatus.Common:
                case GameStatus.Plus4Loop:
                    json["lastCard"] = lastCard == null ? -1 : lastCard.CardId;
                    json["intInfo"] = lastCardInfo;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;
                case GameStatus.QueryPlayer:
                    json["lastCard"] = (player.ingameID==currentPlayerNode.Value.ingameID) ? gainCard.CardId : -1;
                    json["intInfo"] = cardPile.CardPileLeft;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;
                case GameStatus.Plus2Loop:
                    json["lastCard"] = lastCard.CardId;
                    json["intInfo"] = drawingCardCounter;
                    json["turnID"] = currentPlayerNode.Value.ingameID;
                    json["playerID"] = lastCardInfo >> 2;
                    json["time"] = lastTimerWaitSec - (int)(DateTime.Now.Ticks - lastTimerStartMs) / 10000;
                    break;

            }

            return json;
        }

        /// <summary>
        /// 构造游戏状态json，格式见文档
        /// </summary>
        protected JsonData BuildGameStateJson(Player.Player queryPlayer)
        {
            JsonData json = new JsonData {
                ["cardpileLeft"] = cardPile.CardPileLeft,
                ["direction"] = direction,
                ["turnInfo"] = BuildGamePatternJson(queryPlayer),
                ["yourID"] = queryPlayer.ingameID
            };

            json["playerMap"] = new JsonData();
            json["playerMap"].SetJsonType(JsonType.Array);
            foreach(Player.Player p in ingamePlayers)
            {
                json["playerMap"].Add(p.BuildPlayerMapJson(queryPlayer.ingameID == p.ingameID));
            }

            return json;
        }
        
        /// <summary>
        /// 下一个出牌玩家
        /// </summary>
        /// <param name="skip">是否跳过下一个玩家</param>
        protected void Change2NextTurnPlayerNode(bool skip = false)
        {
            if(direction == 1) //正常逆时针
            {
                currentPlayerNode = currentPlayerNode.Next;
                if (currentPlayerNode == null) currentPlayerNode = ingamePlayers.First;
                if (skip) Change2NextTurnPlayerNode(false); //打出了禁止，跳过一个玩家
            }
            else //反向顺时针
            {
                currentPlayerNode = currentPlayerNode.Previous;
                if (currentPlayerNode == null) currentPlayerNode = ingamePlayers.Last;
                if (skip) Change2NextTurnPlayerNode(false); //打出了禁止，跳过一个玩家
            }
        }

    }
}
