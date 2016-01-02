#include "card.h"

#include <cassert>
#include <cstring>

using std::string;
using std::vector;

namespace texas_holdem {

namespace {
const char kRankChars[] = "23456789TJQKA";
const char kSuitChars[] = "SHDC";
}  // namespace

bool Card::Parse(const string& name, Card* card)
{
    if (name.length() != 2)
        return false;
    const char* rank = strchr(kRankChars, name[0]);
    const char* suit = strchr(kSuitChars, name[1]);
    if (!rank || !suit)
        return false;
    card->rank_ = static_cast<Rank>(rank - kRankChars);
    card->suit_ = static_cast<Suit>(suit - kSuitChars);
    return true;
}

string Card::ToString() const
{
    char name[2] = {kRankChars[rank()], kSuitChars[suit()]};
    return string(name, 2);
}

const vector<Card>& Card::Deck()
{
    static const vector<Card> deck(CreateDeck());
    return deck;
}

vector<Card> Card::CreateDeck()
{
    const int num_cards = kNumRanks * kNumSuits;
    vector<Card> deck;
    deck.reserve(num_cards);
    for (int rank = 0; rank < kNumRanks; rank++) {
        for (int suit = 0; suit < kNumSuits; suit++) {
            deck.push_back(Card(static_cast<Rank>(rank),
                                static_cast<Suit>(suit)));
            assert(deck.back().index() == static_cast<int>(deck.size()) - 1);
        }
    }
    return deck;
}

}  // namespace texas_holdem
