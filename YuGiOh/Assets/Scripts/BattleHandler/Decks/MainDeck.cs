using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.BattleHandler.Decks
{
        public class MyRandom
        {
            //
            // Private Constants 
            //
            private const int MBIG = Int32.MaxValue;
            private const int MSEED = 161803398;
            private const int MZ = 0;


            //
            // Member Variables
            //
            private int inext;
            private int inextp;
            private int[] SeedArray = new int[56];

            //
            // Public Constants
            //

            //
            // Native Declarations
            //

            //
            // Constructors
            //

            public MyRandom()
              : this(Environment.TickCount)
            {
            }

            public MyRandom(int Seed)
            {
                int ii;
                int mj, mk;

                //Initialize our Seed array.
                //This algorithm comes from Numerical Recipes in C (2nd Ed.)
                int subtraction = (Seed == Int32.MinValue) ? Int32.MaxValue : Math.Abs(Seed);
                mj = MSEED - subtraction;
                SeedArray[55] = mj;
                mk = 1;
                for (int i = 1; i < 55; i++)
                {  //Apparently the range [1..55] is special (Knuth) and so we're wasting the 0'th position.
                    ii = (21 * i) % 55;
                    SeedArray[ii] = mk;
                    mk = mj - mk;
                    if (mk < 0) mk += MBIG;
                    mj = SeedArray[ii];
                }
                for (int k = 1; k < 5; k++)
                {
                    for (int i = 1; i < 56; i++)
                    {
                        SeedArray[i] -= SeedArray[1 + (i + 30) % 55];
                        if (SeedArray[i] < 0) SeedArray[i] += MBIG;
                    }
                }
                inext = 0;
                inextp = 21;
                Seed = 1;
            }

            //
            // Package Private Methods
            //

            /*====================================Sample====================================
            **Action: Return a new random number [0..1) and reSeed the Seed array.
            **Returns: A double [0..1)
            **Arguments: None
            **Exceptions: None
            ==============================================================================*/
            protected virtual double Sample()
            {
                //Including this division at the end gives us significantly improved
                //random number distribution.
                return (InternalSample() * (1.0 / MBIG));
            }

            private int InternalSample()
            {
                int retVal;
                int locINext = inext;
                int locINextp = inextp;

                if (++locINext >= 56) locINext = 1;
                if (++locINextp >= 56) locINextp = 1;

                retVal = SeedArray[locINext] - SeedArray[locINextp];

                if (retVal == MBIG) retVal--;
                if (retVal < 0) retVal += MBIG;

                SeedArray[locINext] = retVal;

                inext = locINext;
                inextp = locINextp;

                return retVal;
            }

            //
            // Public Instance Methods
            // 


            /*=====================================Next=====================================
            **Returns: An int [0..Int32.MaxValue)
            **Arguments: None
            **Exceptions: None.
            ==============================================================================*/
            public virtual int Next()
            {
                return InternalSample();
            }

            private double GetSampleForLargeRange()
            {
                // The distribution of double value returned by Sample 
                // is not distributed well enough for a large range.
                // If we use Sample for a range [Int32.MinValue..Int32.MaxValue)
                // We will end up getting even numbers only.

                int result = InternalSample();
                // Note we can't use addition here. The distribution will be bad if we do that.
                bool negative = (InternalSample() % 2 == 0) ? true : false;  // decide the sign based on second sample
                if (negative)
                {
                    result = -result;
                }
                double d = result;
                d += (Int32.MaxValue - 1); // get a number in range [0 .. 2 * Int32MaxValue - 1)
                d /= 2 * (uint)Int32.MaxValue - 1;
                return d;
            }


            /*=====================================Next=====================================
            **Returns: An int [minvalue..maxvalue)
            **Arguments: minValue -- the least legal value for the Random number.
            **           maxValue -- One greater than the greatest legal return value.
            **Exceptions: None.
            ==============================================================================*/
            public virtual int Next(int minValue, int maxValue)
            {
                if (minValue > maxValue)
                {
                   return -1;
                   // throw new ArgumentOutOfRangeException("minValue", Environment.GetResourceString("Argument_MinMaxValue", "minValue", "maxValue"));
                }
                //Contract.EndContractBlock();

                long range = (long)maxValue - minValue;
                if (range <= (long)Int32.MaxValue)
                {
                    return ((int)(Sample() * range) + minValue);
                }
                else
                {
                    return (int)((long)(GetSampleForLargeRange() * range) + minValue);
                }
            }


            /*=====================================Next=====================================
            **Returns: An int [0..maxValue)
            **Arguments: maxValue -- One more than the greatest legal return value.
            **Exceptions: None.
            ==============================================================================*/
            public virtual int Next(int maxValue)
            {
                if (maxValue < 0)
                {
                    //  throw new ArgumentOutOfRangeException("maxValue", Environment.GetResourceString("ArgumentOutOfRange_MustBePositive", "maxValue"));
                    return -1;
                }
                //Contract.EndContractBlock();
                return (int)(Sample() * maxValue);
            }


            /*=====================================Next=====================================
            **Returns: A double [0..1)
            **Arguments: None
            **Exceptions: None
            ==============================================================================*/
            public virtual double NextDouble()
            {
                return Sample();
            }


            /*==================================NextBytes===================================
            **Action:  Fills the byte array with random bytes [0..0x7f].  The entire array is filled.
            **Returns:Void
            **Arugments:  buffer -- the array to be filled.
            **Exceptions: None
            ==============================================================================*/
            public virtual void NextBytes(byte[] buffer)
            {
                if (buffer == null) throw new ArgumentNullException("buffer");
                //Contract.EndContractBlock();
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)(InternalSample() % (Byte.MaxValue + 1));
                }
            }
        }

    public class MainDeck
    {
        private static MyRandom _random;
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
            _random = new MyRandom(randomSeed);
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
