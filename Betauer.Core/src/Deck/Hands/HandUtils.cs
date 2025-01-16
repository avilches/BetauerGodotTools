using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class StraightComparer : IComparer<Straight> {
    public int Compare(Straight? x, Straight? y) {
        if (x == null || y == null) return 0;
        return x.IsHigherThan(y) ? 1 : -1;
    }
}

public class FlushComparer : IComparer<IReadOnlyList<Card>> {
    public int Compare(IReadOnlyList<Card>? x, IReadOnlyList<Card>? y) {
        if (x == null || y == null) return 0;
        if (x.Count != y.Count) return x.Count.CompareTo(y.Count);

        // Eliminamos las cartas con mismo rank en ambas manos
        var xRanks = x.Select(c => c.Rank).OrderByDescending(r => r).ToList();
        var yRanks = y.Select(c => c.Rank).OrderByDescending(r => r).ToList();

        // Comparamos rank a rank (ya ordenados de mayor a menor)
        for (var i = 0; i < Math.Min(xRanks.Count, yRanks.Count); i++) {
            if (xRanks[i] != yRanks[i]) {
                return xRanks[i].CompareTo(yRanks[i]);
            }
        }

        return 0; // Son iguales
    }
}

public class Straight {
    public List<Card> Cards { get; }
    public int MinRank { get; }
    public int MaxRank { get; }
    public int MissingCards { get; }
    public bool HasAce => Cards.Any(c => c.Rank == 14);
    public bool IsFlush { get; }
    public bool IsComplete => MissingCards == 0;

    public Straight(List<Card> cards, int minRank, int maxRank) {
        Cards = cards.OrderBy(c => c.Rank).ToList();
        MinRank = minRank;
        MaxRank = maxRank;
        // Contar las cartas que faltan comparando las cartas que tenemos con el rango total
        MissingCards = MaxRank - MinRank + 1 - Cards.Count;
        IsFlush = Cards.Count > 0 && Cards.All(c => c.Suit == Cards[0].Suit);
    }

    public bool IsHigherThan(Straight other) {
        // Primero comparamos por número de huecos (menos huecos es mejor)
        if (MissingCards != other.MissingCards) return MissingCards < other.MissingCards;
        // Si tienen los mismos huecos, comparamos por el rango máximo
        return MaxRank > other.MaxRank;
    }

    public bool IsEquivalent(Straight other) {
        if (other == null) return false;
        if (MinRank != other.MinRank || MaxRank != other.MaxRank || MissingCards != other.MissingCards || Cards.Count != other.Cards.Count) return false;

        // Comparamos los rangos de las cartas
        return Cards.Select(c => c.Rank).SequenceEqual(other.Cards.Select(c => c.Rank));
    }

    public override string ToString() {
        return $"Straight [{MinRank}-{MaxRank}] MissingCards={MissingCards} Cards=[{string.Join(",", Cards)}]";
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
    public static List<Straight> FindStraightSequences(int size, IReadOnlyList<Card> cards) {
        var allStraights = new List<Straight>();

        // Primero intentamos encontrar escaleras de color
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 3) {
                allStraights.AddRange(FindSequences(size, suitCards));
            }
        }

        // Si no encontramos escaleras de color completas (no hay, o hay pero la primera esta incompleta)
        if (allStraights.Count == 0 || !allStraights[0].IsComplete) {
            var regularSequences = FindSequences(size, cards);

            // Añadir solo las escaleras que no son equivalentes a las que ya tenemos
            foreach (var straight in regularSequences) {
                if (!HasEquivalentStraight(allStraights, straight)) {
                    allStraights.Add(straight);
                }
            }
        }

        return allStraights.OrderByDescending(s => s, PokerHandAnalysis.StraightComparer).ToList();
    }

    private static List<Straight> FindSequences(int size, IReadOnlyList<Card> cards) {
        var allSequences = new List<Straight>();

        // Probar todas las posibles posiciones de inicio de escalera (de 1 a 10)
        for (var startRank = 1; startRank <= 10; startRank++) {
            var neededRanks = Enumerable.Range(startRank, size);
            var sequence = neededRanks
                .Select(rank =>
                    cards.FirstOrDefault(c => 
                        // Check both normal rank and Ace as 1 for rank 1
                        c.Rank == rank || (rank == 1 && c.Rank == 14)))
                .Where(c => c != null).ToList();
            var missingCount = size - sequence.Count;

            if (missingCount <= 2) {
                var straight = new Straight(sequence, startRank, startRank + 4);
                if (!HasEquivalentStraight(allSequences, straight)) {
                    allSequences.Add(straight);
                }
            }
        }

        return allSequences;
    }

    private static bool HasEquivalentStraight(List<Straight> straights, Straight newStraight) {
        return straights.Any(s => s.IsEquivalent(newStraight));
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