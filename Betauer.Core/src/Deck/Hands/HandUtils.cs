using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Utility class providing helper methods for poker hand analysis and manipulation.
/// </summary>
public static class HandUtils {

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
    public static IEnumerable<List<Card>> FindStraights(IReadOnlyList<Card> cards) {
        var uniqueRanks = cards.Select(c => c.Rank)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        // Add Ace as 1 if Ace (14) exists
        if (uniqueRanks.Contains(14)) {
            uniqueRanks.Insert(0, 1);
        }

        // Look for 5-card sequences
        for (var i = 0; i <= uniqueRanks.Count - 5; i++) {
            var sequence = uniqueRanks.Skip(i).Take(5).ToList();
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
    
    /// <summary>
    /// Gets cards to discard based on which cards to keep.
    /// Respects maxDiscardCards limit.
    /// </summary>
    public static List<Card> GetDiscardsByKeeping(IReadOnlyList<Card> cardsToKeep, IReadOnlyList<Card> currentHand, int maxDiscardCards) {
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
                        .SequenceEqual([1, 2, 3, 4, 5])) {
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