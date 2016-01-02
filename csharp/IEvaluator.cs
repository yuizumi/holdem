using System.Collections.Generic;

namespace Yuizumi.TexasHoldem
{
    public interface IEvaluator
    {
        IReadOnlyList<Stats> ComputeStats(Table table);
    }
}
