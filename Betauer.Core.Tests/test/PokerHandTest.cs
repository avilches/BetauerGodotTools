using Betauer.Core.Deck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

[TestFixture]
public class PokerHandTest {
    protected PokerHandsManager HandsManager;

    [SetUp]
    public void Setup() {
        HandsManager = new PokerHandsManager();
        HandsManager.RegisterBasicPokerHands();
    }

    protected List<Card> CreateCards(params string[] cards) {
        var parse = new PokerGameConfig().Parse;
        return cards.Select(parse).ToList();
    }

    [Test]
    public void PairFromThreeOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD", "JC", "2C");
        var hands = HandsManager.IdentifyAllHands(cards);

        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(3)); // Tres posibles combinaciones de par de Ases
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True); // Todos son Ases
    }

    [Test]
    public void PairFromFourOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(cards);

        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(6)); // Seis posibles combinaciones de par de Ases
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True);
    }

    [Test]
    public void TwoPair_WithThreePairs_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "KD", "KC", "QH", "QD", "2C");
        var hands = HandsManager.IdentifyAllHands(cards);

        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs.Count, Is.EqualTo(3)); // A-K, A-Q, K-Q
        Assert.That(twoPairs.All(h => h.Cards.Count == 4), Is.True);

        // Verificar que tenemos todas las combinaciones posibles
        var ranks = twoPairs.Select(h => h.Cards.Select(c => c.Rank).OrderBy(r => r).ToList()).ToList();
        Assert.That(ranks, Has.Member(new List<int> { 12, 12, 14, 14 })); // A-Q
        Assert.That(ranks, Has.Member(new List<int> { 13, 13, 14, 14 })); // A-K
        Assert.That(ranks, Has.Member(new List<int> { 12, 12, 13, 13 })); // K-Q
    }

    [Test]
    public void ThreeOfAKind_FromFourOfAKind_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(cards);

        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();
        Assert.That(threeOfKinds.Count, Is.EqualTo(4)); // Cuatro posibles tríos de Ases
        Assert.That(threeOfKinds.All(h => h.Cards.Count == 3), Is.True);
        Assert.That(threeOfKinds.All(h => h.Cards.All(c => c.Rank == 14)), Is.True);
    }

    [Test]
    public void Flush_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        // 6 cartas de corazones, debería generar 6 posibles colores diferentes
        var cards = CreateCards("AH", "KH", "QH", "JH", "TH", "9H", "2D");
        var hands = HandsManager.IdentifyAllHands(cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        // Deberíamos tener 6 combinaciones (6C5 = 6)
        Assert.That(flushes.Count, Is.EqualTo(6));

        // Verificar que todas son diferentes
        var uniqueFlushes = flushes.Select(h => string.Join(",", h.Cards.Select(c => c.ToString()).OrderBy(s => s)))
            .Distinct()
            .Count();
        Assert.That(uniqueFlushes, Is.EqualTo(6));
    }

    [Test]
    public void Straight_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        // 6,7,8,9,10,J - debería generar 2 escaleras diferentes
        var cards = CreateCards("6H", "7H", "8H", "9H", "TH", "JH");
        var hands = HandsManager.IdentifyAllHands(cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

        // Deberíamos tener 2 escaleras: 6-10 y 7-J
        Assert.That(straights.Count, Is.EqualTo(2));
    }

    [Test]
    public void FullHouse_WithFourOfEachRank_ShouldFindAllCombinations() {
        // 4 ases y 4 reyes deberían generar 48 full houses diferentes:
        // - Como trío de Ases:
        //   * 4C3 = 4 combinaciones para los tres ases
        //   * 4C2 = 6 combinaciones para los dos reyes
        //   * Total: 4 * 6 = 24 combinaciones
        // - Como trío de Reyes:
        //   * 4C3 = 4 combinaciones para los tres reyes
        //   * 4C2 = 6 combinaciones para los dos ases
        //   * Total: 4 * 6 = 24 combinaciones
        // Total final: 48 combinaciones diferentes
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH", "KD", "KC");
        var hands = HandsManager.IdentifyAllHands(cards);
        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
    
        Assert.That(fullHouses.Count, Is.EqualTo(48));
    
        // Verificar que todas son diferentes
        var uniqueFullHouses = fullHouses.Select(h => string.Join(",", h.Cards.Select(c => c.ToString()).OrderBy(s => s)))
            .Distinct()
            .Count();
        Assert.That(uniqueFullHouses, Is.EqualTo(48));

        // Verificar que tenemos tanto tríos de Ases como de Reyes
        var trioAces = fullHouses.Count(h => h.Cards.Count(c => c.Rank == 14) == 3);
        var trioKings = fullHouses.Count(h => h.Cards.Count(c => c.Rank == 13) == 3);
        Assert.That(trioAces, Is.EqualTo(24)); // 24 full houses con trío de Ases
        Assert.That(trioKings, Is.EqualTo(24)); // 24 full houses con trío de Reyes
    }
    [Test]
    public void FourOfKind_WithAllSameSuit_ShouldFindOneHand() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hands = HandsManager.IdentifyAllHands(cards);
        var fourOfKinds = hands.Where(h => h is FourOfAKindHand).ToList();

        // Solo debería haber una combinación de póker
        Assert.That(fourOfKinds.Count, Is.EqualTo(1));
    }

    [Test]
    public void StraightFlush_WithSixCards_ShouldFindTwoCombinations() {
        // 6♠,7♠,8♠,9♠,10♠,J♠ debería generar 2 escaleras de color: 6-10 y 7-J
        var cards = CreateCards("6S", "7S", "8S", "9S", "TS", "JS");
        var hands = HandsManager.IdentifyAllHands(cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

        Assert.That(straightFlushes.Count, Is.EqualTo(2));

        // Verificar las dos combinaciones
        var lowestRanks = straightFlushes.Select(h => h.Cards.Min(c => c.Rank)).OrderBy(r => r).ToList();
        Assert.That(lowestRanks[0], Is.EqualTo(6)); // Primera escalera empieza en 6
        Assert.That(lowestRanks[1], Is.EqualTo(7)); // Segunda escalera empieza en 7
    }

    [Test]
    public void TwoPair_WithThreePairsAndExtra_ShouldFindAllCombinations() {
        // Con tres pares (A,A,K,K,Q,Q) y una carta extra, debería encontrar 3 dobles parejas diferentes
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "2C");
        var hands = HandsManager.IdentifyAllHands(cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.That(twoPairs.Count, Is.EqualTo(3));

        // Verificar que tenemos todas las combinaciones (A-K, A-Q, K-Q)
        var pairCombinations = twoPairs
            .Select(h => string.Join("-", h.Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)))
            .ToList();

        Assert.That(pairCombinations, Contains.Item("14-13")); // A-K
        Assert.That(pairCombinations, Contains.Item("14-12")); // A-Q
        Assert.That(pairCombinations, Contains.Item("13-12")); // K-Q
    }

    [Test]
    public void FullHouse_WithMultipleOptions_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "QS", "QH");
        var hands = HandsManager.IdentifyAllHands(cards);

        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
        Assert.That(fullHouses.Count, Is.EqualTo(2)); // AAA-KK y AAA-QQ

        // Verificar que tenemos las combinaciones correctas
        var combinations = fullHouses
            .Select(h => (
                Three: h.Cards.GroupBy(c => c.Rank).First(g => g.Count() == 3).Key,
                Two: h.Cards.GroupBy(c => c.Rank).First(g => g.Count() == 2).Key
            ))
            .ToList();

        Assert.That(combinations, Has.Member((Three: 14, Two: 13))); // AAA-KK
        Assert.That(combinations, Has.Member((Three: 14, Two: 12))); // AAA-QQ
    }

    [Test]
    public void FullHouse_WithFourOfAKind_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH", "2C");
        var hands = HandsManager.IdentifyAllHands(cards);

        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
        Assert.That(fullHouses.Count, Is.EqualTo(4)); // Cuatro posibles AAA-KK
        Assert.That(fullHouses.All(h => h.Cards.Count == 5), Is.True);

        // Todos deben tener tres Ases y dos Reyes
        Assert.That(fullHouses.All(h =>
            h.Cards.Count(c => c.Rank == 14) == 3 &&
            h.Cards.Count(c => c.Rank == 13) == 2), Is.True);
    }

    [Test]
    public void HandIdentifier_WithFourOfAKind_ShouldFindAllSubsets() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(cards);

        // Verificar todas las manos posibles
        Assert.That(hands.Count(h => h is FourOfAKindHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is ThreeOfAKindHand), Is.EqualTo(4));
        Assert.That(hands.Count(h => h is PairHand), Is.EqualTo(6));

        // Verificar orden correcto por multiplicador
        Assert.That(hands.First(), Is.TypeOf<FourOfAKindHand>());
    }

    [Test]
    public void HandIdentifier_WithFullHouse_ShouldFindAllSubsets() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(cards);

        // Verificar todas las manos posibles
        Assert.That(hands.Count(h => h is FullHouseHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is ThreeOfAKindHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is TwoPairHand), Is.EqualTo(1));
        Assert.That(hands.Count(h => h is PairHand), Is.EqualTo(4)); // 3 pares de Ases + 1 par de Reyes

        // Verificar orden correcto por multiplicador
        Assert.That(hands.First(), Is.TypeOf<FullHouseHand>());
    }

    [Test]
    public void HandIdentifier_WithThreePairs_ShouldRankCorrectly() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "JC");
        var hands = HandsManager.IdentifyAllHands(cards);

        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs.Count, Is.EqualTo(3));

        // El primer Two Pair debería ser el de mayor valor (A-K)
        var firstTwoPair = twoPairs.First();
        var ranks = firstTwoPair.Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
        Assert.That(ranks[0], Is.EqualTo(14)); // As
        Assert.That(ranks[2], Is.EqualTo(13)); // Rey
    }
}