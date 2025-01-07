using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Betauer.Core.Deck;

public abstract class PokerHand(string name, int multiplier, IReadOnlyList<Card> cards) {
    public IImmutableList<Card> Cards { get; } = cards.ToImmutableList();
    public string Name { get; set; } = name;

    // Método abstracto que cada mano debe implementar para encontrar todas las posibles
    // combinaciones de este tipo de mano en las cartas dadas
    public abstract List<PokerHand> FindAll(IReadOnlyList<Card> cards);

    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }
}

public class HighCardHand(IReadOnlyList<Card> cards) : PokerHand("High Card", 1, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class PairHand(IReadOnlyList<Card> cards) : PokerHand("Pair", 2, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .SelectMany(g => g.Combinations(2))
            .Select(combo => new PairHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class TwoPairHand(IReadOnlyList<Card> cards) : PokerHand("Two Pair", 3, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
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
                result.Add(new TwoPairHand(highPair.Concat(lowPair).ToList()));
            }
        }

        return result;
    }
}

public class ThreeOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Three of a Kind", 4, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => g.Combinations(3))
            .Select(combo => new ThreeOfAKindHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class StraightHand(IReadOnlyList<Card> cards) : PokerHand("Straight", 5, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.FindStraights()
            .Select(straightCards => new StraightHand(straightCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class FlushHand(IReadOnlyList<Card> cards) : PokerHand("Flush", 6, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.FindFlushes()
            .Select(flushCards => new FlushHand(flushCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class FullHouseHand(IReadOnlyList<Card> cards) : PokerHand("Full House", 7, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        var groups = cards.GroupBy(c => c.Rank)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .ToList();

        var result = new List<PokerHand>();

        foreach (var group in groups.Where(g => g.Count() >= 3)) {
            // Obtener todas las posibles combinaciones de 3 cartas del grupo
            var threeCombinations = group.Combinations(3);

            // Buscar pares potenciales de otros ranks
            var availablePairs = groups
                .Where(g => g.Key != group.Key && g.Count() >= 2)
                .OrderByDescending(g => g.Key);

            foreach (var threeComb in threeCombinations) {
                foreach (var pair in availablePairs) {
                    var fullHouse = threeComb
                        .Concat(pair.Take(2))
                        .ToList();
                    result.Add(new FullHouseHand(fullHouse));
                }
            }
        }

        return result;
    }
}

public class FourOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Four of a Kind", 8, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4)
            .SelectMany(g => g.Combinations(4))
            .Select(combo => new FourOfAKindHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class StraightFlushHand(IReadOnlyList<Card> cards) : PokerHand("Straight Flush", 9, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => g.ToList().FindStraights())
            .Select(straightFlushCards => new StraightFlushHand(straightFlushCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class RoyalFlushHand(IReadOnlyList<Card> cards) : PokerHand("Royal Flush", 10, cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => g.ToList().FindStraights())
            .Where(straight => straight.Max(c => c.Rank) == 14 &&
                               straight.Min(c => c.Rank) == 10)
            .Select(royalFlushCards => new RoyalFlushHand(royalFlushCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

// Actualización del HandIdentifier
public class HandIdentifier {
    private readonly List<PokerHand> handPrototypes;
    private readonly PokerHandScoring scoring;

    public HandIdentifier(PokerHandScoring scoring) {
        this.scoring = scoring;
        handPrototypes = new List<PokerHand> {
            new HighCardHand([]),
            new PairHand([]),
            new TwoPairHand([]),
            new ThreeOfAKindHand([]),
            new StraightHand([]),
            new FlushHand([]),
            new FullHouseHand([]),
            new FourOfAKindHand([]),
            new StraightFlushHand([]),
            new RoyalFlushHand([])
        };
    }

    public List<PokerHand> IdentifyAllHands(IReadOnlyList<Card> cards) {
        var allHands = handPrototypes
            .SelectMany(prototype => prototype.FindAll(cards))
            .OrderByDescending(hand => scoring.GetMultiplier(hand.GetType()))
            .ThenByDescending(hand => scoring.CalculateScore(hand))
            .ToList();

        // Añadir un identificador único para manos del mismo tipo
        var groupedHands = allHands.GroupBy(h => h.Name).ToList();
        foreach (var group in groupedHands.Where(g => g.Count() > 1)) {
            int i = 1;
            foreach (var hand in group) {
                hand.Name = $"{hand.Name} #{i}";
                i++;
            }
        }

        return allHands;
    }
}
public class PokerHandScoring {
    private readonly Dictionary<Type, int> handMultipliers;
    
    public PokerHandScoring() {
        handMultipliers = new Dictionary<Type, int> {
            { typeof(HighCardHand), 1 },
            { typeof(PairHand), 2 },
            { typeof(TwoPairHand), 3 },
            { typeof(ThreeOfAKindHand), 4 },
            { typeof(StraightHand), 5 },
            { typeof(FlushHand), 6 },
            { typeof(FullHouseHand), 7 },
            { typeof(FourOfAKindHand), 8 },
            { typeof(StraightFlushHand), 9 },
            { typeof(RoyalFlushHand), 10 }
        };
    }
    
    public void SetMultiplier(Type handType, int multiplier) {
        if (!handMultipliers.ContainsKey(handType)) {
            throw new ArgumentException($"Invalid hand type: {handType}");
        }
        handMultipliers[handType] = multiplier;
    }
    
    public int GetMultiplier(Type handType) {
        return handMultipliers[handType];
    }
    
    public int CalculateScore(PokerHand hand) {
        return hand.Cards.Sum(c => c.Rank) * handMultipliers[hand.GetType()];
    }
    
    public PokerHandScoring Clone() {
        var clone = new PokerHandScoring();
        foreach (var pair in handMultipliers) {
            clone.handMultipliers[pair.Key] = pair.Value;
        }
        return clone;
    }
}