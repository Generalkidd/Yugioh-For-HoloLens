using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YugiohAPI.Model.Cards;
using PCLCrypto;

namespace YugiohAPI.Managers
{
	public class CardPileManager
	{
		public List<Card> Cards { get; private set; }

		public int CardCount { get { return Cards.Count; } private set { } }

		public CardPileManager(List<Card> cards)
		{
			Cards = cards;
			Shuffle();
		}

		public virtual void Shuffle()
		{
			int n = Cards.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do
				{
					box = WinRTCrypto.CryptographicBuffer.GenerateRandom(1);
				}
				while (!(box[0] < n * (Byte.MaxValue / n)));

				int k = (box[0] % n);
				n--;
				Card value = Cards[k];
				Cards[k] = Cards[n];
				Cards[n] = value;
			}
		}

		public virtual void Add(Card card)
		{
			Cards.Insert(0, card);
		}

		public virtual Card Take()
		{
			Card card = Cards.First();
			Cards.Remove(card);
			return card;
		}

		public virtual Card Retrive(int cardNumber)
		{
			Card card = Cards.Find(c => c.Number == cardNumber);
			Cards.Remove(card);
			return card;
		}

		public virtual Card RetriveSpecific(Card card)
		{
			Card crd = Cards.Find(c => c == card);
			Cards.Remove(card);
			return card;
		}

		public virtual bool Contains(int cardNumber)
		{
			return Cards.Exists(c => c.Number == cardNumber);
		}
	}
}
