This repository contains programs that compute the winning probability
of each player in Texas Hold 'em, given the pockets of all players and
optionally community cards.


Usage
-----

    holdem <Pocket1> <Pocket2> [...] [+<Community>]

`PocketN`/`Community` is a sequence of cards. Each card is specified by
two characters like `2S` or `JH`: the first character indicates the rank
(`T` for 10; obvious for others) and the second the suit (`S` for spades,
`H` for hearts, `D` for diamonds, and `C` for clubs).


Examples
--------

Preflop with 4 players; the first player (10♣2♦) will win the entire pot
(without any ties) by 17.66% and lose the pot by 80.68%:

    $ holdem TC2D 2HQH 7D5D 4H2S
    [0] TC2D:  17.66% /  80.68%
    [1] 2HQH:  35.09% /  63.25%
    [2] 7D5D:  34.06% /  65.11%
    [3] 4H2S:  11.53% /  86.82%

If the flop turns to be 3♦3♥A♦:

    $ holdem TC2D 2HQH 7D5D 4H2S +3D3HAD
    [0] TC2D:   6.10% /  86.34%
    [1] 2HQH:  28.78% /  63.66%
    [2] 7D5D:  44.27% /  50.98%
    [3] 4H2S:  13.29% /  79.15%
