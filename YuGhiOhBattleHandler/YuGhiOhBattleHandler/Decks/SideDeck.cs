using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*         Side Deck base class created by Seth Kitchen --- sjkyv5@mst.edu*/
    /// <summary>
    ///    This is a separate Deck of cards you can use to change your Deck during 
    ///    a Match. After each Duel in a Match, you can swap any card from 
    ///    your Side Deck with a card from your side Deck and/or Extra Deck to                   
     ///   customize your strategy against your opponent. The number of cards in 
     ///   your Side Decks must not exceed 15. The number of cards in your Side 
     ///   Deck before and after you swap any cards must be exactly the same.
     /// </summary>

    public sealed class SideDeck
    {

        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private static Random _random = new Random();

        public static int MinimumNumberOfCardsInSideDeck
        {
            get { return 0; }
        }

        public static int MaximumNumberOfCardsInSideDeck
        {
            get { return 15; }
        }

        private static List<Object> cards
        {
            get; set;
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

        internal void clear()
        {
            cards.Clear();
        }

        public SideDeck()
        {
            cards = new List<Object>();
        }
    }
}
