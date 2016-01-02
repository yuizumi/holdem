// -*- C++ -*-
#ifndef EVAL_H_
#define EVAL_H_

#include <vector>
#include "card.h"

namespace texas_holdem {

struct Stats
{
    int win, lose, split;

    double win_prob() const { return win / denom(); }
    double lose_prob() const { return lose / denom(); }
    double split_prob() const { return split / denom(); }

private:
    double denom() const { return win + lose + split; }
};

std::vector<Stats> ComputeStats(const std::vector<Pocket>& pockets,
                                const std::vector<Card>& community);

}  // namespace texas_holdem

#endif  // EVAL_H_
