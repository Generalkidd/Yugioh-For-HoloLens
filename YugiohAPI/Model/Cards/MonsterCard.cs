using System.Collections.Generic;
using YugiohAPI.Model.Cards.Enums;

namespace YugiohAPI.Model.Cards
{
	public class MonsterCard : Card
	{
		public int Level { get; set; }
		public MonsterAttribute Attribute { get; set; }
		public MonsterKind Kind { get; set; }
		public List<MonsterAbility> Abilities { get; set; }
		public int AttackPoints { get; set; }
		public int DefensePoints { get; set; }
	}
}
