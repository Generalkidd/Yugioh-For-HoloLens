using Assets.Scripts.BattleHandler.Cards;
using Assets.Scripts.BattleHandler.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BattleHandler.Game
{
    public class Game
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
                return player1.MeReadOnly;
            }
            else if (player2.id != myId)
            {
                return player2.MeReadOnly;
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

        internal Result RequestNormalSummon(int idOfAttacker, object cardToSummon, int numberOfMonstersAlreadyPlayed)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1 && !playerHasNormalSummonedThisTurn && numberOfMonstersAlreadyPlayed < 6 && ((4 + sacrifices) >= (cardToSummon as MonsterCard).Level))
            {
                player1.addFaceDownToMonsterZone(cardToSummon);
                playerHasNormalSummonedThisTurn = true;
                return Result.Success;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                Debug.Log("You are player 1 and it is player 2s turn");
                return Result.NotYourTurn;
            }
            else if (!(numberOfMonstersAlreadyPlayed < 6))
            {
                return Result.AlreadyPlayedMaxNumberOfMonsters;
            }
            else if (((4 + sacrifices) < (cardToSummon as MonsterCard).Level))
            {
                return Result.NeedMoreSacrifices;
            }
            else if (player1.id == idOfAttacker)
            {
                return Result.AlreadyNormalSummonedThisTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2 && !playerHasNormalSummonedThisTurn)
            {
                player2.addFaceDownToMonsterZone(cardToSummon);
                playerHasNormalSummonedThisTurn = true;
                return Result.Success;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                Debug.Log("You are player 2 and it is player 1s turn");
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker)
            {
                return Result.AlreadyNormalSummonedThisTurn;
            }
            else
            {
                return Result.InvalidMove;
            }
        }

        public void RequestSetPlayer1Deck(List<Cards.Card> mainDeck)
        {
            MainDeck deck = new MainDeck();
            deck.CardsInDeck = mainDeck;
            //SideDeck sDeck = new SideDeck();
            //sDeck.Set(sideDeck);
            //ExtraDeck exDeck = new ExtraDeck();
            //exDeck.Set(extraDeck);
            player1.setDecks(deck);
        }

        internal Result RequestEndTurn(int idOfAttacker)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                for(int i=0; i<player1.MeReadOnly.FaceUpMonsters.Count; i++)
                {
                    if(player1.MeReadOnly.FaceUpMonsters[i].CardName== "The Wicked Worm Beast")
                    {
                        player1.SendToHand(player1.MeReadOnly.FaceUpMonsters[i], Zone.Monster);
                    }
                }
                playerWhosTurnItIs = 2;
                playerHasNormalSummonedThisTurn = false;
                //player2.NotifyOfYourTurn();
                player2.drawCard();
                player2.allowMonstersToAttack();
                //player1.NotifyOfOppTurn();
                sacrifices = 0;
                return Result.Success;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                {
                    if (player2.MeReadOnly.FaceUpMonsters[i].CardName == "The Wicked Worm Beast")
                    {
                        player2.SendToHand(player1.MeReadOnly.FaceUpMonsters[i], Zone.Monster);
                    }
                }
                playerWhosTurnItIs = 1;
                playerHasNormalSummonedThisTurn = false;
                //player1.NotifyOfYourTurn();
                player1.drawCard();
                player1.allowMonstersToAttack();
                //player2.NotifyOfOppTurn();
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

        internal Result RequestAttackOnFaceDownCard(int idOfAttacker, MonsterCard attackingCard)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                List<MonsterCard> p2cards = player2.FaceDownCardsInMonsterZone;
                MonsterCard toAttack = null;
                if (p2cards.Count > 0)
                {
                    toAttack = p2cards[0];
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
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                List<MonsterCard> p1cards = player1.FaceDownCardsInMonsterZone;
                MonsterCard toAttack = null;
                if (p1cards.Count > 0)
                {
                    toAttack = p1cards[0];
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
                if ((player1.FaceDownCardsInMonsterZone.Contains(attackingCard) || player1.MeReadOnly.FaceUpMonsters.Contains(attackingCard)) && (player2.FaceDownCardsInMonsterZone.Contains(defendingCard) || player2.MeReadOnly.FaceUpMonsters.Contains(defendingCard)))
                {
                    if (attackingCard.CanAttack)
                    {
                        player1.switchFaceDownToFaceUp(attackingCard);
                        player2.switchFaceDownToFaceUp(defendingCard);
                        if (attackingCard.Mode == Mode.Attack)
                        {
                            int attackingWith = attackingCard.AttackPoints;
                            int defendingWith = 0;
                            if (defendingCard.Mode == Mode.Attack)
                            {
                                defendingWith = defendingCard.AttackPoints;
                                if (attackingWith > defendingWith)
                                {
                                    int toTakeOffLifePoints = attackingWith - defendingWith;
                                    player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                    player1.switchCanAttack(attackingCard);
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
                                return Result.Success;
                            }
                            else
                            {
                                defendingWith = defendingCard.DefensePoints;
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
                            if (player1.MeReadOnly.FaceUpMonsters.Contains(attackingCard))
                            {
                                int attackingWith = attackingCard.AttackPoints;
                                int defendingWith = 0;
                                if (defendingCard.Mode == Mode.Attack)
                                {
                                    defendingWith = defendingCard.AttackPoints;
                                    if (attackingWith > defendingWith)
                                    {
                                        int toTakeOffLifePoints = attackingWith - defendingWith;
                                        player2.SendToGraveYard(defendingCard as object, Zone.Monster);
                                        player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                        player1.switchCanAttack(attackingCard);
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
                                    
                                    return Result.Success;
                                }
                                else
                                {
                                    defendingWith = defendingCard.DefensePoints;
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
                if ((player2.FaceDownCardsInMonsterZone.Contains(attackingCard) || player2.MeReadOnly.FaceUpMonsters.Contains(attackingCard)) && (player1.FaceDownCardsInMonsterZone.Contains(defendingCard) || player1.MeReadOnly.FaceUpMonsters.Contains(defendingCard)))
                {
                    if (attackingCard.CanAttack)
                    {
                        player2.switchFaceDownToFaceUp(attackingCard);
                        player1.switchFaceDownToFaceUp(defendingCard);
                        if (attackingCard.Mode == Mode.Attack)
                        {
                            int attackingWith = attackingCard.AttackPoints;
                            int defendingWith = 0;
                            if (defendingCard.Mode == Mode.Attack)
                            {
                                defendingWith = defendingCard.AttackPoints;
                                if (attackingWith > defendingWith)
                                {
                                    int toTakeOffLifePoints = attackingWith - defendingWith;
                                    player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                    player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                    player2.switchCanAttack(attackingCard);
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
                                    player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                }
                                return Result.Success;
                            }
                            else
                            {
                                defendingWith = defendingCard.DefensePoints;
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
                                player2.switchCanAttack(attackingCard);
                                return Result.Success;
                            }
                        }
                        else
                        {
                            RequestChangeModeOfCard(idOfAttacker, attackingCard);
                            if (player2.MeReadOnly.FaceUpMonsters.Contains(attackingCard))
                            {
                                int attackingWith = attackingCard.AttackPoints;
                                int defendingWith = 0;
                                if (defendingCard.Mode == Mode.Attack)
                                {
                                    defendingWith = defendingCard.AttackPoints;
                                    if (attackingWith > defendingWith)
                                    {
                                        int toTakeOffLifePoints = attackingWith - defendingWith;
                                        player1.SendToGraveYard(defendingCard as object, Zone.Monster);
                                        player1.setLifePoints(player1.getLifePoints() - toTakeOffLifePoints);
                                        player2.switchCanAttack(attackingCard);
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
                                        player2.setLifePoints(player2.getLifePoints() - toTakeOffLifePoints);
                                    }

                                    return Result.Success;
                                }
                                else
                                {
                                    defendingWith = defendingCard.DefensePoints;
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
                                    player2.switchCanAttack(attackingCard);
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
                if (attackingCard.CanAttack)
                {
                    if (player2.FaceDownCardsInMonsterZone.Count == 0 && player2.MeReadOnly.FaceUpTraps.Count == 0)
                    {
                        if (player1.MeReadOnly.FaceUpMonsters.Contains(attackingCard) || player1.FaceDownCardsInMonsterZone.Contains(attackingCard))
                        {
                            player1.switchFaceDownToFaceUp(attackingCard);
                            player2.setLifePoints(player2.getLifePoints() - attackingCard.AttackPoints);
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
                if (attackingCard.CanAttack)
                {
                    if (player1.FaceDownCardsInMonsterZone.Count == 0 && player1.MeReadOnly.FaceUpTraps.Count == 0)
                    {
                        if (player2.MeReadOnly.FaceUpMonsters.Contains(attackingCard) || player2.FaceDownCardsInMonsterZone.Contains(attackingCard))
                        {
                            player2.switchFaceDownToFaceUp(attackingCard);
                            player1.setLifePoints(player1.getLifePoints() - attackingCard.AttackPoints);
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
            if (toChangeModeOf.CanAttack)
            {
                if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
                {
                    if (playerTwoTrapHoleEnabled && toChangeModeOf.FacePosition!=Face.Up && toChangeModeOf.AttackPoints > 999)
                    {
                        player1.SendToGraveYard(toChangeModeOf, Zone.Monster);
                        IList<SpellAndTrapCard> spellsAndTrapsP2 = player2.FaceDownTraps;
                        for (int i = 0; i < spellsAndTrapsP2.Count; i++)
                        {
                            if (spellsAndTrapsP2[i].CardName.ToUpper() == "TRAP HOLE")
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
                    if (playerOneTrapHoleEnabled && toChangeModeOf.FacePosition!=Face.Up && toChangeModeOf.AttackPoints > 999)
                    {
                        player2.SendToGraveYard(toChangeModeOf, Zone.Monster);
                        IList<SpellAndTrapCard> spellsAndTrapsP1 = player1.FaceDownTraps;
                        for (int i = 0; i < spellsAndTrapsP1.Count; i++)
                        {
                            if (spellsAndTrapsP1[i].CardName.ToUpper() == "TRAP HOLE")
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

        public void RequestSetPlayer2Deck(List<Cards.Card> mainDeck)
        {
            MainDeck deck = new MainDeck();
            deck.CardsInDeck = mainDeck;
            //SideDeck sDeck = new SideDeck();
            //sDeck.Set(sideDeck);
            //ExtraDeck exDeck = new ExtraDeck();
            //exDeck.Set(extraDeck);
            player2.setDecks(deck);
        }

        public Game(Player p_player1, Player p_player2, int id)
        {
            player1 = p_player1;
            player2 = p_player2;
            gameID = id;
            playerWhosTurnItIs = 1;
            playerHasNormalSummonedThisTurn = false;
            player1.MyCurrentGame=this;
            player2.MyCurrentGame=this;
        }

        internal void setCurrentGame()
        {
            player1.MyCurrentGame = this;
            player2.MyCurrentGame = this;
        }

        internal void MakeGame(Player p_player1, Player p_player2, int id)
        {
            //Game myGame = toAddTo.AddComponent<Game>();
            player1 = p_player1;
            player2 = p_player2;
            gameID = id;
            playerWhosTurnItIs = 1;
            playerHasNormalSummonedThisTurn = false;
        }

        internal void StartGame()
        {
            Debug.Log("Shuffling decks with id=" + gameID);
            player1.shuffleAllDecks(gameID);
            player2.shuffleAllDecks(gameID);
            player1.draw5Cards();
            player2.draw5Cards();
        }

        internal Result RequestDarkHole(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                List<MonsterCard> faceDownMonsters = player1.FaceDownCardsInMonsterZone;
                List<MonsterCard> faceUpMonsters = player1.MeReadOnly.FaceUpMonsters;
                while (faceDownMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player1.FaceDownCardsInMonsterZone;
                }
                while (faceUpMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player1.MeReadOnly.FaceUpMonsters;
                }
                faceDownMonsters = player2.FaceDownCardsInMonsterZone;
                faceUpMonsters = player2.MeReadOnly.FaceUpMonsters;
                while (faceDownMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player2.FaceDownCardsInMonsterZone;
                }
                while (faceUpMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player2.MeReadOnly.FaceUpMonsters;
                }
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                List<MonsterCard> faceDownMonsters = player1.FaceDownCardsInMonsterZone;
                List<MonsterCard> faceUpMonsters = player1.MeReadOnly.FaceUpMonsters;
                while (faceDownMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player1.FaceDownCardsInMonsterZone;
                }
                while (faceUpMonsters.Count > 0)
                {
                    player1.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player1.MeReadOnly.FaceUpMonsters;
                }
                faceDownMonsters = player2.FaceDownCardsInMonsterZone;
                faceUpMonsters = player2.MeReadOnly.FaceUpMonsters;
                while (faceDownMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceDownMonsters[0], Zone.Monster);
                    faceDownMonsters = player2.FaceDownCardsInMonsterZone;
                }
                while (faceUpMonsters.Count > 0)
                {
                    player2.SendToGraveYard(faceUpMonsters[0], Zone.Monster);
                    faceUpMonsters = player2.MeReadOnly.FaceUpMonsters;
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


        internal Result RequestDeSpell(int idOfAttacker, int indexOfOppFaceDownCardToDestroy)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                if (player2.FaceDownTraps.Count > indexOfOppFaceDownCardToDestroy)
                {
                    SpellAndTrapCard toDestroy = player2.FaceDownTraps[indexOfOppFaceDownCardToDestroy];
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player2.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
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
                if (player1.FaceDownTraps.Count > indexOfOppFaceDownCardToDestroy)
                {
                    SpellAndTrapCard toDestroy = player1.FaceDownTraps[indexOfOppFaceDownCardToDestroy];
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player1.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
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

        internal Result RequestDeSpell(int idOfAttacker, SpellAndTrapCard toDestroy)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                if(player1.FaceDownTraps.Contains(toDestroy))
                {
                    if(toDestroy.Attribute==CardAttributeOrType.Spell)
                    {
                        if(toDestroy.EquippedTo!=null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for(int i=0;i<player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if(player1.FaceDownCardsInMonsterZone[i]==toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player1.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
                }
                else if(player1.MeReadOnly.FaceUpTraps.Contains(toDestroy))
                {
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player1.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
                }
                else if(player2.MeReadOnly.FaceUpTraps.Contains(toDestroy))
                {
                    if(toDestroy.Attribute==CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player2.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
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
                if (player2.FaceDownTraps.Contains(toDestroy))
                {
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player2.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
                }
                else if (player1.MeReadOnly.FaceUpTraps.Contains(toDestroy))
                {
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player1.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player1.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player1.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player1.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player1.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player1.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player1.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
                }
                else if (player2.MeReadOnly.FaceUpTraps.Contains(toDestroy))
                {
                    if (toDestroy.Attribute == CardAttributeOrType.Spell)
                    {
                        if (toDestroy.EquippedTo != null)
                        {
                            if (toDestroy.CardName == "Legendary Sword")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Beast Fangs")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Violet Crystal")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Book of Secret Arts")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Power of Kaishin")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Dark Energy")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints -= 300;
                                        mc.AttackPoints -= 300;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                            else if (toDestroy.CardName == "Invigoration")
                            {
                                for (int i = 0; i < player2.FaceDownCardsInMonsterZone.Count; i++)
                                {
                                    if (player2.FaceDownCardsInMonsterZone[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.FaceDownCardsInMonsterZone[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.FaceDownCardsInMonsterZone[i] = mc;
                                        break;
                                    }
                                }
                                for (int i = 0; i < player2.MeReadOnly.FaceUpMonsters.Count; i++)
                                {
                                    if (player2.MeReadOnly.FaceUpMonsters[i] == toDestroy.EquippedTo)
                                    {
                                        MonsterCard mc = player2.MeReadOnly.FaceUpMonsters[i];
                                        mc.DefensePoints += 200;
                                        mc.AttackPoints -= 400;
                                        mc.EquippedTo = null;
                                        player2.MeReadOnly.FaceUpMonsters[i] = mc;
                                        break;
                                    }
                                }
                            }
                        }
                        player2.SendToGraveYard(toDestroy, Zone.SpellTrap);
                    }
                    return Result.Success;
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

        internal Result RequestTwoProngedAttack(int idOfAttacker, MonsterCard mineToDestroy1, MonsterCard mineToDestroy2, MonsterCard theirsToDestroy)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                if (player1.FaceDownCardsInMonsterZone.Contains(mineToDestroy1) || player1.MeReadOnly.FaceUpMonsters.Contains(mineToDestroy1))
                {
                    if (player1.FaceDownCardsInMonsterZone.Contains(mineToDestroy2) || player1.MeReadOnly.FaceUpMonsters.Contains(mineToDestroy2))
                    {
                        if (player2.MeReadOnly.FaceUpMonsters.Contains(theirsToDestroy))
                        {
                            player1.SendToGraveYard(mineToDestroy1, Zone.Monster);
                            player1.SendToGraveYard(mineToDestroy2, Zone.Monster);
                            player2.SendToGraveYard(theirsToDestroy, Zone.Monster);
                            return Result.Success;
                        }
                    }
                }
                return Result.OneOrMoreCardsAreNoLongerOnField;
            }
            else if (player1.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == idOfAttacker && playerWhosTurnItIs == 2)
            {
                if (player2.FaceDownCardsInMonsterZone.Contains(mineToDestroy1) || player2.MeReadOnly.FaceUpMonsters.Contains(mineToDestroy1))
                {
                    if (player2.FaceDownCardsInMonsterZone.Contains(mineToDestroy2) || player2.MeReadOnly.FaceUpMonsters.Contains(mineToDestroy2))
                    {
                        if (player1.MeReadOnly.FaceUpMonsters.Contains(theirsToDestroy))
                        {
                            player2.SendToGraveYard(mineToDestroy1, Zone.Monster);
                            player2.SendToGraveYard(mineToDestroy2, Zone.Monster);
                            player1.SendToGraveYard(theirsToDestroy, Zone.Monster);
                            return Result.Success;
                        }
                    }
                }
                return Result.OneOrMoreCardsAreNoLongerOnField;
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

        internal Result RequestSacrifice(int idOfAttacker, MonsterCard toSacrifice)
        {
            if (player1.id == idOfAttacker && playerWhosTurnItIs == 1)
            {
                player1.SendToGraveYard(toSacrifice, Zone.Monster);
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

        internal Result RequestOokazi(int id)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                player2.setLifePoints(player2.getLifePoints() - 800);
                return Result.Success;
            }
            else if (player1.id == id && playerWhosTurnItIs == 2)
            {
                return Result.NotYourTurn;
            }
            else if (player2.id == id && playerWhosTurnItIs == 2)
            {
                player1.setLifePoints(player1.getLifePoints() - 800);
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
                MonsterCard toDestroy = null;
                IList<MonsterCard> faceUpMonsters = player2.MeReadOnly.FaceUpMonsters;
                foreach (MonsterCard c in faceUpMonsters)
                {
                    if (c.AttackPoints < lowestAttack)
                    {
                        lowestAttack = c.AttackPoints;
                        toDestroy = c;
                    }
                }
                if (toDestroy != null)
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
                IList<MonsterCard> faceUpMonsters = player1.MeReadOnly.FaceUpMonsters;
                foreach (MonsterCard c in faceUpMonsters)
                {
                    if (c.AttackPoints < lowestAttack)
                    {
                        lowestAttack = c.AttackPoints;
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

        internal Result RequestEquip(int id, ref SpellAndTrapCard stc, ref MonsterCard monsterCard)
        {
            if (player1.id == id && playerWhosTurnItIs == 1)
            {
                if (stc.CardName == "Legendary Sword")
                {
                    if (monsterCard.Type.ToUpper().Contains("WARRIOR"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints=monsterCard.AttackPoints + 300;
                        monsterCard.DefensePoints=monsterCard.DefensePoints + 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Beast Fangs")
                {
                    if (monsterCard.Type.ToUpper().Contains("BEAST"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints += 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Violet Crystal")
                {
                    if (monsterCard.Type.ToUpper().Contains("ZOMBIE"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+=300;
                        monsterCard.DefensePoints+= 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Book of Secret Arts")
                {
                    if (monsterCard.Type.ToUpper().Contains("SPELLCASTER"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints+=300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Power of Kaishin")
                {
                    if (monsterCard.Type.ToUpper().Contains("AQUA"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints+= 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if(stc.CardName == "Dark Energy")
                {
                    if (monsterCard.Type.ToUpper().Contains("FIEND"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints += 300;
                        monsterCard.DefensePoints += 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if(stc.CardName == "Invigoration")
                {
                    if (monsterCard.Attribute==CardAttributeOrType.Earth)
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints += 400;
                        monsterCard.DefensePoints -= 200;
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
                if (stc.CardName == "Legendary Sword")
                {
                    if (monsterCard.Type.ToUpper().Contains("WARRIOR"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints+= 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Beast Fangs")
                {
                    if (monsterCard.Type.ToUpper().Contains("BEAST"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints+= 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Violet Crystal")
                {
                    if (monsterCard.Type.ToUpper().Contains("ZOMBIE"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints += 300;
                        monsterCard.DefensePoints += 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Book of Secret Arts")
                {
                    if (monsterCard.Type.ToUpper().Contains("SPELLCASTER"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints+= 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Power of Kaishin")
                {
                    if (monsterCard.Type.ToUpper().Contains("AQUA"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints+= 300;
                        monsterCard.DefensePoints += 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Dark Energy")
                {
                    if (monsterCard.Type.ToUpper().Contains("FIEND"))
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints += 300;
                        monsterCard.DefensePoints += 300;
                    }
                    else
                    {
                        return Result.IneligibleMonsterType;
                    }
                }
                else if (stc.CardName == "Invigoration")
                {
                    if (monsterCard.Attribute == CardAttributeOrType.Earth)
                    {
                        stc.EquippedTo = monsterCard;
                        monsterCard.EquippedTo = stc;
                        monsterCard.AttackPoints += 400;
                        monsterCard.DefensePoints -= 200;
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
