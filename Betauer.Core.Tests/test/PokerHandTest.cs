using Betauer.Core.Deck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

public class PokerHandTestBase {
    protected HandIdentifier handIdentifier;

    [SetUp]
    public void Setup() {
        handIdentifier = new HandIdentifier();
    }

    protected List<Card> CreateCards(params string[] cardStrs) {
        return cardStrs.Select(Card.Parse).ToList();
    }}

[TestFixture]
public class PairHandTests : PokerHandTestBase {
    [Test]
    public void PairFromThreeOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD", "JC", "2C");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(3)); // Tres posibles combinaciones de par de Ases
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True); // Todos son Ases
    }

    [Test]
    public void PairFromFourOfAKind_ShouldFindAllPossiblePairs() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(6)); // Seis posibles combinaciones de par de Ases
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True);
    }
}

[TestFixture]
public class TwoPairHandTests : PokerHandTestBase {
    [Test]
    public void TwoPair_WithThreePairs_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "KD", "KC", "QH", "QD", "2C");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs.Count, Is.EqualTo(3)); // A-K, A-Q, K-Q
        Assert.That(twoPairs.All(h => h.Cards.Count == 4), Is.True);
        
        // Verificar que tenemos todas las combinaciones posibles
        var ranks = twoPairs.Select(h => h.Cards.Select(c => c.Rank).OrderBy(r => r).ToList()).ToList();
        Assert.That(ranks, Has.Member(new List<int> { 12, 12, 14, 14 })); // A-Q
        Assert.That(ranks, Has.Member(new List<int> { 13, 13, 14, 14 })); // A-K
        Assert.That(ranks, Has.Member(new List<int> { 12, 12, 13, 13 })); // K-Q
    }
}

[TestFixture]
public class ThreeOfAKindHandTests : PokerHandTestBase {
    [Test]
    public void ThreeOfAKind_FromFourOfAKind_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();
        Assert.That(threeOfKinds.Count, Is.EqualTo(4)); // Cuatro posibles tríos de Ases
        Assert.That(threeOfKinds.All(h => h.Cards.Count == 3), Is.True);
        Assert.That(threeOfKinds.All(h => h.Cards.All(c => c.Rank == 14)), Is.True);
    }
}

[TestFixture]
public class FullHouseHandTests : PokerHandTestBase {
    [Test]
    public void FullHouse_WithMultipleOptions_ShouldFindAllCombinations() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "QS", "QH");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
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
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var fullHouses = hands.Where(h => h is FullHouseHand).ToList();
        Assert.That(fullHouses.Count, Is.EqualTo(4)); // Cuatro posibles AAA-KK
        Assert.That(fullHouses.All(h => h.Cards.Count == 5), Is.True);
        
        // Todos deben tener tres Ases y dos Reyes
        Assert.That(fullHouses.All(h => 
            h.Cards.Count(c => c.Rank == 14) == 3 && 
            h.Cards.Count(c => c.Rank == 13) == 2), Is.True);
    }
}

[TestFixture]
public class HandIdentifierTests : PokerHandTestBase {
    [Test]
    public void HandIdentifier_WithFourOfAKind_ShouldFindAllSubsets() {
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH", "QD", "JC");
        var hands = handIdentifier.IdentifyAllHands(cards);
        
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
        var hands = handIdentifier.IdentifyAllHands(cards);
        
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
        var hands = handIdentifier.IdentifyAllHands(cards);
        
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs.Count, Is.EqualTo(3));
        
        // El primer Two Pair debería ser el de mayor valor (A-K)
        var firstTwoPair = twoPairs.First();
        var ranks = firstTwoPair.Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
        Assert.That(ranks[0], Is.EqualTo(14)); // As
        Assert.That(ranks[2], Is.EqualTo(13)); // Rey
    }
}