using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]

public class FlushHouseHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFlushHouse_ShouldBeIdentified() {
        // Caso básico: exactamente 3 Ases y 2 Reyes del mismo palo
        var cards = CreateCards("AH", "AH", "AH", "KH", "KH");
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(flushHouses.Count, Is.EqualTo(1), "Should identify exactly one flush house");
            Assert.That(flushHouses[0].Cards.Count, Is.EqualTo(5), "Flush house should have exactly 5 cards");
            Assert.That(flushHouses[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");

            var groups = flushHouses[0].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ToList();
            
            Assert.That(groups.Count, Is.EqualTo(2), "Should have exactly 2 different ranks");
            Assert.That(groups[0].Count(), Is.EqualTo(3), "Should have 3 cards of one rank");
            Assert.That(groups[1].Count(), Is.EqualTo(2), "Should have 2 cards of another rank");
            Assert.That(groups[0].Key, Is.EqualTo(14), "Higher rank should be Ace");
            Assert.That(groups[1].Key, Is.EqualTo(13), "Lower rank should be King");
        });
    }

    [Test]
    public void WithMultiplePossibilities_ShouldIdentifyAllCombinations() {
        // 3 Ases y 3 Reyes del mismo palo - debe generar dos combinaciones
        var cards = CreateCards("AH", "AH", "AH", "KH", "KH", "KH");
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(flushHouses.Count, Is.EqualTo(2), "Should identify two flush houses");
            
            // Verificar las combinaciones (AAA-KK y KKK-AA)
            var combinations = flushHouses.Select(h => {
                var groups = h.Cards
                    .GroupBy(c => c.Rank)
                    .OrderByDescending(g => g.Count())
                    .ToList();
                return (Three: groups[0].Key, Two: groups[1].Key);
            }).ToList();

            Assert.That(combinations, Has.Member((Three: 14, Two: 13)), "Should have AAA-KK combination");
            Assert.That(combinations, Has.Member((Three: 13, Two: 14)), "Should have KKK-AA combination");
            
            // Verificar que todas las cartas son del mismo palo
            Assert.That(flushHouses.All(h => h.Cards.All(c => c.Suit == 'H')), 
                "All cards in both combinations should be hearts");
        });
    }

    [Test]
    public void WithExtraCards_ShouldIdentifyFlushHouses() {
        // Caso con cartas extra: 4 Ases, 3 Reyes y 2 Reinas de corazones
        var cards = CreateCards("AH", "AH", "AH", "AH", "KH", "KH", "KH", "QH", "QH");
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            // Debería encontrar cuatro combinaciones: AAA-KK, AAA-QQ, KKK-AA, KKK-QQ
            Assert.That(flushHouses.Count, Is.EqualTo(4), "Should identify four flush houses");

            var combinations = flushHouses.Select(h => {
                var groups = h.Cards
                    .GroupBy(c => c.Rank)
                    .OrderByDescending(g => g.Count())
                    .ToList();
                return (Three: groups[0].Key, Two: groups[1].Key);
            }).ToList();

            Assert.That(combinations, Has.Member((Three: 14, Two: 13)), "Should have AAA-KK");
            Assert.That(combinations, Has.Member((Three: 14, Two: 12)), "Should have AAA-QQ");
            Assert.That(combinations, Has.Member((Three: 13, Two: 14)), "Should have KKK-AA");
            Assert.That(combinations, Has.Member((Three: 13, Two: 12)), "Should have KKK-QQ");
        });
    }

    [Test]
    public void NotEnoughCards_ShouldNotIdentifyFlushHouse() {
        Assert.Multiple(() => {
            // Caso 1: Solo tres cartas del mismo palo
            var cards1 = CreateCards("AH", "AH", "AH");
            var flushHouses1 = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards1));
            Assert.That(flushHouses1, Is.Empty, "Should not identify flush house with only three cards");

            // Caso 2: Cuatro cartas del mismo palo pero sin la estructura correcta
            var cards2 = CreateCards("AH", "AH", "KH", "QH");
            var flushHouses2 = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards2));
            Assert.That(flushHouses2, Is.Empty, "Should not identify flush house with only four cards");
        });
    }

    [Test]
    public void DifferentSuits_ShouldNotIdentifyFlushHouse() {
        // Full house normal (diferentes palos) no debería identificarse como flush house
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(flushHouses, Is.Empty, "Should not identify flush house with different suits");
    }

    [Test]
    public void SameRankFlush_ShouldNotIdentifyFlushHouse() {
        // 5 Ases del mismo palo no deberían formar un flush house
        var cards = CreateCards("AH", "AH", "AH", "AH", "AH");
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));
        
        Assert.That(flushHouses, Is.Empty, "Should not identify flush house with five cards of same rank");
    }

    [Test]
    public void NoValidGroups_ShouldNotIdentifyFlushHouse() {
        Assert.Multiple(() => {
            // Caso 1: cinco cartas del mismo palo pero sin grupos válidos
            var cards1 = CreateCards("AH", "KH", "QH", "JH", "TH");
            var flushHouses1 = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards1));

            Assert.That(flushHouses1, Is.Empty, 
                "Should not identify flush house with no valid groups");

            // Caso 2: par y tres cartas sueltas del mismo palo
            var cards2 = CreateCards("AH", "AH", "KH", "QH", "JH");
            var flushHouses2 = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards2));

            Assert.That(flushHouses2, Is.Empty, 
                "Should not identify flush house with only one pair");
        });
    }

    [Test]
    public void SuggestDiscards_WhenNoFlushHouse() {
        // Tenemos una pareja de Ases de corazones y un K de corazones y cartas sueltas
        var cards = CreateCards("AH", "AH", "KH", "KS", "QD");
        var hand = new FlushHouseHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            var discard = discards[0];
            // Solo debería descartar la Q que no es de corazones ni forma parte de un grupo
            Assert.That(discard.Count, Is.EqualTo(1), "Should discard 1 card");
            Assert.That(discard, Does.Contain(cards.First(c => c.Rank == 12)), "Should discard Q");
            // No debería descartar KS porque forma parte de un grupo con KH
            Assert.That(discard, Does.Not.Contain(cards.First(c => c.Suit == 'S' && c.Rank == 13)), 
                "Should not discard KS as it's part of a K group");
        });
    }

    [Test]
    public void SuggestDiscards_WithCompleteFlushHouse() {
        // Caso con flush house completo (no debería sugerir descartes)
        var cards = CreateCards("AH", "AH", "AH", "KH", "KH");
        var hand = new FlushHouseHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.That(hand.SuggestDiscards(analysis, 2), Is.Empty,
            "Should not suggest discards with complete flush house");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFlushHouse() {
        var flushHouses = new FlushHouseHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(flushHouses, Is.Empty, "Should not identify any flush house in empty hand");
    }
}