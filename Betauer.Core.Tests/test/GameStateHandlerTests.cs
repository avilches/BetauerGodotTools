using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class GameStateHandlerTests {
    private GameStateHandler _gameStateHandler;
    private PokerGameConfig config;

    [SetUp]
    public void Setup() {
        config = new PokerGameConfig();
        _gameStateHandler = new GameStateHandler(1, config);
        _gameStateHandler.PokerHandsManager.RegisterBasicPokerHands();
    }

    [Test]
    public void NewGame_ShouldStartWithCorrectInitialState() {
        var state = _gameStateHandler.State;
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
        _gameStateHandler.DrawCards();
        var state = _gameStateHandler.State;
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
    public void CompleteGame_ShouldAccumulateScore() {
        var totalScore = 0;
        List<int> handScores = new();

        // Play all hands
        for (int i = 0; i < config.MaxHands; i++) {
            _gameStateHandler.DrawCards();
            var hand = _gameStateHandler.GetPossibleHands()[0]; // Always play best hand
            var result = _gameStateHandler.PlayHand(hand.Cards);
            handScores.Add(result.Score);
            totalScore += result.Score;
        }

        var finalState = _gameStateHandler.State;
        Assert.Multiple(() => {
            Assert.That(finalState.Score, Is.EqualTo(totalScore));
            Assert.That(_gameStateHandler.IsGameOver(), Is.True);

            // Verify history matches our played hands
            var history = finalState.History.GetHistory().Where(a => a.Type == PlayHistory.PlayedActionType.Play).ToList();
            Assert.That(history.Count, Is.EqualTo(config.MaxHands));
            for (int i = 0; i < handScores.Count; i++) {
                Assert.That(history[i].HandScore, Is.EqualTo(handScores[i]));
            }
        });
    }

    private GameStateHandler CreateGameWithInitialHand<T>(int maxAttempts = 1000) where T : PokerHand {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new GameStateHandler(seed, config);
            testGame.PokerHandsManager.RegisterBasicPokerHands();
            testGame.DrawCards();
            var possibleHands = testGame.GetPossibleHands().OfType<T>().ToList();
            if (possibleHands.Count > 0) {
                return testGame;
            }
        }
        throw new InvalidOperationException($"Could not find a game with a {typeof(T).Name} in the initial hand after {maxAttempts} attempts");
    }

    private GameStateHandler CreateGameWithInitialHandSize(int cardCount, int maxAttempts = 1000) {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var testGame = new GameStateHandler(seed, config);
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
        _gameStateHandler = CreateGameWithInitialHand<PairHand>();
        var initialHand = new List<Card>(_gameStateHandler.State.CurrentHand);

        // Find the pair
        var pairHand = _gameStateHandler.GetPossibleHands()
            .OfType<PairHand>()
            .First(); // We know it exists because CreateGameWithInitialHand succeeded

        var pairCards = pairHand.Cards.ToList();
        var unusedCards = initialHand.Except(pairCards).ToList();

        // Play the pair
        _gameStateHandler.PlayHand(pairHand.Cards);

        // Draw new cards
        _gameStateHandler.DrawCards();
        var newHand = _gameStateHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize));
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
        _gameStateHandler = CreateGameWithInitialHandSize(5);
        var initialHand = new List<Card>(_gameStateHandler.State.CurrentHand);

        var fiveCardHand = _gameStateHandler.GetPossibleHands()
            .First(h => h.Cards.Count == 5);

        var usedCards = fiveCardHand.Cards.ToList();
        var unusedCards = initialHand.Except(usedCards).ToList();

        // Play the hand
        _gameStateHandler.PlayHand(fiveCardHand.Cards);

        // Draw new cards
        _gameStateHandler.DrawCards();
        var newHand = _gameStateHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize));
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
        _gameStateHandler.DrawCards();
        var initialHand = new List<Card>(_gameStateHandler.State.CurrentHand);
        Assert.That(initialHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize));

        // Discard 2 cards
        var cardsToDiscard = initialHand.Take(2).ToList();
        var cardsToKeep = initialHand.Skip(2).ToList();
        _gameStateHandler.Discard(cardsToDiscard);

        // Draw new cards
        _gameStateHandler.DrawCards();
        var newHand = _gameStateHandler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize));
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
            _gameStateHandler.DrawCards();
            var hand = _gameStateHandler.GetPossibleHands()[0];
            _gameStateHandler.PlayHand(hand.Cards);
        }

        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards errors: game is over"));
    }

    [Test]
    public void DrawCards_WhenHandAlreadyFull_ShouldThrowGameException() {
        _gameStateHandler.DrawCards(); // Fill the hand

        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards error: hand already full"));
    }

    [Test]
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        _gameStateHandler.DrawCards();
        var possibleHands = _gameStateHandler.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var playedHand = possibleHands[0];
        var result = _gameStateHandler.PlayHand(playedHand.Cards);
        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(_gameStateHandler.PokerHandsManager.CalculateScore(playedHand)));
            Assert.That(_gameStateHandler.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(_gameStateHandler.State.Score, Is.EqualTo(result.Score));
            // Las cartas jugadas deben contener las cartas de la mano jugada (no ser igual)
            Assert.That(_gameStateHandler.State.PlayedCards.Intersect(playedHand.Cards), Is.EquivalentTo(playedHand.Cards));
            Assert.That(_gameStateHandler.State.DiscardedCards, Is.Empty);
            Assert.That(_gameStateHandler.State.CurrentHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize - playedHand.Cards.Count));
            Assert.That(_gameStateHandler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterMaxHands_ShouldThrowGameException() {
        for (int i = 0; i < config.MaxHands - 1; i++) {
            _gameStateHandler.DrawCards();
            _ = _gameStateHandler.PlayHand(_gameStateHandler.GetPossibleHands()[0].Cards);
        }

        // Play final hand
        _gameStateHandler.DrawCards();
        _ = _gameStateHandler.PlayHand(_gameStateHandler.GetPossibleHands()[0].Cards);
        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.IsGameOver(), Is.True);
            // CurrentHand debe tener las cartas que quedaron sin usar de la última mano
            Assert.That(_gameStateHandler.State.CurrentHand.Count, Is.LessThan(_gameStateHandler.Config.HandSize));
            Assert.That(_gameStateHandler.State.History.GetHistory().Count, Is.EqualTo(config.MaxHands));
        });

        Assert.That(_gameStateHandler.GetPossibleHands(), Is.Not.Empty); // Puede haber manos posibles con las cartas que quedaron

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.PlayHand(_gameStateHandler.State.CurrentHand));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game is over"));
        Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.DrawCards());
    }

    [Test]
    public void PlayHand_WhenGameWon_ShouldThrowGameException() {
        _gameStateHandler.State.TotalScore = 1; // Set a low score to make it winnable
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards); // This should make us win the game

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.PlayHand([]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game won"));
    }

    [Test]
    public void PlayHand_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.PlayHand([]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: cannot play hands yet, draw cards first."));
    }

    [Test]
    public void PlayHand_WithInvalidCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.PlayHand(_gameStateHandler.State.AvailableCards.Take(1).ToList()));
        Assert.That(exception.Message, Does.Contain("PlayHand error: hand to play contains cards not in current hand"));
    }

    [Test]
    public void PlayHand_ShouldUpdateGameHistoryActionScores() {
        // Establecer un objetivo total
        _gameStateHandler.State.TotalScore = 300;
        _gameStateHandler.DrawCards();

        // Jugar dos manos para verificar la acumulación del score
        var firstHand = _gameStateHandler.GetPossibleHands()[0];
        var firstResult = _gameStateHandler.PlayHand(firstHand.Cards);
        _gameStateHandler.DrawCards();

        var secondHand = _gameStateHandler.GetPossibleHands()[0];
        var secondResult = _gameStateHandler.PlayHand(secondHand.Cards);

        // Obtener las acciones del historial
        var history = _gameStateHandler.State.History.GetHistory()
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
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];

        _gameStateHandler.PlayHand(hand.Cards);

        Assert.Multiple(() => {
            // Las cartas jugadas no deberían estar en la mano actual
            Assert.That(_gameStateHandler.State.CurrentHand.Intersect(hand.Cards), Is.Empty);
            // Las cartas jugadas deberían estar en la pila de jugadas
            Assert.That(_gameStateHandler.State.PlayedCards.Intersect(hand.Cards), Is.EquivalentTo(hand.Cards));
            // Las cartas jugadas no deberían estar disponibles
            Assert.That(_gameStateHandler.State.AvailableCards.Intersect(hand.Cards), Is.Empty);
        });
    }

    [Test]
    public void PlayHand_WithEmptyHand_ShouldReturnZeroScore() {
        _gameStateHandler.DrawCards();

        var result = _gameStateHandler.PlayHand([]);

        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(0));
            Assert.That(result.Hand, Is.Null);
        });
    }

    [Test]
    public void PlayHand_WithDiscardedCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();

        // Primero descartamos algunas cartas
        var cardsToDiscard = _gameStateHandler.State.CurrentHand.Take(2).ToList();
        _gameStateHandler.Discard(cardsToDiscard);

        // Robamos nuevas cartas para completar la mano
        _gameStateHandler.DrawCards();

        // Intentamos jugar las cartas que fueron descartadas anteriormente
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.PlayHand(cardsToDiscard));
        Assert.That(exception.Message, Does.Contain("PlayHand error: hand to play contains cards not in current hand"));
    }

    [Test]
    public void PlayHand_ShouldUpdateHistory() {
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        var result = _gameStateHandler.PlayHand(hand.Cards);

        var lastHistoryAction = _gameStateHandler.State.History.GetHistory().Last();
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
        _gameStateHandler.State.TotalScore = 300000;
        _gameStateHandler.DrawCards();

        // Jugar tres manos y verificar la acumulación del score
        var firstHand = _gameStateHandler.GetPossibleHands()[0];
        var firstResult = _gameStateHandler.PlayHand(firstHand.Cards);
        _gameStateHandler.DrawCards();

        var secondHand = _gameStateHandler.GetPossibleHands()[0];
        var secondResult = _gameStateHandler.PlayHand(secondHand.Cards);
        _gameStateHandler.DrawCards();

        var thirdHand = _gameStateHandler.GetPossibleHands()[0];
        var thirdResult = _gameStateHandler.PlayHand(thirdHand.Cards);

        Assert.Multiple(() => {
            // Verificar el score final
            var expectedTotalScore = firstResult.Score + secondResult.Score + thirdResult.Score;
            Assert.That(_gameStateHandler.State.Score, Is.EqualTo(expectedTotalScore));
            Assert.That(_gameStateHandler.State.TotalScore, Is.EqualTo(300000));
        });
    }

    [Test]
    public void IsWon_ShouldDependOnTotalScore() {
        // Caso 1: Score bajo, no debería ganar
        _gameStateHandler.State.TotalScore = 1000;
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);
        Assert.That(_gameStateHandler.IsWon(), Is.False, "Game should not be won with low score");

        // Caso 2: Score suficiente, debería ganar
        _gameStateHandler = new GameStateHandler(1, config);
        _gameStateHandler.PokerHandsManager.RegisterBasicPokerHands();
        _gameStateHandler.State.TotalScore = 10; // Un valor bajo que seguro se puede alcanzar con una mano
        _gameStateHandler.DrawCards();
        hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);
        Assert.That(_gameStateHandler.IsWon(), Is.True, "Game should be won when score >= totalScore");
    }

    [Test]
    public void GameProgression_ShouldHandleMultipleLevels() {
        // Simular progresión a través de múltiples niveles
        int[] levels = { 1, 2, 3 };

        foreach (var targetScore in levels) {
            _gameStateHandler.State.TotalScore = targetScore;

            while (!_gameStateHandler.IsWon() && !_gameStateHandler.IsGameOver()) {
                _gameStateHandler.DrawCards();
                var bestHand = _gameStateHandler.GetPossibleHands()[0];
                _gameStateHandler.PlayHand(bestHand.Cards);
            }

            Assert.Multiple(() => {
                Assert.That(_gameStateHandler.State.Score, Is.GreaterThanOrEqualTo(_gameStateHandler.State.TotalScore),
                    $"Score should reach or exceed target {targetScore}");

                // Verificar que el historial refleja correctamente la progresión
                var history = _gameStateHandler.State.History.GetHistory()
                    .Where(a => a.Type == PlayHistory.PlayedActionType.Play);
                foreach (var action in history) {
                    Assert.That(action.TotalScore, Is.EqualTo(targetScore),
                        "Each history action should have correct target score");
                    Assert.That(action.GameScore, Is.LessThanOrEqualTo(_gameStateHandler.State.Score),
                        "Accumulated score in history should not exceed final score");
                }
            });

            // Reiniciar para el siguiente nivel
            _gameStateHandler = new GameStateHandler(1, config);
            _gameStateHandler.PokerHandsManager.RegisterBasicPokerHands();
        }
    }

    [Test]
    public void CalculateScore_ShouldUseInitialScoreAndMultiplier() {
        _gameStateHandler.DrawCards();
        var possibleHands = _gameStateHandler.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var hand = possibleHands[0];
        var config = _gameStateHandler.PokerHandsManager.GetPokerHandConfig(hand);

        var result = _gameStateHandler.PlayHand(hand.Cards);

        var expectedScore = (config.InitialScore + hand.Cards.Sum(c => c.Rank)) * config.Multiplier;
        Assert.That(result.Score, Is.EqualTo(expectedScore),
            $"Score calculation should be (InitialScore({config.InitialScore}) + Sum of Ranks({hand.Cards.Sum(c => c.Rank)})) * Multiplier({config.Multiplier})");
    }

    [Test]
    public void RegisterHand_ShouldStoreInitialScoreCorrectly() {
        // Clear existing hands and register a test hand
        _gameStateHandler.PokerHandsManager.ClearHands();
        var testHand = new HighCardHand(_gameStateHandler.PokerHandsManager, []);
        _gameStateHandler.PokerHandsManager.RegisterHand(testHand, 42, 2);

        var config = _gameStateHandler.PokerHandsManager.GetPokerHandConfig(testHand);
        Assert.Multiple(() => {
            Assert.That(config.InitialScore, Is.EqualTo(42));
            Assert.That(config.Multiplier, Is.EqualTo(2));
        });

        // Crear una mano real con cartas específicas
        var card1 = new Card(10, 'S'); // Rank 10
        var card2 = new Card(12, 'S'); // Rank 12
        var hand = new HighCardHand(_gameStateHandler.PokerHandsManager, [card1, card2]);

        var score = _gameStateHandler.PokerHandsManager.CalculateScore(hand);
        var expectedScore = (42 + card1.Rank + card2.Rank) * 2; // (42 + 10 + 12) * 2 = 128
        Assert.That(score, Is.EqualTo(expectedScore),
            $"Score calculation should be (InitialScore({config.InitialScore}) + Sum of Ranks({card1.Rank + card2.Rank})) * Multiplier({config.Multiplier}) = {expectedScore}");
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        _gameStateHandler.DrawCards();
        var initialState = _gameStateHandler.State;
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();

        var result = _gameStateHandler.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.DiscardedCards, Is.EquivalentTo(cardsToDiscard));
            Assert.That(_gameStateHandler.State.Discards, Is.EqualTo(1));
            Assert.That(_gameStateHandler.State.CurrentHand.Count, Is.EqualTo(_gameStateHandler.Config.HandSize - cardsToDiscard.Count));
            Assert.That(_gameStateHandler.State.CurrentHand.Intersect(cardsToDiscard), Is.Empty);
            // Las cartas descartadas deben contener las cartas que acabamos de descartar (no ser igual)
            Assert.That(_gameStateHandler.State.DiscardedCards.Intersect(cardsToDiscard), Is.EquivalentTo(cardsToDiscard));
            Assert.That(_gameStateHandler.State.PlayedCards, Is.Empty);
            Assert.That(_gameStateHandler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();
        var cardsToDiscard = _gameStateHandler.State.CurrentHand.Take(config.MaxDiscardCards + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.Discard(cardsToDiscard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: discard between 1 and {config.MaxDiscardCards} cards: {cardsToDiscard.Count}"));
        Assert.That(_gameStateHandler.State.Discards, Is.EqualTo(0));
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldThrowGameException() {
        // Use all discards
        for (int i = 0; i < config.MaxDiscards; i++) {
            _gameStateHandler.DrawCards();
            var oneCard = _gameStateHandler.State.CurrentHand.Take(1).ToList();
            _ = _gameStateHandler.Discard(oneCard);
        }

        // Try one more discard
        _gameStateHandler.DrawCards();
        var moreCard = _gameStateHandler.State.CurrentHand.Take(1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => _gameStateHandler.Discard(moreCard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: no discards remaining {_gameStateHandler.State.Discards} of {config.MaxDiscards}"));
    }

    [Test]
    public void Discard_WhenGameWon_ShouldThrowGameException() {
        _gameStateHandler.State.TotalScore = 10; // Set a low score to make it winnable
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards); // This should make us win the game

        // Try to discard
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: game won"));
    }

    [Test]
    public void Discard_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: cannot discard hands yet, draw cards first."));
    }

    [Test]
    public void Discard_WithInvalidCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.Discard(_gameStateHandler.State.AvailableCards.Take(1).ToList()));
        Assert.That(exception.Message, Does.Contain("Discard error: contains cards not in current hand"));
    }

    [Test]
    public void DrawCards_WithSpecificCards_ShouldDrawCorrectly() {
        _gameStateHandler.DrawCards();

        // Jugar una mano para tener espacio para robar más cartas
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);

        // Seleccionar cartas específicas para robar
        var availableCards = _gameStateHandler.State.AvailableCards.Take(hand.Cards.Count).ToList();
        _gameStateHandler.DrawCards(availableCards);

        Assert.Multiple(() => {
            // Verificar que las cartas específicas están en la mano actual
            Assert.That(_gameStateHandler.State.CurrentHand.Intersect(availableCards), Is.EquivalentTo(availableCards));
            // Verificar que las cartas ya no están disponibles
            Assert.That(_gameStateHandler.State.AvailableCards.Intersect(availableCards), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_WithInvalidCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);

        // Primero jugamos una mano para tener espacio para robar
        var playedCard = hand.Cards[0]; // Guardamos una carta que ya ha sido jugada

        // Intentar robar la carta que ya fue jugada (ahora está en PlayedCards, no en AvailableCards)
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.DrawCards([playedCard]));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cards to draw not found in available pile"));
    }

    [Test]
    public void DrawCards_WithTooManyCards_ShouldThrowGameException() {
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);

        // Intentar robar más cartas de las necesarias
        var tooManyCards = _gameStateHandler.State.AvailableCards.Take(hand.Cards.Count + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.DrawCards(tooManyCards));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cannot draw more cards"));
    }

    [Test]
    public void Recover_FromPlayedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in played pile
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);
        var cardToRecover = hand.Cards[0];

        // Act
        _gameStateHandler.Recover([cardToRecover]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.PlayedCards, Does.Not.Contain(cardToRecover));
            Assert.That(_gameStateHandler.State.AvailableCards, Does.Contain(cardToRecover));
        });
    }

    [Test]
    public void Recover_FromDiscardedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in discarded pile
        _gameStateHandler.DrawCards();
        var cardToDiscard = _gameStateHandler.State.CurrentHand.First();
        _gameStateHandler.Discard([cardToDiscard]);

        // Act
        _gameStateHandler.Recover([cardToDiscard]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(_gameStateHandler.State.AvailableCards, Does.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Recover_RandomCardsFromBothPiles_ShouldMoveCardsToAvailablePile() {
        // Prepare some cards in played pile
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);

        // Prepare some cards in discarded pile
        _gameStateHandler.DrawCards();
        var cardToDiscard = _gameStateHandler.State.CurrentHand.First();
        _gameStateHandler.Discard([cardToDiscard]);

        var initialPlayedCount = _gameStateHandler.State.PlayedCards.Count;
        var initialDiscardedCount = _gameStateHandler.State.DiscardedCards.Count;
        var initialAvailableCount = _gameStateHandler.State.AvailableCards.Count;
        var cardsToRecover = 2;

        // Act
        _gameStateHandler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.AvailableCards.Count,
                Is.EqualTo(initialAvailableCount + cardsToRecover));
            Assert.That(_gameStateHandler.State.PlayedCards.Count + _gameStateHandler.State.DiscardedCards.Count,
                Is.EqualTo(initialPlayedCount + initialDiscardedCount - cardsToRecover));
        });
    }

    [Test]
    public void Recover_WithCardsFromBothPiles_ShouldMoveSpecificCardsToAvailablePile() {
        // Prepare a card in played pile
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);
        var playedCard = hand.Cards[0];

        // Prepare a card in discarded pile
        _gameStateHandler.DrawCards();
        var cardToDiscard = _gameStateHandler.State.CurrentHand.First();
        _gameStateHandler.Discard([cardToDiscard]);

        var cardsToRecover = new List<Card> { playedCard, cardToDiscard };

        // Act
        _gameStateHandler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.PlayedCards, Does.Not.Contain(playedCard));
            Assert.That(_gameStateHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(_gameStateHandler.State.AvailableCards.Intersect(cardsToRecover),
                Is.EquivalentTo(cardsToRecover));
        });
    }

    [Test]
    public void Recover_FromDestroyedPile_ShouldMoveCardToAvailablePile() {
        _gameStateHandler.DrawCards();
        var cardToDestroy = _gameStateHandler.State.CurrentHand.First();
        _gameStateHandler.Discard([cardToDestroy]);

        _gameStateHandler.Recover([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.DestroyedCards, Does.Not.Contain(cardToDestroy));
            Assert.That(_gameStateHandler.State.AvailableCards, Does.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromCurrentHand_ShouldMoveCardToDestroyedPile() {
        _gameStateHandler.DrawCards();
        var cardToDestroy = _gameStateHandler.State.CurrentHand.First();

        _gameStateHandler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(_gameStateHandler.State.CurrentHand, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromAvailablePile_ShouldMoveCardToDestroyedPile() {
        var cardToDestroy = _gameStateHandler.State.AvailableCards.First();

        _gameStateHandler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(_gameStateHandler.State.AvailableCards, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromDiscardedPile_ShouldMoveCardToDestroyedPile() {
        _gameStateHandler.DrawCards();
        var cardToDiscard = _gameStateHandler.State.CurrentHand.First();
        _gameStateHandler.Discard([cardToDiscard]);

        _gameStateHandler.Destroy([cardToDiscard]);

        Assert.Multiple(() => {
            Assert.That(_gameStateHandler.State.DestroyedCards, Does.Contain(cardToDiscard));
            Assert.That(_gameStateHandler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Destroy_FromPlayedPile_ShouldMoveCardToDestroyedPile() {
        _gameStateHandler.DrawCards();
        var hand = _gameStateHandler.GetPossibleHands()[0];
        _gameStateHandler.PlayHand(hand.Cards);
        var cardsToDestroy = hand.Cards;

        _gameStateHandler.Destroy(cardsToDestroy);

        Assert.Multiple(() => {
            // Verificar que todas las cartas están en la pila de destruidas
            Assert.That(_gameStateHandler.State.DestroyedCards, Is.EquivalentTo(cardsToDestroy));
            // Verificar que ninguna carta está en la pila de jugadas
            Assert.That(_gameStateHandler.State.PlayedCards.Intersect(cardsToDestroy), Is.Empty);
        });
    }

    [Test]
    public void Destroy_NonExistentCard_ShouldThrowGameException() {
        var nonExistentCard = new Card(25, 'S'); // 25 of Spades que no existe en ninguna pila

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.Destroy([nonExistentCard]));
        Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
    }

    [Test]
    public void Destroy_MultipleTimes_ShouldOnlyDestroyOnce() {
        _gameStateHandler.DrawCards();
        var cardToDestroy = _gameStateHandler.State.CurrentHand.First();

        _gameStateHandler.Destroy([cardToDestroy]);
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            _gameStateHandler.Destroy([cardToDestroy]));

        Assert.Multiple(() => {
            Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
            Assert.That(_gameStateHandler.State.DestroyedCards.Count(c => c == cardToDestroy), Is.EqualTo(1));
        });
    }
}