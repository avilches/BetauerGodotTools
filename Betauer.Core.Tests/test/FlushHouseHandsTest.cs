using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class FlushHouseHandsTest : PokerHandsTestBase {
    [Test]
    public void FlushHouse_CombinationCases() {
        Assert.Multiple(() => {
            // Caso 1: Exactamente 3 Ases y 2 Reyes del mismo suit - debe generar una sola mano
            var cards1 = CreateCards("AH", "AH", "AH", "KH", "KH");
            var hands1 = HandsManager.IdentifyAllHands(Handler, cards1)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands1.Count, Is.EqualTo(1), "With exactly 3 Aces and 2 Kings should generate only one hand");

            // Caso 2: 3 Ases y 3 Reyes del mismo suit - debe generar dos manos
            var cards2 = CreateCards("AH", "AH", "AH", "KH", "KH", "KH");
            var hands2 = HandsManager.IdentifyAllHands(Handler, cards2)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands2.Count, Is.EqualTo(2), "With 3 Aces and 3 Kings should generate two hands");

            // Verificar la estructura de cada mano en el caso 2
            var ranks2 = hands2.Select(h => {
                var groups = h.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
                return (Three: groups[0].Key, Two: groups[1].Key);
            }).ToList();

            Assert.That(ranks2, Has.Member((Three: 14, Two: 13))); // AAA-KK
            Assert.That(ranks2, Has.Member((Three: 13, Two: 14))); // KKK-AA

            // Caso 3: 4 Ases y 3 Reyes del mismo suit - debe generar dos manos
            var cards3 = CreateCards("AH", "AH", "AH", "AH", "KH", "KH", "KH");
            var hands3 = HandsManager.IdentifyAllHands(Handler, cards3)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands3.Count, Is.EqualTo(2), "With 4 Aces and 3 Kings should generate two hands");

            // Caso 4: 4 Ases y 4 Reyes del mismo suit - debe generar dos manos
            var cards4 = CreateCards("AH", "AH", "AH", "AH", "KH", "KH", "KH", "KH");
            var hands4 = HandsManager.IdentifyAllHands(Handler, cards4)
                .Where(h => h is FlushHouseHand).ToList();
            Assert.That(hands4.Count, Is.EqualTo(2), "With 4 Aces and 4 Kings should generate two hands");

            // Verificar que todas las manos tienen el mismo suit
            var allHands = hands1.Concat(hands2).Concat(hands3).Concat(hands4);
            Assert.That(allHands.All(h => h.Cards.All(c => c.Suit == 'H')), Is.True, "All cards should be hearts");

            // Verificar la estructura del flush house (3+2)
            Assert.That(allHands.All(h => {
                var groups = h.Cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
                return groups.Count == 2 && groups[0].Count() == 3 && groups[1].Count() == 2;
            }), Is.True, "Each hand should have exactly three of one rank and two of another");
        });
    }

    [Test]
    public void DifferentSuits_ShouldNotIdentifyFlushHouse() {
        // Full house normal (diferentes palos) no debería identificarse como flush house
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards)
            .Where(h => h is FlushHouseHand).ToList();
        
        Assert.That(hands, Is.Empty, "Should not identify flush house with different suits");
    }

    [Test]
    public void SameRankFlush_ShouldNotIdentifyFlushHouse() {
        // 5 Ases del mismo palo no deberían formar un flush house
        var cards = CreateCards("AH", "AH", "AH", "AH", "AH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards)
            .Where(h => h is FlushHouseHand).ToList();
        
        Assert.That(hands, Is.Empty, "Should not identify flush house with five cards of same rank");
    }

    [Test]
    public void ThreeOfAKindFlush_ShouldNotIdentifyFlushHouse() {
        // Trío del mismo palo no debería formar flush house
        var cards = CreateCards("AH", "AH", "AH", "KH", "QH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards)
            .Where(h => h is FlushHouseHand).ToList();
        
        Assert.That(hands, Is.Empty, "Should not identify flush house with only three of a kind");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFlushHouse() {
        var cards = new List<Card>();
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushHouses = hands.Where(h => h is FlushHouseHand).ToList();
        Assert.That(flushHouses, Is.Empty);
    }
}