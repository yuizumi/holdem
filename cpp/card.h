// -*- C++ -*-
#ifndef CARD_H_
#define CARD_H_

#include <array>
#include <string>
#include <vector>

namespace texas_holdem {

enum Rank
{
    kRank2, kRank3, kRank4, kRank5, kRank6, kRank7, kRank8, kRank9,
    kRank10, kRankJ, kRankQ, kRankK, kRankA, kNumRanks,
};

enum Suit
{
    kSpades, kHearts, kDiamonds, kClubs, kNumSuits,
};

class Card
{
public:
    Card() {}
    ~Card() {}

    Card(const Card&) = default;
    Card& operator=(const Card&) = default;

    Rank rank() const { return rank_; }
    Suit suit() const { return suit_; }
    int index() const { return (rank_ << 2) | suit_; }

    static bool Parse(const std::string& name, Card* card);
    std::string ToString() const;

    static const std::vector<Card>& Deck();

private:
    Rank rank_;
    Suit suit_;

    Card(Rank rank, Suit suit) : rank_(rank), suit_(suit) {}

    static std::vector<Card> CreateDeck();
};

using Pocket = std::array<Card, 2>;
using Community = std::array<Card, 5>;

}  // namespace texas_holdem

#endif  // CARD_H_
