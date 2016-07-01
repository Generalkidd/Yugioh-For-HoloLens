using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          Counter base class created by Seth Kitchen --- sjkyv5@mst.edu */ 
    /// <summary>
    ///      
    ///     Some cards will require markers 
    ///     (called counters) to keep track of 
    ///     things like the number of turns, or a 
    ///     card’s power level. You can use small 
    ///     objects like glass beads, paper clips, 
    ///     or pennies for the counters, which are 
    ///     then placed on these face-up cards.
    ///
    /// </summary>
    public sealed class Counter
    {
        private int count = 0;

        public void incrementCounter()
        {
            count++;
        }

        public void decrementCounter()
        {
            count--;
        }

        public int getCount()
        {
            return count;
        }
    }
}
