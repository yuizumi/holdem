using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Yuizumi.TexasHoldem
{
    public sealed class Pocket : ReadOnlyCollection<Card>
    {
        private Pocket(IList<Card> cards) : base(cards)
        {
        }

        public static Pocket Create(Card card1, Card card2)
        {
            VerifyArg.NotNull(card1, nameof(card1));
            VerifyArg.NotNull(card2, nameof(card2));
            return new Pocket(new List<Card>() {card1, card2});
        }

        public override string ToString()
        {
            return string.Join("", Items);
        }
    }
}
