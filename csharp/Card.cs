using System.Collections.Generic;
using System.Linq;

namespace Yuizumi.TexasHoldem
{
    public enum Rank
    {
        _2, _3, _4, _5, _6, _7, _8, _9, _T, _J, _Q, _K, _A,
    }

    public enum Suit
    {
        Spades, Hearts, Diamonds, Clubs,
    }

    public sealed class Card
    {
        private const string RankChars = "23456789TJQKA";
        private const string SuitChars = "SHDC";

        private Card(int index)
        {
            Index = index;
        }

        static Card()
        {
            Deck = Enumerable.Range(0, 52).Select(index => new Card(index))
                .ToList().AsReadOnly();
        }

        public static IReadOnlyList<Card> Deck { get; }

        public int Index { get; }

        internal int RankIndex
        {
            get { return Index >> 2; }
        }

        internal int SuitIndex
        {
            get { return Index & 3; }
        }

        public Rank Rank
        {
            get { return (Rank) RankIndex; }
        }

        public Suit Suit
        {
            get { return (Suit) SuitIndex; }
        }

        public static Card Parse(string value)
        {
            if (value.Length != 2)
                return null;
            int rank = RankChars.IndexOf(value[0]);
            int suit = SuitChars.IndexOf(value[1]);
            if (rank == -1 || suit == -1)
                return null;
            return Deck[(rank << 2) | suit];
        }

        public override string ToString()
        {
            return $"{RankChars[RankIndex]}{SuitChars[SuitIndex]}";
        }
    }
}
