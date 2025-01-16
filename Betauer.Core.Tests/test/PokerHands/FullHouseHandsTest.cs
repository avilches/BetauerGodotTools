using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class FullHouseHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFullHouse_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fullHouses.Count, Is.EqualTo(1), "Should identify exactly one full house");
            Assert.That(fullHouses[0].Cards.Count, Is.EqualTo(5), "Full house should have 5 cards");

            // Verificar la estructura del full house (AAA-KK)
            var groups = fullHouses[0].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            Assert.That(groups.Count, Is.EqualTo(2), "Should have exactly two groups");
            Assert.That(groups[0].Count(), Is.EqualTo(3), "First group should have three cards");
            Assert.That(groups[1].Count(), Is.EqualTo(2), "Second group should have two cards");
            Assert.That(groups[0].Key, Is.EqualTo(14), "Three of a kind should be Aces");
            Assert.That(groups[1].Key, Is.EqualTo(13), "Pair should be Kings");
        });
    }

    [Test]
    public void WithMultipleOptions_ShouldIdentifyBestFullHouse() {
        // Con AAA KKK, debería identificar AAA-KK, no KKK-AA
        var cards = CreateCards("KS", "KH", "KD", "AS", "AH", "AD");
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fullHouses.Count, Is.EqualTo(1), "Should identify exactly one full house");
            
            var groups = fullHouses[0].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            Assert.That(groups[0].Key, Is.EqualTo(14), "Three of a kind should be Aces (higher rank)");
            Assert.That(groups[1].Key, Is.EqualTo(13), "Pair should be Kings");
        });
    }

    [Test]
    public void WithFourOfAKind_ShouldIdentifyFullHouse() {
        // Con AAAA-KK, debería poder formar full house AAA-KK
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH");
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(fullHouses.Count, Is.EqualTo(1), "Should identify exactly one full house");
            
            var groups = fullHouses[0].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            Assert.That(groups[0].Key, Is.EqualTo(14), "Three of a kind should be Aces");
            Assert.That(groups[1].Key, Is.EqualTo(13), "Pair should be Kings");
        });
    }

    [Test]
    public void ThreeOfAKind_ShouldNotIdentifyFullHouse() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "QH");
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fullHouses, Is.Empty, "Should not identify full house with only three of a kind");
    }

    [Test]
    public void TwoPair_ShouldNotIdentifyFullHouse() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(fullHouses, Is.Empty, "Should not identify full house with only two pair");
    }

    [Test]
    public void SuggestDiscards_WithoutFullHouse_ShouldReturnEmpty() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hand = new FullHouseHand(HandsManager, []);
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(Handler.Config, cards), 3);
        
        // Full house no sugiere descartes porque es una mano compleja de formar
        Assert.That(discards, Is.Empty, "Full house should not suggest discards");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFullHouse() {
        var fullHouses = new FullHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(fullHouses, Is.Empty);
    }
}