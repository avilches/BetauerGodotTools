using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Base class for poker hands. Each specific hand type inherits from this.
/// Provides common functionality and defines the contract for hand identification
/// and improvement suggestions.
/// </summary>
public abstract class PokerHand(PokerHandsManager pokerHandsManager, string name, IReadOnlyList<Card> cards) {
    public PokerHandsManager PokerHandsManager { get; } = pokerHandsManager;
    public IReadOnlyList<Card> Cards { get; } = cards;
    public string Name { get; set; } = name;

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
        return IdentifyHands(currentHand).Count > 0 
            ? [] // If this hand type already exists, no discards needed
            : SuggestDiscards(currentHand, maxDiscardCards); // If not, delegate to specific implementation
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

public class HighCardHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "High Card", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(pokerHandsManager, new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        // Strategy: No discards for high card
        return [];
    }
}

public class PairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Pair", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .SelectMany(g => g.ToList().Combinations(2))
            .Select(combo => new PairHand(PokerHandsManager, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class TwoPairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Two Pair", cards) {
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
                result.Add(new TwoPairHand(PokerHandsManager, highPair.Concat(lowPair).ToList()));
            }
        }

        return result;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class ThreeOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Three of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => g.ToList().Combinations(3))
            .Select(combo => new ThreeOfAKindHand(PokerHandsManager, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class StraightHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Straight", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return HandUtils.FindStraights(cards)
            .Select(straightCards => new StraightHand(PokerHandsManager, straightCards))
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

public class FlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Flush", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                // Generar todas las combinaciones posibles de 5 cartas
                foreach (var combination in suitCards.Combinations(5)) {
                    hands.Add(new FlushHand(PokerHandsManager, combination.ToList()));
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

public class FullHouseHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Full House", cards) {
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
                        hands.Add(new FullHouseHand(PokerHandsManager, fullHouseCards));
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

public class FourOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Four of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4)
            .SelectMany(g => g.ToList().Combinations(4))
            .Select(combo => new FourOfAKindHand(PokerHandsManager, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

public class StraightFlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Straight Flush", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => HandUtils.FindStraights(g.ToList()))
            .Select(straightFlushCards => new StraightFlushHand(PokerHandsManager, straightFlushCards))
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