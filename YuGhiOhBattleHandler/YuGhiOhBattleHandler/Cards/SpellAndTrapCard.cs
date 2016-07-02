using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace YuGhiOhBattleHandler
{

    internal enum Icon
    {
        Equip,
        Field,
        QuickPlay,
        Ritual,
        Continuous,
        Counter,
        Normal
    }
    /// <summary>
    /// Spell Cards can normally be 
    /// activated only during your Main
    /// Phase, and help you out with
    /// different effects.Spell Cards
    /// have many powerful effects,
    /// like destroying other cards or
    /// strengthening monsters.Save
    /// these cards in your hand until
    /// you can get the best results out
    /// of them.
    /// </summary>
    public sealed class SpellAndTrapCard
    {
        private Icon m_icon;
        private bool m_isPlayed;
        private Card c=new Card();
        internal SpellAndTrapCard(string cardName, CardAttributeOrType attributeOrType, Icon ico, string cardDescription, long cardNumber, BitmapImage bi)
        {
            c.setCardName(cardName);
            m_icon = ico;
            c.setAttributeOrType(attributeOrType);
            c.setCardDescrip(cardDescription);
            c.setCardNumb(cardNumber);
            m_isPlayed = false;
            c.setBitmapImage(bi);
        }
        public string getDescription()
        {
            return c.getCardDescrip();
        }


        public string getName()
        {
            return c.getCardName();
        }

        public BitmapImage getImage()
        {
            return c.getImage();
        }

        public void Play(Player user, Player opponent)
        {
            
        }
    }
}
