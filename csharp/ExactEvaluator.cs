using System.Collections.Generic;
using System.Linq;

namespace Yuizumi.TexasHoldem
{
    public sealed class ExactEvaluator : IEvaluator
    {
        public static IEvaluator Evaluator = new ExactEvaluator();

        private ExactEvaluator()
        {
        }

        public IReadOnlyList<Stats> ComputeStats(Table table)
        {
            VerifyArg.NotNull(table, nameof(table));
            return ComputeStatsInternal(
                table.Pockets.Select(ToIndex).ToArray(), ToIndex(table.Community));
        }

        private static int[] ToIndex(IEnumerable<Card> cards)
        {
            return cards.Select(card => card.Index).ToArray();
        }

        private static IReadOnlyList<Stats> ComputeStatsInternal(
            int[][] pockets, int[] commGiven)
        {
            int numPlayers = pockets.Length;
            var stats = new Stats[numPlayers];

            var table1 = new int[numPlayers][][][][];
            var table2 = new int[numPlayers][][][][];
            var table12 = new int[numPlayers][][][];
            for (int i = 0; i < numPlayers; i++) {
                table1[i] = HandScore.Table[pockets[i][0]];
                table2[i] = HandScore.Table[pockets[i][1]];
                table12[i] = table1[i][pockets[i][1]];
            }

            var community = new int[5];
            commGiven.CopyTo(community, 0);

            IEnumerable<int> used = pockets.SelectMany(pocket => pocket).Concat(commGiven);
            int[] deck = Enumerable.Range(0, Card.Deck.Count).Except(used).ToArray();

            var scores = new int[numPlayers];
            int numCombs = 0;
            foreach (var _ in GenerateCommunity(community, commGiven.Length, deck)) {
                int c1 = community[0];
                int c2 = community[1];
                int c3 = community[2];
                int c4 = community[3];
                int c5 = community[4];

                int baseScore = HandScore.Table[c1][c2][c3][c4][c5];
                int maxScore = 0;
                int winner = -1;
                for (int i = 0; i < numPlayers; i++) {
                    scores[i] = GetScore(table1[i], table2[i], table12[i], c1, c2, c3, c4, c5,
                        baseScore);
                    if (scores[i] > maxScore) {
                        maxScore = scores[i];
                        winner = i;
                    } else if (scores[i] == maxScore) {
                        winner = -1;
                    }
                }
                if (winner >= 0) {
                    ++stats[winner].Win;
                } else {
                    for (int i = 0; i < numPlayers; i++) {
                        if (scores[i] == maxScore)
                            ++stats[i].Split;
                    }
                }
                ++numCombs;
            }

            for (int i = 0; i < numPlayers; i++)
                stats[i].Lose = numCombs - (stats[i].Win + stats[i].Split);

            return stats;
        }

        private static IEnumerable<object> GenerateCommunity(
            int[] community, int start, int[] deck)
        {
            var indices = new int[5];

            for (int i = start; i < 5; i++)
                community[i] = deck[indices[i] = i - start];
            yield return null;

            int maxOrigin = deck.Length - 5;
            while (true) {
                int origin, i = 5;
                do {
                    if (--i < start)
                        yield break;
                    origin = indices[i] - i + 1;
                } while (origin > maxOrigin);

                for (; i < 5; i++)
                    community[i] = deck[indices[i] = origin + i];
                yield return null;
            }
        }

        private static int GetScore(int[][][][] _p1_, int[][][][] _p2_, int[][][] _p1__p2_,
            int c1, int c2, int c3, int c4, int c5, int baseScore)
        {
            var _p1__c1_ = _p1_[c1];
            var _p2__c1_ = _p2_[c1];
            var _p1__p2__c1_ = _p1__p2_[c1];

            int maxScore = baseScore, score;
            score = _p1__p2__c1_[c2][c3]; if (maxScore < score) maxScore = score;
            score = _p1__p2__c1_[c2][c4]; if (maxScore < score) maxScore = score;
            score = _p1__p2__c1_[c2][c5]; if (maxScore < score) maxScore = score;
            score = _p1__p2__c1_[c3][c4]; if (maxScore < score) maxScore = score;
            score = _p1__p2__c1_[c3][c5]; if (maxScore < score) maxScore = score;
            score = _p1__p2__c1_[c4][c5]; if (maxScore < score) maxScore = score;
            score = _p1__p2_[c2][c3][c4]; if (maxScore < score) maxScore = score;
            score = _p1__p2_[c2][c3][c5]; if (maxScore < score) maxScore = score;
            score = _p1__p2_[c2][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p1__p2_[c3][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p1__c1_[c2][c3][c4]; if (maxScore < score) maxScore = score;
            score = _p1__c1_[c2][c3][c5]; if (maxScore < score) maxScore = score;
            score = _p1__c1_[c2][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p1__c1_[c3][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p1_[c2][c3][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p2__c1_[c2][c3][c4]; if (maxScore < score) maxScore = score;
            score = _p2__c1_[c2][c3][c5]; if (maxScore < score) maxScore = score;
            score = _p2__c1_[c2][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p2__c1_[c3][c4][c5]; if (maxScore < score) maxScore = score;
            score = _p2_[c2][c3][c4][c5]; if (maxScore < score) maxScore = score;
          
            return maxScore;
        }
    }
}
