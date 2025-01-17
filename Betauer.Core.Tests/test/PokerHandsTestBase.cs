using System;
using Betauer.Core.Deck;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

[TestFixture]
public class PokerHandsTestBase {
    public GameRun GameRun;
    public GameHandler Handler;
    public PokerGameConfig Config;
    public PokerHandsManager HandsManager;

    [SetUp]
    public void Setup() {
        Config = new PokerGameConfig();
        HandsManager = new PokerHandsManager();
        HandsManager.RegisterBasicPokerHands();
        GameRun = new GameRun(Config, HandsManager, 0);
        Handler = GameRun.CreateGameHandler(0);
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
        var config = HandsManager.GetPokerHandConfig(PokerHandType.FiveOfAKind);

        // Verificar que la mano se detecta cuando está habilitada
        var handsEnabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsEnabled, Is.Not.Empty, "Should identify FiveOfAKind when enabled");

        // Deshabilitar la mano
        HandsManager.RegisterHand(PokerHandType.FiveOfAKind,
            config.InitialScore, config.InitialMultiplier, config.ScorePerLevel,
            config.MultiplierPerLevel, 1, false);

        // Verificar que la mano no se detecta cuando está deshabilitada
        var handsDisabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h is FiveOfAKindHand)
            .ToList();
        Assert.That(handsDisabled, Is.Empty, "Should not identify disabled FiveOfAKind");
    }
}
