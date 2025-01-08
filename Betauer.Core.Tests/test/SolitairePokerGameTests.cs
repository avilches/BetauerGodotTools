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
        game = new SolitairePokerGame(config);
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
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        game.DrawCards();
        var possibleHands = game.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var playedHand = possibleHands[0];
        var result = game.PlayHand(playedHand);
        Assert.Multiple(() => {
            Assert.That(result.Success, Is.True);
            Assert.That(game.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(game.State.TotalScore, Is.EqualTo(game.Hands.CalculateScore(playedHand)));
            Assert.That(game.State.PlayedCards, Is.EquivalentTo(playedHand.Cards));
            Assert.That(game.State.DiscardedCards, Is.Empty);
            Assert.That(game.State.CurrentHand.Count, Is.EqualTo(0));
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterMaxHands_ShouldEndGame() {
        for (int i = 0; i < config.MaxHands - 1; i++) {
            game.DrawCards();
            var result = game.PlayHand(game.GetPossibleHands()[0]);
            Assert.That(result.Success, Is.True);
        }

        // Play final hand
        game.DrawCards();
        var finalResult = game.PlayHand(game.GetPossibleHands()[0]);
        Assert.Multiple(() => {
            Assert.That(finalResult.Success, Is.True);
            Assert.That(game.IsGameOver(), Is.True);
            Assert.That(game.State.CurrentHand, Is.Empty);
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(config.MaxHands));
        });

        Assert.That(game.GetPossibleHands(), Is.Empty);

        // Try to play another hand
        var extraResult = game.PlayHand(new PairHand(game.State.CurrentHand));
        Assert.Multiple(() => {
            Assert.That(extraResult.Success, Is.False);
            Assert.That(extraResult.Message, Is.EqualTo("Game is already over"));

            Assert.Throws<InvalidOperationException>(() => game.DrawCards());
        });
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        game.DrawCards();
        var initialState = game.State;
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();

        var result = game.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.Success, Is.True);
            Assert.That(game.State.Discards, Is.EqualTo(1));
            Assert.That(game.State.CurrentHand.Count, Is.EqualTo(0));
            Assert.That(game.State.CurrentHand.Intersect(cardsToDiscard), Is.Empty);
            Assert.That(game.State.DiscardedCards, Is.EquivalentTo(cardsToDiscard));
            Assert.That(game.State.PlayedCards, Is.Empty);
            Assert.That(game.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldFail() {
        game.DrawCards();
        var cardsToDiscard = game.State.CurrentHand.Take(config.MaxDiscardCards + 1).ToList();
        var result = game.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Must discard between 1 and"));
            Assert.That(game.State.Discards, Is.EqualTo(0));
        });
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldFail() {
        // Use all discards
        for (int i = 0; i < config.MaxDiscards; i++) {
            game.DrawCards();
            var oneCard = game.State.CurrentHand.Take(1).ToList();
            var result = game.Discard(oneCard);
            Assert.That(result.Success, Is.True);
        }

        // Try one more discard
        game.DrawCards();
        var moreCard = game.State.CurrentHand.Take(1).ToList();
        var finalResult = game.Discard(moreCard);
        Assert.Multiple(() => {
            Assert.That(finalResult.Success, Is.False);
            Assert.That(finalResult.Message, Is.EqualTo("No discards remaining or game is over"));
        });
    }

    [Test]
    public void CompleteGame_ShouldAccumulateScore() {
        var totalScore = 0;
        List<int> handScores = new();

        // Play all hands
        for (int i = 0; i < config.MaxHands; i++) {
            game.DrawCards();
            var hand = game.GetPossibleHands()[0]; // Always play best hand
            handScores.Add(game.Hands.CalculateScore(hand));
            totalScore += game.Hands.CalculateScore(hand);

            var result = game.PlayHand(hand);
            Assert.That(result.Success, Is.True);
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
}