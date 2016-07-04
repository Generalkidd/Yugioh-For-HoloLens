using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace YuGhiOhBattleHandler
{
    /// <summary>
    /// A class that only one person should have access to which allows a user to make moves and see
    /// their own cards. The network manager should assign a private id to each player class and give
    /// it to the corresponding user. The user can then access the class using Game.myPlayer(myId);
    /// </summary>
    public sealed class Player
    {
        /// <summary>
        /// A variable which makes each player unique and accessible only by one user.
        /// </summary>
        internal readonly int id;

        /// <summary>
        /// Stores the safe part of a player which should be accessible to both duelers.
        /// </summary>
        private ReadOnlyPlayer m_meReadOnly;

        /// <summary>
        /// SubDeck which the player can sub cards from between duels.
        /// </summary>
        private SideDeck m_sideDeck;

        /// <summary>
        /// The 40-60 cards the player is dueling with.
        /// </summary>
        private Deck m_mainDeck;

        /// <summary>
        /// The XYZ, Synchro, and Fusion Monsters the Player is using.
        /// </summary>
        private ExtraDeck m_extraDeck;

        /// <summary>
        /// The cards eligible to be played.
        /// </summary>
        private List<Object> hand=new List<object>();

        /// <summary>
        /// The facedown monster cards the player has played. The face up cards are in m_meReadOnly.
        /// </summary>
        private List<MonsterCard> faceDownCardsInMonsterZone=new List<MonsterCard>();

        internal void addFaceDownToMonsterZone(Object toPlay)
        {
            faceDownCardsInMonsterZone.Add(toPlay as MonsterCard);
            m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInAttackMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInAttackMode() + 1);
            hand.Remove(toPlay);
            (toPlay as MonsterCard).changeIsPlayed();
        }

        public IList<MonsterCard> getFaceDownCardsInMonsterZone()
        {
            return faceDownCardsInMonsterZone;
        }

        public IList<MonsterCard> getFaceUpMonstersInMonsterZone()
        {
            return m_meReadOnly.getFaceUpMonstersInMonsterZone();
        }

        /// <summary>
        /// The facedown spell and trap cards the player has played. The face up cards are in m_meReadOnly.
        /// </summary>
        private List<SpellAndTrapCard> faceDownCardsInSpellAndTrapZone=new List<SpellAndTrapCard>();

        /// <summary>
        /// Says whether or not the decks have already been set.
        /// </summary>
        private bool hasDecks = false;

        private Game myCurrentGame;


        /// <summary>
        /// Purposely private... cannot initiate a player with no data.
        /// </summary>
        private Player()
        {

        }

        /// <summary>
        /// Only Allowable Constructor. Should be Called By Network Manager who will assign a random
        /// userId and only give it to the Game and the corresponding user.
        /// </summary>
        /// <param name="userId">Random Number which makes this player unique.</param>
        /// <param name="userName">What the Player should be called.</param>
        public Player(int userId, string userName)
        {
            id = userId;
            m_meReadOnly = new ReadOnlyPlayer();
            m_meReadOnly.setUserName(userName);
        }
        
        /// <summary>
        /// Set in the Game Constructor.
        /// </summary>
        /// <param name="g">the current game the player is in</param>
        internal void SetCurrentGame(Game g)
        {
            myCurrentGame = g;
        }

        public Game getCurrentGame()
        {
            return myCurrentGame;
        }

        public string getUsername()
        {
            return m_meReadOnly.getUserName();
        }

        /// <summary>
        /// Gives a safe version of player without secrets
        /// </summary>
        /// <returns>the safe version of this player</returns>
        public ReadOnlyPlayer ReadOnlyPlayer()
        {
            return m_meReadOnly;
        }



        public IList<Object> getHand()
        {
            return hand;
        }

        public string NormalSummon(Object monsterToSummon)
        {
            if (monsterToSummon is MonsterCard && hand.Contains(monsterToSummon))
            {
                Game.Result amIAllowedToSummon=myCurrentGame.RequestNormalSummon(id, monsterToSummon,faceDownCardsInMonsterZone.Count+m_meReadOnly.getFaceUpMonstersInMonsterZone().Count);
                if(amIAllowedToSummon.ToString().Equals("Success"))
                {
                    return "";
                }
                else
                {
                    return amIAllowedToSummon.ToString();
                }
            }
            else
            {
                return "Either Card is not a monster or the card is not in your hand!";
            }
        }

        /// <summary>
        /// Set the usable decks for the duel(s). Only callable by innards of this code. The Game will
        /// assign decks based on what was selected in the deck creator.
        /// </summary>
        /// <param name="userName">What the Player should be called.</param>
        /// <param name="mainDeck">What the Player will be dueling majorily with</param>
        /// <param name="sideDeck">What the Player is using for tradeouts</param>
        /// <param name="extraDeck">What the Player is using for xyz,synchro,and fusion</param>
        internal bool setDecks(Deck mainDeck, SideDeck sideDeck, ExtraDeck extraDeck)
        {
            if (!hasDecks)
            {
                m_mainDeck = mainDeck;
                m_sideDeck = sideDeck;
                m_extraDeck = extraDeck;
                hasDecks = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        
        /// <summary>
        /// Called at the start of the game to make each player's hand 5 cards.
        /// </summary>
        internal void draw5Cards()
        {
            for(int i=0; i<5; i++)
            {
                Object cardOnTopOfDeck = m_mainDeck.drawTopCard();
                hand.Add(cardOnTopOfDeck);
            }
        }

        /// <summary>
        /// Called Each Turn to add One card to the hand.
        /// </summary>
        internal void drawCard()
        {
            Object cardOnTopOfDeck = m_mainDeck.drawTopCard();
            hand.Add(cardOnTopOfDeck);
        }

        

        internal void shuffleAllDecks()
        {
            shuffleMainDeck();
            shuffleSideDeck();
            ShuffleExtraDeck();
        }

        internal void ShuffleExtraDeck()
        {
            m_extraDeck.ShuffleDeck();
        }

        private void shuffleSideDeck()
        {
            m_sideDeck.ShuffleDeck();
        }

        private void shuffleMainDeck()
        {
            m_mainDeck.ShuffleDeck();
        }

      
    }
}
