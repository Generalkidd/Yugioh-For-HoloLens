using System;
using YugiohAPI.Model.Cards;
using System.Collections.Generic;

namespace YugiohAPI.Managers
{
	internal class MonsterCardInstance : CardInstance
	{
		public MonsterMode Mode { get; private set; }
		public List<ModifierEffect> ModifierEffects { get; private set; }
		public int DefensePoints { get { return calculateDefensePoints(); } private set { } }
		public int AttackPoints { get { return calculateAttackPoints(); } private set { } }
		public bool CanAttack { get; private set; }
		public bool CanSwitchModes { get; private set; }

		public MonsterCardInstance(MonsterCard monsterCard): base(monsterCard)
		{
		}

		private int calculatePoints(int points, int modifier, ModifierKind kind)
		{
			switch (kind)
			{
				case ModifierKind.Add:
					points += modifier;
					break;
				case ModifierKind.Subtract:
					points -= modifier;
					break;
				case ModifierKind.Multiply:
					points *= modifier;
					break;
				case ModifierKind.Divide:
					points /= modifier;
					break;
			}
			return points;
		}

		private int calculateDefensePoints()
		{
			int points = (Card as MonsterCard).DefensePoints;
			foreach(ModifierEffect modifier in ModifierEffects)
			{
				points = calculatePoints(points, modifier.DefenseModifier, modifier.ModifierKind);
			}
			return points;
		}

		private int calculateAttackPoints()
		{
			int points = (Card as MonsterCard).AttackPoints;
			foreach (ModifierEffect modifier in ModifierEffects)
			{
				points = calculatePoints(points, modifier.AttackModifier, modifier.ModifierKind);
			}
			return points;
		}

		public override void Set()
		{
			if (!IsActivated)
			{
				SwitchMode(MonsterMode.Defense);
				base.Set();
			}
			else
			{
				throw new InvalidOperationException("You can't set a card once it's been activated");
			}
		}

		public void Summon()
		{
			if (!IsActivated)
			{
				SwitchMode(MonsterMode.Attack);
				base.Activate();
			}
			else
			{
				throw new InvalidOperationException("You can't summon a card twice");
			}
		}

		public void FlipSummon()
		{
			if (!IsActivated)
			{
				SwitchMode(MonsterMode.Attack);
				base.Activate();
			}
			else
			{
				throw new InvalidOperationException("You can't summon a card twice");
			}
		}

		public void SwitchMode(MonsterMode mode)
		{
			if (CanSwitchModes)
			{
				Mode = mode;
				CanSwitchModes = false;
			}
			else
			{
				throw new InvalidOperationException("You can't switch the mode of a card which has taken action this turn");
			}
		}
	}
}