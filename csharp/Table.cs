using System.Collections.Generic;
using System.Linq;

namespace Yuizumi.TexasHoldem
{
    public class Table
    {
        public Table(IEnumerable<Pocket> pockets, IEnumerable<Card> community)
        {
            VerifyArg.NoneNull(pockets, nameof(pockets));
            VerifyArg.Nonempty(pockets, nameof(pockets));
            Pockets = pockets.ToList().AsReadOnly();

            VerifyArg.NoneNull(community, nameof(community));
            VerifyArg.True(community.Count() <= 5,
                nameof(community), "Collection must not contain more than 5 items.");
            Community = community.ToList().AsReadOnly();
        }

        public IReadOnlyList<Pocket> Pockets { get; }
        public IReadOnlyList<Card> Community { get; }
    }
}
