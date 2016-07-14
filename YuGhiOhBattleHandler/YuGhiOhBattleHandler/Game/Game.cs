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
        private int sacrifices = 0;
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
            IneligibleMonsterType,
            NeedMoreSacrifices,
            CantAttackBcAlreadyAttackedOrFirstTurnPlayed,
            OneOrMoreCardsAreNoLongerOnField,
            OpponentHasMonstersSoCannotTargetLifePoints
        }

        internal Result RequestNormalSummon(int idOfAttacker, Object cardToSummon, int numberOfMonstersAlreadyPlayed)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1 && !playerHasNormalSummonedThisTurn && numberOfMonstersAlreadyPlayed<6 && ((4+sacrifices)>=(cardToSummon as MonsterCard).getLevel()))
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
            else if(((4 + sacrifices) < (cardToSummon as MonsterCard).getLevel()))
            {
                return Result.NeedMoreSacrifices;
            }
            else if(player1.id==idOfAttacker)
            {
                return Result.AlreadyNormalSummonedThisTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2 && !playerHasNormalSummonedThisTurn)
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
                player2.allowMonstersToAttack();
                player1.NotifyOfOppTurn();
                sacrifices = 0;
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
                player1.allowMonstersToAttack();
                player2.NotifyOfOppTurn();
                sacrifices = 0;
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

        internal Result RequestAttackOnFaceDownCard(int idOfAttacker, MonsterCard attackingCard, Mode m)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                IList<MonsterCard> p2cards=player2.getFaceDownCardsInMonsterZone();
                MonsterCard toAttack = null;
                int i = 0;
                while(toAttack==null && i<p2cards.Count)
                {
                    if(p2cards[i].getBattlePosition()==m)
                    {
                        toAttack = p2cards[i];
                    }
                }
                if(toAttack!=null)
                { 
                    return RequestAttack(idOfAttacker, attackingCard, toAttack);
                }
                else
                {
                    return Result.InvalidMove;
                }
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                IList<MonsterCard> p1cards = player1.getFaceDownCardsInMonsterZone();
                MonsterCard toAttack = null;
                int i = 0;
                while (toAttack == null && i < p1cards.Count)
                {
                    if (p1cards[i].getBattlePosition() == m)
                    {
                        toAttack = p1cards[i];
                    }
                }
                if (toAttack != null)
                {
                    return RequestAttack(idOfAttacker, attackingCard, toAttack);
                }
                else
                {
                    return Result.InvalidMove;
                }
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

        internal Result RequestAttack(int idOfAttacker, MonsterCard attackingCard, MonsterCard defendingCard)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                if((player1.getFaceDownCardsInMonsterZone().Contains(attackingCard) || player1.getFaceUpMonstersInMonsterZone().Contains(attackingCard)) && (player2.getFaceDownCardsInMonsterZone().Contains(defendingCard) || player2.getFaceUpMonstersInMonsterZone().Contains(defendingCard)))
                {
                    if(attackingCard.CanAttack())
                    {
                        player1.switchFaceDownToFaceUp(attackingCard);
                        player2.switchFaceDownToFaceUp(defendingCard);
                        if (attackingCard.getBattlePosition()==Mode.Attack)
                        {
                            int attackingWith = attackingCard.getAttackPoints();
                            int defendingWith = 0;
                            if(defendingCard.getBattlePosition()==Mode.Attack)
                            {
                                defendingWith = defendingCard.getAttackPoints();
                                if(attackingWith>defendingWith)
                                {
                                    int toTakeOffLifePoints = attackingWith - defendingWith;
                                    player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                }
                                else if(attackingWith==defendingWith)
                                {
                                    player1.SendToGraveYard(attackingCard as object, Zone.Monster);
                                    player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                }
                                else
                                {
                                    int toTakeOffLifePoints = defendingWith-attackingWith;
                                    player1.SendToGraveYard(attackingCard as object, Zone.Monster);
                                    player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                }
                                player1.switchCanAttack(attackingCard);
                                return Result.Success;
                            }
                            else
                            {
                                defendingWith = defendingCard.getDefensePoints();
                                if (attackingWith > defendingWith)
                                {
                                    player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                }
                                else if (attackingWith == defendingWith)
                                {
                                    
                                }
                                else
                                {
                                    int toTakeOffLifePoints = defendingWith - attackingWith;
                                    player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                }
                                player1.switchCanAttack(attackingCard);
                                return Result.Success;
                            }
                        }
                        else
                        {
                            RequestChangeModeOfCard(idOfAttacker, attackingCard);
                            if (player1.getFaceUpMonstersInMonsterZone().Contains(attackingCard))
                            {
                                int attackingWith = attackingCard.getAttackPoints();
                                int defendingWith = 0;
                                if (defendingCard.getBattlePosition() == Mode.Attack)
                                {
                                    defendingWith = defendingCard.getAttackPoints();
                                    if (attackingWith > defendingWith)
                                    {
                                        int toTakeOffLifePoints = attackingWith - defendingWith;
                                        player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                        player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    else if (attackingWith == defendingWith)
                                    {
                                        player1.SendToGraveYard(attackingCard as object, Zone.Monster);
                                        player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    }
                                    else
                                    {
                                        int toTakeOffLifePoints = defendingWith - attackingWith;
                                        player1.SendToGraveYard(attackingCard as object, Zone.Monster);
                                        player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    player1.switchCanAttack(attackingCard);
                                    return Result.Success;
                                }
                                else
                                {
                                    defendingWith = defendingCard.getDefensePoints();
                                    if (attackingWith > defendingWith)
                                    {
                                        player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    }
                                    else if (attackingWith == defendingWith)
                                    {

                                    }
                                    else
                                    {
                                        int toTakeOffLifePoints = defendingWith - attackingWith;
                                        player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    player1.switchCanAttack(attackingCard);
                                    return Result.Success;
                                }
                            }
                            else
                            {
                                return Result.OneOrMoreCardsAreNoLongerOnField;
                            }
                        }
                    }
                    else
                    {
                        return Result.CantAttackBcAlreadyAttackedOrFirstTurnPlayed;
                    }
                }
                else
                {
                    return Result.OneOrMoreCardsAreNoLongerOnField;
                }
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                if ((player2.getFaceDownCardsInMonsterZone().Contains(attackingCard) || player2.getFaceUpMonstersInMonsterZone().Contains(attackingCard)) && (player1.getFaceDownCardsInMonsterZone().Contains(defendingCard) || player1.getFaceUpMonstersInMonsterZone().Contains(defendingCard)))
                {
                    if (attackingCard.CanAttack())
                    {
                        if (attackingCard.getBattlePosition() == Mode.Attack)
                        {
                            int attackingWith = attackingCard.getAttackPoints();
                            int defendingWith = 0;
                            if (defendingCard.getBattlePosition() == Mode.Attack)
                            {
                                defendingWith = defendingCard.getAttackPoints();
                                if (attackingWith > defendingWith)
                                {
                                    int toTakeOffLifePoints = attackingWith - defendingWith;
                                    player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                }
                                else if (attackingWith == defendingWith)
                                {
                                    player2.SendToGraveYard(attackingCard as object, Zone.Monster);
                                    player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                }
                                else
                                {
                                    int toTakeOffLifePoints = defendingWith - attackingWith;
                                    player2.SendToGraveYard(attackingCard as object, Zone.Monster);
                                    player2.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                }
                                player2.switchFaceDownToFaceUp(attackingCard);
                                player1.switchFaceDownToFaceUp(defendingCard);
                                return Result.Success;
                            }
                            else
                            {
                                defendingWith = defendingCard.getDefensePoints();
                                if (attackingWith > defendingWith)
                                {
                                    player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                }
                                else if (attackingWith == defendingWith)
                                {

                                }
                                else
                                {
                                    int toTakeOffLifePoints = defendingWith - attackingWith;
                                    player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                }
                                player2.switchFaceDownToFaceUp(attackingCard);
                                player1.switchFaceDownToFaceUp(defendingCard);
                                return Result.Success;
                            }
                        }
                        else
                        {
                            RequestChangeModeOfCard(idOfAttacker, attackingCard);
                            if (player1.getFaceUpMonstersInMonsterZone().Contains(attackingCard))
                            {
                                int attackingWith = attackingCard.getAttackPoints();
                                int defendingWith = 0;
                                if (defendingCard.getBattlePosition() == Mode.Attack)
                                {
                                    defendingWith = defendingCard.getAttackPoints();
                                    if (attackingWith > defendingWith)
                                    {
                                        int toTakeOffLifePoints = attackingWith - defendingWith;
                                        player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                        player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    else if (attackingWith == defendingWith)
                                    {
                                        player2.SendToGraveYard(attackingCard as object, Zone.Monster);
                                        player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    }
                                    else
                                    {
                                        int toTakeOffLifePoints = defendingWith - attackingWith;
                                        player2.SendToGraveYard(attackingCard as object, Zone.Monster);
                                        player2.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    player2.switchFaceDownToFaceUp(attackingCard);
                                    player1.switchFaceDownToFaceUp(defendingCard);
                                    return Result.Success;
                                }
                                else
                                {
                                    defendingWith = defendingCard.getDefensePoints();
                                    if (attackingWith > defendingWith)
                                    {
                                        player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    }
                                    else if (attackingWith == defendingWith)
                                    {

                                    }
                                    else
                                    {
                                        int toTakeOffLifePoints = defendingWith - attackingWith;
                                        player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                    }
                                    player2.switchFaceDownToFaceUp(attackingCard);
                                    player1.switchFaceDownToFaceUp(defendingCard);
                                    return Result.Success;
                                }
                            }
                            else
                            {
                                return Result.OneOrMoreCardsAreNoLongerOnField;
                            }
                        }
                    }
                    else
                    {
                        return Result.CantAttackBcAlreadyAttackedOrFirstTurnPlayed;
                    }
                }
                else
                {
                    return Result.OneOrMoreCardsAreNoLongerOnField;
                }
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

        internal Result RequestAttackLifePoints(int idOfAttacker, MonsterCard attackingCard)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                if (attackingCard.CanAttack())
                {
                    if (player2.getFaceDownCardsInMonsterZone().Count == 0 && player2.getFaceUpCardsInSpellAndTrapZone().Count == 0)
                    {
                        if (player1.getFaceUpMonstersInMonsterZone().Contains(attackingCard) || player1.getFaceDownCardsInMonsterZone().Contains(attackingCard))
                        {
                            player1.switchFaceDownToFaceUp(attackingCard);
                            player2.setLifePoints(player2.getLifePoints() - attackingCard.getAttackPoints());
                            return Result.Success;
                        }
                        else
                        {
                            return Result.OneOrMoreCardsAreNoLongerOnField;
                        }
                    }
                    else
                    {
                        return Result.OpponentHasMonstersSoCannotTargetLifePoints;
                    }
                }
                else
                {
                    return Result.CantAttackBcAlreadyAttackedOrFirstTurnPlayed;
                }
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                if (attackingCard.CanAttack())
                {
                    if (player1.getFaceDownCardsInMonsterZone().Count == 0 && player1.getFaceUpCardsInSpellAndTrapZone().Count == 0)
                    {
                        if (player2.getFaceUpMonstersInMonsterZone().Contains(attackingCard) || player2.getFaceDownCardsInMonsterZone().Contains(attackingCard))
                        {
                            player2.switchFaceDownToFaceUp(attackingCard);
                            player1.setLifePoints(player1.getLifePoints() - attackingCard.getAttackPoints());
                            return Result.Success;
                        }
                        else
                        {
                            return Result.OneOrMoreCardsAreNoLongerOnField;
                        }
                    }
                    else
                    {
                        return Result.OpponentHasMonstersSoCannotTargetLifePoints;
                    }
                }
                else
                {
                    return Result.CantAttackBcAlreadyAttackedOrFirstTurnPlayed;
                }
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

        internal Result RequestChangeModeOfCard(int idOfAttacker, MonsterCard toChangeModeOf)
        {
            if (toChangeModeOf.CanAttack())
            {
                if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
                {
                    if (playerTwoTrapHoleEnabled && !toChangeModeOf.getIsFlipped() && toChangeModeOf.getAttackPoints() > 999)
                    {
                        player1.SendToGraveYard(toChangeModeOf, Zone.Monster);
                        IList<SpellAndTrapCard> spellsAndTrapsP2 = player2.getFaceDownCardsInSpellAndTrapZone();
                        for (int i = 0; i < spellsAndTrapsP2.Count; i++)
                        {
                            if (spellsAndTrapsP2[i].getName().ToUpper() == "TRAP HOLE")
                            {
                                SpellAndTrapCard stc = spellsAndTrapsP2[i];
                                player2.SendToGraveYard(stc, Zone.SpellTrap);
                                break;
                            }
                        }
                    }
                    else
                    {
                        player1.GameChangeModeOfCard(toChangeModeOf);
                    }
                    return Result.Success;
                }
                else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
                {
                    return Result.NotYourTurn;
                }
                else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
                {
                    if (playerOneTrapHoleEnabled && !toChangeModeOf.getIsFlipped() && toChangeModeOf.getAttackPoints() > 999)
                    {
                        player2.SendToGraveYard(toChangeModeOf, Zone.Monster);
                        IList<SpellAndTrapCard> spellsAndTrapsP1 = player1.getFaceDownCardsInSpellAndTrapZone();
                        for (int i = 0; i < spellsAndTrapsP1.Count; i++)
                        {
                            if (spellsAndTrapsP1[i].getName().ToUpper() == "TRAP HOLE")
                            {
                                SpellAndTrapCard stc = spellsAndTrapsP1[i];
                                player1.SendToGraveYard(stc, Zone.SpellTrap);
                                break;
                            }
                        }
                    }
                    else
                    {
                        player2.GameChangeModeOfCard(toChangeModeOf);
                    }
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
            else
            {
                return Result.CantAttackBcAlreadyAttackedOrFirstTurnPlayed;
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

        internal Result RequestSacrifice(int idOfAttacker, MonsterCard toSacrifice)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                player1.SendToGraveYard(toSacrifice, Zone.Monster);
                if(sacrifices==0)
                {
                    sacrifices = 2;
                }
                else
                {
                    sacrifices = 10;
                }
                return Result.Success;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                player2.SendToGraveYard(toSacrifice, Zone.Monster);
                if (sacrifices == 0)
                {
                    sacrifices = 2;
                }
                else
                {
                    sacrifices = 10;
                }
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
