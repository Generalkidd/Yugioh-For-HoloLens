using System.Collections.Generic;
using YugiohAPI.Model.Cards;

namespace YugiohAPI.Managers.Deck
{
	public class PendulumDeckManager : CardPileManager
	{
		public PendulumDeckManager(List<Card> cards) : base(cards)
		{
		}
	}
}