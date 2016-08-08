using Assets.Scripts.BattleHandler.Cards;
using Assets.Scripts.BattleHandler.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BattleHandler.Game
{
    public class Player:MonoBehaviour
    {
        /// <summary>
        /// A variable which makes each player unique and accessible only by one user.
        /// </summary>
        internal int id;

        /// <summary>
        /// Stores the safe part of a player which should be accessible to both duelers.
        /// </summary>
        public ReadOnlyPlayer MeReadOnly
        {
            get; internal set;
        }

        internal GameManager myGm
        {
            get; set;
        }

        /// <summary>
        /// The 40-60 cards the player is dueling with.
        /// </summary>
        public MainDeck MainDeck
        {
            get; internal set;
        }

        /// <summary>
        /// The cards eligible to be played.
        /// </summary>
        public List<Cards.Card> Hand
        {
            get; internal set;
        }

        /// <summary>
        /// The facedown monster cards the player has played. The face up cards are in m_meReadOnly.
        /// </summary>
        public List<MonsterCard> FaceDownCardsInMonsterZone
        {
            get; internal set;
        }

        /// <summary>
        /// Where monsters/spells/traps go when they die.
        /// </summary>
        public List<Cards.Card> GraveYard
        {
            get; internal set;
        }

        public List<SpellAndTrapCard> FaceDownTraps
        {
            get; internal set;
        }

        public Game MyCurrentGame
        {
            get; internal set;
        }

        /// <summary>
        /// Purposely private... cannot initiate a player with no data.
        /// </summary>
        private Player()
        {

        }

        /// <summary>
        /// Should only be called by the Game class after RequestNormalSummon is called. The Game class checks whether this function is allowable.
        /// </summary>
        /// <param name="toPlay">The card which is to be summoned</param>
        internal void addFaceDownToMonsterZone(System.Object toPlay)
        {
            if ((toPlay as MonsterCard).Mode == Mode.Attack)
            {
                (toPlay as MonsterCard).ChangeBattlePosition();
            }
            FaceDownCardsInMonsterZone.Add(toPlay as MonsterCard);
            MeReadOnly.NumberOfFaceDownCardsInMonsterZone = MeReadOnly.NumberOfFaceDownCardsInMonsterZone + 1;
            Hand.Remove(toPlay as Cards.Card);
            MeReadOnly.NumberOfCardsInHand = MeReadOnly.NumberOfCardsInHand - 1;
        }

        internal void switchFaceDownToFaceUp(MonsterCard toSwitch)
        {
            if (FaceDownCardsInMonsterZone.Contains(toSwitch))
            {
                FaceDownCardsInMonsterZone.Remove(toSwitch);
                List<MonsterCard> toSet = MeReadOnly.FaceUpMonsters;
                toSet.Add(toSwitch);
                MeReadOnly.FaceUpMonsters = toSet;
                MeReadOnly.NumberOfFaceDownCardsInMonsterZone=MeReadOnly.NumberOfFaceDownCardsInMonsterZone - 1;
            }
        }

        internal void switchCanAttack(MonsterCard toSwitch)
        {
            List<MonsterCard> toSet = MeReadOnly.FaceUpMonsters;
            int index = toSet.IndexOf(toSwitch);
            toSwitch.CanAttack=false;
            toSet[index] = toSwitch;
            MeReadOnly.FaceUpMonsters=toSet;
        }

        internal void SendToHand(System.Object c, Zone z)
        {
            if (z == Zone.Graveyard)
            {
                if (GraveYard.Contains(c as Cards.Card))
                {
                    GraveYard.Remove(c as Cards.Card);
                }
            }
            else if (z == Zone.Monster)
            {
                if (FaceDownCardsInMonsterZone.Contains(c as MonsterCard))
                {
                    FaceDownCardsInMonsterZone.Remove(c as MonsterCard);
                    MeReadOnly.NumberOfFaceDownCardsInMonsterZone = MeReadOnly.NumberOfFaceDownCardsInMonsterZone - 1;
                }
                else if (MeReadOnly.FaceUpMonsters.Contains(c as MonsterCard))
                {
                    List<MonsterCard> toRemoveFrom = MeReadOnly.FaceUpMonsters;
                    toRemoveFrom.Remove(c as MonsterCard);
                    MeReadOnly.FaceUpMonsters = toRemoveFrom;
                }
            }
            else if (z == Zone.SpellTrap)
            {
                if (FaceDownTraps.Contains(c as SpellAndTrapCard))
                {
                    FaceDownTraps.Remove(c as SpellAndTrapCard);
                    MeReadOnly.NumberOfFaceDownTraps--;
                }
                else if (MeReadOnly.FaceUpTraps.Contains(c as SpellAndTrapCard))
                {
                    List<SpellAndTrapCard> toRemoveFrom = MeReadOnly.FaceUpTraps;
                    toRemoveFrom.Remove(c as SpellAndTrapCard);
                    MeReadOnly.FaceUpTraps = toRemoveFrom;
                }
            }
            Hand.Add(c as Cards.Card);
        }

        internal void SendToGraveYard(System.Object c, Zone z)
        {
            if (z == Zone.Hand)
            {
                if (Hand.Contains(c as Cards.Card))
                {
                    Hand.Remove(c as Cards.Card);
                    MeReadOnly.NumberOfCardsInHand=MeReadOnly.NumberOfCardsInHand - 1;
                }
            }
            else if (z == Zone.Monster)
            {
                if (FaceDownCardsInMonsterZone.Contains(c as MonsterCard))
                {
                    FaceDownCardsInMonsterZone.Remove(c as MonsterCard);
                    SpellAndTrapCard attachedTo = (c as MonsterCard).EquippedTo;
                    if(attachedTo!=null)
                    {
                        SendToGraveYard(attachedTo, Zone.SpellTrap);
                    }
                    MeReadOnly.NumberOfFaceDownCardsInMonsterZone=MeReadOnly.NumberOfFaceDownCardsInMonsterZone - 1;
                }
                else if (MeReadOnly.FaceUpMonsters.Contains(c as MonsterCard))
                {
                    List<MonsterCard> toRemoveFrom = MeReadOnly.FaceUpMonsters;
                    toRemoveFrom.Remove(c as MonsterCard);
                    SpellAndTrapCard attachedTo = (c as MonsterCard).EquippedTo;
                    if (attachedTo != null)
                    {
                        SendToGraveYard(attachedTo, Zone.SpellTrap);
                    }
                    MeReadOnly.FaceUpMonsters=toRemoveFrom;
                }
            }
            else if (z == Zone.SpellTrap)
            {
                if (FaceDownTraps.Contains(c as SpellAndTrapCard))
                {
                    FaceDownTraps.Remove(c as SpellAndTrapCard);
                    MeReadOnly.NumberOfFaceDownTraps--;
                }
                else if (MeReadOnly.FaceUpTraps.Contains(c as SpellAndTrapCard))
                {
                    List<SpellAndTrapCard> toRemoveFrom = MeReadOnly.FaceUpTraps;
                    toRemoveFrom.Remove(c as SpellAndTrapCard);
                    MeReadOnly.FaceUpTraps=toRemoveFrom;
                }
            }
            GraveYard.Add(c as Cards.Card);
        }

        /// <summary>
        /// Gets a safe handle to the opponent to access their Face Up Cards, Number of FaceDown Cards, Etc.
        /// </summary>
        /// <returns>A ReadOnlyPlayer instance of the opponent</returns>
        public ReadOnlyPlayer getOpponent()
        {
            return MyCurrentGame.otherPlayer(id);
        }

        /// <summary>
        /// Tell the game and opponent you are done for this turn. Should be called after you attack, summon, etc.
        /// </summary>
        /// <returns>"" if successfully ended turn. Error string if failed.</returns>
        public string EndTurn()
        {
            Result r = MyCurrentGame.RequestEndTurn(id);
            if (r == Result.Success)
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
            return MeReadOnly.LifePoints;
        }

        internal void setLifePoints(int points)
        {
            MeReadOnly.LifePoints=points;
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
            MeReadOnly = new ReadOnlyPlayer();
            MeReadOnly.UserName=userName;
            Hand = new List<Cards.Card>();
            GraveYard = new List<Cards.Card>();
            FaceDownTraps = new List<SpellAndTrapCard>();
            FaceDownCardsInMonsterZone = new List<MonsterCard>();
        }

        public static Player MakePlayer(GameObject toAddTo, int userId, string userName)
        {
            Player myPlayer = toAddTo.AddComponent<Player>();
            myPlayer.id = userId;
            myPlayer.MeReadOnly = new ReadOnlyPlayer();
            myPlayer.MeReadOnly.UserName = userName;
            myPlayer.Hand = new List<Cards.Card>();
            myPlayer.GraveYard = new List<Cards.Card>();
            myPlayer.FaceDownTraps = new List<SpellAndTrapCard>();
            myPlayer.FaceDownCardsInMonsterZone = new List<MonsterCard>();
            return myPlayer;
        }

        /// <summary>
        /// A request to normal summon the monster is given to the game class. If allowable, it will be summoned.
        /// </summary>
        /// <param name="monsterToSummon">A MonsterCard to play.</param>
        /// <returns>"" if successful. Error string if failed.</returns>
        public string NormalSummon(System.Object monsterToSummon)
        {

            if (monsterToSummon is MonsterCard && Hand.Contains(monsterToSummon as Cards.Card))
            {
                Debug.Log("ID=" + id);
                Debug.Log("game=" + MyCurrentGame);
                Debug.Log("MeReadOnly=" + MeReadOnly);
                Debug.Log("MonsterToSummon=" + monsterToSummon);
                Debug.Log("FacedownCards=" + FaceDownCardsInMonsterZone.Count);
                Debug.Log("FaceUpMonster=" + MeReadOnly.FaceUpMonsters.Count);
                Result amIAllowedToSummon = MyCurrentGame.RequestNormalSummon(id, monsterToSummon, FaceDownCardsInMonsterZone.Count + MeReadOnly.FaceUpMonsters.Count);
                if (amIAllowedToSummon.ToString().Equals("Success"))
                {
                    return "";
                }
                else
                {
                    return amIAllowedToSummon.ToString();
                }
            }
            else if(!(monsterToSummon is MonsterCard))
            {
                return "Card is not MonsterCard";
            }
            else
            {
                return "Card is no longer in your hand!---"+(monsterToSummon as Cards.Card).CardName;
            }
        }

        /// <summary>
        /// A request to cast spell or set trap is given to the game class. If allowable, it will be used.
        /// </summary>
        /// <param name="spellOrTrapToPlay">A SpellAndTrapCard to play.</param>
        /// <returns>"" if successful. Error string if failed.</returns>
        public string CastSpellOrTrap(System.Object spellOrTrapToPlay)
        {
            if (spellOrTrapToPlay is SpellAndTrapCard && Hand.Contains(spellOrTrapToPlay as Cards.Card))
            {
                SpellAndTrapCard stc = spellOrTrapToPlay as SpellAndTrapCard;
                Result amIAllowedToSummon;
                if (stc.CardName.Equals("Dark Hole"))
                {
                    amIAllowedToSummon = MyCurrentGame.RequestDarkHole(id);
                    if (amIAllowedToSummon == Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.CardName.Equals("Red Medicine"))
                {
                    amIAllowedToSummon = MyCurrentGame.RequestRedMedicine(id);
                    if (amIAllowedToSummon == Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.CardName.Equals("Sparks"))
                {
                    amIAllowedToSummon = MyCurrentGame.RequestSparks(id);
                    if (amIAllowedToSummon == Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.CardName.Equals("Fissure"))
                {
                    amIAllowedToSummon = MyCurrentGame.RequestFissure(id);
                    if (amIAllowedToSummon == Result.Success)
                    {
                        SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                    }
                }
                else if (stc.CardName.Equals("Trap Hole"))
                {
                    amIAllowedToSummon = MyCurrentGame.RequestTrapHole(id);
                    if (amIAllowedToSummon == Result.Success)
                    {
                        FaceDownTraps.Add(spellOrTrapToPlay as SpellAndTrapCard);
                        MeReadOnly.NumberOfFaceDownTraps=FaceDownTraps.Count;
                        Hand.Remove(spellOrTrapToPlay as Cards.Card);
                        MeReadOnly.NumberOfCardsInHand--;
                    }
                }
                else if(stc.CardName.Equals("Two-Pronged Attack"))
                {
                    MonsterCard myOne = myGm.PromptForOneOfMyMonstersOnField();
                    MonsterCard myTwo = myGm.PromptForOneOfMyMonstersOnField();
                    MonsterCard possibleTheirsCard = null;
                    int possibleTheirsIndex = -1;
                    myGm.PromptForOneOfOpponentsMonstersOnField(out possibleTheirsCard, out possibleTheirsIndex);
                    if(myOne!=myTwo && myOne!=null && myTwo!=null && possibleTheirsCard!=null)
                    {
                        amIAllowedToSummon = MyCurrentGame.RequestTwoProngedAttack(id, myOne, myTwo, possibleTheirsCard);
                        if(amIAllowedToSummon==Result.Success)
                        {
                            SendToGraveYard(spellOrTrapToPlay, Zone.Hand);
                        }
                    }
                    else
                    {
                        amIAllowedToSummon = Result.InvalidMove;
                    }
                }
                else if(stc.CardName.Equals("De-Spell"))
                {
                    SpellAndTrapCard possibleTheirsCard = null;
                    int possibleTheirsIndex = -1;
                    myGm.PromptForOneOfOpponentsSpellsOrTrapsOnField(out possibleTheirsCard,out possibleTheirsIndex);
                    if (possibleTheirsCard != null)
                    {
                        amIAllowedToSummon = MyCurrentGame.RequestDeSpell(id, possibleTheirsCard);
                    }
                    else if(possibleTheirsIndex!=-1)
                    {
                        amIAllowedToSummon = MyCurrentGame.RequestDeSpell(id, possibleTheirsIndex);
                    }
                    else
                    {
                        amIAllowedToSummon = Result.InvalidMove;
                    }
                }
                else
                {
                    amIAllowedToSummon = Result.InvalidMove;
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

        public string TryEquip(System.Object EquipableCard, string nameOfCardToEquipTo)
        {
            bool found = false;
            if (Hand.Contains(EquipableCard as Cards.Card))
            {
                for (int i = 0; i < FaceDownCardsInMonsterZone.Count && !found; i++)
                {
                    if (FaceDownCardsInMonsterZone[i].CardName.Equals(nameOfCardToEquipTo))
                    {
                        MonsterCard equippingTo = FaceDownCardsInMonsterZone[i];
                        found = true;
                        SpellAndTrapCard stc;
                        if(EquipableCard is SpellAndTrapCard)
                        {
                            stc = EquipableCard as SpellAndTrapCard;
                        }
                        else
                        {
                            return "Not a spell and trap card";
                        }
                        Result r = MyCurrentGame.RequestEquip(id, ref stc, ref equippingTo);
                        if (r == Result.Success)
                        {
                            FaceDownCardsInMonsterZone[i] = equippingTo;
                            MeReadOnly.FaceUpTraps.Add(stc);
                            Hand.Remove(EquipableCard as SpellAndTrapCard);
                            //SendToGraveYard(EquipableCard, Zone.Hand);
                            return "";
                        }
                        else
                        {
                            return r.ToString();
                        }

                    }
                }
                List<MonsterCard> faceUpMonsters = MeReadOnly.FaceUpMonsters;
                for (int i = 0; i < faceUpMonsters.Count && !found; i++)
                {
                    if (faceUpMonsters[i].CardName.Equals(nameOfCardToEquipTo))
                    {
                        found = true;
                        MonsterCard equippingTo = faceUpMonsters[i];
                        SpellAndTrapCard stc;
                        if (EquipableCard is SpellAndTrapCard)
                        {
                            stc = EquipableCard as SpellAndTrapCard;
                        }
                        else
                        {
                            return "Not a spell and trap card";
                        }
                        Result r = MyCurrentGame.RequestEquip(id, ref stc, ref equippingTo);
                        if (r == Result.Success)
                        {
                            faceUpMonsters[i] = equippingTo;
                            MeReadOnly.FaceUpMonsters=faceUpMonsters;
                            MeReadOnly.FaceUpTraps.Add(stc);
                            Hand.Remove(EquipableCard as SpellAndTrapCard);
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

        /// <summary>
        /// Called by the Game class to actually change the mode of the card (to attack mode or defense mode).
        /// </summary>
        /// <param name="toChangeModeOf">to change to attack mode or defense mode</param>
        internal void GameChangeModeOfCard(MonsterCard toChangeModeOf)
        {
            if (FaceDownCardsInMonsterZone.Contains(toChangeModeOf))
            {
                int index = FaceDownCardsInMonsterZone.IndexOf(toChangeModeOf);
                toChangeModeOf.ChangeBattlePosition();
                FaceDownCardsInMonsterZone[index] = toChangeModeOf;
            }
            else if (MeReadOnly.FaceUpMonsters.Contains(toChangeModeOf))
            {
                List<MonsterCard> faceUps = MeReadOnly.FaceUpMonsters;
                int index = faceUps.IndexOf(toChangeModeOf);
                toChangeModeOf.ChangeBattlePosition();
                faceUps[index] = toChangeModeOf;
                MeReadOnly.FaceUpMonsters=faceUps;
            }
        }

        /// <summary>
        /// Asks the game to change the mode of the card (to attack mode or defense mode).
        /// </summary>
        /// <param name="toChangeModeOf">to change to attack mode or defense mode</param>
        /// <returns>"" if successful. Error message if Error</returns>
        public string ChangeModeOfCard(MonsterCard toChangeModeOf)
        {
            Result r = MyCurrentGame.RequestChangeModeOfCard(id, toChangeModeOf);
            if (r.Equals(Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }

        public string AttackLifePoints(MonsterCard attackingCard)
        {
            Result r = MyCurrentGame.RequestAttackLifePoints(id, attackingCard);
            if (r.Equals(Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }

        public string AttackFaceDownOpponent(MonsterCard attackingCard)
        {
            Result r = MyCurrentGame.RequestAttackOnFaceDownCard(id, attackingCard);
            if (r.Equals(Result.Success))
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
            Result r = MyCurrentGame.RequestAttack(id, attackingCard, defendingCard);
            if (r.Equals(Result.Success))
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
        internal void setDecks(MainDeck mainDeck)
        {
            MainDeck = mainDeck;
        }

        /// <summary>
        /// Called at the start of the game to make each player's hand 5 cards.
        /// </summary>
        internal void draw5Cards()
        {
            for (int i = 0; i < 5; i++)
            {
                Cards.Card cardOnTopOfDeck = MainDeck.drawTopCard();
                Hand.Add(cardOnTopOfDeck);
                MeReadOnly.NumberOfCardsInHand=MeReadOnly.NumberOfCardsInHand + 1;
            }
        }

        /// <summary>
        /// Called Each Turn to add One card to the hand.
        /// </summary>
        internal void drawCard()
        {
            Cards.Card cardOnTopOfDeck = MainDeck.drawTopCard();
            Hand.Add(cardOnTopOfDeck as Cards.Card);
            MeReadOnly.NumberOfCardsInHand=MeReadOnly.NumberOfCardsInHand + 1;
        }

        /// <summary>
        /// Shuffle Main Deck, Side Deck, and Extra Deck. Called at the beginning of the game.
        /// </summary>
        internal void shuffleAllDecks(int randomSeed)
        {
            shuffleMainDeck(randomSeed);
            //shuffleSideDeck();
            //ShuffleExtraDeck();
        }

        internal void allowMonstersToAttack()
        {
            for (int i = 0; i < FaceDownCardsInMonsterZone.Count; i++)
            {
                FaceDownCardsInMonsterZone[i].CanAttack=true;
            }
            List<MonsterCard> faceUp = MeReadOnly.FaceUpMonsters;
            for (int i = 0; i < faceUp.Count; i++)
            {
                faceUp[i].CanAttack=true;
            }
            MeReadOnly.FaceUpMonsters=faceUp;
        }

        public string Sacrifice(MonsterCard toSacrifice)
        {
            Result r = MyCurrentGame.RequestSacrifice(id, toSacrifice);
            if (r.Equals(Result.Success))
            {
                return "";
            }
            else
            {
                return r.ToString();
            }
        }
    
        private void shuffleMainDeck(int randomSeed)
        {
            MainDeck.ShuffleDeck(randomSeed);
        }

    }
}
