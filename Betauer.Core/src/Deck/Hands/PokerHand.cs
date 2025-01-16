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
        return Cards.Count == other.Cards.Count &&
               Cards.All(other.Cards.Contains);
    }

    public override int GetHashCode() {
        // XOR of all card hashcodes (order independent)
        return Cards.Aggregate(GetType().GetHashCode(), (current, card) => current ^ card.GetHashCode());
    }
}

public class HighCardHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "High Card", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.Cards
            .OrderByDescending(c => c.Rank)
            .Select(card => new HighCardHand(PokerHandsManager, [card]))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para high card
    }
}

public class PairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Pair", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.Pairs
            .Select(pair => new PairHand(PokerHandsManager, pair.Cards))
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

public class TwoPairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Two Pair", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.TwoPairs viene ordenado por rank, el primero es el mejor
        var bestTwoPair = analysis.TwoPairs.FirstOrDefault();
        return bestTwoPair == default 
            ? [] 
            : [new TwoPairHand(PokerHandsManager, 
                bestTwoPair.FirstPair.Concat(bestTwoPair.SecondPair).ToList())];
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

public class ThreeOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Three of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.ThreeOfAKind viene ordenado por rank, el primero es el mejor
        var highestThree = analysis.ThreeOfAKind.FirstOrDefault();
        return highestThree == default 
            ? [] 
            : [new ThreeOfAKindHand(PokerHandsManager, highestThree.Cards)];
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

public class FullHouseHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Full House", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FullHouses viene ordenado por rank, el primero es el mejor
        var bestFullHouse = analysis.FullHouses.FirstOrDefault();
        return bestFullHouse == default 
            ? [] 
            : [new FullHouseHand(PokerHandsManager, 
                bestFullHouse.ThreeCards.Concat(bestFullHouse.Pair).ToList())];
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para full house
    }
}

public class FourOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Four of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FourOfAKind viene ordenado por rank, el primero es el mejor
        var highestFour = analysis.FourOfAKind.FirstOrDefault();
        return highestFour == default 
            ? [] 
            : [new FourOfAKindHand(PokerHandsManager, highestFour.Cards)];
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un póker o mejor, no sugerimos descartes
        if (analysis.FourOfAKind.Count > 0 || analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class FiveOfAKindHand : PokerHand {
    public FiveOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Five of a Kind", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FiveOfAKind viene ordenado por rank, el primero es el mejor
        var highestFive = analysis.FiveOfAKind.FirstOrDefault();
        return highestFive == default 
            ? [] 
            : [new FiveOfAKindHand(PokerHandsManager, highestFive.Cards)];
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un repóker, no sugerimos descartes
        if (analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class FlushHouseHand : PokerHand {
    public FlushHouseHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Flush House", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.FlushHouses
            .Select(flushHouse => new FlushHouseHand(PokerHandsManager, 
                flushHouse.ThreeCards.Concat(flushHouse.Pair).ToList()))
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

public class StraightHand : PokerHand {
    public StraightHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Straight", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.HasCompleteStraight
            ? [new StraightHand(PokerHandsManager, analysis.Straights[0].Cards)]
            : [];
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

public class StraightFlushHand : PokerHand {
    public StraightFlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Straight Flush", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.HasCompleteStraight && analysis.Straights[0].IsFlush
            ? [new StraightFlushHand(PokerHandsManager, analysis.Straights[0].Cards)]
            : [];
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
public class FlushHand : PokerHand {
    public FlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Flush", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        if (!analysis.HasCompleteFlush) return [];
        
        return [new FlushHand(PokerHandsManager, 
            analysis.Flushes[0]
                .OrderByDescending(c => c.Rank)
                .Take(analysis.Config.FlushSize)
                .ToList())];
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
public class FlushFiveHand : PokerHand {
    public FlushFiveHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Flush Five", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        var flushFives = new List<PokerHand>();
        foreach (var flush in analysis.Flushes.Where(flush => flush.Count >= 5)) {
            var groupsByRank = flush
                .GroupBy(c => c.Rank)
                .Where(g => g.Count() >= 5)
                .OrderByDescending(g => g.Key);
            
            foreach (var group in groupsByRank) {
                flushFives.Add(new FlushFiveHand(PokerHandsManager, group.Take(5).ToList()));
            }
        }
        return flushFives.OrderByDescending(hand => hand.Cards[0].Rank).ToList();
    }
    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return [];
    }
}