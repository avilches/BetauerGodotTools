using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]

public class HighCardHandsTest : PokerHandsTestBase {
    [Test]
    public void IdentifyHighCards_ShouldIdentifyAllCards() {
        // Cinco cartas diferentes
        var cards = CreateCards("AH", "KS", "QD", "JC", "TH");
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(highCards.Count, Is.EqualTo(5), "Should identify each card as a high card");
            
            // Verificar que las cartas están ordenadas por rango descendente
            var ranks = highCards.Select(h => h.Cards[0].Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12, 11, 10 }),
                "Should be ordered by rank descending");
        });
    }

    [Test]
    public void WithDuplicateRanks_ShouldIdentifyAllCards() {
        // Cartas con rangos duplicados
        var cards = CreateCards("AH", "AS", "KD", "KH", "QC");
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(highCards.Count, Is.EqualTo(5), "Should identify each card as a high card");
            
            // Verificar que hay dos Ases y dos Reyes
            var aces = highCards.Count(h => h.Cards[0].Rank == 14);
            var kings = highCards.Count(h => h.Cards[0].Rank == 13);
            Assert.That(aces, Is.EqualTo(2), "Should identify two Aces");
            Assert.That(kings, Is.EqualTo(2), "Should identify two Kings");
        });
    }

    [Test]
    public void WithNumericCards_ShouldIdentifyAllCards() {
        // Cartas numéricas (sin figuras)
        var cards = CreateCards("2H", "5S", "7D", "8C", "TH");
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(highCards.Count, Is.EqualTo(5), "Should identify each card as a high card");
            
            // Verificar que las cartas están ordenadas por rango descendente
            var ranks = highCards.Select(h => h.Cards[0].Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 10, 8, 7, 5, 2 }),
                "Should be ordered by rank descending");
        });
    }

    [Test]
    public void WithSingleCard_ShouldIdentifyOneHighCard() {
        // Una sola carta
        var cards = CreateCards("AH");
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(highCards.Count, Is.EqualTo(1), "Should identify one high card");
            Assert.That(highCards[0].Cards.Count, Is.EqualTo(1), "Should have exactly one card");
            Assert.That(highCards[0].Cards[0].Rank, Is.EqualTo(14), "Should be an Ace");
        });
    }

    [Test]
    public void WithAllSuitsSameRank_ShouldIdentifyAllCards() {
        // Los cuatro palos del mismo rango
        var cards = CreateCards("AH", "AS", "AD", "AC");
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, cards));

        Assert.Multiple(() => {
            Assert.That(highCards.Count, Is.EqualTo(4), "Should identify all four Aces");
            Assert.That(highCards.All(h => h.Cards[0].Rank == 14), "All should be Aces");
            
            // Verificar que están todos los palos
            var suits = highCards.Select(h => h.Cards[0].Suit).ToList();
            Assert.That(suits, Contains.Item('H'), "Should contain hearts");
            Assert.That(suits, Contains.Item('S'), "Should contain spades");
            Assert.That(suits, Contains.Item('D'), "Should contain diamonds");
            Assert.That(suits, Contains.Item('C'), "Should contain clubs");
        });
    }

    [Test]
    public void SuggestDiscards_ShouldReturnEmpty() {
        // Cualquier combinación de cartas
        var cards = CreateCards("AH", "KS", "QD", "JC", "TH");
        var hand = new HighCardHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(Handler.Config, cards);

        Assert.That(hand.SuggestDiscards(analysis, 2), Is.Empty,
            "High card hand should not suggest any discards");
    }

    [Test]
    public void EmptyHand_ShouldReturnNoHighCards() {
        var highCards = new HighCardHand(HandsManager,[]).IdentifyHands(new PokerHandAnalysis(Handler.Config, []));
        Assert.That(highCards, Is.Empty);
    }
}