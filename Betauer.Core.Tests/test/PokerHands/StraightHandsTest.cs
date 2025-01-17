using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class StraightHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicStraight_ShouldBeIdentified() {
        // 10,J,Q,K,A de diferentes palos
        var cards = CreateCards("TC", "JH", "QD", "KS", "AC");
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straights.Count, Is.EqualTo(1), "Should identify exactly one straight");
            Assert.That(straights[0].Cards.Count, Is.EqualTo(5), "Straight should have 5 cards");

            var ranks = straights[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 10, 11, 12, 13, 14 }),
                "Should be 10,J,Q,K,A in order");
        });
    }

    [Test]
    public void BasicStraight_ShouldBeIdentified_WhenDuplicatedCardAppear() {
        var cards = CreateCards("KH", "9D", "9H", "8C", "6D", "5H", "5S", "4C", "7S", "4S");
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straights.Count, Is.EqualTo(2), "Should identify exactly two straights");
            Assert.That(straights[0].Cards.Count, Is.EqualTo(5), "First straight should have 5 cards");
            Assert.That(straights[1].Cards.Count, Is.EqualTo(5), "Second straight should have 5 cards");

            // Verificar la primera escalera (5-9, la más alta)
            var firstStraightRanks = straights[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(firstStraightRanks, Is.EqualTo(new[] { 5, 6, 7, 8, 9 }),
                "First straight should be 5-9 (highest)");

            // Verificar la segunda escalera (4-8)
            var secondStraightRanks = straights[1].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(secondStraightRanks, Is.EqualTo(new[] { 4, 5, 6, 7, 8 }),
                "Second straight should be 4-8");
        });
    }

    [Test]
    public void LowStraight_WithAceAsOne_ShouldBeIdentified() {
        // A,2,3,4,5 de diferentes palos
        var cards = CreateCards("AC", "2H", "3D", "4S", "5C");
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straights.Count, Is.EqualTo(1), "Should identify exactly one straight");

            var orderedCards = straights[0].Cards
                .OrderBy(c => c.Rank == 14 ? 1 : c.Rank)
                .ToList();

            Assert.That(orderedCards.Select(c => c.Rank == 14 ? 1 : c.Rank),
                Is.EqualTo(new[] { 1, 2, 3, 4, 5 }), "Should be A,2,3,4,5 with Ace as 1");
        });
    }

    [Test]
    public void WithDuplicateRanks_ShouldIdentifyStraight() {
        // A♥,2♥,2♣,3♥,4♥,5♥ (2 duplicado)
        var cards = CreateCards("AH", "2H", "2C", "3C", "4H", "5H");
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(straights.Count, Is.EqualTo(1),
            "Should identify straight even with duplicate ranks");
    }

    [Test]
    public void WithTenCards_ShouldFindMultipleStraightPossibilities() {
        // 6,7,8,9,10,J,Q,K,A de diferentes palos + 5
        var cards = CreateCards("5C", "6H", "7D", "8S", "9C", "TC", "JH", "QD", "KS", "AC");
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            Assert.That(analysis.Straights.Count, Is.GreaterThan(1),
                "Should find multiple straight possibilities");

            // Verificar que encontramos la escalera más alta (10-A)
            var highestStraight = analysis.Straights
                .FirstOrDefault(sf => sf.IsComplete &&
                                      sf.Cards
                                          .Select(c => c.Rank)
                                          .OrderBy(r => r)
                                          .SequenceEqual(new[] { 10, 11, 12, 13, 14 }));
            Assert.That(highestStraight, Is.Not.Null,
                "Should find 10-A straight");

            // Verificar que encontramos una escalera baja (5-9)
            var lowestStraight = analysis.Straights
                .FirstOrDefault(sf => sf.IsComplete &&
                                      sf.Cards
                                          .Select(c => c.Rank)
                                          .OrderBy(r => r)
                                          .SequenceEqual(new[] { 5, 6, 7, 8, 9 }));
            Assert.That(lowestStraight, Is.Not.Null,
                "Should find 5-9 straight");
        });
    }

    [Test]
    public void WithSevenConsecutiveCards_ShouldFindMultipleStraightPossibilities() {
        // 5,6,7,8,9,10,J de diferentes palos
        var cards = CreateCards("5C", "6H", "7D", "8S", "9C", "TC", "JH");
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            Assert.That(analysis.Straights.Count, Is.GreaterThan(1),
                "Should find multiple straight possibilities");

            // Verificar la escalera más baja (5-9)
            var lowestStraight = analysis.Straights
                .FirstOrDefault(sf => sf.IsComplete &&
                                      sf.Cards
                                          .Select(c => c.Rank)
                                          .OrderBy(r => r)
                                          .SequenceEqual(new[] { 5, 6, 7, 8, 9 }));
            Assert.That(lowestStraight, Is.Not.Null,
                "Should find 5-9 straight");

            // Verificar la escalera más alta (7-J)
            var highestStraight = analysis.Straights
                .FirstOrDefault(sf => sf.IsComplete &&
                                      sf.Cards
                                          .Select(c => c.Rank)
                                          .OrderBy(r => r)
                                          .SequenceEqual(new[] { 7, 8, 9, 10, 11 }));
            Assert.That(highestStraight, Is.Not.Null,
                "Should find 7-J straight");
        });
    }

    [Test]
    public void WithGap_ShouldNotIdentifyStraight() {
        // 2,3,4,6,7 (falta el 5)
        var cards = CreateCards("2C", "3H", "4D", "6S", "7C");
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(straights, Is.Empty,
            "Should not identify straight when there's a gap");
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraight_OneGap() {
        // 8,9,J,Q,K (falta 10)
        var cards = CreateCards("8H", "9C", "JD", "QS", "KC");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(8),
                "Should discard 8 as it's not part of the best potential straight");
        });
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraight_TwoGaps() {
        // 7,8,10,J,K (faltan 9,Q)
        var cards = CreateCards("7H", "8C", "TD", "JS", "KC");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");

            // Verificamos que descarta las cartas correctas sin importar el orden
            var discardedRanks = discards[0].Select(c => c.Rank).ToList();
            Assert.That(discardedRanks, Is.EquivalentTo(new[] { 13 }),
                "Should discard K (13) as it's less likely to complete a straight");
        });
    }

    [Test]
    public void SuggestDiscards_WithMultiplePotentialStraights() {
        // 7,8,9,J,Q,2,3 (dos posibilidades de escalera)
        var cards = CreateCards("7H", "8C", "9D", "JH", "QC", "2S", "3D");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 3);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(3), "Should suggest discarding 3 cards");
            Assert.That(discards[0].Contains(cards.First(c => c.Rank == 2)), Is.True,
                "Should discard 2");
            Assert.That(discards[0].Contains(cards.First(c => c.Rank == 3)), Is.True,
                "Should discard 3");
            Assert.That(discards[0].Contains(cards.First(c => c.Rank == 7)), Is.True,
                "Should discard 7");
        });
    }

    [Test]
    public void SuggestDiscards_WithLimitedMaxDiscards() {
        // 7,8,9,J,Q,2,3,4 - El código prioriza la escalera más alta posible
        var cards = CreateCards("7H", "8C", "9D", "JH", "QC", "2S", "3D", "4H");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding exactly 2 cards");

            var discardedRanks = discards[0].Select(c => c.Rank).ToList();
            Assert.That(discardedRanks, Contains.Item(4), "Should discard 4");
            Assert.That(discardedRanks, Contains.Item(7), "Should discard 7");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_Complete() {
        // A,2,3,4,5 de diferentes palos
        var cards = CreateCards("AC", "2H", "3D", "4S", "5C");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.True);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight with low Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_Complete() {
        // 10,J,Q,K,A de diferentes palos
        var cards = CreateCards("TC", "JH", "QD", "KS", "AC");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.True);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight with high Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_OneGap() {
        // A,2,4,5,6 (falta 3)
        var cards = CreateCards("AC", "2H", "4D", "5S", "6C");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.False);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(14),
                "Should discard A to try to get 3 and complete 2-6");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_OneGap() {
        // J,Q,K,A,3 (falta 10 para escalera alta)
        var cards = CreateCards("JC", "QH", "KD", "AC", "3S");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.Straight];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(3),
                "Should discard 3 and keep J,Q,K,A as they form potential high straight");
        });
    }

    [Test]
    public void EmptyHand_ShouldReturnNoStraight() {
        var straights = PokerHandDefinition.Prototypes[PokerHandType.Straight].IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(straights, Is.Empty);
    }
}