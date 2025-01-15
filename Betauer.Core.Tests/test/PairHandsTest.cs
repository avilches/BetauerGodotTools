using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class PairHandsTest : PokerHandsTestBase {
    // Tests básicos de identificación de pares
    [Test]
    public void SinglePair_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        
        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(1));
        Assert.That(pairs[0].Cards.Count, Is.EqualTo(2));
        Assert.That(pairs[0].Cards.All(c => c.Rank == 14), Is.True); // Todos son Ases
    }

    [Test]
    public void Pair_WithThreeOfSameRank_ShouldIdentifyOnePair() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD", "JC", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs.Count, Is.EqualTo(1));
        Assert.That(pairs.All(h => h.Cards.Count == 2), Is.True);
        Assert.That(pairs.All(h => h.Cards.All(c => c.Rank == 14)), Is.True); // Todos son Ases
    }

    [Test]
    public void WithMultiplePairs_ShouldIdentifyHighestPairOnly() {
        // Tenemos par de Ases y par de Reyes, solo debe identificar el par de Ases
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.Multiple(() => {
            Assert.That(pairs.Count, Is.EqualTo(1), "Should have exactly 1 pair");
            Assert.That(pairs[0].Cards.Count, Is.EqualTo(2), "Pair should have 2 cards");
            Assert.That(pairs[0].Cards.All(c => c.Rank == 14), Is.True, "Should be pair of Aces");
        });
    }

    // Tests de sugerencia de descartes
    [Test]
    public void SuggestDiscards_WithoutPair_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("KS", "QH", "JD", "TC", "9H");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        
        var pair = new PairHand(HandsManager, []);
        var discards = pair.SuggestDiscards(new PokerHandAnalysis(cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
        Assert.That(cardsToDiscard.All(c => c.Rank <= 10), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingPair_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        
        var pair = new PairHand(HandsManager, []);
        var discards = pair.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when pair exists");
    }

    // Tests de casos límite
    [Test]
    public void EmptyHand_ShouldReturnNoPairs() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        
        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs, Is.Empty);
    }

    [Test]
    public void SingleCard_ShouldReturnNoPairs() {
        var cards = CreateCards("AS");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        
        var pairs = hands.Where(h => h is PairHand).ToList();
        Assert.That(pairs, Is.Empty);
    }
}