using System.Collections.Generic;

namespace Yuizumi.TexasHoldem
{
    public static class HandScore
    {
        internal static int[][][][][] Table = BuildTable();

        public static int GetScore(IReadOnlyList<Card> hand)
        {
            VerifyArg.NoneNull(hand, nameof(hand));
            VerifyArg.Count(hand, nameof(hand), 5);
            return Table[hand[0].Index][hand[1].Index][hand[2].Index][hand[3].Index]
                [hand[4].Index];
        }

        private const int NoPair        = 0x000000;
        private const int OnePair       = 0x100000;
        private const int TwoPair       = 0x200000;
        private const int ThreeOfAKind  = 0x300000;
        private const int Straight      = 0x400000;
        private const int Flush         = 0x500000;
        private const int FullHouse     = 0x600000;
        private const int FourOfAKind   = 0x700000;
        private const int StraightFlush = 0x900000;  // Straight + Flush.

        private const int SuitMask = 15;

        private static int[][][][][] BuildTable()
        {
            var layer5 = new ObjectPool<int, int>(ComputeScore);
            var layer4 = new ObjectPool<int, int[]>(key => {
                var array = new int[Card.Deck.Count];
                for (int i = 0; i < Card.Deck.Count; i++)
                    array[i] = layer5.Get(MergeKey(key, Card.Deck[i]));
                return array;
            });
            var layer3 = new ObjectPool<int, int[][]>(key => {
                var array = new int[Card.Deck.Count][];
                for (int i = 0; i < Card.Deck.Count; i++)
                    array[i] = layer4.Get(MergeKey(key, Card.Deck[i]));
                return array;
            });
            var layer2 = new ObjectPool<int, int[][][]>(key => {
                var array = new int[Card.Deck.Count][][];
                for (int i = 0; i < Card.Deck.Count; i++)
                    array[i] = layer3.Get(MergeKey(key, Card.Deck[i]));
                return array;
            });
            var layer1 = new ObjectPool<int, int[][][][]>(key => {
                var array = new int[Card.Deck.Count][][][];
                for (int i = 0; i < Card.Deck.Count; i++)
                    array[i] = layer2.Get(MergeKey(key, Card.Deck[i]));
                return array;
            });

            var array0 = new int[Card.Deck.Count][][][][];
            for (int i = 0; i < Card.Deck.Count; i++) {
                Card card = Card.Deck[i];
                array0[i] = layer1.Get((card.RankIndex << 4) | card.SuitIndex);
            }

            return array0;
        }
    
        private static int MergeKey(int key, Card card)
        {
            int rank = card.RankIndex;
            int k = 4;
            while (((key >> k) & 15) > rank)
                k += 4;
            int mask = (1 << k) - 1;
            key = ((key & ~mask) << 4) | (rank << k) | (key & mask);
            if ((key & SuitMask) != card.SuitIndex) key |= SuitMask;
            return key;
        }

        private static int ComputeScore(int key)
        {
            int[] ranks = new int[5];
            int[] freqs = new int[13];
            for (int i = 0; i < 5; i++) {
                ranks[i] = (key >> 4 >> (4 * i)) & 15;
                ++freqs[ranks[i]];
            }
            for (int i = 0; i < 5; i++) {
                for (int j = i; j > 0; j--) {
                    int ranks_j = ranks[j];
                    if (freqs[ranks[j - 1]] >= freqs[ranks_j])
                        break;
                    ranks[j] = ranks[j - 1];
                    ranks[j - 1] = ranks_j;
                }
            }

            int score = (ranks[4] << 0) | (ranks[3] << 4) | (ranks[2] << 8)
                | (ranks[1] << 12) | (ranks[0] << 16);
            if (freqs[ranks[0]] == 1) {
                if (score == 0xC3210)  // Reorder to 5-4-3-2-A.
                    score = 0x3210C;
                if (ranks[0] - ranks[4] == 4 || score == 0x3210C)
                    score |= Straight;
            } else if (freqs[ranks[0]] == 2) {
                score |= (freqs[ranks[2]] == 2) ? TwoPair : OnePair;
            } else if (freqs[ranks[0]] == 3) {
                score |= (freqs[ranks[3]] == 2) ? FullHouse : ThreeOfAKind;
            } else if (freqs[ranks[0]] == 4) {
                score |= FourOfAKind;
            }
            bool isFlush = (key & SuitMask) != SuitMask;
            return (isFlush) ? (score + Flush) : score;
        }
    }
}
