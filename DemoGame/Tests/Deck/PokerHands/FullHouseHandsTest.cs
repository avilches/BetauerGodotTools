using System.Linq;
using NUnit.Framework;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Tests.Deck.PokerHands;

[TestFixture]
public class FullHouseHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFullHouse_ShouldBeIdentified() {
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH");
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

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
    public void WithMultipleOptions_ShouldIdentifyBothOrdered() {
        // Con AAA KKK, debería identificar AAA-KK y KKK-AA, en ese orden
        var cards = CreateCards("KS", "KH", "KD", "AS", "AH", "AD");
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

        Assert.Multiple(() => {
            Assert.That(fullHouses.Count, Is.EqualTo(2), "Should identify both full houses");

            // Verificar primer full house (AAA-KK)
            var groups1 = fullHouses[0].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            Assert.That(groups1[0].Key, Is.EqualTo(14), "First full house should have Aces as three of a kind (higher rank)");
            Assert.That(groups1[1].Key, Is.EqualTo(13), "First full house should have Kings as pair");

            // Verificar segundo full house (KKK-AA)
            var groups2 = fullHouses[1].Cards
                .GroupBy(c => c.Rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            Assert.That(groups2[0].Key, Is.EqualTo(13), "Second full house should have Kings as three of a kind");
            Assert.That(groups2[1].Key, Is.EqualTo(14), "Second full house should have Aces as pair");
        });
    }

    [Test]
    public void WithMultipleOptions_ShouldBeOrderedByRank() {
        // Con AAA KKK QQQ, debería poder formar varios full houses ordenados por rango del trío y luego del par
        var cards = CreateCards("AS", "AH", "AD", "KS", "KH", "KD", "QS", "QH", "QD");
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

        Assert.Multiple(() => {
            // Deberían formarse 6 full houses:
            // AAA-KK, AAA-QQ, KKK-AA, KKK-QQ, QQQ-AA, QQQ-KK
            Assert.That(fullHouses.Count, Is.EqualTo(6), "Should identify all possible full houses");

            // Verificar el orden correcto
            var combinations = fullHouses.Select(fh => {
                var groups = fh.Cards
                    .GroupBy(c => c.Rank)
                    .OrderByDescending(g => g.Count())
                    .ThenByDescending(g => g.Key)
                    .ToList();
                return (ThreeOfAKind: groups[0].Key, Pair: groups[1].Key);
            }).ToList();

            // Verificar las combinaciones esperadas en orden
            var expectedCombinations = new[] {
                (ThreeOfAKind: 14, Pair: 13), // AAA-KK
                (ThreeOfAKind: 14, Pair: 12), // AAA-QQ
                (ThreeOfAKind: 13, Pair: 14), // KKK-AA
                (ThreeOfAKind: 13, Pair: 12), // KKK-QQ
                (ThreeOfAKind: 12, Pair: 14), // QQQ-AA
                (ThreeOfAKind: 12, Pair: 13) // QQQ-KK
            };

            for (var i = 0; i < expectedCombinations.Length; i++) {
                Assert.That(combinations[i].ThreeOfAKind, Is.EqualTo(expectedCombinations[i].ThreeOfAKind),
                    $"Full house {i} should have correct three of a kind");
                Assert.That(combinations[i].Pair, Is.EqualTo(expectedCombinations[i].Pair),
                    $"Full house {i} should have correct pair");
            }
        });
    }

    [Test]
    public void WithFourOfAKind_ShouldIdentifyFullHouse() {
        // Con AAAA-KK, debería poder formar full house AAA-KK
        var cards = CreateCards("AS", "AH", "AD", "AC", "KS", "KH");
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

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
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

        Assert.That(fullHouses, Is.Empty, "Should not identify full house with only three of a kind");
    }

    [Test]
    public void TwoPair_ShouldNotIdentifyFullHouse() {
        var cards = CreateCards("AS", "AH", "KS", "KH", "QD");
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), cards));

        Assert.That(fullHouses, Is.Empty, "Should not identify full house with only two pair");
    }

    [Test]
    public void SuggestDiscards_WithoutFullHouse_ShouldReturnEmpty() {
        var cards = CreateCards("AS", "AH", "KH", "QD", "JC");
        var hand = PokerHandDefinition.Prototypes[PokerHandType.FullHouse];
        var discards = hand.SuggestDiscards(new PokerHandAnalysis(new PokerHandConfig(), cards), 3);

        // Full house no sugiere descartes porque es una mano compleja de formar
        Assert.That(discards, Is.Empty, "Full house should not suggest discards");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFullHouse() {
        var fullHouses = PokerHandDefinition.Prototypes[PokerHandType.FullHouse].IdentifyHands(new PokerHandAnalysis(new PokerHandConfig(), []));
        Assert.That(fullHouses, Is.Empty);
    }
}