using Assets.Scripts.BattleHandler.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.BattleHandler.Game
{
    public class ReadOnlyPlayer
    {

        internal ReadOnlyPlayer()
        {
            FaceUpMonsters = new List<MonsterCard>();
            FaceUpTraps = new List<SpellAndTrapCard>();
            LifePoints = 8000;
            NumberOfCardsInHand = 0;
            NumberOfFaceDownCardsInMonsterZone = 0;
            NumberOfFaceDownTraps = 0;
            AbleToDraw = true;
        }

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
