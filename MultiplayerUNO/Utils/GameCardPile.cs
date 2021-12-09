using System;
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

        public GameCardPile()
        {
            for (int i = 0; i < 108; i++)
                CardPile.AddLast(new Card(i));
        }

        public void ShuffleCards()
        {
            CardPile = new LinkedList<Card>(CardPile.Concat(DiscardPile).OrderBy(p => Guid.NewGuid().ToString()));
            DiscardPile.Clear();
        }

        public Card[] DrawCards(int number)
        {
            if (number < 1) throw new ArgumentOutOfRangeException("number should be greater than 0.");
            if (CardPile.Count + DiscardPile.Count < number) throw new TileExceptions();

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
