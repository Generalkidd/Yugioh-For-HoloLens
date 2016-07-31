using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BattleHandler.Cards
{
    public class MonsterCard : Card
    {
        public int Level
        {
            get; internal set;
        }

        public string Type
        {
            get; internal set;
        }

        public int AttackPoints
        {
            get; internal set;
        }

        public int DefensePoints
        {
            get; internal set;
        }

        public bool Pendulum
        {
            get; internal set;
        }

        public bool XYZ
        {
            get; internal set;
        }

        public bool Fusion
        {
            get; internal set;
        }

        public bool Ritual
        {
            get; internal set;
        }

        public Face FacePosition
        {
            get; internal set;
        }

        public bool CanAttack
        {
            get; internal set;
        }

        public bool Synchro
        {
            get; internal set;
        }

        public bool SynchroTuner
        {
            get; internal set;
        }

        public Mode Mode
        {
            get; internal set;
        }

        public SpellAndTrapCard EquippedTo
        {
            get; internal set;
        }

        internal MonsterCard(string cardName, int cardLevel, CardAttributeOrType attributeOrType, string cardType, int attackPoints, int defensePoints, string cardDescription, long cardNumber, bool isPendulum, bool isXyz, bool isSynchro, bool isSynchroTuner, bool isFusion, bool isRitual, Texture bi)
        {
            CardName=cardName;
            Type = cardType;
            AttackPoints = attackPoints;
            DefensePoints = defensePoints;
            Attribute=attributeOrType;
            CardDescription=cardDescription;
            CardNumber=cardNumber;
            Level = cardLevel;
            Pendulum = isPendulum;
            XYZ = isXyz;
            Synchro = isSynchro;
            SynchroTuner = isSynchroTuner;
            FacePosition = Face.Down;
            Mode = Mode.Defense;
            CardImage = bi;
            CanAttack = false;
        }

        internal void ChangeBattlePosition()
        {
            if (Mode == Mode.Attack)
            {
                Mode = Mode.Defense;
            }
            else
            {
                FacePosition = Face.Up;
                Mode = Mode.Attack;
            }
        }

    }
}
