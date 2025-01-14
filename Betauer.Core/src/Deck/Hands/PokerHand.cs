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
        var hands = new List<PokerHand>();
    
        // Agrupar por rank y obtener todos los grupos que tengan 2 o más cartas
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2);
    
        foreach (var group in groups) {
            // Tomar las primeras 2 cartas de cada grupo
            var twoCards = group.Take(2).ToList();
            hands.Add(new PairHand(PokerHandsManager, twoCards));
        }

        return hands;
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
        var hands = new List<PokerHand>();
    
        // Agrupar por rank y obtener todos los grupos que tengan 3 o más cartas
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3);
    
        foreach (var group in groups) {
            // Tomar las primeras 3 cartas de cada grupo
            var threeCards = group.Take(3).ToList();
            hands.Add(new ThreeOfAKindHand(PokerHandsManager, threeCards));
        }

        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

// Escalera
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

// Color
public class FlushHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Flush", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                // Tomar las primeras 5 cartas del mismo palo
                hands.Add(new FlushHand(PokerHandsManager, suitCards.Take(5).ToList()));
            }
        }
        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        var largestGroup = HandUtils.FindLargestSuitGroup(currentHand);
        return [HandUtils.GetDiscardsByKeeping(largestGroup, currentHand, maxDiscardCards)];
    }
}

// Full (trio y pareja)
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

// Poker
public class FourOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) : PokerHand(pokerHandsManager, "Four of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
    
        // Agrupar por rank y obtener todos los grupos que tengan 4 o más cartas
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4);
    
        foreach (var group in groups) {
            // Tomar las primeras 4 cartas de cada grupo
            var fourCards = group.Take(4).ToList();
            hands.Add(new FourOfAKindHand(PokerHandsManager, fourCards));
        }

        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}

// Escalera de color
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

// NEW

// Full (trio y pareja) del mismo color
public class FlushHouseHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
    : PokerHand(pokerHandsManager, "Flush House", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                var groups = suitCards.GroupBy(c => c.Rank)
                    .Where(g => g.Count() >= 2)
                    .OrderByDescending(g => g.Key)
                    .ToList();

                // Si no hay al menos dos grupos, continuamos
                if (groups.Count < 2) continue;

                // Encontrar grupos que pueden formar el trío (3 o más cartas)
                var potentialThrees = groups.Where(g => g.Count() >= 3).ToList();
                if (!potentialThrees.Any()) continue;

                foreach (var firstGroup in potentialThrees) {
                    var threeCards = firstGroup.Take(3).ToList();
                    
                    // Buscar grupos para el par, excluyendo el grupo usado para el trío
                    var secondGroups = groups.Where(g => g.Key != firstGroup.Key).ToList();
                    foreach (var secondGroup in secondGroups) {
                        var twoCards = secondGroup.Take(2).ToList();
                        
                        // Si el primer grupo tiene exactamente 3 y el segundo exactamente 2,
                        // o si ambos grupos tienen 3 o más cartas, generamos las manos correspondientes
                        if ((firstGroup.Count() == 3 && secondGroup.Count() == 2) ||
                            (firstGroup.Count() >= 3 && secondGroup.Count() >= 3)) {
                            
                            // Añadir la primera combinación
                            hands.Add(new FlushHouseHand(PokerHandsManager, threeCards.Concat(twoCards).ToList()));
                            
                            // Si ambos grupos tienen 3 o más cartas, añadir la combinación invertida
                            if (secondGroup.Count() >= 3) {
                                var otherThreeCards = secondGroup.Take(3).ToList();
                                var otherTwoCards = firstGroup.Take(2).ToList();
                                hands.Add(new FlushHouseHand(PokerHandsManager, otherThreeCards.Concat(otherTwoCards).ToList()));
                            }
                            
                            // Una vez que hemos generado las manos para estos dos grupos, podemos romper el bucle interno
                            break;
                        }
                    }
                }
            }
        }
        return hands.Distinct().ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        return [HandUtils.TryDiscardNonDuplicates(currentHand, maxDiscardCards)];
    }
}


// Cinco cartas del mismo rank (poker de 5 cartas)
public class FiveOfAKindHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
    : PokerHand(pokerHandsManager, "Five of a Kind", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
    
        // Agrupar por rank y obtener todos los grupos que tengan 5 o más cartas
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 5);
    
        foreach (var group in groups) {
            // Tomar las primeras 5 cartas de cada grupo
            var fiveCards = group.Take(5).ToList();
            hands.Add(new FiveOfAKindHand(PokerHandsManager, fiveCards));
        }

        return hands;
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        // Agrupar por rank y ordenar por tamaño del grupo
        var groups = currentHand
            .GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ToList();

        if (groups.Count == 0) return [];

        // Encontrar el grupo más grande
        var largestGroup = groups[0].ToList();
        
        // Mantener las cartas del grupo más grande, descartar el resto
        return [HandUtils.GetDiscardsByKeeping(largestGroup, currentHand, maxDiscardCards)];
    }
}


// Cinco cartas idénticas (mismo palo y color)
// NUEVO
public class FlushFiveHand(PokerHandsManager pokerHandsManager, IReadOnlyList<Card> cards) 
    : PokerHand(pokerHandsManager, "Flush Five", cards) {
    public override List<PokerHand> IdentifyHands(IReadOnlyList<Card> cards) {
        return cards
            .GroupBy(c => (c.Rank, c.Suit))
            .Where(g => g.Count() >= 5)
            .Select(g => new FlushFiveHand(PokerHandsManager, g.Take(5).ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<List<Card>> SuggestDiscards(IReadOnlyList<Card> currentHand, int maxDiscardCards) {
        // Agrupar por rango Y palo al mismo tiempo
        var groups = currentHand
            .GroupBy(c => (c.Rank, c.Suit))
            .OrderByDescending(g => g.Count())
            .ToList();

        // Si no hay grupos o el grupo más grande ya tiene 5 o más cartas, no sugerir descartes
        if (!groups.Any() || groups[0].Count() >= 5) return [];

        // Encontrar el grupo más grande de cartas idénticas
        var largestGroup = groups[0].ToList();
        
        // Descartar todas las cartas que no sean del grupo más grande
        return [HandUtils.GetDiscardsByKeeping(largestGroup, currentHand, maxDiscardCards)];
    }
}