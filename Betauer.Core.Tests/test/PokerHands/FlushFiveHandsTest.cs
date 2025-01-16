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
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

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
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

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
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with different suits");
    }

    [Test]
    public void WithDifferentRanks_ShouldNotIdentifyFlushFive() {
        // Cinco cartas del mismo palo pero diferente rango
        var cards = CreateCards("AH", "KH", "QH", "JH", "TH");
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with different ranks");
    }

    [Test]
    public void WithMultipleFlushFives_ShouldIdentifyAll() {
        // Dos grupos de cinco cartas del mismo palo y rango
        var cards = CreateCards(
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "KH", "KH", "KH", "KH", "KH"); // Cinco Reyes de corazones
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(2), "Should identify two flush fives");
        
            // Verificar el primer grupo (Aces, mayor rank)
            Assert.That(flushFives[0].Cards.All(c => c.Rank == 14), 
                "First group should be all Aces (highest rank)");
            Assert.That(flushFives[0].Cards.All(c => c.Suit == 'H'), 
                "First group should be all hearts");
        
            // Verificar el segundo grupo (Kings, menor rank)
            Assert.That(flushFives[1].Cards.All(c => c.Rank == 13), 
                "Second group should be all Kings (second highest rank)");
            Assert.That(flushFives[1].Cards.All(c => c.Suit == 'H'), 
                "Second group should be all hearts");
        });
    }
    
    [Test]
    public void WithAllRanks_ShouldBeOrderedFromHighestToLowest() {
        // Creamos FlushFives de todos los rangos posibles (en orden aleatorio)
        var cards = CreateCards(
            "2H", "2H", "2H", "2H", "2H",  // Dos
            "KH", "KH", "KH", "KH", "KH",  // Rey
            "5H", "5H", "5H", "5H", "5H",  // Cinco
            "AH", "AH", "AH", "AH", "AH",  // As
            "TH", "TH", "TH", "TH", "TH",  // Diez
            "7H", "7H", "7H", "7H", "7H"); // Siete

        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(6), "Should identify six flush fives");

            // Verificar que están ordenados de mayor a menor rank
            var ranks = flushFives.Select(h => h.Cards[0].Rank).ToList();
            var expectedRanks = new[] { 14, 13, 10, 7, 5, 2 }; // A, K, T, 7, 5, 2
            Assert.That(ranks, Is.EqualTo(expectedRanks), 
                "Hands should be ordered by rank from highest to lowest");

            // Verificar que cada grupo tiene el mismo rank y suit
            foreach (var hand in flushFives) {
                Assert.That(hand.Cards.All(c => c.Rank == hand.Cards[0].Rank), 
                    "All cards in a hand should have the same rank");
                Assert.That(hand.Cards.All(c => c.Suit == 'H'), 
                    "All cards should be hearts");
            }
        });
    }

    [Test]
    public void WithFourOfSameSuitAndRank_ShouldNotIdentifyFlushFive() {
        // Cuatro Ases de corazones (no es suficiente)
        var cards = CreateCards("AH", "AH", "AH", "AH");
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.That(flushFives, Is.Empty, 
            "Should not identify flush five with only four cards");
    }

    [Test]
    public void WithMultipleSuits_ShouldIdentifySeparateFlushFives() {
        // Cinco Ases de corazones y cinco Ases de picas
        var cards = CreateCards(
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "AS", "AS", "AS", "AS", "AS"); // Cinco Ases de picas
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

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
        var hand = PokerHand.Prototypes[PokerHandType.FlushFive];
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.That(hand.SuggestDiscards(analysis, 2), Is.Empty,
            "Flush five hand should not suggest any discards");
    }

    [Test]
    public void WithSixCards_OrderedByRank() {
        // Verificar que cuando hay múltiples FlushFive, se ordenan por rango
        var cards = CreateCards(
            "KH", "KH", "KH", "KH", "KH",  // Cinco Reyes de corazones
            "AH", "AH", "AH", "AH", "AH",  // Cinco Ases de corazones
            "QH", "QH", "QH", "QH", "QH"); // Cinco Reinas de corazones
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(3), "Should identify three flush fives");
            
            // Verificar el orden por rango (A > K > Q)
            var ranks = flushFives.Select(h => h.Cards[0].Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12 }), 
                "Should be ordered by rank: A, K, Q");
        });
    }

    [Test]
    public void IdentifyHands_WithTwoFlushFives_ShouldBeOrderedByRank() {
        // Ten y King de corazones, mezclados en el orden de entrada
        var cards = CreateCards(
            "TH", "TH", "TS", "TH", "TH", "TH",  // Cinco 10 de corazones
            "KH", "KH", "KS", "KH", "KH", "KH");  // Cinco Reyes de corazones
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.Multiple(() => {
            Assert.That(flushFives.Count, Is.EqualTo(2), "Should identify two flush fives");

            // Verificar que el primer grupo es de Reyes (rank 13)
            Assert.That(flushFives[0].Cards.All(c => c.Rank == 13),
                "First group should be all Kings (rank 13)");

            // Verificar que el segundo grupo es de 10 (rank 10)
            Assert.That(flushFives[1].Cards.All(c => c.Rank == 10),
                "Second group should be all Tens (rank 10)");
        });
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFlushFive() {
        var flushFives = PokerHand.Prototypes[PokerHandType.FlushFive].IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(flushFives, Is.Empty);
    }
}