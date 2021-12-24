using LitJson;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend.Player
{
    /// <summary>
    /// 游戏后端处理玩家信息的抽象类，实际处理过程不考虑玩家是本地的还是远程的
    /// </summary>
    public abstract class Player
    {
        protected string name;

        public int isRobot; // 是否被AI接管
        public string Name { get { return name; } } // 玩家名称
        public int seatID;  // 玩家的UID，用于客户端辨识
        public int ingameID;    // 玩家在某一局游戏中的ID，代表座次

        public BlockingCollection<string> sendQueue;  // 后端处理线程将

        /// <summary>
        /// 由后端调用，向对应玩家发送json字符串的方法
        /// </summary>
        /// <param name="msg">要发送的JSON字符串</param>
        public void SendMessage(string msg)
        {
            if (isRobot == 1) return;
            sendQueue.Add(msg);
        }


        protected bool isReady; // 玩家是否已经准备
        public bool IsReady
        {
            get { return isReady; }
            set {
                isReady = value;
                playerJsonCache = null; // 状态改变，令玩家Json缓存失效
            }
        }

        protected JsonData playerJsonCache = null; // 玩家json缓存，由于玩家状态变化不频繁，引入缓存

        /// <summary>
        /// 房间为等待状态时，获取玩家状态信息Json
        /// </summary>
        /// <param name="ignoreCache">是否忽略缓存强制刷新</param>
        /// <returns>玩家状态json</returns>
        public JsonData GetPlayerJson(bool ignoreCache = false)
        {
            if (!ignoreCache && playerJsonCache != null) return playerJsonCache;

            JsonData json = new JsonData();
            json["seatID"] = seatID;
            json["name"] = name;
            json["isReady"] = IsReady;

            playerJsonCache = json;
            return playerJsonCache;
        }

        public LinkedList<Card> handCards = new LinkedList<Card>(); // 玩家手牌

        /// <summary>
        /// 后端调用，游戏开始时重置玩家相关信息
        /// </summary>
        public void StartGameReset()
        {
            IsReady = false;
            handCards.Clear();
        }

        /// <summary>
        /// 玩家获得一张手牌
        /// </summary>
        /// <param name="card">获得的牌</param>
        public void GainCard(Card card)
        {
            handCards.AddLast(card);
        }

        /// <summary>
        /// 玩家一次性获得多张手牌
        /// </summary>
        /// <param name="cards">获得的牌</param>
        public void GainCard(Card[] cards)
        {
            foreach (Card card in cards)
                handCards.AddLast(card);
        }

        /// <summary>
        /// 根据牌的编号令玩家失去一张牌
        /// </summary>
        /// <param name="cardID">将要失去牌的编号</param>
        /// <returns>是否成功</returns>
        public bool RemoveCardById(int cardID)
        {
            Card cardObj = null;
            foreach(Card card in handCards)
            {
                if(card.CardId == cardID)
                {
                    cardObj = card;
                    break;
                }
            }

            if (cardObj == null) return false;
            return handCards.Remove(cardObj);
        }

        /// <summary>
        /// 令玩家失去一张牌
        /// </summary>
        /// <param name="card">要失去的牌</param>
        /// <returns>是否成功</returns>
        public bool RemoveCard(Card card)
        {
            bool res = handCards.Remove(card);
            // if (handCards.Count <= 0) throw new PlayerFinishException(this);
            return res;
        }

        /// <summary>
        /// 构造PlayerMap Json
        /// </summary>
        /// <param name="containsHandcards">是否包含玩家手牌信息</param>
        /// <returns>构造出的Json</returns>
        public JsonData BuildPlayerMapJson(bool containsHandcards)
        {
            JsonData json = new JsonData
            {
                ["name"] = name,
                ["playerID"] = ingameID,
                ["cardsCount"] = handCards.Count,
                ["isRobot"] = isRobot
            };
            if (containsHandcards)  // 如果包含手牌信息
            {
                json["handcards"] = new JsonData();
                json["handcards"].SetJsonType(JsonType.Array);
                foreach(Card card in handCards)
                    json["handcards"].Add(card.CardId);
            }

            return json;
        }

        /// <summary>
        /// 检测玩家是否没有其他牌，只能打出+4牌
        /// </summary>
        /// <param name="lastCard">上一张牌</param>
        /// <param name="colorID">上一张牌的颜色</param>
        /// <returns>是否可以打出</returns>
        public bool CanHandoutPlus4(Card lastCard, int colorID)
        {
            foreach(Card card in handCards)
            {
                if (card.CardId >= 104) continue;
                if (lastCard == null) return false;
                if (card.CanResponseTo(lastCard, (Card.CardColor)colorID)) return false;
            }

            return true;
        }
    }
}
