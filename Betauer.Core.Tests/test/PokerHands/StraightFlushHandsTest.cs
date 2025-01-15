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
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

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
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

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
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

        Assert.That(straightFlushes.Count, Is.EqualTo(1),
            "Should identify straight flush even with duplicate ranks in different suit");
    }

    [Test]
    public void WithTenCards_ShouldFindMultipleStraightFlushPossibilities() {
        // 6,7,8,9,10,J,Q,K,A todos de corazones + 5 de corazones
        var cards = CreateCards("5H", "6H", "7H", "8H", "9H", "TH", "JH", "QH", "KH", "AH");
        var analysis = new PokerHandAnalysis(cards);

        Assert.Multiple(() => {
            Assert.That(analysis.Straights.Count, Is.GreaterThan(1),
                "Should find multiple straight possibilities");
            Assert.That(analysis.Straights.All(sf => sf.Cards.All(c => c.Suit == 'H')),
                "All straights should be hearts");

            // Verificar que encontramos la escalera más alta (10-A)
            var highestStraight = analysis.Straights
                .FirstOrDefault(sf => !sf.Incomplete && 
                                    sf.Cards
                                        .Select(c => c.Rank)
                                        .OrderBy(r => r)
                                        .SequenceEqual(new[] { 10, 11, 12, 13, 14 }));
            Assert.That(highestStraight, Is.Not.Null,
                "Should find 10-A straight flush");

            // Verificar que encontramos una escalera baja (5-9)
            var lowestStraight = analysis.Straights
                .FirstOrDefault(sf => !sf.Incomplete && 
                                    sf.Cards
                                        .Select(c => c.Rank)
                                        .OrderBy(r => r)
                                        .SequenceEqual(new[] { 5, 6, 7, 8, 9 }));
            Assert.That(lowestStraight, Is.Not.Null,
                "Should find 5-9 straight flush");
        });
    }

    [Test]
    public void WithSevenConsecutiveCards_ShouldFindMultipleStraightPossibilities() {
        // 5♥,6♥,7♥,8♥,9♥,T♥,J♥
        var cards = CreateCards("5H", "6H", "7H", "8H", "9H", "TH", "JH");
        var analysis = new PokerHandAnalysis(cards);

        Assert.Multiple(() => {
            Assert.That(analysis.Straights.Count, Is.GreaterThan(1),
                "Should find multiple straight possibilities");

            // Verificar la escalera más baja (5-9)
            var lowestStraight = analysis.Straights
                .FirstOrDefault(sf => !sf.Incomplete && 
                                    sf.Cards
                                        .Select(c => c.Rank)
                                        .OrderBy(r => r)
                                        .SequenceEqual(new[] { 5, 6, 7, 8, 9 }));
            Assert.That(lowestStraight, Is.Not.Null,
                "Should find 5-9 straight flush");

            // Verificar la escalera más alta (7-J)
            var highestStraight = analysis.Straights
                .FirstOrDefault(sf => !sf.Incomplete && 
                                    sf.Cards
                                        .Select(c => c.Rank)
                                        .OrderBy(r => r)
                                        .SequenceEqual(new[] { 7, 8, 9, 10, 11 }));
            Assert.That(highestStraight, Is.Not.Null,
                "Should find 7-J straight flush");
        });
    }

    [Test]
    public void WithSameColorButNoStraight_ShouldNotIdentifyStraightFlush() {
        // 2,3,4,6,7 todos de tréboles (falta el 5)
        var cards = CreateCards("2C", "3C", "4C", "6C", "7C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

        Assert.That(straightFlushes, Is.Empty,
            "Should not identify straight flush when there's a gap");
    }

    [Test]
    public void WithStraightButDifferentColors_ShouldNotIdentifyStraightFlush() {
        // 2,3,4,5,6 de diferentes palos
        var cards = CreateCards("2H", "3S", "4D", "5C", "6H");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();

        Assert.That(straightFlushes, Is.Empty,
            "Should not identify straight flush when cards are different suits");
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraightFlush_OneGap() {
        // 8♥,9♥,J♥,Q♥,K♣ (falta 10♥)
        var cards = CreateCards("8H", "9H", "JH", "QH", "KC");
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
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
        var hand = new StraightFlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
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
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();
        Assert.That(straightFlushes, Is.Empty);
    }
}