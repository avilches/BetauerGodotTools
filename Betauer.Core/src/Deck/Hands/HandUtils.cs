using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Utility class providing helper methods for poker hand analysis and manipulation.
/// </summary>
public static class HandUtils {
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
    /// Finds straight sequences, prioritizing same-suit sequences (straight flushes).
    /// First attempts to find sequences within each suit, and if no complete straight flush
    /// is found, then looks for regular straights.
    /// </summary>
    public static (List<List<Card>> complete, List<List<Card>> oneGap, List<List<Card>> twoGaps)
        FindStraightSequences(IReadOnlyList<Card> cards) {
        var complete = new List<List<Card>>();
        var oneGap = new List<List<Card>>();
        var twoGaps = new List<List<Card>>();

        // Primero intentamos encontrar escaleras de color
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 4) { // Necesitamos al menos 4 cartas del mismo palo
                var suitSequences = FindSequences(suitCards);
                complete.AddRange(suitSequences.complete);
                oneGap.AddRange(suitSequences.oneGap);
                twoGaps.AddRange(suitSequences.twoGaps);
            }
        }

        // Si no encontramos escaleras de color completas, buscamos escaleras normales
        if (complete.Count == 0) {
            var regularSequences = FindSequences(cards);
            complete.AddRange(regularSequences.complete);
            // Solo añadimos oneGap y twoGaps si no encontramos ninguna secuencia de color
            if (oneGap.Count == 0) oneGap.AddRange(regularSequences.oneGap);
            if (twoGaps.Count == 0) twoGaps.AddRange(regularSequences.twoGaps);
        }

        return (
            complete.OrderBySequenceValue().ToList(),
            oneGap.OrderBySequenceValue().ToList(),
            twoGaps.OrderBySequenceValue().ToList()
        );
    }

    /// <summary>
    /// Internal method that finds sequences in a given set of cards.
    /// </summary>
    private static (List<List<Card>> complete, List<List<Card>> oneGap, List<List<Card>> twoGaps)
        FindSequences(IReadOnlyList<Card> cards) {
        var complete = new List<List<Card>>();
        var oneGap = new List<List<Card>>();
        var twoGaps = new List<List<Card>>();

        // Convertir las cartas a un HashSet de ranks para búsqueda rápida
        var rankSet = cards.Select(c => c.Rank).ToHashSet();
        var ranks = rankSet.OrderBy(r => r).ToList();

        // Si tenemos un As, añadirlo también como 1
        if (rankSet.Contains(14)) {
            ranks.Insert(0, 1);
        }

        // Para cada rank inicial posible
        foreach (var startRank in ranks) {
            var neededRanks = new[] { startRank, startRank + 1, startRank + 2, startRank + 3, startRank + 4 };

            // Si algún rank es mayor que 14, ignoramos esta secuencia
            if (neededRanks.Any(r => r > 14)) continue;

            // Contar cuántas cartas faltan
            var missingCount = neededRanks.Count(r => !rankSet.Contains(r == 1 ? 14 : r));

            // Obtener las cartas que sí tenemos en esta secuencia
            var sequence = cards.Where(c => neededRanks.Contains(c.Rank) ||
                                            (c.Rank == 14 && neededRanks.Contains(1)))
                .ToList();

            if (missingCount == 0) {
                complete.Add(sequence);
            } else if (missingCount == 1) {
                oneGap.Add(sequence);
            } else if (missingCount == 2) {
                twoGaps.Add(sequence);
            }
        }

        return (complete, oneGap, twoGaps);
    }


    private static IOrderedEnumerable<List<Card>> OrderBySequenceValue(this List<List<Card>> sequences) {
        return sequences.OrderByDescending(seq => {
            // Primero por cantidad de cartas
            if (seq.Any(c => c.Rank == 14) &&
                seq.Select(c => c.Rank == 14 ? 1 : c.Rank)
                    .OrderBy(r => r)
                    .SequenceEqual([1, 2, 3, 4, 5])) {
                return 5;
            }
            return seq.Sum(c => c.Rank);
        });
    }
}