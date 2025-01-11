using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public record DiscardOptionsResult(
    List<DiscardOption> DiscardOptions,
    TimeSpan ElapsedTime,
    int TotalSimulations,
    int TotalCombinations) {
    public IOrderedEnumerable<DiscardOption> GetDiscards() {
        return DiscardOptions.OrderByDescending(option => option.GetBestHand().PotentialScore);
    }
}