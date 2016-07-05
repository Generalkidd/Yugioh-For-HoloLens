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
            InvalidMove
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
    }
}
