using System.Collections.Generic;
using YugiohAPI.Model.Cards;

namespace YugiohAPI.Managers.Deck
{
	public class DeckManager : CardPileManager
	{
		public DeckManager(List<Card> cards) : base(cards)
		{
		}
	}
}