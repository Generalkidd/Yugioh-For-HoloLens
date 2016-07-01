using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string getCardName()
        {
            return m_cardName;
        }

        public void setCardName(string s)
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

        public void setCardNumb(long s)
        {
            m_cardNumber = s;
        }

        public string getCardDescrip()
        {
            return m_cardDescription;
        }

        public void setCardDescrip(string s)
        {
            m_cardDescription = s;
        }

    }
}
