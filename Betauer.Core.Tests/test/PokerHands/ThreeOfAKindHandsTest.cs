using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]

public class ThreeOfAKindHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicThreeOfAKind_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void WithTwoThreeOfKinds_ShouldIdentifyBothOrdered() {
        // Con KKK AAA, debería identificar solo AAA
        var cards = CreateCards("KS", "KH", "KD", "AS", "AH", "AD", "2C");
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(2), "Should identify both three of a kinds");

            // El primero debe ser AAA
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "First three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True,
                "First should be three Aces (highest rank)");

            // El segundo debe ser KKK
            Assert.That(threeOfKinds[1].Cards.Count, Is.EqualTo(3), "Second three of a kind should have 3 cards");
            Assert.That(threeOfKinds[1].Cards.All(c => c.Rank == 13), Is.True,
                "Second should be three Kings");
        });
    }

    [Test]
    public void WithMultipleThreeOfKinds_ShouldBeOrderedByRank() {
        // Con QQQ KKK AAA, debería identificar todos ordenados por rango
        var cards = CreateCards(
            "AS", "AH", "AD",
            "KS", "KH", "KD",
            "QS", "QH", "QD"
        );
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(3), "Should identify all three combinations");

            // Verificar el orden: AAA, KKK, QQQ
            var ranks = threeOfKinds.Select(h => h.Cards[0].Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12 }),
                "Should be ordered by rank (Aces, Kings, Queens)");

            // Verificar que cada mano tiene exactamente 3 cartas del mismo rango
            for (var i = 0; i < threeOfKinds.Count; i++) {
                Assert.That(threeOfKinds[i].Cards.Count, Is.EqualTo(3),
                    $"Hand {i} should have exactly 3 cards");
                Assert.That(threeOfKinds[i].Cards.All(c => c.Rank == ranks[i]), Is.True,
                    $"All cards in hand {i} should have rank {ranks[i]}");
            }
        });
    }

    [Test]
    public void ThreeOfAKind_WithFourOfAKind_ShouldBeIdentified() {
        // Con póker de Ases (AAAA), debería poder identificar el trío
        var cards = CreateCards("AS", "AH", "AD", "AC", "KH");
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

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
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(threeOfKinds.Count, Is.EqualTo(1), "Should identify exactly one three of a kind");
            Assert.That(threeOfKinds[0].Cards.Count, Is.EqualTo(3), "Three of a kind should have 3 cards");
            Assert.That(threeOfKinds[0].Cards.All(c => c.Rank == 14), Is.True, "Should be three Aces");
        });
    }

    [Test]
    public void TwoPair_ShouldNotIdentifyThreeOfAKind() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(threeOfKinds, Is.Empty, "Should not identify three of a kind with only pairs");
    }

    [Test]
    public void SuggestDiscards_WithoutThreeOfAKind_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hand = PokerHand.Prototypes[PokerHandType.ThreeOfAKind];
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);

        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.All(c => c.Rank <= 13), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingThreeOfAKind_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "AD", "KH", "QD");
        var hand = PokerHand.Prototypes[PokerHandType.ThreeOfAKind];
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);

        Assert.That(discards, Is.Empty, "Should not suggest discards when three of a kind exists");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoThreeOfAKind() {
        var threeOfKinds = PokerHand.Prototypes[PokerHandType.ThreeOfAKind].IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(threeOfKinds, Is.Empty);
    }
}