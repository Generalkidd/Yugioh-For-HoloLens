using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.BattleHandler.Cards
{
    public enum Result
    {
        Success,
        NotYourTurn,
        AlreadyNormalSummonedThisTurn,
        AlreadyPlayedMaxNumberOfMonsters,
        InvalidMove,
        IneligibleMonsterType,
        NeedMoreSacrifices,
        CantAttackBcAlreadyAttackedOrFirstTurnPlayed,
        OneOrMoreCardsAreNoLongerOnField,
        OpponentHasMonstersSoCannotTargetLifePoints
    }
}
