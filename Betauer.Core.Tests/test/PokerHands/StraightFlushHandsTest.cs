using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class StraightFlushHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicStraightFlush_ShouldBeIdentified() {
        // 10,J,Q,K,A todos de corazones
        var cards = CreateCards("TH", "JH", "QH", "KH", "AH");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straightFlushes.Count, Is.EqualTo(1), "Should identify exactly one straight flush");
            Assert.That(straightFlushes[0].Cards.Count, Is.EqualTo(5), "Straight flush should have 5 cards");

            var ranks = straightFlushes[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 10, 11, 12, 13, 14 }),
                "Should be 10,J,Q,K,A in order");
            Assert.That(straightFlushes[0].Cards.All(c => c.Suit == 'H'),
                "All cards should be hearts");
        });
    }

    [Test]
    public void LowStraightFlush_WithAceAsOne_ShouldBeIdentified() {
        // A,2,3,4,5 todos de picas
        var cards = CreateCards("AS", "2S", "3S", "4S", "5S");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straightFlushes.Count, Is.EqualTo(1), "Should identify exactly one straight flush");

            var orderedCards = straightFlushes[0].Cards
                .OrderBy(c => c.Rank == 14 ? 1 : c.Rank)
                .ToList();

            Assert.That(orderedCards.Select(c => c.Rank == 14 ? 1 : c.Rank),
                Is.EqualTo(new[] { 1, 2, 3, 4, 5 }), "Should be A,2,3,4,5 with Ace as 1");
            Assert.That(straightFlushes[0].Cards.All(c => c.Suit == 'S'),
                "All cards should be spades");
        });
    }

    [Test]
    public void WithDuplicateRanks_ShouldIdentifyStraightFlush() {
        // A♥,2♥,2♣,3♥,4♥,5♥ (2 duplicado pero en diferente palo)
        var cards = CreateCards("AH", "2H", "2C", "3H", "4H", "5H");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(straightFlushes.Count, Is.EqualTo(1),
            "Should identify straight flush even with duplicate ranks in different suit");
    }

    [Test]
    public void WithTenCards_ShouldFindMultipleStraightFlushPossibilities() {
        // 6,7,8,9,10,J,Q,K,A todos de corazones + 5 de corazones
        var cards = CreateCards("5H", "6H", "7H", "8H", "9H", "TH", "JH", "QH", "KH", "AH");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straightFlushes.Count, Is.GreaterThan(1),
                "Should find multiple straight flush possibilities");

            // Verificar que todas son del mismo palo
            Assert.That(straightFlushes.All(sf => sf.Cards.All(c => c.Suit == 'H')),
                "All straight flushes should be hearts");

            // Verificar la escalera más alta (10-A)
            var highestStraight = straightFlushes
                .FirstOrDefault(sf => {
                    var orderedRanks = sf.Cards
                        .Select(c => c.Rank)
                        .OrderBy(r => r)
                        .ToList();
                    return orderedRanks.SequenceEqual(new[] { 10, 11, 12, 13, 14 });
                });
            Assert.That(highestStraight, Is.Not.Null,
                "Should find 10-A straight flush");

            // Verificar la escalera más baja (5-9)
            var lowestStraight = straightFlushes
                .FirstOrDefault(sf => {
                    var orderedRanks = sf.Cards
                        .Select(c => c.Rank)
                        .OrderBy(r => r)
                        .ToList();
                    return orderedRanks.SequenceEqual(new[] { 5, 6, 7, 8, 9 });
                });
            Assert.That(lowestStraight, Is.Not.Null,
                "Should find 5-9 straight flush");

            // Verificar que las straight flushes están ordenadas por la carta más alta
            var firstRanks = straightFlushes[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            var lastRanks = straightFlushes[^1].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();

            Assert.That(firstRanks[^1], Is.GreaterThan(lastRanks[^1]),
                "First straight flush should have higher top card than last one");
        });
    }

    [Test]
    public void WithSevenConsecutiveCards_ShouldFindMultipleStraightPossibilities() {
        // 5♥,6♥,7♥,8♥,9♥,T♥,J♥
        var cards = CreateCards("5H", "6H", "7H", "8H", "9H", "TH", "JH");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var straightFlushes = hand.IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straightFlushes.Count, Is.GreaterThan(1),
                "Should find multiple straight flush possibilities");

            // Verificar la escalera más alta (7-J)
            var highestStraight = straightFlushes[0];
            var highestRanks = highestStraight.Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(highestRanks, Is.EqualTo(new[] { 7, 8, 9, 10, 11 }),
                "First straight flush should be 7-J (highest possible)");

            // Verificar que la siguiente es 6-10
            var middleStraight = straightFlushes[1];
            var middleRanks = middleStraight.Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(middleRanks, Is.EqualTo(new[] { 6, 7, 8, 9, 10 }),
                "Second straight flush should be 6-10");

            // Verificar que la última es 5-9
            var lowestStraight = straightFlushes[2];
            var lowestRanks = lowestStraight.Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(lowestRanks, Is.EqualTo(new[] { 5, 6, 7, 8, 9 }),
                "Third straight flush should be 5-9 (lowest possible)");

            // Verificar que todas son del mismo palo
            Assert.That(straightFlushes.All(sf => sf.Cards.All(c => c.Suit == 'H')),
                "All straight flushes should be hearts");
        });
    }

    [Test]
    public void WithMultipleStraightFlushes_ShouldBeOrderedByHighestCard() {
        // Dos straight flushes: T-A de corazones y 9-K de picas
        var cards = CreateCards(
            "TH", "JH", "QH", "KH", "AH", // T-A hearts
            "9S", "TS", "JS", "QS", "KS" // 9-K spades
        );
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(straightFlushes.Count, Is.EqualTo(2),
                "Should identify both straight flushes");

            // Verificar la primera straight flush (T-A de corazones)
            var highestRanks = straightFlushes[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(highestRanks, Is.EqualTo(new[] { 10, 11, 12, 13, 14 }),
                "First should be 10-A in hearts (highest possible)");
            Assert.That(straightFlushes[0].Cards.All(c => c.Suit == 'H'),
                "First straight flush should be hearts");

            // Verificar la segunda straight flush (9-K de picas)
            var lowestRanks = straightFlushes[1].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(lowestRanks, Is.EqualTo(new[] { 9, 10, 11, 12, 13 }),
                "Second should be 9-K in spades");
            Assert.That(straightFlushes[1].Cards.All(c => c.Suit == 'S'),
                "Second straight flush should be spades");
        });
    }

    [Test]
    public void WithSameColorButNoStraight_ShouldNotIdentifyStraightFlush() {
        // 2,3,4,6,7 todos de tréboles (falta el 5)
        var cards = CreateCards("2C", "3C", "4C", "6C", "7C");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(straightFlushes, Is.Empty,
            "Should not identify straight flush when there's a gap");
    }

    [Test]
    public void WithStraightButDifferentColors_ShouldNotIdentifyStraightFlush() {
        // 2,3,4,5,6 de diferentes palos
        var cards = CreateCards("2H", "3S", "4D", "5C", "6H");
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(straightFlushes, Is.Empty,
            "Should not identify straight flush when cards are different suits");
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraightFlush_OneGap() {
        // 8♥,9♥,J♥,Q♥,K♣ (falta 10♥)
        var cards = CreateCards("8H", "9H", "JH", "QH", "KC");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Suit, Is.EqualTo('C'), "Should discard the non-heart card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(13), "Should discard K♣");
        });
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraightFlush_TwoGaps() {
        // 7♥,8♥,T♥,J♥,K♥ (faltan 9,Q)
        var cards = CreateCards("7H", "8H", "TH", "JH", "KH");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0].Select(c => c.Rank).ToList(),
                Is.EquivalentTo(new[] { 13 }),
                "Should discard K as it's less likely to complete a straight flush");
        });
    }

    [Test]
    public void SuggestDiscards_WithMultiplePotentialStraightFlushes() {
        // 7♥,8♥,9♥,J♥,Q♥,2♣,3♣ (dos posibilidades en corazones)
        var cards = CreateCards("7H", "8H", "9H", "JH", "QH", "2C", "3C");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            Assert.That(analysis.Straights.Count(s => s.IsFlush), Is.GreaterThan(0));
            var discards = hand.SuggestDiscards(analysis, 3);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(3), "Should suggest discarding 3 cards");
            Assert.That(discards[0].Contains(cards.First(c => c.Suit == 'C' && c.Rank == 2)), Is.True,
                "Should discard 2♣");
            Assert.That(discards[0].Contains(cards.First(c => c.Suit == 'C' && c.Rank == 3)), Is.True,
                "Should discard 3♣");
            Assert.That(discards[0].Contains(cards.First(c => c.Suit == 'H' && c.Rank == 7)), Is.True,
                "Should discard 7♥");
        });
    }

    [Test]
    public void SuggestDiscards_WithLimitedMaxDiscards() {
        // 7♥,8♥,9♥,J♥,Q♥,2♣,3♣,4♣ (tres tréboles, pero solo podemos descartar 2)
        var cards = CreateCards("7H", "8H", "9H", "JH", "QH", "2C", "3C", "4C");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding exactly 2 cards");
            Assert.That(discards[0].All(c => c.Suit == 'C'),
                "Should discard only club cards");
            // Verificar que descarta las cartas más bajas
            Assert.That(discards[0].Select(c => c.Rank).OrderBy(r => r).ToList(),
                Is.EquivalentTo(new[] { 2, 3 }), "Should discard lowest club cards");
        });
    }

    [Test]
    public void SuggestDiscards_WithMultipleSuits() {
        // 7♥,8♥,9♥,J♥,Q♥,2♣,3♣,4♦,5♦
        var cards = CreateCards("7H", "8H", "9H", "JH", "QH", "2C", "3C", "4D", "5D");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 3);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(3), "Should suggest discarding 3 cards");
            Assert.That(discards[0].All(c => c.Suit != 'H'),
                "Should not discard any heart cards");
            // Verificar que descarta las cartas más bajas primero
            var discardedRanks = discards[0].Select(c => c.Rank).OrderBy(r => r).ToList();
            Assert.That(discardedRanks, Is.EqualTo(new[] { 2, 3, 4 }),
                "Should discard lowest non-heart cards");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_Complete() {
        // A,2,3,4,5 todos de picas
        var cards = CreateCards("AS", "2S", "3S", "4S", "5S");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.True);
            Assert.That(analysis.Straights[0].IsFlush, Is.True);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight flush with low Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_Complete() {
        // 10,J,Q,K,A todos de corazones
        var cards = CreateCards("TH", "JH", "QH", "KH", "AH");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.True);
            Assert.That(analysis.Straights[0].IsFlush, Is.True);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight flush with high Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_OneGap() {
        // A♥,2♥,4♥,5♥,6♥ (falta 3♥)
        var cards = CreateCards("AH", "2H", "4H", "5H", "6H");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            Assert.That(analysis.HasCompleteStraight, Is.False);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(14),
                "Should discard A♥ to try to get 3♥ and complete 2-6");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_OneGap() {
        // J♥,Q♥,K♥,A♥,3♥ (falta T para escalera alta)
        var cards = CreateCards("JH", "QH", "KH", "AH", "3H");
        var hand = PokerHand.Prototypes[PokerHandType.StraightFlush];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);
        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(3),
                "Should discard 3♥ and keep J,Q,K,A as they form potential high straight flush");
        });
    }

    [Test]
    public void EmptyHand_ShouldReturnNoStraightFlush() {
        var straightFlushes = PokerHand.Prototypes[PokerHandType.StraightFlush].IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(straightFlushes, Is.Empty);
    }
}