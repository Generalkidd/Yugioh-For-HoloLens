using System.Collections.Generic;
using YugiohAPI.Model.Cards;

namespace YugiohAPI.Model.Player
{
	public class Deck
	{
		public List<Card> MainDeck { get; set; }
		public List<Card> ExtraDeck { get; set; }
		public List<Card> PendulumDeck { get; set; }
	}
}