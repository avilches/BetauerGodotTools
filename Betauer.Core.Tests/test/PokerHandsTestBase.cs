using System;
using Betauer.Core.Deck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Tests;

[TestFixture]
public class PokerHandsTestBase {
    protected PokerHandsManager HandsManager => Handler.PokerHandsManager;
    protected GameStateHandler Handler;

    [SetUp]
    public void Setup() {
        Handler = new GameStateHandler(0, new PokerGameConfig());
        HandsManager.RegisterBasicPokerHands();
    }

    protected List<Card> CreateCards(params string[] cards) {
        var parse = new PokerGameConfig().Parse;
        return cards.Select(parse).ToList();
    }
}

[TestFixture]
public class PokerHandsTest : PokerHandsTestBase {
    [Test]
    public void DisabledHand_ShouldNotBeIdentified() {
        var currentHand = CreateCards("AS", "AH", "AD", "AC", "AH");
        var config = HandsManager.GetPokerHandConfig(new FiveOfAKindHand(HandsManager, []));

        // Verificar que la mano se detecta cuando está habilitada
        var handsEnabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsEnabled, Is.Not.Empty, "Should identify FiveOfAKind when enabled");

        // Deshabilitar la mano
        HandsManager.RegisterHand(new FiveOfAKindHand(HandsManager, []),
            config.InitialScore, config.InitialMultiplier, config.ScorePerLevel,
            config.MultiplierPerLevel, false);

        // Verificar que la mano no se detecta cuando está deshabilitada
        var handsDisabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsDisabled, Is.Empty, "Should not identify disabled FiveOfAKind");
    }
}

[TestFixture]
public class FlushHandsTest : PokerHandsTestBase {
    [Test]
    public void Flush_WithMoreThanFiveCards_ShouldFindAllCombinations() {
        // 6 cartas de corazones, debería generar un color con las primeras 5
        var cards = CreateCards("AH", "KH", "QH", "JH", "TH", "9H", "2D");
        var hands = HandsManager.IdentifyAllHands(Handler, cards);
        var flushes = hands.Where(h => h is FlushHand).ToList();

        Assert.Multiple(() => {
            // Debería haber solo un color
            Assert.That(flushes.Count, Is.EqualTo(1), "Should have exactly 1 flush");
            Assert.That(flushes[0].Cards.Count, Is.EqualTo(5), "Flush should have exactly 5 cards");
            Assert.That(flushes[0].Cards.All(c => c.Suit == 'H'), "All cards should be hearts");

            // Verificar que tenemos las cartas más altas (A,K,Q,J,10)
            var ranks = flushes[0].Cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
            Assert.That(ranks, Is.EqualTo(new[] { 14, 13, 12, 11, 10 }),
                "Should have Ace, King, Queen, Jack, Ten of hearts");
        });
    }
}