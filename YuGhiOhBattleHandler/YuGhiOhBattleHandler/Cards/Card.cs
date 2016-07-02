using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace YuGhiOhBattleHandler
{

    internal enum CardAttributeOrType
    {
        Dark,
        Earth,
        Fire,
        Fight,
        Water,
        Wind,
        Spell,
        Trap,
        Light
    }

    /// <summary>
    /// Base class for YuGhiOh cards.
    /// </summary>
    public sealed class Card:Object
    {
        private string m_cardName;
        private CardAttributeOrType m_attribute;
        private long m_cardNumber;
        private string m_cardDescription;
        private BitmapImage cardImage;

        public string getCardName()
        {
            return m_cardName;
        }

        internal void setCardName(string s)
        {
            m_cardName = s;
        }

        internal CardAttributeOrType getAttributeOrType()
        {
            return m_attribute;
        }

        internal void setAttributeOrType(CardAttributeOrType s)
        {
            m_attribute = s;
        }

        public long getCardNumb()
        {
            return m_cardNumber;
        }

        internal void setCardNumb(long s)
        {
            m_cardNumber = s;
        }

        public string getCardDescrip()
        {
            return m_cardDescription;
        }

        public BitmapImage getImage()
        {
            return cardImage;
        }

        internal void setBitmapImage(BitmapImage bi)
        {
            cardImage = bi;
        }

        internal void setCardDescrip(string s)
        {
            m_cardDescription = s;
        }

    }
}
