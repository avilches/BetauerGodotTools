using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public abstract class PokerHand(string name, IReadOnlyList<Card> cards) {
    public IReadOnlyList<Card> Cards { get; } = cards;
    public string Name { get; set; } = name;

    // Método abstracto que cada mano debe implementar para encontrar todas las posibles
    // combinaciones de este tipo de mano en las cartas dadas
    public abstract List<PokerHand> FindAll(IReadOnlyList<Card> cards);

    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }
}

public class HighCardHand(IReadOnlyList<Card> cards) : PokerHand("High Card", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class PairHand(IReadOnlyList<Card> cards) : PokerHand("Pair", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .SelectMany(g => g.Combinations(2))
            .Select(combo => new PairHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class TwoPairHand(IReadOnlyList<Card> cards) : PokerHand("Two Pair", cards) {
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

public class ThreeOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Three of a Kind", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => g.Combinations(3))
            .Select(combo => new ThreeOfAKindHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class StraightHand(IReadOnlyList<Card> cards) : PokerHand("Straight", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.FindStraights()
            .Select(straightCards => new StraightHand(straightCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class FlushHand(IReadOnlyList<Card> cards) : PokerHand("Flush", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                // Generar todas las combinaciones posibles de 5 cartas
                foreach (var combination in suitCards.Combinations(5)) {
                    hands.Add(new FlushHand(combination.ToList()));
                }
            }
        }
        return hands;
    }
}

public class FullHouseHand(IReadOnlyList<Card> cards) : PokerHand("Full House", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        var groups = cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Key)
            .ToList();

        foreach (var threeGroup in groups.Where(g => g.Count() >= 3)) {
            // Generar todas las combinaciones posibles de 3 cartas para el trío
            foreach (var threeCombination in threeGroup.Combinations(3)) {
                foreach (var twoGroup in groups.Where(g => g.Key != threeGroup.Key && g.Count() >= 2)) {
                    // Generar todas las combinaciones posibles de 2 cartas para el par
                    foreach (var twoCombination in twoGroup.Combinations(2)) {
                        var fullHouseCards = threeCombination.Concat(twoCombination).ToList();
                        hands.Add(new FullHouseHand(fullHouseCards));
                    }
                }
            }
        }
        return hands;
    }
}

public class FourOfAKindHand(IReadOnlyList<Card> cards) : PokerHand("Four of a Kind", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4)
            .SelectMany(g => g.Combinations(4))
            .Select(combo => new FourOfAKindHand(combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class StraightFlushHand(IReadOnlyList<Card> cards) : PokerHand("Straight Flush", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => g.ToList().FindStraights())
            .Select(straightFlushCards => new StraightFlushHand(straightFlushCards))
            .Cast<PokerHand>()
            .ToList();
    }
}

public class RoyalFlushHand(IReadOnlyList<Card> cards) : PokerHand("Royal Flush", cards) {
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