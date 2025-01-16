using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class GameHandlerTests {
    private GameHandler _gameHandler;
    private PokerGameConfig config;

    [SetUp]
    public void Setup() {
        config = new PokerGameConfig();
        _gameHandler = new GameHandler(1, config);
        _gameHandler.PokerHandsManager.RegisterBasicPokerHands();
    }

    [Test]
    public void TestLevelProgression() {
        var state = _gameHandler.State;

        // Test that Level affects scoring for a specific hand type
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        var scoreAtLevel0 = _gameHandler.CalculateScore(hand);

        state.SetPokerHandLevel(hand, 1);
        var scoreAtLevel1 = _gameHandler.CalculateScore(hand);
        Assert.That(scoreAtLevel1, Is.GreaterThan(scoreAtLevel0));

        state.SetPokerHandLevel(hand, 2);
        var scoreAtLevel2 = _gameHandler.CalculateScore(hand);
        Assert.That(scoreAtLevel2, Is.GreaterThan(scoreAtLevel1));
    }

    [Test]
    public void TestScoringAtDifferentLevels() {
        var pairHand = new PairHand(_gameHandler.PokerHandsManager, new List<Card> {
            new(12, 'H'), // Queen of Hearts
            new(12, 'S'), // Queen of Spades
        });

        // Test at Level 0
        var scoreLevel0 = _gameHandler.CalculateScore(pairHand);
        var expectedLevel0 = (10 + (12 + 12)) * 2; // (initialScore + ranks) * initialMultiplier
        Assert.That(scoreLevel0, Is.EqualTo(expectedLevel0));

        // Test at Level 1
        _gameHandler.State.SetPokerHandLevel(pairHand, 1);
        var scoreLevel1 = _gameHandler.CalculateScore(pairHand);
        var expectedLevel1 = (25 + (12 + 12)) * 3; // ((10 + 15) + ranks) * (2 + 1)
        Assert.That(scoreLevel1, Is.EqualTo(expectedLevel1));

        // Test at Level 2
        _gameHandler.State.SetPokerHandLevel(pairHand, 2);
        var scoreLevel2 = _gameHandler.CalculateScore(pairHand);
        var expectedLevel2 = (40 + (12 + 12)) * 4; // ((10 + 30) + ranks) * (2 + 2)
        Assert.That(scoreLevel2, Is.EqualTo(expectedLevel2));
    }

    [Test]
    public void GameProgression_ShouldHandleMultipleLevels() {
        var levels = new[] { 0, 1, 2 };

        foreach (var level in levels) {
            _gameHandler = new GameHandler(1, config);
            _gameHandler.PokerHandsManager.RegisterBasicPokerHands();
            _gameHandler.State.TotalScore = 1000;

            while (!_gameHandler.IsWon() && !_gameHandler.IsGameOver()) {
                _gameHandler.DrawCards();
                var bestHand = _gameHandler.GetPossibleHands()[0];

                // Establecer el nivel específico para este tipo de mano
                _gameHandler.State.SetPokerHandLevel(bestHand, level);

                var result = _gameHandler.PlayHand(bestHand.Cards);

                // Verify that score increases with level
                Assert.That(result.Score, Is.GreaterThan(0));
                if (level > 0) {
                    // Recreate the same hand but with level 0
                    _gameHandler.State.SetPokerHandLevel(bestHand, 0);
                    var baseScore = _gameHandler.CalculateScore(bestHand);
                    _gameHandler.State.SetPokerHandLevel(bestHand, level);
                    var levelScore = _gameHandler.CalculateScore(bestHand);
                    Assert.That(levelScore, Is.GreaterThan(baseScore));
                }
            }
        }
    }

    [Test]
    public void CompleteGame_ShouldAccumulateScore() {
        var totalScore = 0;
        List<int> handScores = [];

        // Set level 1 for the game
        _gameHandler.State.Level = 1;

        // Play all hands
        for (int i = 0; i < config.MaxHands; i++) {
            _gameHandler.DrawCards();
            var hand = _gameHandler.GetPossibleHands()[0]; // Always play best hand
            var result = _gameHandler.PlayHand(hand.Cards);
            handScores.Add(result.Score);
            totalScore += result.Score;
        }

        var finalState = _gameHandler.State;
        Assert.Multiple(() => {
            Assert.That(finalState.Score, Is.EqualTo(totalScore));
            Assert.That(_gameHandler.IsGameOver(), Is.True);

            // Verify history matches our played hands
            var history = finalState.History.GetHistory().Where(a => a.Type == PlayHistory.PlayedActionType.Play).ToList();
            Assert.That(history.Count, Is.EqualTo(config.MaxHands));
            for (int i = 0; i < handScores.Count; i++) {
                Assert.That(history[i].HandScore, Is.EqualTo(handScores[i]));
            }
        });
    }

    [Test]
    public void DrawCards_ShouldDrawCorrectNumber() {
        _gameHandler.DrawCards();
        Assert.That(_gameHandler.State.CurrentHand.Count, Is.EqualTo(config.HandSize));
    }

    [Test]
    public void Discard_ShouldDecrementRemainingDiscards() {
        _gameHandler.DrawCards();
        var initialDiscards = _gameHandler.State.Discards;
        var cardToDiscard = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDiscard]);
        Assert.That(_gameHandler.State.Discards, Is.EqualTo(initialDiscards + 1));
    }

    [Test]
    public void PlayHand_WithInvalidCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();
        var invalidCards = _gameHandler.State.AvailableCards.Take(1).ToList();
        Assert.Throws<SolitairePokerGameException>(() => _gameHandler.PlayHand(invalidCards));
    }

    [Test]
    public void IsWon_ShouldDependOnTotalScoreAndLevel() {
        // Test at different levels
        for (int level = 0; level <= 2; level++) {
            _gameHandler.State.Level = level;
            _gameHandler.State.TotalScore = 100;

            // Should not be won initially
            Assert.That(_gameHandler.IsWon(), Is.False);

            // Set score to meet total score
            _gameHandler.State.Score = 100;
            Assert.That(_gameHandler.IsWon(), Is.True);

            // Reset for next iteration
            _gameHandler = new GameHandler(1, config);
            _gameHandler.PokerHandsManager.RegisterBasicPokerHands();
        }
    }

    [Test]
    public void NewGame_ShouldStartWithCorrectInitialState() {
        var state = _gameHandler.State;
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
        _gameHandler.DrawCards();
        var state = _gameHandler.State;
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

    private GameHandler CreateGameWithInitialHand<T>(int maxAttempts = 1000) where T : PokerHand {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new GameHandler(seed, config);
            testGame.PokerHandsManager.RegisterBasicPokerHands();
            testGame.DrawCards();
            var possibleHands = testGame.GetPossibleHands().OfType<T>().ToList();
            if (possibleHands.Count > 0) {
                return testGame;
            }
        }
        throw new InvalidOperationException($"Could not find a game with a {typeof(T).Name} in the initial hand after {maxAttempts} attempts");
    }

    private GameHandler CreateGameWithInitialHandSize(int cardCount, int maxAttempts = 1000) {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new GameHandler(seed, config);
            testGame.PokerHandsManager.RegisterBasicPokerHands();
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
        _gameHandler = CreateGameWithInitialHand<PairHand>();
        var initialHand = new List<Card>(_gameHandler.State.CurrentHand);

        // Find the pair
        var pairHand = _gameHandler.GetPossibleHands()
            .OfType<PairHand>()
            .First(); // We know it exists because CreateGameWithInitialHand succeeded

        var pairCards = pairHand.Cards.ToList();
        var unusedCards = initialHand.Except(pairCards).ToList();

        // Play the pair
        _gameHandler.PlayHand(pairHand.Cards);

        // Draw new cards
        _gameHandler.DrawCards();
        var newHand = _gameHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameHandler.Config.HandSize));
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
        _gameHandler = CreateGameWithInitialHandSize(5);
        var initialHand = new List<Card>(_gameHandler.State.CurrentHand);

        var fiveCardHand = _gameHandler.GetPossibleHands()
            .First(h => h.Cards.Count == 5);

        var usedCards = fiveCardHand.Cards.ToList();
        var unusedCards = initialHand.Except(usedCards).ToList();

        // Play the hand
        _gameHandler.PlayHand(fiveCardHand.Cards);

        // Draw new cards
        _gameHandler.DrawCards();
        var newHand = _gameHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameHandler.Config.HandSize));
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
        _gameHandler.DrawCards();
        var initialHand = new List<Card>(_gameHandler.State.CurrentHand);
        Assert.That(initialHand.Count, Is.EqualTo(_gameHandler.Config.HandSize));

        // Discard 2 cards
        var cardsToDiscard = initialHand.Take(2).ToList();
        var cardsToKeep = initialHand.Skip(2).ToList();
        _gameHandler.Discard(cardsToDiscard);

        // Draw new cards
        _gameHandler.DrawCards();
        var newHand = _gameHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameHandler.Config.HandSize));
            // Verify we kept the non-discarded cards
            Assert.That(newHand.Intersect(cardsToKeep).Count(), Is.EqualTo(cardsToKeep.Count));
            // Verify we drew exactly the number of discarded cards
            Assert.That(newHand.Except(cardsToKeep).Count(), Is.EqualTo(cardsToDiscard.Count));
            // Verify we didn't get back any of the discarded cards
            Assert.That(newHand.Intersect(cardsToDiscard), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_WhenGameOver_ShouldThrowGameException() {
        for (int i = 0; i < config.MaxHands; i++) {
            _gameHandler.DrawCards();
            var hand = _gameHandler.GetPossibleHands()[0];
            _gameHandler.PlayHand(hand.Cards);
        }

        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards errors: game is over"));
    }

    [Test]
    public void DrawCards_WhenHandAlreadyFull_ShouldThrowGameException() {
        _gameHandler.DrawCards(); // Fill the hand

        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards error: hand already full"));
    }

    [Test]
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        _gameHandler.DrawCards();
        var possibleHands = _gameHandler.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var playedHand = possibleHands[0];
        var result = _gameHandler.PlayHand(playedHand.Cards);
        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(_gameHandler.CalculateScore(playedHand)));
            Assert.That(_gameHandler.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(_gameHandler.State.Score, Is.EqualTo(result.Score));
            // Las cartas jugadas deben contener las cartas de la mano jugada (no ser igual)
            Assert.That(_gameHandler.State.PlayedCards.Intersect(playedHand.Cards), Is.EquivalentTo(playedHand.Cards));
            Assert.That(_gameHandler.State.DiscardedCards, Is.Empty);
            Assert.That(_gameHandler.State.CurrentHand.Count, Is.EqualTo(_gameHandler.Config.HandSize - playedHand.Cards.Count));
            Assert.That(_gameHandler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterMaxHands_ShouldThrowGameException() {
        for (int i = 0; i < config.MaxHands - 1; i++) {
            _gameHandler.DrawCards();
            _ = _gameHandler.PlayHand(_gameHandler.GetPossibleHands()[0].Cards);
        }

        // Play final hand
        _gameHandler.DrawCards();
        _ = _gameHandler.PlayHand(_gameHandler.GetPossibleHands()[0].Cards);
        Assert.Multiple(() => {
            Assert.That(_gameHandler.IsGameOver(), Is.True);
            // CurrentHand debe tener las cartas que quedaron sin usar de la última mano
            Assert.That(_gameHandler.State.CurrentHand.Count, Is.LessThan(_gameHandler.Config.HandSize));
            Assert.That(_gameHandler.State.History.GetHistory().Count, Is.EqualTo(config.MaxHands));
        });

        Assert.That(_gameHandler.GetPossibleHands(), Is.Not.Empty); // Puede haber manos posibles con las cartas que quedaron

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.PlayHand(_gameHandler.State.CurrentHand));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game is over"));
        Assert.Throws<SolitairePokerGameException>(() => _gameHandler.DrawCards());
    }

    [Test]
    public void PlayHand_WhenGameWon_ShouldThrowGameException() {
        _gameHandler.State.TotalScore = 1; // Set a low score to make it winnable
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards); // This should make us win the game

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.PlayHand([]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game won"));
    }

    [Test]
    public void PlayHand_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.PlayHand([]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: cannot play hands yet, draw cards first."));
    }

    [Test]
    public void PlayHand_ShouldUpdateGameHistoryActionScores() {
        // Establecer un objetivo total
        _gameHandler.State.TotalScore = 300;
        _gameHandler.DrawCards();

        // Jugar dos manos para verificar la acumulación del score
        var firstHand = _gameHandler.GetPossibleHands()[0];
        var firstResult = _gameHandler.PlayHand(firstHand.Cards);
        _gameHandler.DrawCards();

        var secondHand = _gameHandler.GetPossibleHands()[0];
        var secondResult = _gameHandler.PlayHand(secondHand.Cards);

        // Obtener las acciones del historial
        var history = _gameHandler.State.History.GetHistory()
            .Where(a => a.Type == PlayHistory.PlayedActionType.Play)
            .ToList();

        Assert.Multiple(() => {
            // Verificar primera acción
            Assert.That(history[0].HandScore, Is.EqualTo(firstResult.Score));
            Assert.That(history[0].GameScore, Is.EqualTo(firstResult.Score));
            Assert.That(history[0].TotalScore, Is.EqualTo(300));

            // Verificar segunda acción
            Assert.That(history[1].HandScore, Is.EqualTo(secondResult.Score));
            Assert.That(history[1].GameScore, Is.EqualTo(firstResult.Score + secondResult.Score));
            Assert.That(history[1].TotalScore, Is.EqualTo(300));
        });
    }

    [Test]
    public void PlayHand_ShouldMoveCardsToPlayedPile() {
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];

        _gameHandler.PlayHand(hand.Cards);

        Assert.Multiple(() => {
            // Las cartas jugadas no deberían estar en la mano actual
            Assert.That(_gameHandler.State.CurrentHand.Intersect(hand.Cards), Is.Empty);
            // Las cartas jugadas deberían estar en la pila de jugadas
            Assert.That(_gameHandler.State.PlayedCards.Intersect(hand.Cards), Is.EquivalentTo(hand.Cards));
            // Las cartas jugadas no deberían estar disponibles
            Assert.That(_gameHandler.State.AvailableCards.Intersect(hand.Cards), Is.Empty);
        });
    }

    [Test]
    public void PlayHand_WithEmptyHand_ShouldReturnZeroScore() {
        _gameHandler.DrawCards();

        var result = _gameHandler.PlayHand([]);

        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(0));
            Assert.That(result.Hand, Is.Null);
        });
    }

    [Test]
    public void PlayHand_WithDiscardedCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();

        // Primero descartamos algunas cartas
        var cardsToDiscard = _gameHandler.State.CurrentHand.Take(2).ToList();
        _gameHandler.Discard(cardsToDiscard);

        // Robamos nuevas cartas para completar la mano
        _gameHandler.DrawCards();

        // Intentamos jugar las cartas que fueron descartadas anteriormente
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.PlayHand(cardsToDiscard));
        Assert.That(exception.Message, Does.Contain("PlayHand error: hand to play contains cards not in current hand"));
    }

    [Test]
    public void PlayHand_ShouldUpdateHistory() {
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        var result = _gameHandler.PlayHand(hand.Cards);

        var lastHistoryAction = _gameHandler.State.History.GetHistory().Last();
        Assert.Multiple(() => {
            Assert.That(lastHistoryAction.Type, Is.EqualTo(PlayHistory.PlayedActionType.Play));
            Assert.That(lastHistoryAction.HandScore, Is.EqualTo(result.Score));
            Assert.That(lastHistoryAction.GameScore, Is.EqualTo(result.Score));
            Assert.That(lastHistoryAction.Cards, Is.EquivalentTo(hand.Cards));
        });
    }

    [Test]
    public void GameState_ShouldTrackScoreProgress() {
        // Establecer un objetivo total
        _gameHandler.State.TotalScore = 300000;
        _gameHandler.DrawCards();

        // Jugar tres manos y verificar la acumulación del score
        var firstHand = _gameHandler.GetPossibleHands()[0];
        var firstResult = _gameHandler.PlayHand(firstHand.Cards);
        _gameHandler.DrawCards();

        var secondHand = _gameHandler.GetPossibleHands()[0];
        var secondResult = _gameHandler.PlayHand(secondHand.Cards);
        _gameHandler.DrawCards();

        var thirdHand = _gameHandler.GetPossibleHands()[0];
        var thirdResult = _gameHandler.PlayHand(thirdHand.Cards);

        Assert.Multiple(() => {
            // Verificar el score final
            var expectedTotalScore = firstResult.Score + secondResult.Score + thirdResult.Score;
            Assert.That(_gameHandler.State.Score, Is.EqualTo(expectedTotalScore));
            Assert.That(_gameHandler.State.TotalScore, Is.EqualTo(300000));
        });
    }

    [Test]
    public void IsWon_ShouldDependOnTotalScore() {
        // Caso 1: Score bajo, no debería ganar
        _gameHandler.State.TotalScore = 1000;
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);
        Assert.That(_gameHandler.IsWon(), Is.False, "Game should not be won with low score");

        // Caso 2: Score suficiente, debería ganar
        _gameHandler = new GameHandler(1, config);
        _gameHandler.PokerHandsManager.RegisterBasicPokerHands();
        _gameHandler.State.TotalScore = 10; // Un valor bajo que seguro se puede alcanzar con una mano
        _gameHandler.DrawCards();
        hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);
        Assert.That(_gameHandler.IsWon(), Is.True, "Game should be won when score >= totalScore");
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        _gameHandler.DrawCards();
        var initialState = _gameHandler.State;
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();

        var result = _gameHandler.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.DiscardedCards, Is.EquivalentTo(cardsToDiscard));
            Assert.That(_gameHandler.State.Discards, Is.EqualTo(1));
            Assert.That(_gameHandler.State.CurrentHand.Count, Is.EqualTo(_gameHandler.Config.HandSize - cardsToDiscard.Count));
            Assert.That(_gameHandler.State.CurrentHand.Intersect(cardsToDiscard), Is.Empty);
            // Las cartas descartadas deben contener las cartas que acabamos de descartar (no ser igual)
            Assert.That(_gameHandler.State.DiscardedCards.Intersect(cardsToDiscard), Is.EquivalentTo(cardsToDiscard));
            Assert.That(_gameHandler.State.PlayedCards, Is.Empty);
            Assert.That(_gameHandler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();
        var cardsToDiscard = _gameHandler.State.CurrentHand.Take(config.MaxDiscardCards + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.Discard(cardsToDiscard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: discard between 1 and {config.MaxDiscardCards} cards: {cardsToDiscard.Count}"));
        Assert.That(_gameHandler.State.Discards, Is.EqualTo(0));
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldThrowGameException() {
        // Use all discards
        for (int i = 0; i < config.MaxDiscards; i++) {
            _gameHandler.DrawCards();
            var oneCard = _gameHandler.State.CurrentHand.Take(1).ToList();
            _ = _gameHandler.Discard(oneCard);
        }

        // Try one more discard
        _gameHandler.DrawCards();
        var moreCard = _gameHandler.State.CurrentHand.Take(1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameHandler.Discard(moreCard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: no discards remaining {_gameHandler.State.Discards} of {config.MaxDiscards}"));
    }

    [Test]
    public void Discard_WhenGameWon_ShouldThrowGameException() {
        _gameHandler.State.TotalScore = 10; // Set a low score to make it winnable
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards); // This should make us win the game

        // Try to discard
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: game won"));
    }

    [Test]
    public void Discard_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: cannot discard hands yet, draw cards first."));
    }

    [Test]
    public void Discard_WithInvalidCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.Discard(_gameHandler.State.AvailableCards.Take(1).ToList()));
        Assert.That(exception.Message, Does.Contain("Discard error: contains cards not in current hand"));
    }

    [Test]
    public void DrawCards_WithSpecificCards_ShouldDrawCorrectly() {
        _gameHandler.DrawCards();

        // Jugar una mano para tener espacio para robar más cartas
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);

        // Seleccionar cartas específicas para robar
        var availableCards = _gameHandler.State.AvailableCards.Take(hand.Cards.Count).ToList();
        _gameHandler.DrawCards(availableCards);

        Assert.Multiple(() => {
            // Verificar que las cartas específicas están en la mano actual
            Assert.That(_gameHandler.State.CurrentHand.Intersect(availableCards), Is.EquivalentTo(availableCards));
            // Verificar que las cartas ya no están disponibles
            Assert.That(_gameHandler.State.AvailableCards.Intersect(availableCards), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_WithInvalidCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);

        // Primero jugamos una mano para tener espacio para robar
        var playedCard = hand.Cards[0]; // Guardamos una carta que ya ha sido jugada

        // Intentar robar la carta que ya fue jugada (ahora está en PlayedCards, no en AvailableCards)
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.DrawCards([playedCard]));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cards to draw not found in available pile"));
    }

    [Test]
    public void DrawCards_WithTooManyCards_ShouldThrowGameException() {
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);

        // Intentar robar más cartas de las necesarias
        var tooManyCards = _gameHandler.State.AvailableCards.Take(hand.Cards.Count + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.DrawCards(tooManyCards));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cannot draw more cards"));
    }

    [Test]
    public void Recover_FromPlayedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in played pile
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);
        var cardToRecover = hand.Cards[0];

        // Act
        _gameHandler.Recover([cardToRecover]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.PlayedCards, Does.Not.Contain(cardToRecover));
            Assert.That(_gameHandler.State.AvailableCards, Does.Contain(cardToRecover));
        });
    }

    [Test]
    public void Recover_FromDiscardedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in discarded pile
        _gameHandler.DrawCards();
        var cardToDiscard = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDiscard]);

        // Act
        _gameHandler.Recover([cardToDiscard]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(_gameHandler.State.AvailableCards, Does.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Recover_RandomCardsFromBothPiles_ShouldMoveCardsToAvailablePile() {
        // Prepare some cards in played pile
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);

        // Prepare some cards in discarded pile
        _gameHandler.DrawCards();
        var cardToDiscard = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDiscard]);

        var initialPlayedCount = _gameHandler.State.PlayedCards.Count;
        var initialDiscardedCount = _gameHandler.State.DiscardedCards.Count;
        var initialAvailableCount = _gameHandler.State.AvailableCards.Count;
        var cardsToRecover = 2;

        // Act
        _gameHandler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.AvailableCards.Count,
                Is.EqualTo(initialAvailableCount + cardsToRecover));
            Assert.That(_gameHandler.State.PlayedCards.Count + _gameHandler.State.DiscardedCards.Count,
                Is.EqualTo(initialPlayedCount + initialDiscardedCount - cardsToRecover));
        });
    }

    [Test]
    public void Recover_WithCardsFromBothPiles_ShouldMoveSpecificCardsToAvailablePile() {
        // Prepare a card in played pile
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);
        var playedCard = hand.Cards[0];

        // Prepare a card in discarded pile
        _gameHandler.DrawCards();
        var cardToDiscard = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDiscard]);

        var cardsToRecover = new List<Card> { playedCard, cardToDiscard };

        // Act
        _gameHandler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.PlayedCards, Does.Not.Contain(playedCard));
            Assert.That(_gameHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(_gameHandler.State.AvailableCards.Intersect(cardsToRecover),
                Is.EquivalentTo(cardsToRecover));
        });
    }

    [Test]
    public void Recover_FromDestroyedPile_ShouldMoveCardToAvailablePile() {
        _gameHandler.DrawCards();
        var cardToDestroy = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDestroy]);

        _gameHandler.Recover([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.DestroyedCards, Does.Not.Contain(cardToDestroy));
            Assert.That(_gameHandler.State.AvailableCards, Does.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromCurrentHand_ShouldMoveCardToDestroyedPile() {
        _gameHandler.DrawCards();
        var cardToDestroy = _gameHandler.State.CurrentHand.First();

        _gameHandler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(_gameHandler.State.CurrentHand, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromAvailablePile_ShouldMoveCardToDestroyedPile() {
        var cardToDestroy = _gameHandler.State.AvailableCards.First();

        _gameHandler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(_gameHandler.State.AvailableCards, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromDiscardedPile_ShouldMoveCardToDestroyedPile() {
        _gameHandler.DrawCards();
        var cardToDiscard = _gameHandler.State.CurrentHand.First();
        _gameHandler.Discard([cardToDiscard]);

        _gameHandler.Destroy([cardToDiscard]);

        Assert.Multiple(() => {
            Assert.That(_gameHandler.State.DestroyedCards, Does.Contain(cardToDiscard));
            Assert.That(_gameHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Destroy_FromPlayedPile_ShouldMoveCardToDestroyedPile() {
        _gameHandler.DrawCards();
        var hand = _gameHandler.GetPossibleHands()[0];
        _gameHandler.PlayHand(hand.Cards);
        var cardsToDestroy = hand.Cards;

        _gameHandler.Destroy(cardsToDestroy);

        Assert.Multiple(() => {
            // Verificar que todas las cartas están en la pila de destruidas
            Assert.That(_gameHandler.State.DestroyedCards, Is.EquivalentTo(cardsToDestroy));
            // Verificar que ninguna carta está en la pila de jugadas
            Assert.That(_gameHandler.State.PlayedCards.Intersect(cardsToDestroy), Is.Empty);
        });
    }

    [Test]
    public void Destroy_NonExistentCard_ShouldThrowGameException() {
        var nonExistentCard = new Card(25, 'S'); // 25 of Spades que no existe en ninguna pila

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.Destroy([nonExistentCard]));
        Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
    }

    [Test]
    public void Destroy_MultipleTimes_ShouldOnlyDestroyOnce() {
        _gameHandler.DrawCards();
        var cardToDestroy = _gameHandler.State.CurrentHand.First();

        _gameHandler.Destroy([cardToDestroy]);
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameHandler.Destroy([cardToDestroy]));

        Assert.Multiple(() => {
            Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
            Assert.That(_gameHandler.State.DestroyedCards.Count(c => c == cardToDestroy), Is.EqualTo(1));
        });
    }
}