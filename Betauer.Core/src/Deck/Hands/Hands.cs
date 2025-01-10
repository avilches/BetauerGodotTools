using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Tracks statistics for a specific hand type, including occurrences, scores, and probabilities.
/// Used in Monte Carlo simulations to evaluate potential hand improvements.
/// </summary>
public class HandTypeStats(int score) {
    public int Count { get; private set; } = 1;
    public int MaxScore { get; private set; } = score;
    public int MinScore { get; private set; } = score;
    public int AccumulatedScore { get; private set; } = score;
    public float AvgScore => (float)AccumulatedScore / Count;
    public float Probability { get; set; }
    public float PotentialScore => AvgScore * Probability;

    public void AddScore(int score) {
        Count++;
        MaxScore = Math.Max(MaxScore, score);
        MinScore = Math.Min(MinScore, score);
        AccumulatedScore += score;
    }
}

/// <summary>
/// Represents a possible discard option with its potential outcomes.
/// Includes statistics about possible hand improvements and their probabilities.
/// </summary>
public class DiscardOption {
    public List<Card> CardsToKeep { get; }
    public List<Card> CardsToDiscard { get; }
    public Dictionary<Type, HandTypeStats> HandOccurrences { get; }
    public int TotalSimulations { get; }
    public int TotalCombinations { get; }

    public DiscardOption(
        List<Card> cardsToKeep,
        List<Card> cardsToDiscard,
        Dictionary<Type, HandTypeStats> handOccurrences,
        int totalSimulations,
        int totalCombinations
    ) {
        CardsToKeep = cardsToKeep;
        CardsToDiscard = cardsToDiscard;
        HandOccurrences = handOccurrences;
        TotalSimulations = totalSimulations;
        TotalCombinations = totalCombinations;
        foreach (var (handType, stats) in HandOccurrences) {
            stats.Probability = (float)stats.Count / TotalSimulations;
        }
    }

    /// <summary>
    /// Returns the hands sorted by potential score with a probability greater than the risk threshold.
    /// </summary>
    /// <param name="risk">Minimum probability threshold (0.0 to 1.0). Higher values are more conservative.</param>
    /// <returns>The highest average score of hands that meet the probability threshold, or 0 if no hands qualify.</returns>
    public IOrderedEnumerable<HandTypeStats> GetBestHands(float risk) {
        return HandOccurrences.Values
            .Where(stats => stats.Probability > risk)
            .OrderByDescending(stats => stats.PotentialScore);
    }

    public HandTypeStats? GetBestHand(float risk) {
        return GetBestHands(risk).DefaultIfEmpty(null).First();
    }

    /// <summary>
    /// Returns the best potential score among hands that meet the probability threshold, or 0 if no hands qualify.
    /// </summary>
    /// <param name="risk">Minimum probability threshold (0.0 to 1.0). Higher values are more conservative.</param>
    /// <returns>The highest average score of hands that meet the probability threshold, or 0 if no hands qualify.</returns>
    public float GetBestPotentialScore(float risk) {
        return HandOccurrences.Values
            .Where(stats => stats.Probability > risk)
            .Select(stats => stats.PotentialScore)
            .DefaultIfEmpty(0)
            .Max();
    }
}

public static class CardExtensions {
    /// <summary>
    /// Finds all possible straight sequences in a set of cards.
    /// Handles Ace as both high (14) and low (1) when appropriate.
    /// 
    /// Algorithm:
    /// 1. Get unique ranks, adding Ace as 1 if Ace (14) exists
    /// 2. Look for 5-card sequences in order
    /// 3. For each sequence found:
    ///    - Map ranks back to actual cards
    ///    - Handle Ace properly (use 14 when rank is 1)
    /// </summary>
    public static IEnumerable<List<Card>> FindStraights(this IEnumerable<Card> cards) {
        var uniqueRanks = cards.Select(c => c.Rank)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        // Add Ace as 1 if Ace (14) exists
        if (uniqueRanks.Contains(14)) {
            uniqueRanks.Insert(0, 1);
        }

        // Look for 5-card sequences
        for (int i = 0; i <= uniqueRanks.Count - 5; i++) {
            var sequence = uniqueRanks.Skip(i).Take(5);
            if (sequence.Zip(sequence.Skip(1)).All(p => p.Second - p.First == 1)) {
                // For each rank in the straight, find matching card
                var straightCards = sequence.Select(rank =>
                    rank == 1
                        ? cards.First(c => c.Rank == 14) // Use Ace (14) when rank is 1
                        : cards.First(c => c.Rank == rank)
                ).ToList();

                yield return straightCards;
            }
        }
    }
}

/// <summary>
/// Base class for poker hands. Each specific hand type inherits from this.
/// Provides common functionality and defines the contract for hand identification
/// and improvement suggestions.
/// </summary>
public abstract class PokerHand(PokerHands pokerHands, string name, IReadOnlyList<Card> cards) {
    public PokerHands PokerHands { get; } = pokerHands;
    public IReadOnlyList<Card> Cards { get; } = cards;
    public string Name { get; set; } = name;

    public int CalculateScore() {
        if (PokerHands == null) throw new InvalidOperationException("PokerHand not initialized with PokerHands");
        return PokerHands.CalculateScore(this);
    }

    /// <summary>
    /// Identifies all possible hands of this type in the given cards.
    /// Must be implemented by each specific hand type.
    /// </summary>
    public abstract List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards);

    /// <summary>
    /// Public method to get discard suggestions. First checks if the hand already exists.
    /// If it does, returns empty list (no discards needed).
    /// If it doesn't, calls the specific hand type's SuggestDiscards method.
    /// </summary>
    public List<List<Card>> GetBestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        // If this hand type already exists, no discards needed
        if (IdentifyHands(currentHand).Count > 0) return [];
        // If not, delegate to specific implementation
        return SuggestDiscards(currentHand, maxDiscardCards);
    }

    /// <summary>
    /// Protected method that each hand type must implement to suggest discards.
    /// Called only when the hand doesn't already exist in the current cards.
    /// Should return list of cards to discard to potentially achieve this hand type.
    /// </summary>
    public abstract List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards);

    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }

    public override bool Equals(object obj) {
        if (obj is not PokerHand other) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (GetType() != obj.GetType()) return false;

        // Compare cards regardless of order
        return Cards.Count == other.Cards.Count &&
               Cards.All(other.Cards.Contains);
    }

    public override int GetHashCode() {
        // XOR of all card hashcodes (order independent)
        return Cards.Aggregate(GetType().GetHashCode(), (current, card) => current ^ card.GetHashCode());
    }
}

public class HighCardHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "High Card", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(pokerHands, new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        // Strategy: No discards for high card
        return [];
    }
}

public class PairHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Pair", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .SelectMany(g => g.ToList().Combinations(2))
            .Select(combo => new PairHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class TwoPairHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Two Pair", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var pairs = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Key)
            .ToList();

        if (pairs.Count < 2) return new List<PokerHand>();

        var result = new List<PokerHand>();

        // Para cada par más alto, combina con todos los pares más bajos
        for (int i = 0; i < pairs.Count - 1; i++) {
            var highPair = pairs[i].Take(2).ToList();
            for (int j = i + 1; j < pairs.Count; j++) {
                var lowPair = pairs[j].Take(2).ToList();
                result.Add(new TwoPairHand(PokerHands, highPair.Concat(lowPair).ToList()));
            }
        }

        return result;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class ThreeOfAKindHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Three of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => g.ToList().Combinations(3))
            .Select(combo => new ThreeOfAKindHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class StraightHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Straight", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.FindStraights()
            .Select(straightCards => new StraightHand(PokerHands, straightCards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var sequences = HandUtils.FindStraightSequences(currentHand);
        if (sequences.Count == 0) return [];

        var bestSequence = sequences[0];
        var cardsToKeep = bestSequence;
        return [HandUtils.GetDiscardsByKeeping(cardsToKeep, currentHand, maxDiscardCards)];
    }
}

public class FlushHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Flush", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                // Generar todas las combinaciones posibles de 5 cartas
                foreach (var combination in suitCards.Combinations(5)) {
                    hands.Add(new FlushHand(PokerHands, combination.ToList()));
                }
            }
        }
        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var largestGroup = HandUtils.FindLargestSuitGroup(currentHand);
        return [HandUtils.GetDiscardsByKeeping(largestGroup, currentHand, maxDiscardCards)];
    }
}

public class FullHouseHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Full House", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Key)
            .ToList();

        foreach (var threeGroup in groups.Where(g => g.Count() >= 3)) {
            // Generar todas las combinaciones posibles de 3 cartas para el trío
            foreach (var threeCombination in threeGroup.ToList().Combinations(3)) {
                foreach (var twoGroup in groups.Where(g => g.Key != threeGroup.Key && g.Count() >= 2)) {
                    // Generar todas las combinaciones posibles de 2 cartas para el par
                    foreach (var twoCombination in twoGroup.ToList().Combinations(2)) {
                        var fullHouseCards = threeCombination.Concat(twoCombination).ToList();
                        hands.Add(new FullHouseHand(PokerHands, fullHouseCards));
                    }
                }
            }
        }
        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class FourOfAKindHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Four of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4)
            .SelectMany(g => g.ToList().Combinations(4))
            .Select(combo => new FourOfAKindHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class StraightFlushHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Straight Flush", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => g.ToList().FindStraights())
            .Select(straightFlushCards => new StraightFlushHand(PokerHands, straightFlushCards))
            .Cast<PokerHand>()
            .ToList();
    }


    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var keeps = new List<List<Card>>();
        foreach (var suitGroup in currentHand.GroupBy(c => c.Suit)) {
            var suitCards = suitGroup.ToList();
            if (suitCards.Count >= 3) {
                var sequences = HandUtils.FindStraightSequences(suitCards);
                keeps.AddRange(sequences.Select(seq =>
                    HandUtils.GetDiscardsByKeeping(seq, currentHand, maxDiscardCards)));
            }
        }
        return keeps;
    }
}

/// <summary>
/// Utility class providing helper methods for poker hand analysis and manipulation.
/// </summary>
public static class HandUtils {
    /// <summary>
    /// Gets cards to discard based on which cards to keep.
    /// Respects maxDiscardCards limit.
    /// </summary>
    public static List<Card> GetDiscardsByKeeping(List<Card> cardsToKeep, IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var discards = currentHand.Where(c => !cardsToKeep.Contains(c)).ToList();
        if (discards.Count > maxDiscardCards) {
            // If too many discards, keep lowest cards up to maxDiscardCards
            discards = discards.OrderBy(c => c.Rank).Take(maxDiscardCards).ToList();
        }
        return discards;
    }

    /// <summary>
    /// Tries to discard non-duplicate cards up to maxDiscardCards.
    /// Used by hands that rely on matched cards (pairs, three of a kind, etc.)
    /// </summary>
    public static List<Card> TryDiscardNonDuplicates(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var repeatedGroups = FindRepeatedCards(currentHand);
        if (repeatedGroups.Count == 0) {
            // If no duplicates:
            // - Try to discard half of lowest cards
            // - But not more than maxDiscardCards
            var cardsToDiscard = Math.Min(maxDiscardCards, currentHand.Count / 2);
            return currentHand.OrderBy(c => c.Rank)
                .Take(cardsToDiscard)
                .ToList();
        }
        // Keep duplicated cards, discard others (up to maxDiscardCards)
        var duplicatedCards = repeatedGroups.SelectMany(g => g).ToList();
        return GetDiscardsByKeeping(duplicatedCards, currentHand, maxDiscardCards);
    }

    /// <summary>
    /// Returns groups of cards that share the same rank.
    /// Groups are ordered by size (largest first) and then by rank.
    /// </summary>
    public static List<IGrouping<int, Card>> FindRepeatedCards(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();
    }

    /// <summary>
    /// Finds potential straight sequences in cards. Returns list ordered by priority:
    /// 1. Complete straight (if exists, only returns this)
    /// 2. Groups of 4 consecutive with a gap and possible extra card
    /// 3. Groups of 3 or more consecutive
    /// 4. Groups of 2 consecutive with one gap between them
    /// </summary>
    public static List<List<Card>> FindStraightSequences(IReadOnlyList<Card> cards) {
        var sequences = new List<List<Card>>();
        var cardsByRank = cards.OrderBy(c => c.Rank).ToList();
        var ranks = cardsByRank.Select(c => c.Rank).Distinct().OrderBy(r => r).ToList();

        // Si tenemos un As (14), añadimos el 1 al principio
        var hasAce = ranks.Contains(14);
        if (hasAce) {
            ranks.Insert(0, 1);
        }

        // Buscar escaleras completas primero (sin huecos)
        for (int i = 0; i <= ranks.Count - 5; i++) {
            var window = ranks.Skip(i).Take(5).ToList();
            if (window.Zip(window.Skip(1)).All(p => p.Second - p.First == 1)) {
                sequences.Add(ConvertRanksToCards(cards, window));
            }
        }

        // Si no hay escaleras completas, buscar secuencias con un hueco
        if (sequences.Count == 0) {
            for (int i = 0; i <= ranks.Count - 5; i++) {
                var window = ranks.Skip(i).Take(6).ToList(); // Tomamos 6 para poder tener un hueco
                if (window.Count >= 5) {
                    // Probar cada posible posición del hueco
                    for (int gapPos = 0; gapPos < window.Count - 1; gapPos++) {
                        var sequence = new List<int>();
                        var hasGap = false;

                        for (int j = 0; j < window.Count - 1; j++) {
                            if (sequence.Count == 0) {
                                sequence.Add(window[j]);
                            }

                            var diff = window[j + 1] - window[j];
                            if (diff == 1) {
                                sequence.Add(window[j + 1]);
                            } else if (diff == 2 && !hasGap) {
                                // Encontramos un hueco válido
                                hasGap = true;
                                sequence.Add(window[j + 1]);
                            }
                        }

                        if (sequence.Count >= 5) {
                            sequences.Add(ConvertRanksToCards(cards, sequence.Take(5).ToList()));
                        }
                    }
                }
            }
        }

        // Si aún no hay secuencias, buscar secuencias de 4 cartas
        if (sequences.Count == 0) {
            for (int i = 0; i <= ranks.Count - 4; i++) {
                var window = ranks.Skip(i).Take(4).ToList();
                if (window.Zip(window.Skip(1)).All(p => p.Second - p.First == 1)) {
                    sequences.Add(ConvertRanksToCards(cards, window));
                }
            }
        }

        return sequences
            .OrderByDescending(seq => seq.Count)
            .ThenByDescending(seq => {
                if (seq.Any(c => c.Rank == 14) &&
                    seq.Select(c => c.Rank == 14 ? 1 : c.Rank)
                        .OrderBy(r => r)
                        .SequenceEqual(new[] { 1, 2, 3, 4, 5 })) {
                    return 5;
                }
                return seq.Sum(c => c.Rank);
            })
            .ToList();
    }

    private static List<Card> ConvertRanksToCards(IReadOnlyList<Card> cards, List<int> ranks) {
        return ranks.Select(rank => {
            if (rank == 1) {
                return cards.First(c => c.Rank == 14);
            }
            return cards.First(c => c.Rank == rank);
        }).ToList();
    }

    /// <summary>
    /// Returns the largest group of cards of the same suit
    /// </summary>
    public static List<Card> FindLargestSuitGroup(IReadOnlyList<Card> cards) {
        return cards.Count == 0
            ? []
            : cards.GroupBy(c => c.Suit)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Max(c => c.Rank))
                .First()
                .ToList();
    }
}