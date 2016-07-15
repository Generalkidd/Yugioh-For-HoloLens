using Assets.Scripts.BattleHandler.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.BattleHandler.Game
{
    public class ReadOnlyPlayer
    {
        public string UserName
        {
            get; internal set;
        }

        public int LifePoints
        {
            get; internal set;
        }

        public int NumberOfCardsInHand
        {
            get; internal set;
        }

        public int NumberOfFaceDownCardsInMonsterZone
        {
            get; internal set;
        }

        public List<MonsterCard> FaceUpMonsters
        {
            get; internal set;
        }

        public List<SpellAndTrapCard> FaceUpTraps
        {
            get; internal set;
        }

        public int NumberOfFaceDownTraps
        {
            get; internal set;
        }

        public bool AbleToDraw
        {
            get; internal set;
        }
    }
}
