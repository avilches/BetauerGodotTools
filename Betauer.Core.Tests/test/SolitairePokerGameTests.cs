using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class SolitairePokerGameTests {
    private SolitairePokerGame game;
    private PokerGameConfig config;

    [SetUp]
    public void Setup() {
        config = new PokerGameConfig();
        game = new SolitairePokerGame(new Random(1), config);
        game.Hands.RegisterBasicPokerHands();
    }

    [Test]
    public void NewGame_ShouldStartWithCorrectInitialState() {
        var state = game.State;
        Assert.Multiple(() => {
            Assert.That(state.HandsPlayed, Is.EqualTo(0));
            Assert.That(state.TotalScore, Is.EqualTo(0));
            Assert.That(state.Discards, Is.EqualTo(0));
            Assert.That(state.CurrentHand.Count, Is.EqualTo(0));
            Assert.That(state.DiscardedCards.Count, Is.EqualTo(0));
            Assert.That(state.PlayedCards.Count, Is.EqualTo(0));
            Assert.That(state.AvailableCards.Count, Is.EqualTo(52));
            Assert.That(state.History.GetHistory(), Is.Empty);
        });
    }

    [Test]
    public void NewGame_DrawCardsState() {
        game.DrawCards();
        var state = game.State;
        Assert.Multiple(() => {
            Assert.That(state.HandsPlayed, Is.EqualTo(0));
            Assert.That(state.TotalScore, Is.EqualTo(0));
            Assert.That(state.Discards, Is.EqualTo(0));
            Assert.That(state.CurrentHand.Count, Is.EqualTo(config.HandSize));
            Assert.That(state.DiscardedCards.Count, Is.EqualTo(0));
            Assert.That(state.PlayedCards.Count, Is.EqualTo(0));
            Assert.That(state.AvailableCards.Count, Is.EqualTo(52 - config.HandSize));
            Assert.That(state.History.GetHistory(), Is.Empty);
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldThrowGameException() {
        game.DrawCards();
        var cardsToDiscard = game.State.CurrentHand.Take(config.MaxDiscardCards + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => game.Discard(cardsToDiscard));
        Assert.That(exception.Message, Does.Contain("Must discard between 1 and"));
        Assert.That(game.State.Discards, Is.EqualTo(0));
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldThrowGameException() {
        // Use all discards
        for (int i = 0; i < config.MaxDiscards; i++) {
            game.DrawCards();
            var oneCard = game.State.CurrentHand.Take(1).ToList();
            _ = game.Discard(oneCard);
        }

        // Try one more discard
        game.DrawCards();
        var moreCard = game.State.CurrentHand.Take(1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => game.Discard(moreCard));
        Assert.That(exception.Message, Is.EqualTo("No discards remaining or game is over"));
    }

    [Test]
    public void CompleteGame_ShouldAccumulateScore() {
        var totalScore = 0;
        List<int> handScores = new();

        // Play all hands
        for (int i = 0; i < config.MaxHands; i++) {
            game.DrawCards();
            var hand = game.GetPossibleHands()[0]; // Always play best hand
            var result = game.PlayHand(hand);
            handScores.Add(result.Score);
            totalScore += result.Score;
        }

        var finalState = game.State;
        Assert.Multiple(() => {
            Assert.That(finalState.TotalScore, Is.EqualTo(totalScore));
            Assert.That(game.IsGameOver(), Is.True);

            // Verify history matches our played hands
            var history = finalState.History.GetHistory().Where(a => a.Type == GameHistory.GameActionType.Play).ToList();
            Assert.That(history.Count, Is.EqualTo(config.MaxHands));
            for (int i = 0; i < handScores.Count; i++) {
                Assert.That(history[i].Score, Is.EqualTo(handScores[i]));
            }
        });
    }

    private SolitairePokerGame CreateGameWithInitialHand<T>(int maxAttempts = 1000) where T : PokerHand {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new SolitairePokerGame(new Random(seed), config);
            testGame.Hands.RegisterBasicPokerHands();
            testGame.DrawCards();
            var possibleHands = testGame.GetPossibleHands().OfType<T>().ToList();
            if (possibleHands.Count > 0) {
                return testGame;
            }
        }
        throw new InvalidOperationException($"Could not find a game with a {typeof(T).Name} in the initial hand after {maxAttempts} attempts");
    }

    private SolitairePokerGame CreateGameWithInitialHandSize(int cardCount, int maxAttempts = 1000) {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new SolitairePokerGame(new Random(seed), config);
            testGame.Hands.RegisterBasicPokerHands();
            testGame.DrawCards();
            var possibleHands = testGame.GetPossibleHands()
                .Where(h => h.Cards.Count == cardCount)
                .ToList();
            if (possibleHands.Count > 0) {
                return testGame;
            }
        }
        throw new InvalidOperationException($"Could not find a game with a {cardCount}-card hand in the initial hand after {maxAttempts} attempts");
    }

    [Test]
    public void DrawCards_AfterPlayingPair_ShouldOnlyDrawUsedCards() {
        // Get a game that has a pair in the initial hand
        game = CreateGameWithInitialHand<PairHand>();
        var initialHand = new List<Card>(game.State.CurrentHand);

        // Find the pair
        var pairHand = game.GetPossibleHands()
            .OfType<PairHand>()
            .First(); // We know it exists because CreateGameWithInitialHand succeeded

        var pairCards = pairHand.Cards.ToList();
        var unusedCards = initialHand.Except(pairCards).ToList();

        // Play the pair
        game.PlayHand(pairHand);

        // Draw new cards
        game.DrawCards();
        var newHand = game.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(game.Config.HandSize));
            // Verify we kept all unused cards
            Assert.That(newHand.Intersect(unusedCards).Count(), Is.EqualTo(unusedCards.Count));
            // Verify we drew exactly the number of used cards
            Assert.That(newHand.Except(unusedCards).Count(), Is.EqualTo(pairCards.Count));
            // Verify we didn't get back any of the played cards
            Assert.That(newHand.Intersect(pairCards), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_AfterPlayingFullHouse_ShouldVerifyPartialHandUsage() {
        // Get a game that has a 5-card hand in the initial hand
        game = CreateGameWithInitialHandSize(5);
        var initialHand = new List<Card>(game.State.CurrentHand);

        var fiveCardHand = game.GetPossibleHands()
            .First(h => h.Cards.Count == 5);

        var usedCards = fiveCardHand.Cards.ToList();
        var unusedCards = initialHand.Except(usedCards).ToList();

        // Play the hand
        game.PlayHand(fiveCardHand);

        // Draw new cards
        game.DrawCards();
        var newHand = game.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(game.Config.HandSize));
            // Verify we kept all unused cards
            Assert.That(newHand.Intersect(unusedCards).Count(), Is.EqualTo(unusedCards.Count));
            // Verify we drew exactly the number of used cards
            Assert.That(newHand.Except(unusedCards).Count(), Is.EqualTo(usedCards.Count));
            // Verify we didn't get back any of the played cards
            Assert.That(newHand.Intersect(usedCards), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_AfterDiscard_ShouldOnlyDrawDiscardedCards() {
        game.DrawCards();
        var initialHand = new List<Card>(game.State.CurrentHand);
        Assert.That(initialHand.Count, Is.EqualTo(game.Config.HandSize));

        // Discard 2 cards
        var cardsToDiscard = initialHand.Take(2).ToList();
        var cardsToKeep = initialHand.Skip(2).ToList();
        game.Discard(cardsToDiscard);

        // Draw new cards
        game.DrawCards();
        var newHand = game.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(game.Config.HandSize));
            // Verify we kept the non-discarded cards
            Assert.That(newHand.Intersect(cardsToKeep).Count(), Is.EqualTo(cardsToKeep.Count));
            // Verify we drew exactly the number of discarded cards
            Assert.That(newHand.Except(cardsToKeep).Count(), Is.EqualTo(cardsToDiscard.Count));
            // Verify we didn't get back any of the discarded cards
            Assert.That(newHand.Intersect(cardsToDiscard), Is.Empty);
        });
    }

    [Test]
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        game.DrawCards();
        var possibleHands = game.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var playedHand = possibleHands[0];
        var result = game.PlayHand(playedHand);
        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(game.Hands.CalculateScore(playedHand)));
            Assert.That(game.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(game.State.TotalScore, Is.EqualTo(result.Score));
            // Las cartas jugadas deben contener las cartas de la mano jugada (no ser igual)
            Assert.That(game.State.PlayedCards.Intersect(playedHand.Cards), Is.EquivalentTo(playedHand.Cards));
            Assert.That(game.State.DiscardedCards, Is.Empty);
            Assert.That(game.State.CurrentHand.Count, Is.EqualTo(game.Config.HandSize - playedHand.Cards.Count));
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterMaxHands_ShouldThrowGameException() {
        for (int i = 0; i < config.MaxHands - 1; i++) {
            game.DrawCards();
            _ = game.PlayHand(game.GetPossibleHands()[0]);
        }

        // Play final hand
        game.DrawCards();
        _ = game.PlayHand(game.GetPossibleHands()[0]);
        Assert.Multiple(() => {
            Assert.That(game.IsGameOver(), Is.True);
            // CurrentHand debe tener las cartas que quedaron sin usar de la última mano
            Assert.That(game.State.CurrentHand.Count, Is.LessThan(game.Config.HandSize));
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(config.MaxHands));
        });

        Assert.That(game.GetPossibleHands(), Is.Not.Empty); // Puede haber manos posibles con las cartas que quedaron

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() => game.PlayHand(new PairHand(game.State.CurrentHand)));
        Assert.That(exception.Message, Is.EqualTo("Game is already over"));
        Assert.Throws<SolitairePokerGameException>(() => game.DrawCards());
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        game.DrawCards();
        var initialState = game.State;
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();

        var result = game.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.DiscardedCards, Is.EquivalentTo(cardsToDiscard));
            Assert.That(game.State.Discards, Is.EqualTo(1));
            Assert.That(game.State.CurrentHand.Count, Is.EqualTo(game.Config.HandSize - cardsToDiscard.Count));
            Assert.That(game.State.CurrentHand.Intersect(cardsToDiscard), Is.Empty);
            // Las cartas descartadas deben contener las cartas que acabamos de descartar (no ser igual)
            Assert.That(game.State.DiscardedCards.Intersect(cardsToDiscard), Is.EquivalentTo(cardsToDiscard));
            Assert.That(game.State.PlayedCards, Is.Empty);
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }
}