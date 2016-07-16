using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.BattleHandler.Cards
{
    public class Card
    {
        public string CardName
        {
            get; internal set;
        }

        public CardAttributeOrType Attribute
        {
            get; internal set;
        }

        public long CardNumber
        {
            get; internal set;
        }
        public string CardDescription
        {
            get; internal set;
        }

        public Texture CardImage
        {
            get; internal set;
        }
    }
}
