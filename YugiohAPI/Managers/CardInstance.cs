using YugiohAPI.Model.Cards;

namespace YugiohAPI.Managers
{
	public class CardInstance
	{
		public Card Card { get; private set; }
		public bool IsSet { get; private set; }
		public bool IsActivated { get; private set; }

		public CardInstance(Card card)
		{
			Card = card;
		}

		public virtual void Set()
		{
			if (!IsActivated)
			{
				IsSet = true;
			}
		}

		public virtual void Activate()
		{
			IsSet = false;
			IsActivated = true;
		}
	}
}