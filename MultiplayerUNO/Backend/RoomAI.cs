using LitJson;
using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Backend
{
    public partial class Room
    {
        /// <summary>
        /// 出牌的AI
        /// </summary>
        protected JsonData AutoPseudoActPlayer(Card lastCard, Player.Player turnPlayer, bool ai = false)
        {
            if(currentStatus == GameStatus.Plus4Loop)
            {
                return new JsonData // 不质疑
                {
                    ["state"] = 4,
                    ["queryID"] = queryID
                };
            }

            if(currentStatus == GameStatus.Plus2Loop)
            {
                return AutoPlus2Action(ai);
            }


            JsonData json = new JsonData
            {
                ["state"] = 2,
                ["queryID"] = queryID
            }; // 默认不出
            
            if(!ai) return json;

            // AI策略：能出就出

            Card intendCard = null;
            if(lastCard != null)
            {
                foreach (Card card in turnPlayer.handCards)
                {
                    if (card.CanResponseTo(lastCard))
                    {
                        intendCard = card;
                        break;
                    }
                }
            }
            else
            {
                intendCard = turnPlayer.handCards.First.Value;
            }

            if (intendCard == null) return json; //无牌可出
            
            json["state"] = 1; // 有牌出
            json["card"] = intendCard.CardId;
            json["color"] = intendCard.Color == Card.CardColor.Invalid ? intendCard.CardId & 3 : -1;
            return json;
        }

        /// <summary>
        /// 摸上牌后是否响应的AI
        /// </summary>
        protected JsonData AutoResponseOrNot(bool ai = false)
        {
            JsonData json = new JsonData
            {
                ["state"] = 1,
                ["action"] = 0,
                ["color"] = 0,
                ["queryID"] = queryID
            };
            // 默认不出
            if (!ai || (lastCard != null && !gainCard.CanResponseTo(lastCard, (Card.CardColor)(lastCardInfo & 3)))) return json;
            // AI策略：能出就出
            json["action"] = 1;
            if (gainCard.Color == Card.CardColor.Invalid) json["color"] = gainCard.CardId & 3;
            return json;
        }

        /// <summary>
        /// 叠+2时的AI
        /// </summary>
        protected JsonData AutoPlus2Action(bool ai = false)
        {
            JsonData json = new JsonData
            {
                ["state"] = 4,
                ["queryID"] = queryID
            }; // 默认不出接受摸牌

            if (!ai) return json;
            
            // AI策略：能出就出
            Card intendCard = null;
            foreach (Card card in currentPlayerNode.Value.handCards)
            {
                if (card.Number == 11) // +2牌
                {
                    intendCard = card;
                    break;
                }
            }
            if (intendCard == null) return json; //无牌可出

            // +2叠起来
            json["state"] = 3; // 有牌出
            json["card"] = intendCard.CardId;
            return json;
        }
    }
}
