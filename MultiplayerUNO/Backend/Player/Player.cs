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
    public abstract class Player
    {
        protected string name;

        public int isRobot;
        public string Name { get { return name; } }
        public int seatID;
        public int ingameID;

        public BlockingCollection<string> sendQueue;

        public void SendMessage(string msg)
        {
            if (isRobot == 1) return;
            sendQueue.Add(msg);
        }


        protected bool isReady;
        public bool IsReady
        {
            get { return isReady; }
            set {
                IsReady = value;
                playerJsonCache = null; // 缓存失效
            }
        }

        protected JsonData playerJsonCache = null;
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

        public LinkedList<Card> handCards = new LinkedList<Card>();

        public void StartGameReset()
        {
            isReady = false;
            handCards.Clear();
        }

        public void GainCard(Card card)
        {
            handCards.AddLast(card);
        }

        public void GainCard(Card[] cards)
        {
            foreach (Card card in cards)
                handCards.AddLast(card);
        }

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

        public bool RemoveCard(Card card)
        {
            bool res = handCards.Remove(card);
            // if (handCards.Count <= 0) throw new PlayerFinishException(this);
            return res;
        }

        public JsonData BuildPlayerMapJson(bool containsHandcards)
        {
            JsonData json = new JsonData
            {
                ["name"] = name,
                ["playerID"] = ingameID,
                ["cardsCount"] = handCards.Count,
                ["isRobot"] = isRobot
            };
            if (containsHandcards)
            {
                json["handcards"] = new JsonData();
                json["handcards"].SetJsonType(JsonType.Array);
                foreach(Card card in handCards)
                    json["handcards"].Add(card.CardId);
            }

            return json;
        }

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
