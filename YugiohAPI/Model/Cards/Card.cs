using YugiohAPI.Model.Cards.Enums;

namespace YugiohAPI.Model.Cards
{
	public class Card
	{
		public string Name { get; set; }
		public int Number { get; set; }
		public CardRarity Rarity { get; set; }
		public string Description { get; set; }
	}
}
