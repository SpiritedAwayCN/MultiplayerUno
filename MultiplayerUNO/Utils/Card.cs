using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// 表示一张牌
    /// </summary>
    public class Card
    {
        public enum CardColor
        {
            Invalid = -1,
            Red,
            Yellow,
            Green,
            Blue
        }

        public Card(int cardId)
        {
            CardId = cardId;
            if (cardId < 4 || cardId >= 104) Color = CardColor.Invalid; //万能与+4，颜色无意义
            else Color = (CardColor)(cardId & 3);
        }

        public CardColor Color { get; }
        public int CardId { get; }
        public int Number
        {
            get
            {
                if (CardId < 4) return -1; //万能牌，Number无意义
                if (CardId >= 104) return 0xf; // +4牌，返回15
                return CardId >> 3; //其余，参考手册
            }
        }

        /// <summary>
        /// 牌是否可以相应
        /// </summary>
        /// <param name="newCard">响应的牌</param>
        /// <param name="oldCard">被响应的牌</param>
        /// <param name="color">被响应牌的颜色（仅万能/+4）时有效</param>
        /// <returns></returns>
        public static bool CanResponseTo(Card newCard, Card oldCard, CardColor color = CardColor.Invalid)
        {
            if (newCard.Color == CardColor.Invalid) return true; // 万能与+4能响应任何牌
            if (newCard.Color == oldCard.Color) return true; // 颜色一致
            if (oldCard.Color == CardColor.Invalid && newCard.Color == color) return true; //万能/+4与声明的一致
            return newCard.Number == oldCard.Number; // 数字/图案，万能与+4已在第一个if中直接返回true
        }

        /// <summary>
        /// 是否可以响应另一张牌
        /// </summary>
        /// <param name="oldCard">被响应的牌</param>
        /// <param name="color">被响应牌的颜色（仅万能/+4）时有效</param>
        /// <returns></returns>
        public bool CanResponseTo(Card oldCard, CardColor color = CardColor.Invalid)
        {
            return CanResponseTo(this, oldCard, color);
        }

        /// <summary>
        /// 是+4吗
        /// </summary>
        /// <returns></returns>
        public bool IsPlus4()
        {
            return CardId >= 104;
        }
        /// <summary>
        /// 是+2吗
        /// </summary>
        /// <returns></returns>
        public bool IsPlus2() {
            return (CardId >> 3) == 11;
        }
        /// <summary>
        /// 是翻转吗
        /// </summary>
        /// <returns></returns>
        public bool IsReverse() {
            return (CardId >> 3) == 12;
        }
    }
}
