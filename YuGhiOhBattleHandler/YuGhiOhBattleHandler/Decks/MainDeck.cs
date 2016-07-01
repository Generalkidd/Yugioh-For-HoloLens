using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          Deck base class created by Seth Kitchen --- sjkyv5@mst.edu */ 
    /// <summary>
    ///     
    ///      Assemble your favorite cards into a Deck that follows these rules: 
    ///           -The Deck must be 40 to 60 cards.
    ///           -You can only have up to 3 copies of the same card in your Deck, Extra Deck and Side Deck combined.
    ///      Also, some cards are Forbidden, Limited or Semi-Limited in official tournaments. (See page 45 for details.)
    ///    Try to keep your Deck close to the 40-card minimum. Having a Deck with too many cards makes it hard to draw your best cards when you need them. This Deck is called your Main Deck.
    /// </summary>
    public sealed class Deck:Object
    {
        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private static Random _random = new Random();

        public static int MinimumNumberOfCardsInDeck
        {
            get { return 40; }
        }

        public static int MaximumNumberOfCardsInDeck
        {
            get { return 60; }
        }

        private List<Object> cards;

        internal void clear()
        {
            cards.Clear();
        }

        /// <summary>
        /// Returns the top card and discards it from the deck.
        /// </summary>
        /// <returns></returns>
        internal Object drawTopCard()
        {
            Object toReturn = cards[0];
            cards.RemoveAt(0);
            return toReturn;
        }

        internal void Set(IList<Object> card)
        {
            cards = card as List<Object>;
        }

        internal void Add(Object c)
        {
            cards.Add(c);
        }

        internal void RemoveAt(int index)
        {
            cards.RemoveAt(index);
        }

        internal void ShuffleDeck()
        {
            Object[] toShuffle = cards.ToArray();
            Shuffle<Object>(toShuffle);
            cards = toShuffle.ToList<Object>();
        }

        /// <summary>
        /// Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        private static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(_random.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        public Deck()
        {
            cards = new List<Object>();
        }
    }
}
