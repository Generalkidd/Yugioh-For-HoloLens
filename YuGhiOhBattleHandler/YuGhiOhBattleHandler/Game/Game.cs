using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{
    public sealed class Game
    {
        private Player player1;
        private Player player2;
        private int gameID;
        private int playerWhosTurnItIs;
        private bool playerHasNormalSummonedThisTurn;
        private bool playerOneTrapHoleEnabled = false;
        private bool playerTwoTrapHoleEnabled = false;

        /// <summary>
        /// We can grab the otherPlayer's data using our own username, but we won't be able to do actions with it.
        /// </summary>
        /// <param name="myId">Same id used in myPlayer given by Network Manager</param>
        /// <returns></returns>
        public ReadOnlyPlayer otherPlayer(int myId)
        {
            if (player1.id != myId)
            {
                return player1.ReadOnlyPlayer();
            }
            else if (player2.id != myId)
            {
                return player2.ReadOnlyPlayer();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// We can grab our own player to do actions with.
        /// </summary>
        /// <param name="myId">Unique int given by Network Manager</param>
        /// <returns></returns>
        public Player myPlayer(int myId)
        {
            if (player1.id == myId)
            {
                return player1;
            }
            else if (player2.id == myId)
            {
                return player2;
            }
            else
            {
                return null;
            }
        }

        internal enum Result
        {
            Success,
            NotYourTurn,
            AlreadyNormalSummonedThisTurn,
            AlreadyPlayedMaxNumberOfMonsters,
            InvalidMove,
            IneligibleMonsterType
        }

        internal Result RequestNormalSummon(int idOfAttacker, Object cardToSummon, int numberOfMonstersAlreadyPlayed)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1 && !playerHasNormalSummonedThisTurn && numberOfMonstersAlreadyPlayed<6)
            {
                player1.addFaceDownToMonsterZone(cardToSummon);
                playerHasNormalSummonedThisTurn = true;
                return Result.Success;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if(!(numberOfMonstersAlreadyPlayed<6))
            {
                return Result.AlreadyPlayedMaxNumberOfMonsters;
            }
            else if(player1.id==idOfAttacker)
            {
                return Result.AlreadyNormalSummonedThisTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2 && !playerHasNormalSummonedThisTurn && numberOfMonstersAlreadyPlayed<6)
            {
                player2.addFaceDownToMonsterZone(cardToSummon);
                playerHasNormalSummonedThisTurn = true;
                return Result.Success;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs==1)
            {
                return Result.NotYourTurn;
            }
            else if(player2.id==idOfAttacker)
            {
                return Result.AlreadyNormalSummonedThisTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        public bool RequestSetPlayer1Deck(IList<Object> mainDeck, IList<Object> sideDeck, IList<Object> extraDeck)
        {
            Deck deck = new Deck();
            deck.Set(mainDeck);
            SideDeck sDeck = new SideDeck();
            sDeck.Set(sideDeck);
            ExtraDeck exDeck = new ExtraDeck();
            exDeck.Set(extraDeck);
            return player1.setDecks(deck, sDeck, exDeck);
        }

        internal Result RequestEndTurn(int idOfAttacker)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                playerWhosTurnItIs = 2;
                playerHasNormalSummonedThisTurn = false;
                player2.NotifyOfYourTurn();
                player2.drawCard();
                player1.NotifyOfOppTurn();
                return Result.Success;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                playerWhosTurnItIs = 1;
                playerHasNormalSummonedThisTurn = false;
                player1.NotifyOfYourTurn();
                player1.drawCard();
                player2.NotifyOfOppTurn();
                return Result.Success;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        public bool RequestSetPlayer2Deck(IList<Object> mainDeck, IList<Object> sideDeck, IList<Object> extraDeck)
        {
            Deck deck = new Deck();
            deck.Set(mainDeck);
            SideDeck sDeck = new SideDeck();
            sDeck.Set(sideDeck);
            ExtraDeck exDeck = new ExtraDeck();
            exDeck.Set(extraDeck);
            return player2.setDecks(deck, sDeck, exDeck);
        }

        public Game(Player p_player1, Player p_player2, int id)
        {
            player1 = p_player1;
            player2 = p_player2;
            gameID = id;
            playerWhosTurnItIs = 1;
            playerHasNormalSummonedThisTurn = false;
            player1.SetCurrentGame(this);
            player2.SetCurrentGame(this);
        }
        
        public void StartGame()
        {
            player1.shuffleAllDecks();
            player2.shuffleAllDecks();
            player1.draw5Cards();
            player2.draw5Cards();
        }

        internal Result RequestDarkHole(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                IList<MonsterCard> faceDownMonsters = player1.getFaceDownCardsInMonsterZone();
                IList<MonsterCard> faceUpMonsters = player1.getFaceUpMonstersInMonsterZone();
                while(faceDownMonsters.Count>0)
                { 
                    player1.SendToGraveYard(faceDownMonsters[0],Zone.Monster);
                    faceDownMonsters = player1.getFaceDownCardsInMonsterZone();
                }
                while(faceUpMonsters.Count>0)
                {
                    player1.SendToGraveYard(faceUpMonsters[0],Zone.Monster);
                    faceUpMonsters = player1.getFaceUpMonstersInMonsterZone();
                }
                faceDownMonsters = player2.getFaceDownCardsInMonsterZone();
                faceUpMonsters = player2.getFaceUpMonstersInMonsterZone();
                while (faceDownMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player2.getFaceDownCardsInMonsterZone();
                }
                while (faceUpMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player2.getFaceUpMonstersInMonsterZone();
                }
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                IList<MonsterCard> faceDownMonsters = player1.getFaceDownCardsInMonsterZone();
                IList<MonsterCard> faceUpMonsters = player1.getFaceUpMonstersInMonsterZone();
                while (faceDownMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player1.getFaceDownCardsInMonsterZone();
                }
                while (faceUpMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player1.getFaceUpMonstersInMonsterZone();
                }
                faceDownMonsters = player2.getFaceDownCardsInMonsterZone();
                faceUpMonsters = player2.getFaceUpMonstersInMonsterZone();
                while (faceDownMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player2.getFaceDownCardsInMonsterZone();
                }
                while (faceUpMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player2.getFaceUpMonstersInMonsterZone();
                }
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        internal Result RequestRedMedicine(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                player1.setLifePoints(player1.getLifePoints() + 500);
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                player2.setLifePoints(player2.getLifePoints() + 500);
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        internal Result RequestSparks(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                player2.setLifePoints(player2.getLifePoints() - 500);
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                player1.setLifePoints(player1.getLifePoints() - 500);
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        internal Result RequestFissure(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                int lowestAttack = int.MaxValue;
                MonsterCard toDestroy=null;
                IList<MonsterCard> faceUpMonsters = player2.getFaceUpMonstersInMonsterZone();
                foreach (MonsterCard c in faceUpMonsters)
                {
                    if(c.getAttackPoints()<lowestAttack)
                    {
                        lowestAttack = c.getAttackPoints();
                        toDestroy = c;
                    }
                }
                if(toDestroy!=null)
                {
                    player2.SendToGraveYard(toDestroy, Zone.Monster);
                }
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                int lowestAttack = int.MaxValue;
                MonsterCard toDestroy = null;
                IList<MonsterCard> faceUpMonsters = player1.getFaceUpMonstersInMonsterZone();
                foreach (MonsterCard c in faceUpMonsters)
                {
                    if (c.getAttackPoints() < lowestAttack)
                    {
                        lowestAttack = c.getAttackPoints();
                        toDestroy = c;
                    }
                }
                if (toDestroy != null)
                {
                    player1.SendToGraveYard(toDestroy, Zone.Monster);
                }
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        internal Result RequestTrapHole(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                playerOneTrapHoleEnabled = true;
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                playerTwoTrapHoleEnabled = true;
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        internal Result RequestEquip(int id, object equipableCard, ref MonsterCard monsterCard)
        {
            if (player1.id == id && playerWhosTurnItIs == 1 && equipableCard is SpellAndTrapCard)
            {
                SpellAndTrapCard stc = equipableCard as SpellAndTrapCard;
                if(stc.getName()== "Legendary Sword")
                {
                    if(monsterCard.getYuGhiOhType().ToUpper().Contains("WARRIOR"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if(stc.getName() == "Beast Fangs")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("BEAST"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Violet Crystal")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("ZOMBIE"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Book of Secret Arts")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("SPELLCASTER"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Power of Kaishin")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("AQUA"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else
                {
                    return Result.InvalidMove;
                }
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                SpellAndTrapCard stc = equipableCard as SpellAndTrapCard;
                if (stc.getName() == "Legendary Sword")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("WARRIOR"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Beast Fangs")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("BEAST"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Violet Crystal")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("ZOMBIE"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Book of Secret Arts")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("SPELLCASTER"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.getName() == "Power of Kaishin")
                {
                    if (monsterCard.getYuGhiOhType().ToUpper().Contains("AQUA"))
                    {
                        monsterCard.setAttackPoints(monsterCard.getAttackPoints() + 300);
                        monsterCard.setDefensePoints(monsterCard.getDefensePoints() + 300);
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else
                {
                    return Result.InvalidMove;
                }
                return Result.Success;
            }
            else if (player2.id == id && playerWhosTurnItIs == 1)
            {
                return Result.NotYourTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }
    }
}
