#include "hand.h"

#include <algorithm>
#include <array>
#include <numeric>

using std::array;

namespace texas_holdem {

namespace {
enum : HandScore {
    kNoPair        = 0x000000,
    kOnePair       = 0x100000,
    kTwoPair       = 0x200000,
    kThreeOfAKind  = 0x300000,
    kStraight      = 0x400000,
    kFlush         = 0x500000,
    kFullHouse     = 0x600000,
    kFourOfAKind   = 0x700000,
    kStraightFlush = 0x900000,  // kStraight + kFlush
};
}  // namespace

HandEvaluator::HandEvaluator()
{
    for (int r1 = 0; r1 < kNumRanks; r1++)
    for (int r2 = 0; r2 <= r1; r2++)
    for (int r3 = 0; r3 <= r2; r3++)
    for (int r4 = 0; r4 <= r3; r4++)
    for (int r5 = 0; r5 <= r4; r5++) {
        array<int, 5> ranks = {{r1, r2, r3, r4, r5}};
        array<int, kNumRanks> freqs = {{0}};
        for (size_t i = 0; i < 5; i++)
            ++freqs[ranks[i]];
        std::stable_sort(ranks.begin(), ranks.end(), [&freqs](int x, int y) {
            return freqs[x] > freqs[y];
        });

        HandScore rank_score = std::accumulate(
            ranks.begin(), ranks.end(), 0,
            [](HandScore acc, int rank) { return (acc << 4) | rank; });
        HandScore main_score = kNoPair;
        if (freqs[ranks[0]] == 1) {
            if (rank_score == 0xC3210)  // Reorder to 5-4-3-2-A.
                rank_score = 0x3210C;
            if (ranks[0] - ranks[4] == 4 || rank_score == 0x3210C)
                main_score = kStraight;
        } else if (freqs[ranks[0]] == 2) {
            main_score = (freqs[ranks[2]] == 2) ? kTwoPair : kOnePair;
        } else if (freqs[ranks[0]] == 3) {
            main_score = (freqs[ranks[3]] == 2) ? kFullHouse : kThreeOfAKind;
        } else if (freqs[ranks[0]] == 4) {
            main_score = kFourOfAKind;
        }

        const HandScore score = main_score | rank_score;
        ranks = {{r5, r4, r3, r2, r1}};
        do {
            table_[ranks[0]][ranks[1]][ranks[2]][ranks[3]][ranks[4]] = score;
        } while (std::next_permutation(ranks.begin(), ranks.end()));
    }
}

HandScore HandEvaluator::Evaluate(const Pocket& pocket,
                                  const Community& community) const
{
    const Card& c1 = pocket[0];
    const Card& c2 = pocket[1];
    const Card& c3 = community[0];
    const Card& c4 = community[1];
    const Card& c5 = community[2];
    const Card& c6 = community[3];
    const Card& c7 = community[4];

    HandScore score = 0;
    using std::max;
    score = max(score, Evaluate(c1, c2, c3, c4, c5));
    score = max(score, Evaluate(c1, c2, c3, c4, c6));
    score = max(score, Evaluate(c1, c2, c3, c4, c7));
    score = max(score, Evaluate(c1, c2, c3, c5, c6));
    score = max(score, Evaluate(c1, c2, c3, c5, c7));
    score = max(score, Evaluate(c1, c2, c3, c6, c7));
    score = max(score, Evaluate(c1, c2, c4, c5, c6));
    score = max(score, Evaluate(c1, c2, c4, c5, c7));
    score = max(score, Evaluate(c1, c2, c4, c6, c7));
    score = max(score, Evaluate(c1, c2, c5, c6, c7));
    score = max(score, Evaluate(c1, c3, c4, c5, c6));
    score = max(score, Evaluate(c1, c3, c4, c5, c7));
    score = max(score, Evaluate(c1, c3, c4, c6, c7));
    score = max(score, Evaluate(c1, c3, c5, c6, c7));
    score = max(score, Evaluate(c1, c4, c5, c6, c7));
    score = max(score, Evaluate(c2, c3, c4, c5, c6));
    score = max(score, Evaluate(c2, c3, c4, c5, c7));
    score = max(score, Evaluate(c2, c3, c4, c6, c7));
    score = max(score, Evaluate(c2, c3, c5, c6, c7));
    score = max(score, Evaluate(c2, c4, c5, c6, c7));
    score = max(score, Evaluate(c3, c4, c5, c6, c7));

    return score;
}

HandScore HandEvaluator::Evaluate(const Card& c1, const Card& c2, const Card& c3,
                                  const Card& c4, const Card& c5) const
{
    const bool is_flush = (c1.suit() == c2.suit() && c2.suit() == c3.suit() &&
                           c3.suit() == c4.suit() && c4.suit() == c5.suit());
    HandScore score = table_[c1.rank()][c2.rank()][c3.rank()][c4.rank()][c5.rank()];
    return (is_flush) ? (score + kFlush) : score;
}

}  // namespace texas_holdem
