using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class ThreeOfAKindHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicThreeOfAKind_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void WithTwoThreeOfKinds_ShouldIdentifyHighestOnly() {
        // Con AAA KKK, debería identificar solo AAA
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "KD", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void ThreeOfAKind_WithFourOfAKind_ShouldBeIdentified() {
        // Con póker de Ases (AAAA), debería poder identificar el trío
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void ThreeOfAKind_WithFullHouse_ShouldBeIdentified() {
        // Con AAA KK, debería poder identificar el trío
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void TwoPair_ShouldNotIdentifyThreeOfAKind() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();
        
        Assert.That(threeOfKinds, Is.Empty, "Should not identify three of a kind with only pairs");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoThreeOfAKind() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var threeOfKinds = hands.Where(h => h is ThreeOfAKindHand).ToList();
        Assert.That(threeOfKinds, Is.Empty);
    }

    [Test]
    public void SuggestDiscards_WithoutThreeOfAKind_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hand = new ThreeOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.All(c => c.Rank <= 13), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingThreeOfAKind_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var hand = new ThreeOfAKindHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when three of a kind exists");
    }
}