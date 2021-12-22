﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MultiplayerUNO.Backend;

namespace MultiplayerUNO.Utils
{
    public class GameCardPile
    {
        private LinkedList<Card> CardPile = new LinkedList<Card>();
        private LinkedList<Card> DiscardPile = new LinkedList<Card>();

        public int CardPileLeft { get { return CardPile.Count; } }
        public int DiscardPileLeft { get { return DiscardPile.Count; } }

        public GameCardPile()
        {
            for (int i = 0; i < 108; i++)
                CardPile.AddLast(new Card(i));
        }

        public void ShuffleCards() {
            CardPile = new LinkedList<Card>(CardPile.Concat(DiscardPile).OrderBy(p => Guid.NewGuid().ToString()));
            // TODO 
            // 用于 UI 调试, 定制手牌 START
            int[] cardID = new int[] {
                //// +4
                //104,0,8,9,10,11,16,
                //12,13,14,15,20,24,28,
                //1,2,3,4,
                
                //// +2
                //104,0,88,89,10,11,16,
                //92,93,14,15,20,24,28,
                //1,2,3,4,

                //// 转
                96,97,1,2
            };
            for (int i = cardID.Length - 1; i >= 0; --i) {
                Card rec = null;
                int skip = cardID.Length - 1 - i; // 忽略前几张牌
                foreach (var c in CardPile) {
                    if (--skip >= 0) { continue; }
                    if (c.CardId == cardID[i]) {
                        rec = c;
                        break;
                    }
                }
                if (rec != null) {
                    CardPile.Remove(rec);
                    CardPile.AddFirst(rec);
                }
            }
            // 用于 UI 调试, 定制手牌 END
            DiscardPile.Clear();
        }

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

        public Card DrawOneCard()
        {
            return DrawCards(1)[0];
        }

        public void Move2DiscardPile(Card card)
        {
            DiscardPile.AddLast(card);
        }
    }
}
