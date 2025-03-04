using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Tests.Deck;

[TestFixture]
public class PokerHandsTestBase {
    public GameRun GameRun;
    public GameHandler Handler;
    public PokerGameConfig Config;
    public PokerHandsManager HandsManager;

    [SetUp]
    public void Setup() {
        Config = new PokerGameConfig();
        HandsManager = new PokerHandsManager(new Random(0), new PokerHandConfig());
        HandsManager.RegisterBasicPokerHands();
        GameRun = new GameRun(Config, HandsManager, 0);
        Handler = GameRun.CreateGameHandler(0, DeckBuilder.ClassicPokerDeck());
    }


    protected List<Card> CreateCards(params string[] cards) {
        return cards.Select(Card.Parse).ToList();
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
            .Where(h => h.HandType == PokerHandType.FiveOfAKind)
            .ToList();
        Assert.That(handsEnabled, Is.Not.Empty, "Should identify FiveOfAKind when enabled");

        // Deshabilitar la mano
        HandsManager.RegisterHand(PokerHandType.FiveOfAKind,
            config.InitialScore, config.InitialMultiplier, config.ScorePerLevel,
            config.MultiplierPerLevel, 1, false);

        // Verificar que la mano no se detecta cuando está deshabilitada
        var handsDisabled = HandsManager.IdentifyAllHands(Handler, currentHand)
            .Where(h => h.HandType == PokerHandType.FiveOfAKind)
            .ToList();
        Assert.That(handsDisabled, Is.Empty, "Should not identify disabled FiveOfAKind");
    }
}
