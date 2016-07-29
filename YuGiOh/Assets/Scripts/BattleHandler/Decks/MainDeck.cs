using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.BattleHandler.Decks
{
    public class MainDeck
    {
        private static Random _random;
        public List<Cards.Card> CardsInDeck
        {
            get; internal set;
        }

        internal void clear()
        {
            CardsInDeck.Clear();
        }

        /// <summary>
        /// Returns the top card and discards it from the deck.
        /// </summary>
        /// <returns></returns>
        internal Cards.Card drawTopCard()
        {
            Cards.Card toReturn = CardsInDeck[0];
            CardsInDeck.RemoveAt(0);
            return toReturn;
        }

        internal void ShuffleDeck(int randomSeed)
        {
            _random = new Random(randomSeed);
            Cards.Card[] toShuffle = CardsInDeck.ToArray();
            Shuffle<Cards.Card>(toShuffle);
            CardsInDeck = toShuffle.ToList<Cards.Card>();
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

        public MainDeck()
        {
            CardsInDeck = new List<Cards.Card>();
        }

    }
}
