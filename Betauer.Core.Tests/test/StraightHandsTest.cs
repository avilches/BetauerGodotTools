using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
[Only]
public class StraightHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicStraight_ShouldBeIdentified() {
        // 10,J,Q,K,A
        var cards = CreateCards("TS", "JH", "QD", "KC", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

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
    public void LowStraight_WithAceAsOne_ShouldBeIdentified() {
        // A,2,3,4,5 (As bajo)
        var cards = CreateCards("AS", "2H", "3D", "4C", "5H");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

        Assert.Multiple(() => {
            Assert.That(straights.Count, Is.EqualTo(1), "Should identify exactly one straight");
            
            var orderedCards = straights[0].Cards
                .OrderBy(c => c.Rank == 14 ? 1 : c.Rank) // As cuenta como 1
                .ToList();
            
            Assert.That(orderedCards.Select(c => c.Rank == 14 ? 1 : c.Rank),
                Is.EqualTo(new[] { 1, 2, 3, 4, 5 }), "Should be A,2,3,4,5 with Ace as 1");
        });
    }

    [Test]
    public void WithSixConsecutiveCards_ShouldFindTwoStraights() {
        // 6,7,8,9,10,J - debería generar 2 escaleras diferentes
        var cards = CreateCards("6H", "7H", "8H", "9H", "TH", "JH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

        Assert.Multiple(() => {
            // Deberíamos tener 2 escaleras: 6-10 y 7-J
            Assert.That(straights.Count, Is.EqualTo(2), "Should find two different straights");

            // Verificar primera escalera (6-10)
            var straight1Ranks = straights[0].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(straight1Ranks, Is.EqualTo(new[] { 6, 7, 8, 9, 10 }), 
                "First straight should be 6,7,8,9,10");

            // Verificar segunda escalera (7-J)
            var straight2Ranks = straights[1].Cards
                .Select(c => c.Rank)
                .OrderBy(r => r)
                .ToList();
            Assert.That(straight2Ranks, Is.EqualTo(new[] { 7, 8, 9, 10, 11 }), 
                "Second straight should be 7,8,9,10,J");
        });
    }

    [Test]
    public void WithGap_ShouldNotIdentifyStraight() {
        // 2,3,4,6,7 (falta el 5)
        var cards = CreateCards("2H", "3H", "4H", "6H", "7H");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();
        
        Assert.That(straights, Is.Empty, "Should not identify straight with a gap");
    }

    [Test]
    public void WithDuplicateRanks_ShouldIdentifyStraight() {
        // A,2,2,3,4,5 (As bajo, 2 duplicado)
        var cards = CreateCards("AS", "2H", "2D", "3C", "4H", "5S");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();

        Assert.That(straights.Count, Is.EqualTo(1), 
            "Should identify straight even with duplicate ranks");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoStraight() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var straights = hands.Where(h => h is StraightHand).ToList();
        Assert.That(straights, Is.Empty);
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraight_OneGap_Middle() {
        // 8,9,J,Q,K (falta 10)
        var cards = CreateCards("8H", "9H", "JH", "QH", "KH");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(1));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].All(c => c.Rank >= 12), Is.True,
                "Should discard Q,K and keep 8,9,J which form the longest sequence");
        });
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraight_TwoGaps_Consecutive() {
        // 8,9,Q,K,A (faltan 10,J)
        var cards = CreateCards("8H", "9H", "QH", "KH", "AH");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(1));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].All(c => c.Rank >= 13), Is.True,
                "Should discard K,A and keep 8,9,Q which form the longest sequence");
        });
    }

    [Test]
    public void SuggestDiscards_WithPotentialStraight_TwoGaps_Separated() {
        // 7,8,T,J,K (faltan 9,Q)
        var cards = CreateCards("7H", "8H", "TH", "JH", "KH");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(1));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].Contains(cards.First(c => c.Rank == 10)), Is.True,
                "Should discard T as it breaks the sequence");
            Assert.That(discards[0].Contains(cards.First(c => c.Rank == 13)), Is.True,
                "Should discard K as it's too far from the sequence");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_Complete() {
        // A,2,3,4,5
        var cards = CreateCards("AS", "2H", "3D", "4C", "5H");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights.Count, Is.EqualTo(1));
            Assert.That(analysis.PotentialStraights, Is.Empty);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight with low Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_Complete() {
        // 10,J,Q,K,A
        var cards = CreateCards("TS", "JH", "QD", "KC", "AH");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights.Count, Is.EqualTo(1));
            Assert.That(analysis.PotentialStraights, Is.Empty);
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards, Is.Empty, "Should not suggest discards for complete straight with high Ace");
        });
    }

    [Test]
    public void SuggestDiscards_WithLowAce_OneGap() {
        // A,2,4,5,6 (falta 3)
        var cards = CreateCards("AS", "2H", "4D", "5C", "6H");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(1));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].All(c => c.Rank >= 5), Is.True,
                "Should discard 5,6 and keep A,2,4 as they form potential low straight");
        });
    }

    [Test]
    public void SuggestDiscards_WithHighAce_OneGap() {
        // J,Q,K,A,3 (falta 2 para straight bajo)
        var cards = CreateCards("JH", "QH", "KH", "AH", "3H");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(1));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(3),
                "Should discard 3 and keep J,Q,K,A as they form potential high straight");
        });
    }

    [Test]
    [Only]
    public void SuggestDiscards_WithAce_TwoGaps() {
        // A,3,4,6,7 (faltan 2,5)
        var cards = CreateCards("AS", "3H", "4D", "6C", "TH");
        var hand = new StraightHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);
        Assert.Multiple(() => {
            Assert.That(analysis.CompleteStraights, Is.Empty);
            Assert.That(analysis.PotentialStraights.Count, Is.EqualTo(2));
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].All(c => c.Rank >= 6), Is.True,
                "Should discard 6,7 and keep A,3,4 as they form potential low straight");
        });
    }

    [Test]
    public void SuggestDiscards_WithExistingStraight_ShouldNotSuggestDiscards() {
        var cards = CreateCards("6H", "7H", "8H", "9H", "TH");
        var hand = new StraightHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when straight exists");
    }
}