using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    /// <summary>
    /// A class that is safe and fair for both players to see. A player should be able to
    /// see the other player's lifepoints, number of cards, cards in play, etc. So this class
    /// let's them see those stats without giving them access to the player's hand or secret info.
    /// </summary>
    public sealed class ReadOnlyPlayer
    {
        /// <summary>
        /// Ie YuGhi
        /// </summary>
        private string userName = "";

        /// <summary>
        /// Ie YuGhi
        /// </summary>
        public string getUserName()
        {
            return userName;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to setTheUsername. This is done by the Game class.
        /// </summary>
        internal void setUserName(string toSet)
        {
            userName = toSet;
        }

        /// <summary>
        /// If this hits 0 the player loses.
        /// </summary>
        private int lifePoints = 8000;

        /// <summary>
        /// If this hits 0 the player loses.
        /// </summary>
        public int getLifePoints()
        {
            return lifePoints ;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set the lifepoints. This is done by the Game class.
        /// </summary>
        internal void setLifePoints(int toSet)
        {
            lifePoints = toSet;
        }

        /// <summary>
        /// The more cards the opponent has the more chances they have to beat you.
        /// </summary>
        private int numberOfCardsInHand = 5;

        /// <summary>
        /// The more cards the opponent has the more chances they have to beat you.
        /// </summary>
        public int getCardsInHand()
        {
            return numberOfCardsInHand;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set the number of cards in hand. This is done by the Game class.
        /// </summary>
        internal void setCardsInHand(int toSet)
        {
            numberOfCardsInHand = toSet;
        }

        /// <summary>
        /// The number of facedown monster cards played by opponent currently in AttackMode
        /// </summary>
        private int numberOfFaceDownCardsInMonsterZoneInAttackMode = 0;

        /// <summary>
        /// The number of monster cards played by opponent currently in AttackMode
        /// </summary>
        public int getNumberOfFaceDownCardsInMonsterZoneInAttackMode()
        {
            return numberOfFaceDownCardsInMonsterZoneInAttackMode;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setNumberOfFaceDownCardsInMonsterZoneInAttackMode(int toSet)
        {
            numberOfFaceDownCardsInMonsterZoneInAttackMode = toSet;
        }

        /// <summary>
        /// The number of facedown monster cards played by opponent currently in DefenseMode
        /// </summary>
        private int numberOfFaceDownCardsInMonsterZoneInDefenseMode = 0;

        /// <summary>
        /// The number of monster cards played by opponent currently in DefenseMode
        /// </summary>
        public int getNumberOfFaceDownCardsInMonsterZoneInDefenseMode()
        {
            return numberOfFaceDownCardsInMonsterZoneInDefenseMode;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setNumberOfFaceDownCardsInMonsterZoneInDefenseMode(int toSet)
        {
            numberOfFaceDownCardsInMonsterZoneInDefenseMode = toSet;
        }

        /// <summary>
        /// The cards in the monster zone which are face up and readable by everyone
        /// </summary>
        private List<MonsterCard> faceUpMonsters=new List<MonsterCard>();

        /// <summary>
        /// The cards in the monster zone which are face up and readable by everyone
        /// </summary>
        public IList<MonsterCard> getFaceUpMonstersInMonsterZone()
        {
            return faceUpMonsters;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setFaceUpMonstersInMonsterZone(IList<MonsterCard> toSet)
        {
            faceUpMonsters = toSet as List<MonsterCard>;
        }

        /// <summary>
        /// The cards in the Spell and Trap zone which are face up and readable by everyone
        /// </summary>
        private List<SpellAndTrapCard> faceUpSpellsAndTraps=new List<SpellAndTrapCard>();

        /// <summary>
        /// The cards in the Spell and Trap zone which are face up and readable by everyone
        /// </summary>
        public IList<SpellAndTrapCard> getFaceUpSpellsAndTraps()
        {
            return faceUpSpellsAndTraps;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setFaceUpSpellsAndTraps(IList<SpellAndTrapCard> toSet)
        {
            faceUpSpellsAndTraps = toSet as List<SpellAndTrapCard>;
        }

        /// <summary>
        /// A criteria for winning is whether or not the player can draw. If they run out of cards that's game.
        /// </summary>
        private bool ableToDraw = true;

        /// <summary>
        /// A criteria for winning is whether or not the player can draw. If they run out of cards that's game.
        /// </summary>
        public bool getAbleToDraw()
        {
            return ableToDraw;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setAbleToDraw(bool toSet)
        {
            ableToDraw = toSet;
        }

        /// <summary>
        /// Whether or not the player has lost yet.
        /// </summary>
        private bool lost = false;

        /// <summary>
        /// Whether or not the player has lost yet.
        /// </summary>
        public bool getLost()
        {
            return lost;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setLost(bool toSet)
        {
           lost = toSet;
        }

        /// <summary>
        /// Whether or not the player has won yet.
        /// </summary>
        private bool won = false;

        /// <summary>
        /// Whether or not the player has won yet.
        /// </summary>
        public bool getWon()
        {
            return won;
        }

        /// <summary>
        /// Function not visible to the public on purpose. Only the innards of this
        /// this code should be able to set this property. This is done by the Game class.
        /// </summary>
        internal void setWon(bool toSet)
        {
            won = toSet;
        }

    }

}
