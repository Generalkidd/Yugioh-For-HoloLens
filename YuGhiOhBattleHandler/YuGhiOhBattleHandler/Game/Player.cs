using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace YuGhiOhBattleHandler
{

    internal enum Zone
    {
        Hand,
        Graveyard,
        Monster,
        SpellTrap
    }

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

        /// <summary>
        /// Where monsters/spells/traps go when they die.
        /// </summary>
        private List<Object> graveYard = new List<object>();

        /// <summary>
        /// Should only be called by the Game class after RequestNormalSummon is called. The Game class checks whether this function is allowable.
        /// </summary>
        /// <param name="toPlay">The card which is to be summoned</param>
        internal void addFaceDownToMonsterZone(Object toPlay)
        {
            faceDownCardsInMonsterZone.Add(toPlay as MonsterCard);
            m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInAttackMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInAttackMode() + 1);
            hand.Remove(toPlay);
            m_meReadOnly.setCardsInHand(m_meReadOnly.getCardsInHand() - 1);
            (toPlay as MonsterCard).changeIsPlayed();
        }

        internal void switchFaceDownToFaceUp(MonsterCard toSwitch)
        {
            if(faceDownCardsInMonsterZone.Contains(toSwitch))
            {
                faceDownCardsInMonsterZone.Remove(toSwitch);
                toSwitch.setCanAttack(false);
                IList<MonsterCard> toSet = m_meReadOnly.getFaceUpMonstersInMonsterZone();
                toSet.Add(toSwitch);
                m_meReadOnly.setFaceUpMonstersInMonsterZone(toSet);
                if (toSwitch.getBattlePosition() == Mode.Attack)
                {
                    m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInAttackMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInAttackMode() - 1);
                }
                else
                {
                    m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInDefenseMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInDefenseMode() - 1);
                }
            }
            else if(m_meReadOnly.getFaceUpMonstersInMonsterZone().Contains(toSwitch))
            {
                IList<MonsterCard> toSet = m_meReadOnly.getFaceUpMonstersInMonsterZone();
                int index = toSet.IndexOf(toSwitch);
                toSwitch.setCanAttack(false);
                toSet[index] = toSwitch;
                m_meReadOnly.setFaceUpMonstersInMonsterZone(toSet);
            }
        }

        /// <summary>
        /// This is how the Front End grabs the data to print it to the screen. The Opponent should not be able to access this function although it is public
        /// because they should never have a handle to the their opponent's Player class.
        /// </summary>
        /// <returns>A list of the face down monsters that have been summoned and are in this players monster zone.</returns>
        public IList<MonsterCard> getFaceDownCardsInMonsterZone()
        {
            return faceDownCardsInMonsterZone;
        }

        /// <summary>
        /// This is how the Front End grabs the data to print it to the screen. The Opponent should be able to access this function through ReadOnlyPlayer.getFaceUpMonsters().
        /// </summary>
        /// <returns>A list of the face up monsters that have been summoned and are in this players monster zone.</returns>
        public IList<MonsterCard> getFaceUpMonstersInMonsterZone()
        {
            return m_meReadOnly.getFaceUpMonstersInMonsterZone();
        }

        /// <summary>
        /// The facedown spell and trap cards the player has played. The face up cards are in m_meReadOnly.
        /// </summary>
        private List<SpellAndTrapCard> faceDownCardsInSpellAndTrapZone=new List<SpellAndTrapCard>();

        /// <summary>
        /// This is how the Front End grabs the data to print it to the screen. The Opponent should not be able to access this function although it is public
        /// because they should never have a handle to the their opponent's Player class.
        /// </summary>
        /// <returns>A list of the face down spells and traps that have been placed in this players spell and trap zone.</returns>
        public IList<SpellAndTrapCard> getFaceDownCardsInSpellAndTrapZone()
        {
            return faceDownCardsInSpellAndTrapZone;
        }

        /// <summary>
        /// This is how the Front End grabs the data to print it to the screen. The Opponent should be able to access this function through ReadOnlyPlayer.getFaceUpSpellsAndTraps().
        /// </summary>
        /// <returns>A list of the face up spells and traps that have been placed in this players spell and trap zone.</returns>
        public IList<SpellAndTrapCard> getFaceUpCardsInSpellAndTrapZone()
        {
            return m_meReadOnly.getFaceUpSpellsAndTraps();
        }

        /// <summary>
        /// Says whether or not the decks have already been set.
        /// </summary>
        private bool hasDecks = false;

        /// <summary>
        /// Game this player is currently in. Should be set by the Game Class itself.
        /// </summary>
        private Game myCurrentGame;


        /// <summary>
        /// Purposely private... cannot initiate a player with no data.
        /// </summary>
        private Player()
        {

        }

        /// <summary>
        /// Posts a Windows 10 Toast Notification saying "Its Your Turn!" Called when someone ends their turn.
        /// </summary>
        internal void NotifyOfYourTurn()
        {
            // template to load for showing Toast Notification
            var xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                                     "<visual>" +
                                       "<binding template =\"ToastGeneric\">" +
                                         "<text>Turn</text>" +
                                         "<text>" +
                                           "It's your turn!" +
                                         "</text>" +
                                       "</binding>" +
                                     "</visual>" +
                                   "</toast>";
            // load the template as XML document
            var xmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDocument.LoadXml(xmlToastTemplate);
            ToastNotification toast = new ToastNotification(xmlDocument);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        /// <summary>
        /// Posts a Windows 10 Toast Notification saying "Its Your Opponent's Turn!" Called when someone ends their turn.
        /// </summary>
        internal void NotifyOfOppTurn()
        {
            // template to load for showing Toast Notification
            var xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                                     "<visual>" +
                                       "<binding template =\"ToastGeneric\">" +
                                         "<text>Turn</text>" +
                                         "<text>" +
                                           "It's the opponent's turn!" +
                                         "</text>" +
                                       "</binding>" +
                                     "</visual>" +
                                   "</toast>";
            // load the template as XML document
            var xmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDocument.LoadXml(xmlToastTemplate);
            ToastNotification toast = new ToastNotification(xmlDocument);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        internal void SendToGraveYard(Object c, Zone z)
        {
            if (z == Zone.Hand)
            {
                if (hand.Contains(c))
                {
                    hand.Remove(c);
                    m_meReadOnly.setCardsInHand(m_meReadOnly.getCardsInHand() - 1);
                }
            }
            else if(z==Zone.Monster)
            {
                if(faceDownCardsInMonsterZone.Contains(c as MonsterCard))
                {
                    faceDownCardsInMonsterZone.Remove(c as MonsterCard);
                    if ((c as MonsterCard).getBattlePosition() == Mode.Attack)
                    {
                        m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInAttackMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInAttackMode() - 1);
                    }
                    else if((c as MonsterCard).getBattlePosition() == Mode.Defense)
                    {
                        m_meReadOnly.setNumberOfFaceDownCardsInMonsterZoneInDefenseMode(m_meReadOnly.getNumberOfFaceDownCardsInMonsterZoneInDefenseMode() - 1);
                    }
                }
                else if(m_meReadOnly.getFaceUpMonstersInMonsterZone().Contains(c as MonsterCard))
                {
                    IList<MonsterCard> toRemoveFrom = m_meReadOnly.getFaceUpMonstersInMonsterZone();
                    toRemoveFrom.Remove(c as MonsterCard);
                    m_meReadOnly.setFaceUpMonstersInMonsterZone(toRemoveFrom);
                }
            }
            else if(z==Zone.SpellTrap)
            {
                if (faceDownCardsInSpellAndTrapZone.Contains(c as SpellAndTrapCard))
                {
                    faceDownCardsInSpellAndTrapZone.Remove(c as SpellAndTrapCard);
                    m_meReadOnly.setNumberOfFaceDownSpellsAndTraps(m_meReadOnly.getNumberOfFaceDownSpellsAndTraps() - 1);
                }
                else if (m_meReadOnly.getFaceUpSpellsAndTraps().Contains(c as SpellAndTrapCard))
                {
                    IList<SpellAndTrapCard> toRemoveFrom = m_meReadOnly.getFaceUpSpellsAndTraps();
                    toRemoveFrom.Remove(c as SpellAndTrapCard);
                    m_meReadOnly.setFaceUpSpellsAndTraps(toRemoveFrom);
                }
            }
            graveYard.Add(c);

        }

        /// <summary>
        /// Gets a safe handle to the opponent to access their Face Up Cards, Number of FaceDown Cards, Etc.
        /// </summary>
        /// <returns>A ReadOnlyPlayer instance of the opponent</returns>
        public ReadOnlyPlayer getOpponent()
        {
            return myCurrentGame.otherPlayer(id);
        }

        /// <summary>
        /// Tell the game and opponent you are done for this turn. Should be called after you attack, summon, etc.
        /// </summary>
        /// <returns>"" if successfully ended turn. Error string if failed.</returns>
        public string EndTurn()
        {
            Game.Result r = myCurrentGame.RequestEndTurn(id);
            if(r==Game.Result.Success)
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }

        public int getLifePoints()
        {
            return m_meReadOnly.getLifePoints();
        }

        internal void setLifePoints(int points)
        {
            m_meReadOnly.setLifePoints(points);
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

        /// <summary>
        /// Not sure why the user would need to access the current game, so this may be turned to internal for safety reasons.
        /// </summary>
        /// <returns>the game this user is currently in</returns>
        public Game getCurrentGame()
        {
            return myCurrentGame;
        }

        /// <summary>
        /// Did you forget your name? Sure you can grab it.
        /// </summary>
        /// <returns>this users name</returns>
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


        /// <summary>
        /// This is how the Front End grabs the data to print it to the screen. Although it is public, the Opponent should not be able to access this function because they won't have a handle to their
        /// Opponent's Player Class.
        /// </summary>
        /// <returns>A List of Cards playable by this user.</returns>
        public IList<Object> getHand()
        {
            return hand;
        }

        /// <summary>
        /// A request to normal summon the monster is given to the game class. If allowable, it will be summoned.
        /// </summary>
        /// <param name="monsterToSummon">A MonsterCard to play.</param>
        /// <returns>"" if successful. Error string if failed.</returns>
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
        /// A request to cast spell or set trap is given to the game class. If allowable, it will be used.
        /// </summary>
        /// <param name="spellOrTrapToPlay">A SpellAndTrapCard to play.</param>
        /// <returns>"" if successful. Error string if failed.</returns>
        public string CastSpellOrTrap(Object spellOrTrapToPlay)
        {
            if (spellOrTrapToPlay is SpellAndTrapCard && hand.Contains(spellOrTrapToPlay))
            {
                SpellAndTrapCard stc = spellOrTrapToPlay as SpellAndTrapCard;
                Game.Result amIAllowedToSummon;
                if (stc.getName().Equals("Dark Hole"))
                {
                    amIAllowedToSummon = myCurrentGame.RequestDarkHole(id);
                    if(amIAllowedToSummon==Game.Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if(stc.getName().Equals("Red Medicine"))
                {
                    amIAllowedToSummon = myCurrentGame.RequestRedMedicine(id);
                    if (amIAllowedToSummon == Game.Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.getName().Equals("Sparks"))
                {
                    amIAllowedToSummon = myCurrentGame.RequestSparks(id);
                    if (amIAllowedToSummon == Game.Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.getName().Equals("Fissure"))
                {
                    amIAllowedToSummon = myCurrentGame.RequestFissure(id);
                    if (amIAllowedToSummon == Game.Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if(stc.getName().Equals("Trap Hole"))
                {
                    amIAllowedToSummon = myCurrentGame.RequestTrapHole(id);
                    if (amIAllowedToSummon == Game.Result.Success)
                    {
                        faceDownCardsInSpellAndTrapZone.Add(spellOrTrapToPlay as SpellAndTrapCard);
                        m_meReadOnly.setNumberOfFaceDownSpellsAndTraps(faceDownCardsInSpellAndTrapZone.Count);
                        hand.Remove(spellOrTrapToPlay);
                        m_meReadOnly.setCardsInHand(m_meReadOnly.getCardsInHand() - 1);
                    }
                }
                else
                {
                    amIAllowedToSummon = Game.Result.InvalidMove;
                }
                if (amIAllowedToSummon.ToString().Equals("Success"))
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

        public string TryEquip(Object EquipableCard, string nameOfCardToEquipTo)
        {
            bool found = false;
            if(hand.Contains(EquipableCard))
            {
                for(int i=0; i<faceDownCardsInMonsterZone.Count && !found; i++)
                {
                    if(faceDownCardsInMonsterZone[i].getName().Equals(nameOfCardToEquipTo))
                    {
                        MonsterCard equippingTo = faceDownCardsInMonsterZone[i];
                        found = true;
                        Game.Result r=myCurrentGame.RequestEquip(id, EquipableCard, ref equippingTo);
                        if (r == Game.Result.Success)
                        {
                            faceDownCardsInMonsterZone[i] = equippingTo;
                            SendToGraveYard(EquipableCard, Zone.Hand);
                            return "";
                        }
                        else
                        {
                            return r.ToString();
                        }
                        
                    }
                }
                IList<MonsterCard> faceUpMonsters = m_meReadOnly.getFaceUpMonstersInMonsterZone();
                for (int i = 0; i < faceUpMonsters.Count && !found; i++)
                {
                    if (faceUpMonsters[i].getName().Equals(nameOfCardToEquipTo))
                    {
                        found = true;
                        MonsterCard equippingTo = faceUpMonsters[i];
                        Game.Result r = myCurrentGame.RequestEquip(id, EquipableCard, ref equippingTo);
                        if (r == Game.Result.Success)
                        {
                            faceUpMonsters[i] = equippingTo;
                            m_meReadOnly.setFaceUpMonstersInMonsterZone(faceUpMonsters);
                            SendToGraveYard(EquipableCard, Zone.Hand);
                            return "";
                        }
                        else
                        {
                            return r.ToString();
                        }
                    }
                }
                return "Failed to find card";
            }
            else
            {
                return "Equipable Card not in hand anymore.";
            }
        }

        public string AttackLifePoints(MonsterCard attackingCard)
        {
            Game.Result r = myCurrentGame.RequestAttackLifePoints(id, attackingCard);
            if (r.Equals(Game.Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }
        
        public string AttackFaceDownOpponent(MonsterCard attackingCard, Mode faceDownCardsMode)
        {
            Game.Result r= myCurrentGame.RequestAttackOnFaceDownCard(id, attackingCard, faceDownCardsMode);
            if(r.Equals(Game.Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }

        public string AttackFaceUpOpponent(MonsterCard attackingCard, MonsterCard defendingCard)
        {
            Game.Result r =myCurrentGame.RequestAttack(id, attackingCard, defendingCard);
            if (r.Equals(Game.Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
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
                m_meReadOnly.setCardsInHand(m_meReadOnly.getCardsInHand() + 1);
            }
        }

        /// <summary>
        /// Called Each Turn to add One card to the hand.
        /// </summary>
        internal void drawCard()
        {
            Object cardOnTopOfDeck = m_mainDeck.drawTopCard();
            hand.Add(cardOnTopOfDeck);
            m_meReadOnly.setCardsInHand(m_meReadOnly.getCardsInHand() + 1);
        }

        /// <summary>
        /// Shuffle Main Deck, Side Deck, and Extra Deck. Called at the beginning of the game.
        /// </summary>
        internal void shuffleAllDecks()
        {
            shuffleMainDeck();
            shuffleSideDeck();
            ShuffleExtraDeck();
        }

        internal void allowMonstersToAttack()
        {
            for(int i=0; i<faceDownCardsInMonsterZone.Count; i++)
            {
                faceDownCardsInMonsterZone[i].setCanAttack(true);
            }
            List<MonsterCard> faceUp = m_meReadOnly.getFaceUpMonstersInMonsterZone() as List<MonsterCard>;
            for(int i=0; i<faceUp.Count; i++)
            {
                faceUp[i].setCanAttack(true);
            }
            m_meReadOnly.setFaceUpMonstersInMonsterZone(faceUp);
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

        public object getField()
        {
            throw new NotImplementedException();
        }
    }
}
