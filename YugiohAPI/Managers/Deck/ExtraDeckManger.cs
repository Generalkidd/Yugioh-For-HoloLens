using System.Collections.Generic;
using YugiohAPI.Model.Cards;

namespace YugiohAPI.Managers.Deck
{
	public class ExtraDeckManger : CardPileManager
	{
		public ExtraDeckManger(List<Card> cards) : base(cards)
		{
		}
	}
}