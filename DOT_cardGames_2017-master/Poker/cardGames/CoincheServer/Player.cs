using System.Collections.Generic;
using System.Linq;

namespace CoincheServer
{
    class Player
    {
        public Player(int pn, string channel)
        {
            PlayerNbr = pn;
            ChannelId = channel;
            Hand = new List<Card>();
            Coin = 1500;
            HasPassed = false;
            Lost = false;
        }

        public int Coin { get; set; }

        public int PlayerNbr { get; set; }

        public string ChannelId { get; set; }

        public List<Card> Hand { get; set; }

        public bool HasPassed { get; set; }

        public bool Lost { get; set; }


        public void AddCard(Card c)
        {
           Hand.Add(c);
        }

        public void ClearHand()
        {
            Hand.Clear();
        }

        public string RetHand()
        {
            var cards = Hand.Aggregate("", (current, c) => current + (c.Type.ToString() + c.Number.ToString() + " "));

            return (cards);
        }
    }
}
