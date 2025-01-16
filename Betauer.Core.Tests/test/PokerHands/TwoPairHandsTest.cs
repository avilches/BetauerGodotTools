using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class TwoPairHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicTwoPair_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var twoPairs = new TwoPairHand(HandsManager, []).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

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
    public void WithThreePairs_ShouldIdentifyAllOrdered() {
        // Con tres pares (A,A,K,K,Q,Q), debería identificar las tres combinaciones ordenadas
        var cards = CreateCards("AS", "AH", "KS", "KH", "QS", "QH", "2C");
        var twoPairs = new TwoPairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(twoPairs.Count, Is.EqualTo(3), "Should identify three possible two pair combinations");

            // Primera combinación: A-K
            var firstPairRanks = twoPairs[0].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(firstPairRanks, Is.EqualTo(new[] { 14, 13 }),
                "First combination should be Aces and Kings");

            // Segunda combinación: A-Q
            var secondPairRanks = twoPairs[1].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(secondPairRanks, Is.EqualTo(new[] { 14, 12 }),
                "Second combination should be Aces and Queens");

            // Tercera combinación: K-Q
            var thirdPairRanks = twoPairs[2].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(thirdPairRanks, Is.EqualTo(new[] { 13, 12 }),
                "Third combination should be Kings and Queens");

            // Verificar que cada combinación tiene 4 cartas
            Assert.That(twoPairs.All(tp => tp.Cards.Count == 4), Is.True,
                "All combinations should have exactly 4 cards");
        });
    }

    [Test]
    public void TwoPair_WithFullHouse_ShouldIdentifyOnePair() {
        // Con full house (AAA KK), debería identificar solo una doble pareja AA KK
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var twoPairs = new TwoPairHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(twoPairs.Count, Is.EqualTo(1), "Should identify exactly one two pair hand");
            Assert.That(twoPairs[0].Cards.Count, Is.EqualTo(4), "Two pair should have 4 cards");

            // Verificar que son Ases y Reyes
            var ranks = twoPairs[0].Cards
                .GroupBy(c => c.Rank)
                .Select(g => g.Key)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13 }), "Should be Aces and Kings");

            // Verificar que usa exactamente dos Ases
            var aceCount = twoPairs[0].Cards.Count(c => c.Rank == 14);
            Assert.That(aceCount, Is.EqualTo(2), "Should use exactly two Aces");

            // Verificar que usa dos Reyes
            var kingCount = twoPairs[0].Cards.Count(c => c.Rank == 13);
            Assert.That(kingCount, Is.EqualTo(2), "Should use exactly two Kings");
        });
    }

    [Test]
    public void SinglePair_ShouldNotIdentifyTwoPair() {
        var cards = CreateCards("AS", "AH", "KS", "QH", "JD");
        var twoPairs = new TwoPairHand(HandsManager, []).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(twoPairs, Is.Empty, "Should not identify two pair with only one pair");
    }

    [Test]
    public void SuggestDiscards_WithoutTwoPair_ShouldSuggestDiscardingLowestCards() {
        var cards = CreateCards("AS", "KH", "QD", "JC", "TH");
        var hand = new TwoPairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 2);

        Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
        var cardsToDiscard = discards[0];
        Assert.That(cardsToDiscard.All(c => c.Rank <= 11), Is.True, "Should discard lowest cards");
    }

    [Test]
    public void SuggestDiscards_WithExistingTwoPair_ShouldNotSuggestDiscards() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var hand = new TwoPairHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);

        Assert.That(discards, Is.Empty, "Should not suggest discards when two pair exists");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoTwoPairs() {
        var twoPairs = new TwoPairHand(HandsManager, []).IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(twoPairs, Is.Empty);
    }
}