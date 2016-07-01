using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          Dice base class created by Seth Kitchen --- sjkyv5@mst.edu */
    /// <summary>
    ///      
    ///     Just like the coin, there are some 
    ///     cards that need a die roll.If you play
    ///     with these, have a standard die ready
    ///     with numbers 1 through 6.
    ///
    /// </summary>
    public sealed class Dice
    {
        Random r = new Random();

        /// <summary>
        /// Will figurative roll a dice and return 1-6.
        /// </summary>
        public int roll()
        {
            return r.Next(5)+1;
        }
    }
}
