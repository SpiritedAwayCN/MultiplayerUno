using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MultiplayerUNO.Backend;

namespace MultiplayerUNO.Utils
{
    /// <summary>
    /// 处理一局游戏的牌堆，包含牌堆与弃牌堆
    /// </summary>
    public class GameCardPile
    {
        private LinkedList<Card> CardPile = new LinkedList<Card>(); // 牌堆
        private LinkedList<Card> DiscardPile = new LinkedList<Card>(); // 弃牌堆

        public int CardPileLeft { get { return CardPile.Count; } }
        public int DiscardPileLeft { get { return DiscardPile.Count; } }

        public GameCardPile()
        {
            for (int i = 0; i < 108; i++)
                CardPile.AddLast(new Card(i)); // 初始化：牌堆108张洗过的牌
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleCards() {
            //弃牌堆concat进牌堆，然后将牌堆打乱
            CardPile = new LinkedList<Card>(CardPile.Concat(DiscardPile).OrderBy(p => Guid.NewGuid().ToString()));
            
            // TODO UI 测试的配置
            // 用于 UI 调试, 定制手牌 START
            //int[] cardID = new int[] {
            //    //// +4
            //    //104,0,8,9,10,11,16,
            //    //12,13,14,15,20,24,28,
            //    //1,2,3,4,

            //    //// +2
            //    //104,0,88,89,10,11,16,
            //    //92,93,14,15,20,24,28,
            //    //1,2,3,4,

            //    //// 转
            //    96,97,11,21
            //};
            //for (int i = cardID.Length - 1; i >= 0; --i) {
            //    Card rec = null;
            //    int skip = cardID.Length - 1 - i; // 忽略前几张牌
            //    foreach (var c in CardPile) {
            //        if (--skip >= 0) { continue; }
            //        if (c.CardId == cardID[i]) {
            //            rec = c;
            //            break;
            //        }
            //    }
            //    if (rec != null) {
            //        CardPile.Remove(rec);
            //        CardPile.AddFirst(rec);
            //    }
            //}
            // 用于 UI 调试, 定制手牌 END
            DiscardPile.Clear(); // 清空弃牌堆
        }

        /// <summary>
        /// 摸牌
        /// </summary>
        /// <param name="number">摸牌数</param>
        /// <returns>摸上来的牌</returns>
        public Card[] DrawCards(int number)
        {
            if (number < 1) throw new ArgumentOutOfRangeException("number should be greater than 0.");
            if (CardPile.Count + DiscardPile.Count < number) throw new TieExceptions();

            if (CardPile.Count < number) ShuffleCards();

            Card[] res = new Card[number];

            for(int i = 0; i < number; i++)
            {
                res[i] = CardPile.First.Value;
                CardPile.RemoveFirst();
            }

            return res;
        }

        /// <summary>
        /// 摸一张牌
        /// </summary>
        /// <returns>摸上来的爬</returns>
        public Card DrawOneCard()
        {
            return DrawCards(1)[0];
        }

        /// <summary>
        /// 将一张牌置入弃牌堆
        /// </summary>
        /// <param name="card">被置入的牌</param>
        public void Move2DiscardPile(Card card)
        {
            DiscardPile.AddLast(card);
        }
    }
}
