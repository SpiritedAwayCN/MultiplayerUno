using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerUNO.Utils
{
    // 一张牌
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

        public static bool CanResponseTo(Card newCard, Card oldCard, CardColor color = CardColor.Invalid)
        {
            if (newCard.Color == CardColor.Invalid) return true; // 万能与+4能响应任何牌
            if (newCard.Color == oldCard.Color) return true; // 颜色一致
            if (oldCard.Color == CardColor.Invalid && newCard.Color == color) return true; //万能/+4与声明的一致
            return newCard.Number == oldCard.Number; // 数字/图案，万能与+4已在第一个if中直接返回true
        }

        public bool CanResponseTo(Card oldCard, CardColor color = CardColor.Invalid)
        {
            return CanResponseTo(this, oldCard, color);
        }

        public bool IsPlus4()
        {
            return CardId >= 104;
        }
    }
}
