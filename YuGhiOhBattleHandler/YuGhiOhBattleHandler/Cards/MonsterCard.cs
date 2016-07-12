using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace YuGhiOhBattleHandler
{

    public enum Mode
    {
        Attack,
        Defense
    }

    public enum Face
    {
        Up,
        Down
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
        private Mode m_Mode;
        private Face facePosition;
        private bool canAttack;

        internal MonsterCard(string cardName, int cardLevel, CardAttributeOrType attributeOrType, string cardType, int attackPoints, int defensePoints, string cardDescription, long cardNumber, bool isPendulum, bool isXyz, bool isSynchro, bool isSynchroTuner, bool isFusion, bool isRitual, BitmapImage bi)
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
            m_Mode = Mode.Attack;
            facePosition = Face.Down;
            c.setBitmapImage(bi);
            canAttack = false;
        }

        public bool CanAttack()
        {
            return canAttack;
        }

        internal void setCanAttack(bool toSet)
        {
            canAttack = toSet;
        }

        public int getLevel()
        {
            return m_level;
        }

        public int getAttackPoints()
        {
            return m_attackPoints;
        }

        internal void setAttackPoints(int toSet)
        {
            m_attackPoints = toSet;
        }

        public int getDefensePoints()
        {
            return m_defensePoints;
        }

        internal void setDefensePoints(int toSet)
        {
            m_defensePoints = toSet;
        }

        internal void FlipFaceUp()
        {
            facePosition = Face.Up;
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

        public Mode getBattlePosition()
        {
            return m_Mode;
        }

        internal void changeIsPlayed()
        {
            m_isPlayed = !m_isPlayed;
        }

        public bool getIsPlayed()
        {
            return m_isPlayed;
        }

        internal string getYuGhiOhType()
        {
            return m_type;
        }

        internal void ChangeBattlePosition()
        {
            if (m_Mode == Mode.Attack)
            {
                m_Mode = Mode.Defense;
            }
            else
            {
                m_Mode = Mode.Attack;
            }
        }
    }
}
