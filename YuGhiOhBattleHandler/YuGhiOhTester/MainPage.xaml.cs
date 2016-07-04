using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YuGhiOhBattleHandler;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YuGhiOhTester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static Random rand = new Random();
        private Player me;
        private Object myCurrentlySelectedCard;
        private StackPanel myCurrentlySelectedStackPanel;

        public MainPage()
        {
            this.InitializeComponent();

            //Here is where code should go to build personal Decks. For now we make a random deck (first 40 cards in database).
            //Both players will use the same deck for this test app.
            MainDeckBuilder mdb = new MainDeckBuilder();
            List<Object> randomDeck = mdb.getRandomDeck() as List<Object>;
            List<Object> randomSideDeck = new List<Object>();
            List<Object> randomExtraDeck = new List<Object>();

            //Build the players just like a Network Manager would do.
            int random1 = rand.Next();
            int random2 = rand.Next();
            int randomGameId = rand.Next();
            Player p1 = new Player(random1, "SethRocks!");
            Player p2 = new Player(random2, "BobSucks!");

            //Now the network manager would give a handle to the users and to the game.
            //Since this is just for testing we will control both players from this one class.
            Game g = new Game(p1, p2, randomGameId);
            if (g.RequestSetPlayer1Deck(randomDeck, randomSideDeck, randomExtraDeck) && g.RequestSetPlayer2Deck(randomDeck, randomSideDeck, randomExtraDeck))
            {
                g.StartGame();
                MyUserName.Text = p1.getUsername();
                OpponentUsernameBlock.Text = p2.getUsername();

                //Normally only the game and corresponding player will be able to use the get hand function
                //because they will be the only entities which have a handle to the player
                me = p1;
                List<Object> hand = me.getHand() as List<Object>;
                for (int i = 0; i < hand.Count; i++)
                {
                    placeHandCardOnGUI(hand[i], i);
                }
                collapseUnused();
            }
        }

        private void collapseUnused()
        {
            if (MyHandOneImage.Source == null)
            {
                MyHandOneZone.Visibility = Visibility.Collapsed;
            }
            if (MyHandTwoImage.Source == null)
            {
                MyHandTwoZone.Visibility = Visibility.Collapsed;
            }
            if (MyHandThreeImage.Source == null)
            {
                MyHandThreeZone.Visibility = Visibility.Collapsed;
            }
            if (MyHandFourImage.Source == null)
            {
                MyHandFourZone.Visibility = Visibility.Collapsed;
            }
            if (MyHandFiveImage.Source == null)
            {
                MyHandFiveZone.Visibility = Visibility.Collapsed;
            }
            if (MyHandSixImage.Source == null)
            {
                MyHandSixZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterOneImage.Source == null)
            {
                MyMonsterOneZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterTwoImage.Source == null)
            {
                MyMonsterTwoZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterThreeImage.Source == null)
            {
                MyMonsterThreeZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterFourImage.Source == null)
            {
                MyMonsterFourZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterFiveImage.Source == null)
            {
                MyMonsterFiveZone.Visibility = Visibility.Collapsed;
            }
            if (MyMonsterSixImage.Source == null)
            {
                MyMonsterSixZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellOneImage.Source == null)
            {
                MySpellOneZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellTwoImage.Source == null)
            {
                MySpellTwoZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellThreeImage.Source == null)
            {
                MySpellThreeZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellFourImage.Source == null)
            {
                MySpellFourZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellFiveImage.Source == null)
            {
                MySpellFiveZone.Visibility = Visibility.Collapsed;
            }
            if (MySpellSixImage.Source == null)
            {
                MySpellSixZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterOneImage.Source == null)
            {
                OppMonsterOneZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterTwoImage.Source == null)
            {
                OppMonsterTwoZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterThreeImage.Source == null)
            {
                OppMonsterThreeZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterFourImage.Source == null)
            {
                OppMonsterFourZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterFiveImage.Source == null)
            {
                OppMonsterFiveZone.Visibility = Visibility.Collapsed;
            }
            if (OppMonsterSixImage.Source == null)
            {
                OppMonsterSixZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellOneImage.Source == null)
            {
                OppSpellOneZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellTwoImage.Source == null)
            {
                OppSpellTwoZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellThreeImage.Source == null)
            {
                OppSpellThreeZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellFourImage.Source == null)
            {
                OppSpellFourZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellFiveImage.Source == null)
            {
                OppSpellFiveZone.Visibility = Visibility.Collapsed;
            }
            if (OppSpellSixImage.Source == null)
            {
                OppSpellSixZone.Visibility = Visibility.Collapsed;
            }

        }

        private void placeMyMonsterCardsOnGUI()
        {
            List<MonsterCard> faceDownCards = me.getFaceDownCardsInMonsterZone() as List<MonsterCard>;
            List<MonsterCard> faceUpCards = me.getFaceUpMonstersInMonsterZone() as List<MonsterCard>;
            for (int i = 0; i < faceDownCards.Count; i++)
            {
                if (i == 0)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterOneZone.Visibility = Visibility.Visible;
                    MyMonsterOneAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterOneDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterOneAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterOneDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterOneNameBlock.Text = c.getName();
                    MyMonsterOneImage.Source = c.getImage();
                }
                else if (i == 1)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterTwoZone.Visibility = Visibility.Visible;
                    MyMonsterTwoAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterTwoDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterTwoAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterTwoDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterTwoNameBlock.Text = c.getName();
                    MyMonsterTwoImage.Source = c.getImage();
                }
                else if (i == 2)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterThreeZone.Visibility = Visibility.Visible;
                    MyMonsterThreeAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterThreeDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterThreeAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterThreeDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterThreeNameBlock.Text = c.getName();
                    MyMonsterThreeImage.Source = c.getImage();
                }
                else if (i == 3)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterFourZone.Visibility = Visibility.Visible;
                    MyMonsterFourAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterFourDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterFourAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterFourDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterFourNameBlock.Text = c.getName();
                    MyMonsterFourImage.Source = c.getImage();
                }
                else if (i == 4)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterFiveZone.Visibility = Visibility.Visible;
                    MyMonsterFiveAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterFiveDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterFiveAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterFiveDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterFiveNameBlock.Text = c.getName();
                    MyMonsterFiveImage.Source = c.getImage();
                }
                else if (i == 5)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterSixZone.Visibility = Visibility.Visible;
                    MyMonsterSixAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterSixDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterSixAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterSixDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterSixNameBlock.Text = c.getName();
                    MyMonsterSixImage.Source = c.getImage();
                }
            }
            for (int i = 0; i < faceUpCards.Count; i++)
            {
                if (i + faceDownCards.Count == 0)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterOneZone.Visibility = Visibility.Visible;
                    MyMonsterOneAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterOneDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterOneAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterOneDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterOneNameBlock.Text = c.getName();
                    MyMonsterOneImage.Source = c.getImage();
                }
                else if (i + faceDownCards.Count == 1)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterTwoZone.Visibility = Visibility.Visible;
                    MyMonsterTwoAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterTwoDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterTwoAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterTwoDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterTwoNameBlock.Text = c.getName();
                    MyMonsterTwoImage.Source = c.getImage();
                }
                else if (i + faceDownCards.Count == 2)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterThreeZone.Visibility = Visibility.Visible;
                    MyMonsterThreeAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterThreeDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterThreeAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterThreeDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterThreeNameBlock.Text = c.getName();
                    MyMonsterThreeImage.Source = c.getImage();
                }
                else if (i + faceDownCards.Count == 3)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterFourZone.Visibility = Visibility.Visible;
                    MyMonsterFourAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterFourDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterFourAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterFourDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterFourNameBlock.Text = c.getName();
                    MyMonsterFourImage.Source = c.getImage();
                }
                else if (i + faceDownCards.Count == 4)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterFiveZone.Visibility = Visibility.Visible;
                    MyMonsterFiveAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterFiveDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterFiveAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterFiveDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterFiveNameBlock.Text = c.getName();
                    MyMonsterFiveImage.Source = c.getImage();
                }
                else if (i + faceDownCards.Count == 5)
                {
                    MonsterCard c = faceDownCards[i];
                    MyMonsterSixZone.Visibility = Visibility.Visible;
                    MyMonsterSixAttackPoints.Visibility = Visibility.Visible;
                    MyMonsterSixDefensePoints.Visibility = Visibility.Visible;
                    MyMonsterSixAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyMonsterSixDefensePoints.Text = "DEF: " + c.getDefensePoints() + " FDown";
                    MyMonsterSixNameBlock.Text = c.getName();
                    MyMonsterSixImage.Source = c.getImage();
                }
            }

        }

        private void placeHandCardOnGUI(object cardToPlace, int index)
        {
            if (index == 0)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandOneAttackPoints.Visibility = Visibility.Visible;
                    MyHandOneDefensePoints.Visibility = Visibility.Visible;
                    MyHandOneAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandOneDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandOneNameBlock.Text = c.getName();
                    MyHandOneImage.Source = c.getImage();
                    MyHandOneDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandOneAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandOneDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandOneNameBlock.Text = c.getName();
                    MyHandOneImage.Source = c.getImage();
                    MyHandOneDescBlock.Visibility = Visibility.Visible;
                    MyHandOneDescBlock.Text = c.getDescription();
                }
            }
            else if (index == 1)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandTwoAttackPoints.Visibility = Visibility.Visible;
                    MyHandTwoDefensePoints.Visibility = Visibility.Visible;
                    MyHandTwoAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandTwoDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandTwoNameBlock.Text = c.getName();
                    MyHandTwoImage.Source = c.getImage();
                    MyHandTwoDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandTwoAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandTwoDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandTwoNameBlock.Text = c.getName();
                    MyHandTwoImage.Source = c.getImage();
                    MyHandTwoDescBlock.Visibility = Visibility.Visible;
                    MyHandTwoDescBlock.Text = c.getDescription();
                }
            }
            else if (index == 2)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandThreeAttackPoints.Visibility = Visibility.Visible;
                    MyHandThreeDefensePoints.Visibility = Visibility.Visible;
                    MyHandThreeAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandThreeDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandThreeNameBlock.Text = c.getName();
                    MyHandThreeImage.Source = c.getImage();
                    MyHandThreeDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandThreeAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandThreeDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandThreeNameBlock.Text = c.getName();
                    MyHandThreeImage.Source = c.getImage();
                    MyHandThreeDescBlock.Visibility = Visibility.Visible;
                    MyHandThreeDescBlock.Text = c.getDescription();
                }
            }
            else if (index == 3)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandFourAttackPoints.Visibility = Visibility.Visible;
                    MyHandFourDefensePoints.Visibility = Visibility.Visible;
                    MyHandFourAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandFourDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandFourNameBlock.Text = c.getName();
                    MyHandFourImage.Source = c.getImage();
                    MyHandFourDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandFourAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandFourDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandFourNameBlock.Text = c.getName();
                    MyHandFourImage.Source = c.getImage();
                    MyHandFourDescBlock.Visibility = Visibility.Visible;
                    MyHandFourDescBlock.Text = c.getDescription();
                }
            }
            else if (index == 4)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandFiveAttackPoints.Visibility = Visibility.Visible;
                    MyHandFiveDefensePoints.Visibility = Visibility.Visible;
                    MyHandFiveAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandFiveDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandFiveNameBlock.Text = c.getName();
                    MyHandFiveImage.Source = c.getImage();
                    MyHandFiveDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandFiveAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandFiveDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandFiveNameBlock.Text = c.getName();
                    MyHandFiveImage.Source = c.getImage();
                    MyHandFiveDescBlock.Visibility = Visibility.Visible;
                    MyHandFiveDescBlock.Text = c.getDescription();
                }
            }
            else if (index == 5)
            {
                if (cardToPlace is MonsterCard)
                {
                    MonsterCard c = cardToPlace as MonsterCard;
                    MyHandSixAttackPoints.Visibility = Visibility.Visible;
                    MyHandSixDefensePoints.Visibility = Visibility.Visible;
                    MyHandSixAttackPoints.Text = "ATK: " + c.getAttackPoints() + "";
                    MyHandSixDefensePoints.Text = "DEF: " + c.getDefensePoints() + "";
                    MyHandSixNameBlock.Text = c.getName();
                    MyHandSixImage.Source = c.getImage();
                    MyHandSixDescBlock.Visibility = Visibility.Collapsed;
                }
                else if (cardToPlace is SpellAndTrapCard)
                {
                    SpellAndTrapCard c = cardToPlace as SpellAndTrapCard;
                    MyHandSixAttackPoints.Visibility = Visibility.Collapsed;
                    MyHandSixDefensePoints.Visibility = Visibility.Collapsed;
                    MyHandSixNameBlock.Text = c.getName();
                    MyHandSixImage.Source = c.getImage();
                    MyHandSixDescBlock.Visibility = Visibility.Visible;
                    MyHandSixDescBlock.Text = c.getDescription();
                }
            }
        }

        private void MyHandOneZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[0];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void MyHandTwoZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[1];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void MyHandThreeZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[2];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void MyHandFourZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[3];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void MyHandFiveZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[4];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void MyHandSixZone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedStackPanel == null || (myCurrentlySelectedStackPanel != (sender as StackPanel)))
            {
                if (myCurrentlySelectedStackPanel != null)
                {
                    myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
                }
                StackPanel s = sender as StackPanel;
                s.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0));
                myCurrentlySelectedCard = me.getHand()[5];
                myCurrentlySelectedStackPanel = s;
            }
            else
            {
                disEngageCurrentlySelected();
            }
        }

        private void disEngageCurrentlySelected()
        {
            if (myCurrentlySelectedStackPanel != null)
            {
                myCurrentlySelectedStackPanel.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 0, 128));
            }
            myCurrentlySelectedStackPanel = null;
            myCurrentlySelectedCard = null;
        }

        private void clearHandOnGUI()
        {
            MyHandOneAttackPoints.Visibility = Visibility.Collapsed;
            MyHandOneDefensePoints.Visibility = Visibility.Collapsed;
            MyHandOneAttackPoints.Text = "";
            MyHandOneDefensePoints.Text = "";
            MyHandOneNameBlock.Text = "";
            MyHandOneImage.Source = null;
            MyHandOneDescBlock.Visibility = Visibility.Collapsed;

            MyHandTwoAttackPoints.Visibility = Visibility.Collapsed;
            MyHandTwoDefensePoints.Visibility = Visibility.Collapsed;
            MyHandTwoAttackPoints.Text = "";
            MyHandTwoDefensePoints.Text = "";
            MyHandTwoNameBlock.Text = "";
            MyHandTwoImage.Source = null;
            MyHandTwoDescBlock.Visibility = Visibility.Collapsed;

            MyHandThreeAttackPoints.Visibility = Visibility.Collapsed;
            MyHandThreeDefensePoints.Visibility = Visibility.Collapsed;
            MyHandThreeAttackPoints.Text = "";
            MyHandThreeDefensePoints.Text = "";
            MyHandThreeNameBlock.Text = "";
            MyHandThreeImage.Source = null;
            MyHandThreeDescBlock.Visibility = Visibility.Collapsed;

            MyHandFourAttackPoints.Visibility = Visibility.Collapsed;
            MyHandFourDefensePoints.Visibility = Visibility.Collapsed;
            MyHandFourAttackPoints.Text = "";
            MyHandFourDefensePoints.Text = "";
            MyHandFourNameBlock.Text = "";
            MyHandFourImage.Source = null;
            MyHandFourDescBlock.Visibility = Visibility.Collapsed;

            MyHandFiveAttackPoints.Visibility = Visibility.Collapsed;
            MyHandFiveDefensePoints.Visibility = Visibility.Collapsed;
            MyHandFiveAttackPoints.Text = "";
            MyHandFiveDefensePoints.Text = "";
            MyHandFiveNameBlock.Text = "";
            MyHandFiveImage.Source = null;
            MyHandFiveDescBlock.Visibility = Visibility.Collapsed;

            MyHandSixAttackPoints.Visibility = Visibility.Collapsed;
            MyHandSixDefensePoints.Visibility = Visibility.Collapsed;
            MyHandSixAttackPoints.Text = "";
            MyHandSixDefensePoints.Text = "";
            MyHandSixNameBlock.Text = "";
            MyHandSixImage.Source = null;
            MyHandSixDescBlock.Visibility = Visibility.Collapsed;
        }

        private void MyMonsterZonePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (myCurrentlySelectedCard != null)
            {
                if (myCurrentlySelectedCard is MonsterCard)
                {
                    string result = me.NormalSummon(myCurrentlySelectedCard);
                    if (result == "")
                    {
                        disEngageCurrentlySelected();
                        placeMyMonsterCardsOnGUI();
                        clearHandOnGUI();
                        List<Object> hand = me.getHand() as List<Object>;
                        for (int i = 0; i < hand.Count; i++)
                        {
                            placeHandCardOnGUI(hand[i], i);
                        }
                    }
                    else
                    {
                        // template to load for showing Toast Notification
                        var xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                                                 "<visual>" +
                                                   "<binding template =\"ToastGeneric\">" +
                                                     "<text>Can't Summon Monster</text>" +
                                                     "<text>" +
                                                       result +
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
                }
            }
        }
    }
}
