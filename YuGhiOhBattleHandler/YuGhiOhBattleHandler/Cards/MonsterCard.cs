using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuGhiOhBattleHandler
{

    public enum Mode
    {
        Attack,
        Defense
    }


    /// <summary>
    /// Monster Cards are used to battle and defeat your opponent. Battles 
    /// between Monster Cards are the foundation of any Duel.
    /// </summary>
    public sealed class MonsterCard
    {
        private Card c = new Card();
        private int m_level;
        private string m_type;
        private int m_attackPoints;
        private int m_defensePoints;
        private bool m_isPendulum;
        private bool m_isXyz;
        private bool m_isSynchro;
        private bool m_isSynchroTuner;
        private bool m_isFusion;
        private bool m_isRitual;
        private bool m_isPlayed;
        private bool m_isAttackMode;


        internal MonsterCard(string cardName, int cardLevel, CardAttributeOrType attributeOrType, string cardType,int attackPoints, int defensePoints, string cardDescription, long cardNumber, bool isPendulum, bool isXyz, bool isSynchro, bool isSynchroTuner, bool isFusion, bool isRitual)
        {
            c.setCardName(cardName);
            m_type = cardType;
            m_attackPoints = attackPoints;
            m_defensePoints = defensePoints;
            c.setAttributeOrType(attributeOrType);
            c.setCardDescrip(cardDescription);
            c.setCardNumb(cardNumber);
            m_level = cardLevel;
            m_isPendulum = isPendulum;
            m_isXyz = isXyz;
            m_isSynchro = isSynchro;
            m_isSynchroTuner = isSynchroTuner;
            m_isPlayed = false;
            m_isAttackMode = false;
        }

        public void Play(Player user, Player opponent)
        {

        }
        public bool ChangeBattlePosition(Mode mode)
        {
            return true;
        }

        public bool Battle(Player user, Player opponent, int indexOfOpponentsMonsterToBattle)
        {
            return true;
        }

        public void Play(Player user, Player opponent, Mode mode)
        {

        }
    }
}
