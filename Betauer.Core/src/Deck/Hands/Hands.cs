using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class HandImprovement {
    public List<Card> CardsToKeep { get; }
    public List<Card> CardsToDiscard { get; }
    public PokerHand TargetHand { get; }
    public double Probability { get; }
    public double Score { get; }

    public HandImprovement(List<Card> cardsToKeep, List<Card> cardsToDiscard, PokerHand targetHand, double probability) {
        CardsToKeep = cardsToKeep;
        CardsToDiscard = cardsToDiscard;
        TargetHand = targetHand;
        Probability = probability;
        Score = probability * targetHand.CalculateScore();
    }
}

public class DiscardOption {
    public List<Card> CardsToDiscard { get; }
    public List<HandImprovement> PossibleImprovements { get; }
    public double TotalScore { get; }

    public DiscardOption(List<Card> cardsToDiscard, List<HandImprovement> improvements) {
        CardsToDiscard = cardsToDiscard;
        PossibleImprovements = improvements.OrderByDescending(i => i.Score).ToList();
        TotalScore = improvements.Sum(i => i.Score);
    }
}

public static class EnumerableExtensions {
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k) {
        return k == 0
            ? [Array.Empty<T>()]
            : elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }
}

public static class CardExtensions {
    // Encuentra todas las escaleras posibles en un conjunto de cartas
    public static IEnumerable<List<Card>> FindStraights(this IEnumerable<Card> cards) {
        var uniqueRanks = cards.Select(c => c.Rank)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        // Añadir el As como 1 si existe un As (14)
        if (uniqueRanks.Contains(14)) {
            uniqueRanks.Insert(0, 1);
        }

        // Buscar secuencias de 5 cartas
        for (int i = 0; i <= uniqueRanks.Count - 5; i++) {
            var sequence = uniqueRanks.Skip(i).Take(5);
            if (sequence.Zip(sequence.Skip(1)).All(p => p.Second - p.First == 1)) {
                // Para cada rango en la escalera, encuentra todas las cartas posibles
                var straightCards = sequence.Select(rank =>
                    rank == 1
                        ? cards.First(c => c.Rank == 14)
                        : // Usar el As (14) cuando el rank es 1
                        cards.First(c => c.Rank == rank)
                ).ToList();

                yield return straightCards;
            }
        }
    }
}

public abstract class PokerHand {
    public PokerHands PokerHands { get; }
    public IReadOnlyList<Card> Cards { get; }
    public string Name { get; set; }

    protected PokerHand(PokerHands pokerHands, string name, IReadOnlyList<Card> cards) {
        PokerHands = pokerHands;
        Name = name;
        Cards = cards;
    }

    public int CalculateScore() {
        if (PokerHands == null) throw new InvalidOperationException("PokerHand not initialized with PokerHands");
        return Cards.Sum(c => c.Rank) * PokerHands.GetMultiplier(this);
    }

    public abstract List<PokerHand> FindAll(IReadOnlyList<Card> cards);
    public abstract List<HandImprovement> FindPossibleImprovements(IReadOnlyList<Card> currentHand, IReadOnlyList<Card> availableCards, int maxDiscardCards);

    public override string ToString() {
        var cardsStr = string.Join(" ", Cards);
        return $"{Name} ({cardsStr})";
    }
}

public class HighCardHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "High Card", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.OrderByDescending(c => c.Rank)
            .Select(c => new HighCardHand(pokerHands, new List<Card> { c }))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        // High Card no tiene manos potenciales, siempre devuelve lista vacía
        return [];
    }
}

public class PairHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Pair", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .SelectMany(g => g.Combinations(2))
            .Select(combo => new PairHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        foreach (var card in currentHand.OrderByDescending(c => c.Rank)) {
            var matchingCards = availableCards.Where(c => c.Rank == card.Rank).ToList();
            if (matchingCards.Any()) {
                var cardsToKeep = new List<Card> { card };
                var otherCards = currentHand.Where(c => c != card).ToList();

                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();
                        var probability = (double)matchingCards.Count /
                                          (availableCards.Count - (maxDiscardCards - cardsToDiscard.Count));

                        var targetHand = new PairHand(PokerHands,
                            new List<Card> { card, matchingCards[0] });

                        improvements.Add(new HandImprovement(
                            cardsToKeep,
                            cardsToDiscard,
                            targetHand,
                            probability));
                    }
                }
            }
        }

        return improvements;
    }
}

public class TwoPairHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Two Pair", cards) {
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
                result.Add(new TwoPairHand(PokerHands, highPair.Concat(lowPair).ToList()));
            }
        }

        return result;
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Caso 1: Ya tenemos una pareja
        var existingPairs = currentHand.GroupBy(c => c.Rank)
            .Where(g => g.Count() == 2)
            .ToList();

        if (existingPairs.Any()) {
            var pairCards = existingPairs.First().ToList();

            // Buscar otra pareja potencial
            foreach (var card in currentHand.Where(c => !pairCards.Contains(c))
                         .OrderByDescending(c => c.Rank)) {
                var matchingCards = availableCards.Where(c => c.Rank == card.Rank).ToList();
                if (matchingCards.Any()) {
                    var cardsToKeep = new List<Card>(pairCards) { card };
                    var otherCards = currentHand.Where(c => !cardsToKeep.Contains(c)).ToList();

                    for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                        foreach (var discardCombo in otherCards.Combinations(n)) {
                            var cardsToDiscard = discardCombo.ToList();
                            var probability = (double)matchingCards.Count /
                                              (availableCards.Count - (maxDiscardCards - cardsToDiscard.Count));

                            var targetHand = new TwoPairHand(PokerHands,
                                [..pairCards, card, matchingCards[0]]);

                            improvements.Add(new HandImprovement(
                                cardsToKeep,
                                cardsToDiscard,
                                targetHand,
                                probability));
                        }
                    }
                }
            }
        }

        return improvements;
    }
}

public class ThreeOfAKindHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Three of a Kind", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 3)
            .SelectMany(g => g.Combinations(3))
            .Select(combo => new ThreeOfAKindHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Solo buscar tríos si ya tenemos una pareja
        var pairs = currentHand.GroupBy(c => c.Rank)
            .Where(g => g.Count() == 2)
            .ToList();

        foreach (var pair in pairs) {
            var pairCards = pair.ToList();
            var matchingCards = availableCards.Where(c => c.Rank == pair.Key).ToList();

            if (matchingCards.Any()) {
                var otherCards = currentHand.Where(c => !pairCards.Contains(c)).ToList();

                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();

                        // La probabilidad es el número de cartas que nos sirven dividido por el total de cartas disponibles
                        var probability = (double)matchingCards.Count / availableCards.Count;

                        var targetHand = new ThreeOfAKindHand(PokerHands,
                            [..pairCards, matchingCards[0]]);

                        improvements.Add(new HandImprovement(
                            pairCards,
                            cardsToDiscard,
                            targetHand,
                            probability));
                    }
                }
            }
        }

        return improvements;
    }
}

public class StraightHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Straight", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.FindStraights()
            .Select(straightCards => new StraightHand(PokerHands, straightCards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Primero encontramos la secuencia más larga de cartas conectadas
        var ranks = currentHand.Select(c => c.Rank)
            .OrderBy(r => r)
            .Distinct()
            .ToList();

        // Si hay un As, considerarlo también como 1
        if (ranks.Contains(14)) {
            ranks.Insert(0, 1);
        }

        // Buscar la secuencia más larga
        var longestSequence = new List<int>();
        var currentSequence = new List<int>();

        for (int i = 0; i < ranks.Count - 1; i++) {
            if (currentSequence.Count == 0) {
                currentSequence.Add(ranks[i]);
            }

            if (ranks[i + 1] - ranks[i] == 1) {
                currentSequence.Add(ranks[i + 1]);
            } else {
                if (currentSequence.Count > longestSequence.Count) {
                    longestSequence = new List<int>(currentSequence);
                }
                currentSequence = new List<int> { ranks[i + 1] };
            }
        }

        if (currentSequence.Count > longestSequence.Count) {
            longestSequence = currentSequence;
        }

        // Si tenemos al menos 3 cartas conectadas
        if (longestSequence.Count >= 3) {
            // Obtener las cartas actuales que forman parte de la secuencia
            var sequenceCards = currentHand
                .Where(c => longestSequence.Contains(c.Rank == 1 ? 14 : c.Rank))
                .ToList();

            var otherCards = currentHand
                .Where(c => !sequenceCards.Contains(c))
                .ToList();

            // Encontrar rangos necesarios para completar la escalera
            var neededRanks = new HashSet<int>();

            // Rangos antes de la secuencia
            if (longestSequence[0] > 1) {
                neededRanks.Add(longestSequence[0] - 1);
            }

            // Rangos después de la secuencia
            if (longestSequence[^1] < 14) {
                neededRanks.Add(longestSequence[^1] + 1);
            }

            // Encontrar cartas disponibles que pueden completar la escalera
            var availableNeededCards = availableCards
                .Where(c => neededRanks.Contains(c.Rank))
                .ToList();

            foreach (var neededCard in availableNeededCards) {
                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();
                        var probability = (double)1 / availableCards.Count;

                        var targetCards = new List<Card>(sequenceCards) { neededCard };
                        targetCards = targetCards.OrderBy(c => c.Rank).ToList();

                        if (IsValidStraight(targetCards)) {
                            improvements.Add(new HandImprovement(
                                sequenceCards,
                                cardsToDiscard,
                                new StraightHand(PokerHands, targetCards),
                                probability));
                        }
                    }
                }
            }
        }

        return improvements;
    }

    private static bool IsValidStraight(IReadOnlyList<Card> cards) {
        if (cards.Count != 5) return false;

        var orderedRanks = cards.Select(c => c.Rank)
            .OrderBy(r => r)
            .ToList();

        // Comprobar secuencia normal
        if (orderedRanks[4] - orderedRanks[0] == 4 &&
            orderedRanks.Zip(orderedRanks.Skip(1)).All(p => p.Second - p.First == 1)) {
            return true;
        }

        // Comprobar escalera con As bajo (A,2,3,4,5)
        if (orderedRanks[4] == 14) { // Si la última carta es un As
            var lowAceRanks = orderedRanks.Take(4).ToList();
            lowAceRanks.Insert(0, 1); // Insertar el As como 1
            lowAceRanks = lowAceRanks.Take(5).OrderBy(r => r).ToList();

            return lowAceRanks[4] - lowAceRanks[0] == 4 &&
                   lowAceRanks.Zip(lowAceRanks.Skip(1)).All(p => p.Second - p.First == 1);
        }

        return false;
    }
}

public class FlushHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Flush", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        var hands = new List<PokerHand>();
        foreach (var suit in cards.Select(c => c.Suit).Distinct()) {
            var suitCards = cards.Where(c => c.Suit == suit).ToList();
            if (suitCards.Count >= 5) {
                // Generar todas las combinaciones posibles de 5 cartas
                foreach (var combination in suitCards.Combinations(5)) {
                    hands.Add(new FlushHand(PokerHands, combination.ToList()));
                }
            }
        }
        return hands;
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Considerar grupos de 2 o más cartas del mismo palo
        var suitGroups = currentHand.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 2)
            .OrderByDescending(g => g.Count());

        foreach (var suitGroup in suitGroups) {
            var suitCards = suitGroup.OrderByDescending(c => c.Rank).ToList();
            var availableSuitCards = availableCards
                .Where(c => c.Suit == suitGroup.Key)
                .OrderByDescending(c => c.Rank)
                .ToList();

            var cardsNeeded = 5 - suitCards.Count;

            if (availableSuitCards.Count >= cardsNeeded) {
                var otherCards = currentHand.Where(c => !suitCards.Contains(c)).ToList();

                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();
                        var probability = CalculateFlushProbability(
                            availableSuitCards.Count,
                            availableCards.Count,
                            cardsNeeded);

                        var neededCards = availableSuitCards.Take(cardsNeeded).ToList();
                        var targetCards = new List<Card>(suitCards);
                        targetCards.AddRange(neededCards);

                        var targetHand = new FlushHand(PokerHands, targetCards);

                        improvements.Add(new HandImprovement(
                            suitCards,
                            cardsToDiscard,
                            targetHand,
                            probability));
                    }
                }
            }
        }

        return improvements;
    }

    private double CalculateFlushProbability(int availableSuitCards, int totalAvailableCards,
        int cardsNeeded) {
        // Si no hay suficientes cartas del palo
        if (availableSuitCards < cardsNeeded) return 0;

        // Si solo necesitamos una carta y hay al menos una disponible del palo correcto
        if (cardsNeeded == 1 && availableSuitCards >= 1) {
            return 1.0; // Si tenemos la carta que necesitamos, la probabilidad es 1
        }

        // Para casos más complejos (necesitamos más de una carta)
        double probability = 1.0;
        for (int i = 0; i < cardsNeeded; i++) {
            probability *= (double)(availableSuitCards - i) / (totalAvailableCards - i);
        }
        return probability;
    }
}

public class FullHouseHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Full House", cards) {
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
                        hands.Add(new FullHouseHand(PokerHands, fullHouseCards));
                    }
                }
            }
        }
        return hands;
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Caso 1: Ya tenemos un trío
        var triplets = currentHand.GroupBy(c => c.Rank)
            .Where(g => g.Count() == 3)
            .ToList();

        foreach (var triplet in triplets) {
            var tripletCards = triplet.ToList();

            // Buscar par potencial en las cartas disponibles
            var potentialPairs = availableCards
                .Where(c => c.Rank != triplet.Key)
                .GroupBy(c => c.Rank)
                .Where(g => g.Count() >= 2);

            foreach (var potentialPair in potentialPairs) {
                var otherCards = currentHand.Where(c => !tripletCards.Contains(c)).ToList();

                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();

                        // Para hacer un par necesitamos sacar dos cartas específicas
                        var totalCards = availableCards.Count;
                        var probability = (double)1 / totalCards * 1 / (totalCards - 1);

                        var targetCards = new List<Card>(tripletCards);
                        targetCards.AddRange(potentialPair.Take(2));
                        var targetHand = new FullHouseHand(PokerHands, targetCards);

                        improvements.Add(new HandImprovement(
                            tripletCards,
                            cardsToDiscard,
                            targetHand,
                            probability));
                    }
                }
            }
        }

        // Caso 2: Tenemos dos pares y queremos mejorar uno a trío
        var pairs = currentHand.GroupBy(c => c.Rank)
            .Where(g => g.Count() == 2)
            .ToList();

        if (pairs.Count >= 2) {
            foreach (var pair in pairs) {
                var pairCards = pair.ToList();
                var matchingCards = availableCards.Where(c => c.Rank == pair.Key).ToList();

                if (matchingCards.Any()) {
                    var otherPair = pairs.First(p => p.Key != pair.Key);
                    var otherPairCards = otherPair.ToList();
                    var cardsToKeep = new List<Card>();
                    cardsToKeep.AddRange(pairCards);
                    cardsToKeep.AddRange(otherPairCards);

                    var otherCards = currentHand.Where(c => !cardsToKeep.Contains(c)).ToList();

                    for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                        foreach (var discardCombo in otherCards.Combinations(n)) {
                            var cardsToDiscard = discardCombo.ToList();

                            // Para mejorar a trío solo necesitamos una carta específica
                            var probability = (double)matchingCards.Count / availableCards.Count;

                            var targetCards = new List<Card>(pairCards);
                            targetCards.Add(matchingCards[0]);
                            targetCards.AddRange(otherPairCards);
                            var targetHand = new FullHouseHand(PokerHands, targetCards);

                            improvements.Add(new HandImprovement(
                                cardsToKeep,
                                cardsToDiscard,
                                targetHand,
                                probability));
                        }
                    }
                }
            }
        }

        return improvements;
    }
}

public class FourOfAKindHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Four of a Kind", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 4)
            .SelectMany(g => g.Combinations(4))
            .Select(combo => new FourOfAKindHand(PokerHands, combo.ToList()))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Solo buscar poker si ya tenemos un trío
        var triplets = currentHand.GroupBy(c => c.Rank)
            .Where(g => g.Count() == 3)
            .ToList();

        foreach (var triplet in triplets) {
            var tripletCards = triplet.ToList();
            var matchingCards = availableCards
                .Where(c => c.Rank == triplet.Key)
                .ToList();

            if (matchingCards.Any()) {
                var otherCards = currentHand
                    .Where(c => !tripletCards.Contains(c))
                    .ToList();

                for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                    foreach (var discardCombo in otherCards.Combinations(n)) {
                        var cardsToDiscard = discardCombo.ToList();
                        var probability = (double)matchingCards.Count /
                                          (availableCards.Count - (maxDiscardCards - cardsToDiscard.Count));

                        var targetHand = new FourOfAKindHand(PokerHands,
                            [..tripletCards, matchingCards[0]]);

                        improvements.Add(new HandImprovement(
                            tripletCards,
                            cardsToDiscard,
                            targetHand,
                            probability));
                    }
                }
            }
        }

        return improvements;
    }
}

public class StraightFlushHand(PokerHands pokerHands, IReadOnlyList<Card> cards) : PokerHand(pokerHands, "Straight Flush", cards) {
    public override List<PokerHand> FindAll(IReadOnlyList<Card> cards) {
        return cards.GroupBy(c => c.Suit)
            .Where(g => g.Count() >= 5)
            .SelectMany(g => g.ToList().FindStraights())
            .Select(straightFlushCards => new StraightFlushHand(PokerHands, straightFlushCards))
            .Cast<PokerHand>()
            .ToList();
    }

    public override List<HandImprovement> FindPossibleImprovements(
        IReadOnlyList<Card> currentHand,
        IReadOnlyList<Card> availableCards,
        int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        // Agrupar por palo
        var suitGroups = currentHand.GroupBy(c => c.Suit);
        foreach (var suitGroup in suitGroups) {
            var suitCards = suitGroup.OrderBy(c => c.Rank).ToList();
            if (suitCards.Count >= 4) {
                var ranks = suitCards.Select(c => c.Rank).ToList();
                if (ranks.Contains(14)) ranks.Insert(0, 1);

                // Buscar secuencias de 4 cartas consecutivas
                for (int i = 0; i <= ranks.Count - 4; i++) {
                    var sequence = ranks.Skip(i).Take(4).ToList();
                    if (sequence.Zip(sequence.Skip(1)).All(p => p.Second - p.First == 1)) {
                        var neededRanks = new List<int>();
                        if (sequence[0] > 1) neededRanks.Add(sequence[0] - 1);
                        neededRanks.Add(sequence[^1] + 1);

                        var availableNeededCards = availableCards
                            .Where(c => c.Suit == suitGroup.Key && neededRanks.Contains(c.Rank))
                            .ToList();

                        if (availableNeededCards.Any()) {
                            var sequenceCards = suitCards
                                .Where(c => sequence.Contains(c.Rank == 1 ? 14 : c.Rank))
                                .ToList();
                            var otherCards = currentHand
                                .Where(c => !sequenceCards.Contains(c))
                                .ToList();

                            for (int n = 1; n <= Math.Min(maxDiscardCards, otherCards.Count); n++) {
                                foreach (var discardCombo in otherCards.Combinations(n)) {
                                    var cardsToDiscard = discardCombo.ToList();
                                    var probability = (double)availableNeededCards.Count /
                                                      (availableCards.Count - (maxDiscardCards - cardsToDiscard.Count));

                                    var targetHand = new StraightFlushHand(PokerHands,
                                        [..sequenceCards, availableNeededCards[0]]);

                                    improvements.Add(new HandImprovement(
                                        sequenceCards,
                                        cardsToDiscard,
                                        targetHand,
                                        probability));
                                }
                            }
                        }
                    }
                }
            }
        }

        return improvements;
    }
}