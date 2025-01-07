using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class SolitairePokerGameTests {
    private SolitairePokerGame game;
    private HandIdentifier handIdentifier;

    [SetUp]
    public void Setup() {
        game = new SolitairePokerGame();
        handIdentifier = new HandIdentifier();
    }

    [Test]
    public void NewGame_ShouldStartWithCorrectInitialState() {
        var state = game.GetState();
        Assert.Multiple(() => {
            Assert.That(state.HandsPlayed, Is.EqualTo(0));
            Assert.That(state.TotalScore, Is.EqualTo(0));
            Assert.That(state.RemainingDiscards, Is.EqualTo(GameState.MAX_DISCARDS));
            Assert.That(state.CurrentHand.Count, Is.EqualTo(GameState.HAND_SIZE));
            Assert.That(state.History.GetHistory(), Is.Empty);
        });
    }

    [Test]
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        var possibleHands = game.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");
        
        var result = game.PlayHand(possibleHands[0]);
        Assert.Multiple(() => {
            Assert.That(result.Success, Is.True);
            Assert.That(result.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(result.State.TotalScore, Is.EqualTo(possibleHands[0].Score));
            Assert.That(result.State.CurrentHand.Count, Is.EqualTo(GameState.HAND_SIZE));
            Assert.That(result.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterFourHands_ShouldEndGame() {
        for (int i = 0; i < GameState.MAX_HANDS - 1; i++) {
            var result = game.PlayHand(game.GetPossibleHands()[0]);
            Assert.That(result.Success, Is.True);
        }

        // Play final hand
        var finalResult = game.PlayHand(game.GetPossibleHands()[0]);
        Assert.Multiple(() => {
            Assert.That(finalResult.Success, Is.True);
            Assert.That(finalResult.State.IsGameOver(), Is.True);
            Assert.That(finalResult.State.CurrentHand, Is.Empty);
            Assert.That(finalResult.State.History.GetHistory().Count, Is.EqualTo(GameState.MAX_HANDS));
        });

        // Try to play another hand
        var extraResult = game.PlayHand(new PairHand(game.GetState().CurrentHand));
        Assert.Multiple(() => {
            Assert.That(extraResult.Success, Is.False);
            Assert.That(extraResult.Message, Is.EqualTo("Game is already over"));
        });
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        var initialState = game.GetState();
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();
        var remainingCards = initialState.CurrentHand.Skip(3).ToList();

        var result = game.Discard(cardsToDiscard);
        
        Assert.Multiple(() => {
            Assert.That(result.Success, Is.True);
            Assert.That(result.State.RemainingDiscards, Is.EqualTo(GameState.MAX_DISCARDS - 1));
            Assert.That(result.State.CurrentHand.Count, Is.EqualTo(GameState.HAND_SIZE));
            // Verify remaining cards are still in hand
            Assert.That(result.State.CurrentHand.Intersect(remainingCards).Count(), Is.EqualTo(remainingCards.Count));
            Assert.That(result.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldFail() {
        var cardsToDiscard = game.GetState().CurrentHand.Take(GameState.MAX_DISCARD_CARDS + 1).ToList();
        var result = game.Discard(cardsToDiscard);
        
        Assert.Multiple(() => {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Must discard between 1 and"));
            Assert.That(result.State.RemainingDiscards, Is.EqualTo(GameState.MAX_DISCARDS));
        });
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldFail() {
        
        
        // Use all discards
        for (int i = 0; i < GameState.MAX_DISCARDS; i++) {
            var oneCard = game.GetState().CurrentHand.Take(1).ToList();
            var result = game.Discard(oneCard);
            Assert.That(result.Success, Is.True);
        }

        // Try one more discard
        var moreCard = game.GetState().CurrentHand.Take(1).ToList();
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
        for (int i = 0; i < GameState.MAX_HANDS; i++) {
            var hand = game.GetPossibleHands()[0]; // Always play best hand
            handScores.Add(hand.Score);
            totalScore += hand.Score;
            
            var result = game.PlayHand(hand);
            Assert.That(result.Success, Is.True);
        }

        var finalState = game.GetState();
        Assert.Multiple(() => {
            Assert.That(finalState.TotalScore, Is.EqualTo(totalScore));
            Assert.That(finalState.IsGameOver(), Is.True);
            
            // Verify history matches our played hands
            var history = finalState.History.GetHistory().Where(a => a.Type == "PLAY").ToList();
            Assert.That(history.Count, Is.EqualTo(GameState.MAX_HANDS));
            for (int i = 0; i < handScores.Count; i++) {
                Assert.That(history[i].Score, Is.EqualTo(handScores[i]));
            }
        });
    }

    [Test]
    public void GameState_ShouldBeImmutable() {
        var initialState = game.GetState();
        var initialHand = new List<Card>(initialState.CurrentHand);

        // Play a hand
        var result = game.PlayHand(game.GetPossibleHands()[0]);
        
        Assert.Multiple(() => {
            // Initial state should not be modified
            Assert.That(initialState.HandsPlayed, Is.EqualTo(0));
            Assert.That(initialState.TotalScore, Is.EqualTo(0));
            Assert.That(initialState.CurrentHand, Is.EqualTo(initialHand));
            
            // New state should have updates
            Assert.That(result.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(result.State.TotalScore, Is.GreaterThan(0));
        });
    }
}