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

public class PokerHand(PokerHandType handType, IReadOnlyList<Card> cards) {
    public IReadOnlyList<Card> Cards { get; } = cards;
    public string Name { get; set; }
    public PokerHandType HandType { get; } = handType;
    
    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }

    public override bool Equals(object? obj) {
        if (obj is not PokerHand other) return false;
        return HandType == other.HandType && Cards.SequenceEqual(other.Cards);
    }

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(HandType);
        foreach (var card in Cards) hash.Add(card);
        return hash.ToHashCode();
    }
}
public abstract class PokerHandDefinition(PokerHandType handType, string name) {
    public string Name { get; set; } = name;
    public PokerHandType HandType { get; } = handType;

    public static Dictionary<PokerHandType, PokerHandDefinition> Prototypes = CreateDict();

    private static Dictionary<PokerHandType, PokerHandDefinition> CreateDict() {
        var dict = new Dictionary<PokerHandType, PokerHandDefinition> {
            { PokerHandType.HighCard, new HighCardHand() },
            { PokerHandType.Pair, new PairHand() },
            { PokerHandType.TwoPair, new TwoPairHand() },
            { PokerHandType.ThreeOfAKind, new ThreeOfAKindHand() },
            { PokerHandType.Straight, new StraightHand() },
            { PokerHandType.Flush, new FlushHand() },
            { PokerHandType.FullHouse, new FullHouseHand() },
            { PokerHandType.FourOfAKind, new FourOfAKindHand() },
            { PokerHandType.StraightFlush, new StraightFlushHand() },
            { PokerHandType.FiveOfAKind, new FiveOfAKindHand() },
            { PokerHandType.FlushHouse, new FlushHouseHand() },
            { PokerHandType.FlushFive, new FlushFiveHand() }
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

    protected PokerHand CreatePokerHand(List<Card> groupCards) {
        var pokerHand = new PokerHand(HandType, groupCards) {
            Name = Name
        };
        return pokerHand;
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


    private class HighCardHand() : PokerHandDefinition(PokerHandType.HighCard, "High Card") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            return analysis.Cards
                .OrderByDescending(c => c.Rank)
                .Select(card => CreatePokerHand([card]))
                .ToList();
        }

        public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
            return []; // No hay estrategia de descarte para high card
        }
    }

    private class PairHand() : PokerHandDefinition(PokerHandType.Pair, "Pair") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            return analysis.Pairs
                .Select(group => CreatePokerHand(group.Cards))
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

    private class TwoPairHand() : PokerHandDefinition(PokerHandType.TwoPair, "Two Pair") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            return analysis.TwoPairs
                .Select(group => CreatePokerHand([..group.FirstPair, ..group.SecondPair]))
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

    private class ThreeOfAKindHand() : PokerHandDefinition(PokerHandType.ThreeOfAKind, "Three of a Kind") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            return analysis.ThreeOfAKind
                .Select(group => CreatePokerHand(group.Cards))
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

    private class FullHouseHand() : PokerHandDefinition(PokerHandType.FullHouse, "Full House") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            return analysis.FullHouses
                .Select(group => CreatePokerHand([..group.ThreeCards, ..group.Pair]))
                .ToList();
        }

        public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
            return []; // No hay estrategia de descarte para full house
        }
    }

    private class FourOfAKindHand() : PokerHandDefinition(PokerHandType.FourOfAKind, "Four of a Kind") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.FourOfAKind viene ordenado por rank, el primero es el mejor
            return analysis.FourOfAKind
                .Select(group => CreatePokerHand(group.Cards))
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

    private class FiveOfAKindHand() : PokerHandDefinition(PokerHandType.FiveOfAKind, "Five of a Kind") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.FiveOfAKind viene ordenado por rank, el primero es el mejor
            return analysis.FiveOfAKind
                .Select(group => CreatePokerHand(group.Cards))
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

    private class FlushHouseHand() : PokerHandDefinition(PokerHandType.FlushHouse, "Flush House") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.FlushHouses viene ordenado por rank, el primero es el mejor
            return analysis.FlushHouses
                .Select(flushHouse => CreatePokerHand(flushHouse.ThreeCards.Concat(flushHouse.Pair).ToList()))
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

    private class StraightHand() : PokerHandDefinition(PokerHandType.Straight, "Straight") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.Straights viene ordenado por rank, el primero es el mejor
            return analysis.Straights
                .Where(straight => straight.IsComplete && !straight.IsFlush)
                // Straight ya vienen todas con el tamaño maximo
                .Select(straight => CreatePokerHand(straight.Cards))
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

    private class StraightFlushHand() : PokerHandDefinition(PokerHandType.StraightFlush, "Straight Flush") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.Straights viene ordenado por rank, el primero es el mejor
            return analysis.Straights
                .Where(straight => straight.IsComplete && straight.IsFlush)
                // Straight ya vienen todas con el tamaño maximo
                .Select(straight => CreatePokerHand(straight.Cards))
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

    private class FlushHand() : PokerHandDefinition(PokerHandType.Flush, "Flush") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Los flush ya tiene limitado el tamaño al maximo necesario, pero pueden tener menos de los esperados
            // Ya que analysis.Flushes viene ordenado por rank, el primero es el mejor
            return analysis.Flushes
                .Where(cards => cards.Count >= analysis.Config.FlushSize)
                .Select(cards => CreatePokerHand(cards.OrderByDescending(c => c.Rank)
                    .Take(analysis.Config.FlushSize)
                    .ToList()))
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
    private class FlushFiveHand() : PokerHandDefinition(PokerHandType.FlushFive, "Flush Five") {
        public override List<PokerHand> IdentifyHands(PokerHandAnalysis analysis) {
            // Ya que analysis.FlushFives viene ordenado por rank, el primero es el mejor
            return analysis.FlushFives
                .Select(CreatePokerHand)
                .ToList();
        }

        public override List<List<Card>> SuggestDiscards(PokerHandAnalysis analysis, int maxDiscardCards) {
            return [];
        }
    }
}