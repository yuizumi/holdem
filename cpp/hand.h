// -*- C++ -*-
#ifndef HAND_H_
#define HAND_H_

#include "card.h"
#include "macros.h"

namespace texas_holdem {

using HandScore = uint32_t;

class HandEvaluator
{
public:
    HandEvaluator();
    ~HandEvaluator() {}

    HandScore Evaluate(const Pocket& pocket, const Community& community) const;

private:
    HandScore table_[kNumRanks][kNumRanks][kNumRanks][kNumRanks][kNumRanks];

    HandScore Evaluate(const Card& c1, const Card& c2, const Card& c3,
                       const Card& c4, const Card& c5) const;

    DISALLOW_COPY_AND_ASSIGN(HandEvaluator);
};

}  // namespace texas_holdem

#endif  // HANDSCORE_H_
