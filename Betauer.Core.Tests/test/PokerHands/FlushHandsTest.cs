using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.PokerHands;

[TestFixture]
public class FlushHandsTest : PokerHandsTestBase {
    [Test]
    public void BasicFlush_ShouldBeIdentified() {
        // 5 cartas del mismo palo, no consecutivas
        var cards = CreateCards("2H", "5H", "7H", "JH", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify exactly one flush");
            Assert.That(flushes[0].Cards.Count, Is.EqualTo(5), "Flush should have exactly 5 cards");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");

            // Verificar que las cartas están ordenadas por rango descendente
            var ranks = flushes[0].Cards.Select(c => c.Rank).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 13, 11, 7, 5, 2 }),
                "Cards should be ordered by rank descending");
        });
    }

    [Test]
    public void WithMoreThanFiveCards_ShouldSelectHighestFive() {
        // 7 cartas del mismo palo
        var cards = CreateCards("2H", "5H", "7H", "9H", "JH", "QH", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify exactly one flush");
            Assert.That(flushes[0].Cards.Count, Is.EqualTo(5), "Should select exactly 5 cards");

            // Verificar que se seleccionaron las 5 cartas más altas
            var ranks = flushes[0].Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 13, 12, 11, 9, 7 }),
                "Should select the highest 5 cards");
        });
    }

    [Test]
    public void WithMultipleSuits_ShouldSelectHighestFlush() {
        // Dos flushes posibles: 5 corazones y 5 picas
        var cards = CreateCards("2H", "5H", "7H", "JH", "KH", "3S", "6S", "8S", "TS", "QS");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify only the highest flush");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'),
                "Should select hearts as they contain the King");
            var ranks = flushes[0].Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EquivalentTo(new[] { 13, 11, 7, 5, 2 }),
                "Should have correct ranks for heart flush");
        });
    }

    [Test]
    public void WithFourCardsOfSameSuit_ShouldNotIdentifyFlush() {
        var cards = CreateCards("2H", "5H", "7H", "JH", "KC");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.That(flushes, Is.Empty, "Should not identify flush with only 4 cards of same suit");
    }

    [Test]
    public void WithStraightFlush_ShouldIdentifyBothStraightFlushAndFlush() {
        var cards = CreateCards("9H", "TH", "JH", "QH", "KH");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);

        Assert.Multiple(() => {
            var straightFlushes = hands.Where(h => h is StraightFlushHand).ToList();
            Assert.That(straightFlushes.Count, Is.EqualTo(1), "Should identify straight flush");

            var flushes = hands.Where(h => h is FlushHand).ToList();
            Assert.That(flushes.Count, Is.EqualTo(1), "Should also identify as flush");
        });
    }

    [Test]
    public void SuggestDiscards_WithFourOfSameSuit() {
        // 4 corazones y una carta diferente
        var cards = CreateCards("7H", "9H", "JH", "KH", "4C");
        var hand = new FlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(1), "Should suggest discarding 1 card");
            Assert.That(discards[0][0].Suit, Is.EqualTo('C'), "Should discard the non-heart card");
            Assert.That(discards[0][0].Rank, Is.EqualTo(4), "Should discard the 4 of clubs");
        });
    }

    [Test]
    public void SuggestDiscards_WithThreeOfSameSuit() {
        // 3 corazones y dos cartas diferentes
        var cards = CreateCards("7H", "JH", "KH", "4C", "8D");
        var hand = new FlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 2);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(2), "Should suggest discarding 2 cards");
            Assert.That(discards[0].All(c => c.Suit != 'H'), "Should discard non-heart cards");
            var discardedRanks = discards[0].Select(c => c.Rank).OrderBy(r => r).ToList();
            Assert.That(discardedRanks, Is.EqualTo(new[] { 4, 8 }),
                "Should discard the 4 and 8 of non-heart suits");
        });
    }

    [Test]
    public void SuggestDiscards_WithCompleteFlush() {
        // Flush completo
        var cards = CreateCards("2H", "5H", "7H", "JH", "KH");
        var hand = new FlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        var discards = hand.SuggestDiscards(analysis, 2);
        Assert.That(discards.Count, Is.EqualTo(0),
            "Should not suggest any discards with complete flush");
    }

    [Test]
    public void SuggestDiscards_WithNoFlushPossibility() {
        // Todas las cartas de diferente palo
        var cards = CreateCards("2H", "5S", "7D", "JC", "KH");
        var hand = new FlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        var discards = hand.SuggestDiscards(analysis, 2);
        Assert.That(discards.Count, Is.EqualTo(0),
            "Should not suggest discards when no flush is possible");
    }

    [Test]
    public void SuggestDiscards_WithMultipleFlushPossibilities() {
        // 3 corazones y 3 picas
        var cards = CreateCards("7H", "JH", "KH", "8S", "TS", "QS");
        var hand = new FlushHand(HandsManager, []);
        var analysis = new PokerHandAnalysis(cards);

        Assert.Multiple(() => {
            var discards = hand.SuggestDiscards(analysis, 3);
            Assert.That(discards.Count, Is.EqualTo(1), "Should suggest one discard option");
            Assert.That(discards[0].Count, Is.EqualTo(3), "Should suggest discarding 3 cards");
            Assert.That(discards[0].All(c => c.Suit == 'S'),
                "Should discard spades as hearts have higher cards");
        });
    }

    [Test]
    public void EmptyHand_ShouldReturnNoFlush() {
        var hands = HandsManager.IdentifyAllHands(Handler, []);
        var flushes = hands.Where(h => h is FlushHand).ToList();
        Assert.That(flushes, Is.Empty);
    }

    [Test]
    public void WithEqualNumberOfCards_ShouldOrderByHighestCard() {
        // Dos flushes con el mismo número de cartas pero diferentes valores altos
        var cards = CreateCards(
            // Flush de corazones con K alta
            "2H", "5H", "7H", "TH", "KH",
            // Flush de picas con Q alta
            "3S", "6S", "8S", "JS", "QS"
        );
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify only the highest flush");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'),
                "Should select hearts as they contain the King");
        });
    }

    [Test]
    public void WithSameHighCard_ShouldCompareNextHighestCard() {
        // Dos flushes con K alta pero diferentes segundas cartas
        var cards = CreateCards(
            // Flush de corazones: K, Q, 7, 5, 2
            "2H", "5H", "7H", "QH", "KH",
            // Flush de picas: K, J, 8, 6, 3
            "3S", "6S", "8S", "JS", "KS"
        );
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify only the highest flush");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'),
                "Should select hearts as they contain K,Q vs K,J");
            var secondHighestCard = flushes[0].Cards
                .OrderByDescending(c => c.Rank)
                .Skip(1)
                .First();
            Assert.That(secondHighestCard.Rank, Is.EqualTo(12),
                "Second highest card should be Queen");
        });
    }

    [Test]
    public void WithIdenticalRanks_ShouldSelectFirst() {
        // Dos flushes con exactamente los mismos rangos
        var cards = CreateCards(
            "2H", "5H", "7H", "JH", "KH",
            "2S", "5S", "7S", "JS", "KS"
        );
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            Assert.That(flushes.Count, Is.EqualTo(1), "Should identify only one flush");
            // En este caso, cualquiera de los dos flushes es válido,
            // pero solo debe seleccionar uno
            var ranks = flushes[0].Cards
                .Select(c => c.Rank)
                .OrderByDescending(r => r)
                .ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 13, 11, 7, 5, 2 }),
                "Should have correct ranks regardless of suit");
        });
    }
}