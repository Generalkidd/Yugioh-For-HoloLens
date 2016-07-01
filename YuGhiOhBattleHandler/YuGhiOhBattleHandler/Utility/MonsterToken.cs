using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /*          MonsterToken base class created by Seth Kitchen --- sjkyv5@mst.edu */
    /// <summary>
    ///      
    ///     Tokens are used to represent 
    ///     monsters that can be created by card
    ///     effects.Any object used for a Token
    ///     needs to have two distinct ways to
    ///     place it that can clearly indicate the
    ///     monster’s battle position. (See page 
    ///     45 for details.)
    ///
    /// </summary>
    public sealed class MonsterToken
    {
        /// <summary>
        /// If True this Monster is in Attack Mode Else it is in Defense Mode.
        /// </summary>
        private bool isAttackMode=true;

        /// <summary>
        /// If this Monster is in Attack Mode this puts it in Defense Mode. Else, Put in Attack Mode.
        /// </summary>
        public void changeStance()
        {
            isAttackMode = !isAttackMode;
        }

        /// <summary>
        /// If True this Monster is in Attack Mode Else it is in Defense Mode.
        /// </summary>
        public bool getAttackMode()
        {
            return isAttackMode;
        }
    }
}
