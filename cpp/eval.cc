#include "eval.h"

#include <algorithm>
#include <array>
#include <cassert>
#include <set>
#include "hand.h"

using std::array;
using std::set;
using std::vector;

namespace texas_holdem {
namespace {

class CommunityGenerator {
public:
    CommunityGenerator(Community* community, int start,
                       const vector<Card>& deck)
        : community_(community), start_(start), deck_(deck),
          max_origin_(deck.size() - 5)
    {
        assert(deck_.size() >= 5 - start);
        for (int i = start; i < 5; i++) {
            (*community_)[i] = deck_[cursor_[i] = i - start];
        }
    }

    bool MoveNext()
    {
        for (int i = 4; i >= start_; i--) {
            const int origin = cursor_[i] - i + 1;
            if (origin > max_origin_)
                continue;
            for (int j = i; j < 5; j++) {
                (*community_)[j] = deck_[cursor_[j] = origin + j];
            }
            return true;
        }
        return false;
    }

private:
    Community* community_;
    const int start_;
    const vector<Card>& deck_;
    const int max_origin_;
    array<int, 5> cursor_;
};

vector<Card> GetUnusedCards(const vector<Pocket>& pockets,
                            const vector<Card>& community)
{
    vector<Card> deck(Card::Deck());
    set<int> used_indices;

    for (const Pocket& pocket : pockets) {
        used_indices.insert(pocket[0].index());
        used_indices.insert(pocket[1].index());
    }
    for (const Card& comm_card : community)
        used_indices.insert(comm_card.index());

    const auto new_deck_end = std::remove_if(
        deck.begin(), deck.end(),
        [&](const Card& card) { return used_indices.count(card.index()); });
    deck.erase(new_deck_end, deck.end());

    return deck;
}

}  // namespace

vector<Stats> ComputeStats(const vector<Pocket>& pockets,
                           const vector<Card>& community)
{
    static const HandEvaluator hand_evaluator;

    const size_t num_players = pockets.size();
    vector<Stats> stats(num_players);

    vector<Card> deck(GetUnusedCards(pockets, community));
    Community community1;
    std::copy(community.begin(), community.end(), community1.begin());
    CommunityGenerator generator(&community1, community.size(), deck);

    vector<HandScore> scores(num_players);
    int num_combs = 0;
    do {
        HandScore max_score = 0;
        int winner = -1;
        for (size_t i = 0; i < num_players; i++) {
            scores[i] = hand_evaluator.Evaluate(pockets[i], community1);
            if (scores[i] > max_score) {
                max_score = scores[i];
                winner = i;
            } else if (scores[i] == max_score) {
                winner = -1;
            }
        }
        if (winner >= 0) {
            ++stats[winner].win;
        } else {
            for (size_t i = 0; i < num_players; i++) {
                if (scores[i] == max_score)
                    ++stats[i].split;
            }
        }
        ++num_combs;
    } while (generator.MoveNext());

    for (size_t i = 0; i < num_players; i++)
        stats[i].lose = num_combs - (stats[i].win + stats[i].split);

    return stats;
}

}  // namespace texas_holdem
