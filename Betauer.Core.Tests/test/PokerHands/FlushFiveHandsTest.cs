using Betauer.Core.Deck.Hands;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class FlushFiveHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFlushFive_ShouldBeIdentified() {
        // Cinco Ases de corazones
        var cards = CreateCards("AH", "AH", "AH", "AH", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(1), "Should identify exactly one flush five");
            Assert.That(flushFives[0].Cards.Count, Is.EqualTo(5), "Should have exactly 5 cards");
            Assert.That(flushFives[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");
            Assert.That(flushFives[0].Cards.All(c => c.Rank == 14), "All cards should be Aces");
        });
    }

    [Test]
    public void WithMoreThanFiveCards_ShouldSelectExactlyFive() {
        // Seis Ases de corazones
        var cards = CreateCards("AH", "AH", "AH", "AH", "AH", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(1), "Should identify exactly one flush five");
            Assert.That(flushFives[0].Cards.Count, Is.EqualTo(5), "Should select exactly 5 cards");
            Assert.That(flushFives[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");
            Assert.That(flushFives[0].Cards.All(c => c.Rank == 14), "All cards should be Aces");
        });
    }

    [Test]
    public void WithDifferentSuits_ShouldNotIdentifyFlushFive() {
        // Cinco Ases de diferentes palos
        var cards = CreateCards("AH", "AS", "AD", "AC", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with different suits");
    }

    [Test]
    public void WithDifferentRanks_ShouldNotIdentifyFlushFive() {
        // Cinco cartas del mismo palo pero diferente rango
        var cards = CreateCards("AH", "KH", "QH", "JH", "TH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with different ranks");
    }

    [Test]
    public void WithMultipleFlushFives_ShouldIdentifyAll() {
        // Dos grupos de cinco cartas del mismo palo y rango
        var cards = CreateCards(
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "KH", "KH", "KH", "KH", "KH"); // Cinco Reyes de corazones
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand)
            .OrderByDescending(h => h.Cards[0].Rank)
            .ToList();

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(2), "Should identify two flush fives");
            
            // Verificar el grupo de Ases
            Assert.That(flushFives[0].Cards.All(c => c.Rank == 14), 
                "First group should be all Aces");
            Assert.That(flushFives[0].Cards.All(c => c.Suit == 'H'), 
                "First group should be all hearts");
            
            // Verificar el grupo de Reyes
            Assert.That(flushFives[1].Cards.All(c => c.Rank == 13), 
                "Second group should be all Kings");
            Assert.That(flushFives[1].Cards.All(c => c.Suit == 'H'), 
                "Second group should be all hearts");
        });
    }

    [Test]
    public void WithFourOfSameSuitAndRank_ShouldNotIdentifyFlushFive() {
        // Cuatro Ases de corazones (no es suficiente)
        var cards = CreateCards("AH", "AH", "AH", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with only four cards");
    }

    [Test]
    public void WithMultipleSuits_ShouldIdentifySeparateFlushFives() {
        // Cinco Ases de corazones y cinco Ases de picas
        var cards = CreateCards(
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "AS", "AS", "AS", "AS", "AS"); // Cinco Ases de picas
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(2), "Should identify two flush fives");
            Assert.That(flushFives.Count(h => h.Cards.All(c => c.Suit == 'H')), Is.EqualTo(1),
                "Should have one hearts flush five");
            Assert.That(flushFives.Count(h => h.Cards.All(c => c.Suit == 'S')), Is.EqualTo(1),
                "Should have one spades flush five");
            Assert.That(flushFives.All(h => h.Cards.All(c => c.Rank == 14)),
                "All cards should be Aces");
        });
    }

    [Test]
    public void SuggestDiscards_ShouldReturnEmpty() {
        // Las sugerencias de descarte no aplican para FlushFive
        var cards = CreateCards("AH", "AH", "AH", "AS", "AD");
        var hand = new FlushFiveHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        Assert.That(hand.SuggestDiscards(analysis, 2), Is.Empty,
            "Flush five hand should not suggest any discards");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFlushFive() {
        var hands = HandsManager.IdentifyAllHands(Handler, []);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();
        Assert.That(flushFives, Is.Empty);
    }

    [Test]
    public void WithSixCards_OrderedByRank() {
        // Verificar que cuando hay múltiples FlushFive, se ordenan por rango
        var cards = CreateCards(
            "KH", "KH", "KH", "KH", "KH",  // Cinco Reyes de corazones
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "QH", "QH", "QH", "QH", "QH"); // Cinco Reinas de corazones
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushFives = hands.Where(h => h is FlushFiveHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(3), "Should identify three flush fives");
            
            // Verificar el orden por rango (A > K > Q)
            var ranks = flushFives.Select(h => h.Cards[0].Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12 }), 
                "Should be ordered by rank: A, K, Q");
        });
    }
}