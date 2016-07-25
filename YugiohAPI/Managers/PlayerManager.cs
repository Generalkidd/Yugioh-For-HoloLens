using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YugiohAPI.Managers.Deck;
using YugiohAPI.Managers.Zones;
using YugiohAPI.Model.Cards;
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

		public PlayerManager(Player player)
		{
			Player = player;
			Deck = new DeckManager(Player.Deck.MainDeck);
			Graveyard = new GraveyardManager();
			Hand = new HandManager();
			MonsterZone = new MonsterZoneManager();
			SpellZone = new SpellZoneManager();
			FieldZone = new FieldZoneManager();
			ExtraDeck = new ExtraDeckManger(Player.Deck.ExtraDeck);
			PendulumDeck = new PendulumDeckManager(Player.Deck.PendulumDeck);
		}

		public void Draw(int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				Card card = Deck.Take();
				Hand.Add(card);
			}
		}

		public void DiscardSpecific(Card card)
		{
			Card handCard = Hand.RetriveSpecific(card);
			Graveyard.Add(handCard);
		}

		public void Discard(int cardNumber)
		{
			Card handCard = Hand.Retrive(cardNumber);
			Graveyard.Add(handCard);
		}

		public CardInstance SetCard(Card card)
		{
			Type cardType = card.GetType();
			CardInstance cardInstance = null;
			if(cardType == typeof(MonsterCard))
			{
				cardInstance = new MonsterCardInstance(card as MonsterCard);
			}
			else if(cardType == typeof(SpellCard))
			{
				cardInstance = new SpellCardInstance(card as SpellCard);
			}
			else if (cardType == typeof(TrapCard))
			{
				cardInstance = new TrapCardInstance(card as TrapCard);
			}

			cardInstance.Set();

			return cardInstance;
		}
	}
}
