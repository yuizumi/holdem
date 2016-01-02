#include <cstdlib>
#include <set>
#include <string>
#include <vector>
#include "card.h"
#include "eval.h"

using std::set;
using std::string;
using std::vector;

namespace {
const char* g_argv0;

[[noreturn]] void Quit(const string& message)
{
    fprintf(stderr, "%s: %s\n", g_argv0, message.c_str());
    exit(1);
}
}  // namespace

namespace texas_holdem {

vector<Card> ParseCards(const string& arg)
{
    vector<Card> cards;
    for (size_t i = 0; i < arg.length(); i += 2) {
        const string card_str = arg.substr(i, 2);
        Card card;
        if (!Card::Parse(card_str, &card))
            Quit("Invalid card string -- " + card_str + ".");
        cards.push_back(card);
    }
    return cards;
}

void ParseArgs(const vector<string>& args,
               vector<Pocket>* pockets,
               vector<Card>* community)
{
    for (size_t i = 0; i < args.size(); i++) {
        if (args[i].c_str()[0] == '+') {
            if (i != args.size() - 1)
                Quit("Community must be specified last.");
            *community = ParseCards(args[i].substr(1));
            if (community->size() > 5)
                Quit("Too many community cards -- " + args[i] + ".");
        } else {
            vector<Card> cards = ParseCards(args[i]);
            if (cards.size() != 2)
                Quit("Not a valid pocket -- " + args[i] + ".");
            pockets->push_back({{cards[0], cards[1]}});
        }
    }

    if (pockets->size() < 2)
        Quit("Too few players.");

    set<int> used_indices;

    for (const Pocket& pocket : *pockets) {
        if (!used_indices.insert(pocket[0].index()).second)
            Quit("Card used multiple times -- " + pocket[0].ToString() + ".");
        if (!used_indices.insert(pocket[1].index()).second)
            Quit("Card used multiple times -- " + pocket[1].ToString() + ".");
    }
    for (const Card& comm_card : *community) {
        if (!used_indices.insert(comm_card.index()).second)
            Quit("Card used multiple times -- " + comm_card.ToString() + ".");
    }
}

void Main(const vector<string>& args)
{
    vector<Pocket> pockets;
    vector<Card> community;

    ParseArgs(args, &pockets, &community);

    const vector<Stats> stats = ComputeStats(pockets, community);
    for (size_t i = 0; i < pockets.size(); i++) {
        printf("[%d] %s: %6.2f%% / %6.2f%%\n",
               static_cast<int>(i),
               (pockets[i][0].ToString() + pockets[i][1].ToString()).c_str(),
               stats[i].win_prob() * 100.0, stats[i].lose_prob() * 100.0);
    }
}

}  // namespace texas_holdem


int main(int argc, char* argv[])
{
    g_argv0 = argv[0];
    texas_holdem::Main(vector<string>(argv + 1, argv + argc));
    return 0;
}
