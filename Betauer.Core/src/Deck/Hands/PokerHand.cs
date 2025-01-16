using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public enum PokerHandType {
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
    FiveOfAKind,
    FlushHouse,
    FlushFive
}

/// <summary>
/// Base class for poker hands. Each specific hand type inherits from this.
/// Provides common functionality and defines the contract for hand identification
/// and improvement suggestions.
/// </summary>
public abstract class PokerHand(string name, IReadOnlyList<Card> cards, PokerHandType handType) {
    public IReadOnlyList<Card> Cards { get; } = cards;
    public string Name { get; set; } = name;
    public PokerHandType HandType { get; } = handType;

    public static Dictionary<PokerHandType, PokerHand> Prototypes = CreateDict();

    private static Dictionary<PokerHandType, PokerHand> CreateDict() {
        var dict = new Dictionary<PokerHandType, PokerHand> {
            { PokerHandType.HighCard, new HighCardHand(null!) },
            { PokerHandType.Pair, new PairHand(null!) },
            { PokerHandType.TwoPair, new TwoPairHand(null!) },
            { PokerHandType.ThreeOfAKind, new ThreeOfAKindHand(null!) },
            { PokerHandType.Straight, new StraightHand(null!) },
            { PokerHandType.Flush, new FlushHand(null!) },
            { PokerHandType.FullHouse, new FullHouseHand(null!) },
            { PokerHandType.FourOfAKind, new FourOfAKindHand(null!) },
            { PokerHandType.StraightFlush, new StraightFlushHand(null!) },
            { PokerHandType.FiveOfAKind, new FiveOfAKindHand(null!) },
            { PokerHandType.FlushHouse, new FlushHouseHand(null!) },
            { PokerHandType.FlushFive, new FlushFiveHand(null!) }
        };
        
        // Validate all enum values are present in dictionary
        var enumValues = Enum.GetValues(typeof(PokerHandType));
        foreach (PokerHandType handType in enumValues) {
            if (!dict.ContainsKey(handType)) {
                throw new InvalidOperationException($"Missing PokerHandType {handType} in dictionary");
            }
        }
        var enumCount = enumValues.Length;
        if (dict.Count != enumCount) {
            throw new InvalidOperationException($"Dictionary count ({dict.Count}) does not match enum count ({enumCount})");
        }

        // Validate HandType matches dictionary key for each entry
        foreach (var kvp in dict.Where(kvp => kvp.Key != kvp.Value.HandType)) {
            throw new InvalidOperationException($"HandType mismatch for {kvp.Key}: dictionary key is {kvp.Key} but hand type is {kvp.Value.HandType}");
        }
        
        return dict;
    }

    /// <summary>
    /// Identifies all possible hands of this type in the given cards.
    /// Must be implemented by each specific hand type.
    /// </summary>
    public abstract List<PokerHand> IdentifyHands(PokerHandAnalysis analysis);

    /// <summary>
    /// Protected method that each hand type must implement to suggest discards.
    /// Called only when the hand doesn't already exist in the current cards.
    /// Should return list of cards to discard to potentially achieve this hand type.
    /// </summary>
    public abstract List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards);

    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }

    public override bool Equals(object obj) {
        if (obj is not PokerHand other) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (GetType() != obj.GetType()) return false;

        // Compare cards regardless of order
        return Cards.Count == other.Cards.Count && Cards.All(other.Cards.Contains);
    }

    public override int GetHashCode() {
        // XOR of all card hashcodes (order independent)
        return Cards.Aggregate(GetType().GetHashCode(), (current, card) => current ^ card.GetHashCode());
    }
}

public class HighCardHand(IReadOnlyList<Card> cards) : PokerHand("High Card", cards, PokerHandType.HighCard) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.Cards
            .OrderByDescending(c => c.Rank)
            .Select(card => new HighCardHand([card]))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para high card
    }
}

public class PairHand(IReadOnlyList<Card> cards) : PokerHand("Pair", cards, PokerHandType.Pair) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.Pairs
            .Select(group => new PairHand(group.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un par o mejor (trío, póker), no sugerimos descartes
        if (analysis.Pairs.Count > 0 ||
            analysis.ThreeOfAKind.Count > 0 ||
            analysis.FourOfAKind.Count > 0 ||
            analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        // Si no tenemos ningún grupo de cartas iguales, descartamos las cartas más bajas
        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class TwoPairHand(IReadOnlyList<Card> cards) : PokerHand("Two Pair", cards, PokerHandType.TwoPair) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.TwoPairs
            .Select(group => new TwoPairHand([..group.FirstPair, ..group.SecondPair]))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos dos pares o mejor, no sugerimos descartes
        if (analysis.TwoPairs.Count > 0 ||
            analysis.ThreeOfAKind.Count > 0 ||
            analysis.FourOfAKind.Count > 0 ||
            analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class ThreeOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Three of a Kind", cards, PokerHandType.ThreeOfAKind) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.ThreeOfAKind
            .Select(group => new ThreeOfAKindHand(group.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un trío o mejor, no sugerimos descartes
        if (analysis.ThreeOfAKind.Count > 0 || analysis.FourOfAKind.Count > 0 ||
            analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class FullHouseHand(IReadOnlyList<Card> cards) : PokerHand("Full House", cards, PokerHandType.FullHouse) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.FullHouses
            .Select(group => new FullHouseHand([..group.ThreeCards, ..group.Pair]))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para full house
    }
}

public class FourOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Four of a Kind", cards, PokerHandType.FourOfAKind) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FourOfAKind viene ordenado por rank, el primero es el mejor
        return analysis.FourOfAKind
            .Select(group => new FourOfAKindHand(group.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un póker o mejor, no sugerimos descartes
        if (analysis.FourOfAKind.Count > 0 || analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class FiveOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Five of a Kind", cards, PokerHandType.FiveOfAKind) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FiveOfAKind viene ordenado por rank, el primero es el mejor
        return analysis.FiveOfAKind
            .Select(group => new FiveOfAKindHand(group.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un repóker, no sugerimos descartes
        if (analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class FlushHouseHand(IReadOnlyList<Card> cards) : PokerHand("Flush House", cards, PokerHandType.FlushHouse) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FlushHouses viene ordenado por rank, el primero es el mejor
        return analysis.FlushHouses
            .Select(flushHouse => new FlushHouseHand(flushHouse.ThreeCards.Concat(flushHouse.Pair).ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un flush house, no sugerimos descartes
        if (analysis.FlushHouses.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class StraightHand(IReadOnlyList<Card> cards) : PokerHand("Straight", cards, PokerHandType.Straight) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.Straights viene ordenado por rank, el primero es el mejor
        return analysis.Straights
            .Where(straight => straight.IsComplete && !straight.IsFlush)
            // Straight ya vienen todas con el tamaño maximo
            .Select(straight => new StraightHand(straight.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos una escalera completa, no sugerimos descartes
        if (analysis.HasCompleteStraight) return [];

        // Solo sugerimos descartes si hay escaleras potenciales
        if (analysis.Straights.Count == 0) return [];

        // Elegimos la mejor secuencia (la primera, que ya viene ordenada)
        var bestSequence = analysis.Straights[0];

        // Calculamos los descartes basándonos en la mejor secuencia
        var discards = analysis.Cards
            .Where(c => !bestSequence.Cards.Contains(c))
            .OrderByDescending(c => c.Rank)
            .Take(maxDiscardCards)
            .ToList();

        return discards.Count > 0 ? [discards] : [];
    }
}

public class StraightFlushHand(IReadOnlyList<Card> cards) : PokerHand("Straight Flush", cards, PokerHandType.StraightFlush) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.Straights viene ordenado por rank, el primero es el mejor
        return analysis.Straights
            .Where(straight => straight.IsComplete && straight.IsFlush)
            // Straight ya vienen todas con el tamaño maximo
            .Select(straight => new StraightFlushHand(straight.Cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si hay una escalera completa flush, no descartamos nada
        if (analysis.HasCompleteStraight && analysis.Straights[0].IsFlush) {
            return [];
        }

        // Buscar la mejor escalera potencial flush
        var bestFlushStraight = analysis.Straights
            .FirstOrDefault(s => s.IsFlush);

        if (bestFlushStraight == null) {
            return [];
        }

        // Descartamos todas las cartas que no forman parte de la mejor escalera potencial
        var discards = analysis.Cards
            .Where(c => !bestFlushStraight.Cards.Contains(c))
            .OrderBy(c => c.Rank)
            .Take(maxDiscardCards)
            .ToList();

        return discards.Count > 0 ? [discards] : [];
    }
}

public class FlushHand(IReadOnlyList<Card> cards) : PokerHand("Flush", cards, PokerHandType.Flush) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Los flush ya tiene limitado el tamaño al maximo necesario, pero pueden tener menos de los esperados
        // Ya que analysis.Flushes viene ordenado por rank, el primero es el mejor
        return analysis.Flushes
            .Where(cards => cards.Count >= analysis.Config.FlushSize)
            .Select(cards => new FlushHand(cards.OrderByDescending(c => c.Rank)
                    .Take(analysis.Config.FlushSize)
                    .ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un color completo, no sugerimos descartes
        if (analysis.HasCompleteFlush) return [];

        // Solo sugerimos descartes si hay colores potenciales
        if (analysis.Flushes.Count == 0) return [];

        var bestFlush = analysis.Flushes[0];
        return [HandUtils.GetDiscardsByKeeping(bestFlush, analysis.Cards, maxDiscardCards)];
    }
}

// Cinco cartas idénticas (mismo palo y color)
// NUEVO
public class FlushFiveHand(IReadOnlyList<Card> cards) : PokerHand("Flush Five", cards, PokerHandType.FlushFive) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FlushFives viene ordenado por rank, el primero es el mejor
        return analysis.FlushFives
            .Select(cards => new FlushFiveHand(cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return [];
    }
}