using System;
using System.Collections.Generic;

namespace Veronenger.Game.Deck.Hands;

public record DiscardOptionsResult(
    List<DiscardOption> Discards,
    TimeSpan ElapsedTime,
    int TotalSimulations,
    int TotalCombinations);