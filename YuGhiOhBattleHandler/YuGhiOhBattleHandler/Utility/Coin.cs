using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          Coin base class created by Seth Kitchen --- sjkyv5@mst.edu*/
    /// <summary>
    ///       
    ///     Some cards require a coin toss. If you 
    ///     play with these, have a coin ready to 
    ///     flip.
    /// </summary>
    public sealed class Coin
    {
        Random r = new Random();

        /// <summary>
        /// Will figurative flip a coin and return true for heads and false for tails.
        /// </summary>
        public bool flip()
        {
            int result=r.Next(1);
            if(result==0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
