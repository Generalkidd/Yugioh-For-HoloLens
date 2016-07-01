using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          ExtraDeck base class created by Seth Kitchen --- sjkyv5@mst.edu */
    /// <summary>
    ///  This Deck consists of Xyz Monsters, Synchro Monsters and Fusion 
    ///  Monsters, which can be used during the game if you meet certain
    ///  requirements.The rules for Extra Decks are: 
    ///     -You can have up to 15 cards in the Extra Deck.
    ///     -The Extra Deck can contain Xyz Monsters, Synchro Monsters and
    ///      Fusion Monsters, in any combination.
    ///     -These cards are not counted towards the 40-card minimum limit \ 
    ///      of your Main Deck.
    /// </summary>
    public sealed class ExtraDeck
    {
        /// <summary>
        /// Used in Shuffle(T).
        /// </summary>
        private static Random _random = new Random();

        public static int MinimumNumberOfCardsInExtraDeck
        {
            get { return 0; }
        }

        public static int MaximumNumberOfCardsInExtraDeck
        {
            get { return 15; }
        }

        private List<Object> cards;

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

        internal void clear()
        {
            cards.Clear();
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
   
        public ExtraDeck()
        {
            cards = new List<Object>();
        }
    }
}
