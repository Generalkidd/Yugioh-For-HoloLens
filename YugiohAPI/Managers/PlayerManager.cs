using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YugiohAPI.Model.Player;

namespace YugiohAPI.Managers
{
	public class PlayerManager
	{
		public Player Player { get; set; }
		public DeckManager Deck { get; set; }
		public GraveyardManager Graveyard { get; set; }
		public HandManager Hand { get; set; }
		public MonsterZoneManager MonsterZone { get; set; }
		public SpellZoneManager SpellZone { get; set; }
		public FieldZoneManager FieldZone { get; set; }
		public ExtraDeckManger ExtraDeck { get; set; }
		public PendulumDeckManager PendulumDeck	{ get; set; }
	}
}
