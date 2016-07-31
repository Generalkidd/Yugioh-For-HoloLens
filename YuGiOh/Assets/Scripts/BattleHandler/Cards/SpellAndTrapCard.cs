using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BattleHandler.Cards
{
    public class SpellAndTrapCard:Card
    {
        public Icon Icon
        {
            get; internal set;
        }

        public MonsterCard EquippedTo
        {
            get; internal set;
        }

        internal SpellAndTrapCard(string cardName, CardAttributeOrType attributeOrType, Icon ico, string cardDescription, long cardNumber, Texture bi)
        {
            CardName=cardName;
            Icon = ico;
            Attribute=attributeOrType;
            CardDescription=cardDescription;
            CardNumber=cardNumber;
            CardImage = bi;
        }
    }
}
