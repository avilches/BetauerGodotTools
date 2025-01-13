using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public record DiscardOptionsResult(
    List<DiscardOption> Discards,
    TimeSpan ElapsedTime,
    int TotalSimulations,
    int TotalCombinations);