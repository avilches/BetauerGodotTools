using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class TwoPairHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicTwoPair_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.Multiple(() => {
            Assert.That(twoPairs.Count, Is.EqualTo(1), "Should identify exactly one two pair hand");
            Assert.That(twoPairs[0].Cards.Count, Is.EqualTo(4), "Two pair should have 4 cards");
            
            // Verificar que son los pares más altos (Ases y Reyes)
            var ranks = twoPairs[0].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13 }), "Should be Aces and Kings");
        });
    }

    [Test]
    public void WithThreePairs_ShouldIdentifyHighestTwoPairs() {
        // Con tres pares (A,A,K,K,Q,Q), debería identificar solo A-K
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "2C");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.Multiple(() => {
            Assert.That(twoPairs.Count, Is.EqualTo(1), "Should identify exactly one two pair hand");
            Assert.That(twoPairs[0].Cards.Count, Is.EqualTo(4), "Two pair should have 4 cards");
            
            // Verificar que son los pares más altos (Ases y Reyes)
            var ranks = twoPairs[0].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13 }), "Should be Aces and Kings");
        });
    }

    [Test]
    public void TwoPair_WithFullHouse_ShouldBeIdentified() {
        // Con full house (AAA KK), debería poder identificar doble pareja AK
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();

        Assert.Multiple(() => {
            Assert.That(twoPairs.Count, Is.EqualTo(1), "Should identify exactly one two pair hand");
            Assert.That(twoPairs[0].Cards.Count, Is.EqualTo(4), "Two pair should have 4 cards");
            
            var ranks = twoPairs[0].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13 }), "Should be Aces and Kings");
        });
    }

    [Test]
    public void SinglePair_ShouldNotIdentifyTwoPair() {
        var cards = CreateCards("AS", "AH", "KS", "QH", "JD");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        
        Assert.That(twoPairs, Is.Empty, "Should not identify two pair with only one pair");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoTwoPairs() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var twoPairs = hands.Where(h => h is TwoPairHand).ToList();
        Assert.That(twoPairs, Is.Empty);
    }

    [Test]
    public void SuggestDiscards_WithoutTwoPair_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "KH", "QD", "JC", "TH");
        var hand = new TwoPairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 2);
        
        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.All(c => c.Rank <= 11), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingTwoPair_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var hand = new TwoPairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(cards), 3);
        
        Assert.That(discards, Is.Empty, "Should not suggest discards when two pair exists");
    }
}