using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Represents a possible discard option with its potential outcomes.
/// Includes statistics about possible hand improvements and their probabilities.
/// </summary>
public class DiscardOption {
    public List<Card> CardsToKeep { get; }
    public List<Card> CardsToDiscard { get; }
    public ReadOnlyDictionary<PokerHandType, HandTypeStats> HandOccurrences { get; }
    public int TotalSimulations { get; }
    public int TotalCombinations { get; }

    public DiscardOption(
        List<Card> cardsToKeep,
        List<Card> cardsToDiscard,
        Dictionary<PokerHandType, HandTypeStats> handOccurrences,
        int totalSimulations,
        int totalCombinations
    ) {
        CardsToKeep = cardsToKeep;
        CardsToDiscard = cardsToDiscard;
        HandOccurrences = handOccurrences.AsReadOnly();
        TotalSimulations = totalSimulations;
        TotalCombinations = totalCombinations;
        foreach (var (_, stats) in HandOccurrences) {
            stats.Probability = (float)stats.Count / TotalSimulations;
        }
    }

    /// <summary>
    /// Returns all the hands sorted by potential score
    /// </summary>
    public IOrderedEnumerable<HandTypeStats> GetBestHands() {
        return HandOccurrences.Values.OrderByDescending(stats => stats.PotentialScore);
    }

    /// <summary>
    /// Returns the best hand by potential score 
    /// </summary>
    /// <returns></returns>
    public HandTypeStats GetBestHand() {
        return GetBestHands().First();
    }

    /// <summary>
    /// Returns the hand stat by type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public HandTypeStats? GetBestHand(PokerHandType type) {
        return HandOccurrences.GetValueOrDefault(type, null);
    }
}