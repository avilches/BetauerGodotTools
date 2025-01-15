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

public class HighCardHand : PokerHand {
    public HighCardHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "High Card", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.Cards
            .OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(PokerHandsManager, new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para high card
    }
}

public class PairHand : PokerHand {
    public PairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Pair", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Si hay tríos o mejor, no devolver pares de ese rank
        var excludedRanks = analysis.ThreeOfAKind
            .Concat(analysis.FourOfAKind)
            .Concat(analysis.FiveOfAKind)
            .Select(g => g.rank)
            .ToHashSet();

        return analysis.Pairs
            .Where(pair => !excludedRanks.Contains(pair.rank))
            .Select(pair => new PairHand(PokerHandsManager, pair.cards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos un par o mejor (trío, póker), no sugerimos descartes
        if (analysis.Pairs.Count > 0 || analysis.ThreeOfAKind.Count > 0 || 
            analysis.FourOfAKind.Count > 0 || analysis.FiveOfAKind.Count > 0) {
            return [];
        }

        // Si no tenemos ningún grupo de cartas iguales, descartamos las cartas más bajas
        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class TwoPairHand : PokerHand {
    public TwoPairHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Two Pair", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.TwoPairs viene ordenado por rank, el primero es el mejor
        var bestTwoPair = analysis.TwoPairs.FirstOrDefault();
        return bestTwoPair == default 
            ? [] 
            : [new TwoPairHand(PokerHandsManager, 
                bestTwoPair.firstPair.Concat(bestTwoPair.secondPair).ToList())];
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos dos pares o mejor, no sugerimos descartes
        if (analysis.TwoPairs.Count > 0 || analysis.ThreeOfAKind.Count > 0 || 
            analysis.FourOfAKind.Count > 0 || analysis.FiveOfAKind.Count > 0) {
            return [];
        }
        
        return [HandUtils.TryDiscardNonDuplicates(analysis.Cards, maxDiscardCards)];
    }
}

public class ThreeOfAKindHand : PokerHand {
    public ThreeOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Three of a Kind", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.ThreeOfAKind viene ordenado por rank, el primero es el mejor
        var highestThree = analysis.ThreeOfAKind.FirstOrDefault();
        return highestThree == default 
            ? [] 
            : [new ThreeOfAKindHand(PokerHandsManager, highestThree.cards)];
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

public class FullHouseHand : PokerHand {
    public FullHouseHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Full House", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FullHouses viene ordenado por rank, el primero es el mejor
        var bestFullHouse = analysis.FullHouses.FirstOrDefault();
        return bestFullHouse == default 
            ? [] 
            : [new FullHouseHand(PokerHandsManager, 
                bestFullHouse.threeCards.Concat(bestFullHouse.pair).ToList())];
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return []; // No hay estrategia de descarte para full house
    }
}

public class FourOfAKindHand : PokerHand {
    public FourOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Four of a Kind", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Ya que analysis.FourOfAKind viene ordenado por rank, el primero es el mejor
        var highestFour = analysis.FourOfAKind.FirstOrDefault();
        return highestFour == default 
            ? [] 
            : [new FourOfAKindHand(PokerHandsManager, highestFour.cards)];
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
            : [new FiveOfAKindHand(PokerHandsManager, highestFive.cards)];
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
                flushHouse.threeCards.Concat(flushHouse.pair).ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        var keeps = new List<List<Card>>();
        foreach (var flush in analysis.PotentialFlushes) {
            var flushAnalysis = new PokerHandAnalysis(flush);
            if (flushAnalysis.ThreeOfAKind.Count > 0) {
                keeps.Add(HandUtils.GetDiscardsByKeeping(flushAnalysis.ThreeOfAKind[0].cards, analysis.Cards, maxDiscardCards));
            }
        }
        return keeps;
    }
}

public class StraightHand : PokerHand {
    public StraightHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Straight", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.CompleteStraights
            .Select(straight => new StraightHand(PokerHandsManager, straight))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Si ya tenemos una escalera completa, no sugerimos descartes
        if (analysis.CompleteStraights.Count > 0) return [];
    
        // Solo sugerimos descartes si hay escaleras potenciales
        if (analysis.PotentialStraights.Count == 0) return [];
    
        // Elegimos la mejor secuencia (la primera, que ya viene ordenada por valor)
        var bestSequence = analysis.PotentialStraights[0];
    
        // Calculamos los descartes basándonos en la mejor secuencia
        var discards = analysis.Cards
            .Where(c => !bestSequence.Contains(c))
            .OrderByDescending(c => c.Rank)
            .Take(maxDiscardCards)
            .ToList();
        
        // Solo devolvemos una lista con la mejor opción de descarte
        return discards.Count > 0 ? [discards] : [];
    }
}

public class StraightFlushHand : PokerHand {
    public StraightFlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Straight Flush", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.CompleteStraights
            .Where(straight => straight.All(card => card.Suit == straight[0].Suit))
            .Select(straight => new StraightFlushHand(PokerHandsManager, straight))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        // Buscar el palo con más cartas potenciales para escalera
        var bestSuit = ' ';
        List<Card> bestPotentialStraight = null;
        
        foreach (var suitGroup in analysis.Cards.GroupBy(c => c.Suit)) {
            if (suitGroup.Count() < 4) continue; // Necesitamos al menos 4 cartas del mismo palo
            
            // Analizar solo las cartas de este palo
            var suitCards = suitGroup.ToList();
            var sequences = HandUtils.FindStraightSequences(suitCards);
            
            // Si encontramos una secuencia potencial en este palo
            if (sequences.oneGap.Count > 0 || sequences.twoGaps.Count > 0) {
                var potentialStraight = sequences.oneGap.FirstOrDefault() ?? sequences.twoGaps.FirstOrDefault();
                if (potentialStraight != null && (bestPotentialStraight == null || 
                                                  potentialStraight.Count > bestPotentialStraight.Count)) {
                    bestSuit = suitGroup.Key;
                    bestPotentialStraight = potentialStraight;
                }
            }
        }
        
        if (bestPotentialStraight == null) return [];
        
        // Descartar todas las cartas que no sean del palo seleccionado
        var discards = analysis.Cards
            .Where(c => c.Suit != bestSuit)
            .OrderBy(c => c.Rank) // Ordenamos por rank para tomar las más bajas primero
            .Take(maxDiscardCards) // Tomamos solo hasta el máximo permitido
            .ToList();
        
        return discards.Count > 0 ? [discards] : [];
    }
}

public class FlushHand : PokerHand {
    public FlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Flush", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        return analysis.CompleteFlushes
            .Select(flush => new FlushHand(PokerHandsManager, 
                flush.OrderByDescending(c => c.Rank).Take(5).ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        if (analysis.PotentialFlushes.Count == 0) return [];
        var bestFlush = analysis.PotentialFlushes[0];
        return [HandUtils.GetDiscardsByKeeping(bestFlush, analysis.Cards, maxDiscardCards)];
    }
}

// Cinco cartas idénticas (mismo palo y color)
// NUEVO
public class FlushFiveHand : PokerHand {
    public FlushFiveHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
        : base(pokerHandsManager, "Flush Five", cards) { }

    public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
        // Como esto es un caso especial, necesitaremos agrupar por rank Y palo al mismo tiempo
        // Todos los FlushFive estarán dentro de cada grupo de CompleteFlushes
        var flushFives = new List<PokerHand>();
        foreach (var flush in analysis.CompleteFlushes) {
            var groupsByRank = flush
                .GroupBy(c => c.Rank)
                .Where(g => g.Count() >= 5)
                .OrderByDescending(g => g.Key);
            
            foreach (var group in groupsByRank) {
                flushFives.Add(new FlushFiveHand(PokerHandsManager, group.Take(5).ToList()));
            }
        }
        return flushFives;
    }

    public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
        return [];
    }
}