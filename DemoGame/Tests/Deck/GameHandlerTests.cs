using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;
using NUnit.Framework;using Veronenger.Game.Deck.Hands;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Tests.Deck;

[TestFixture]
public class GameHandlerTests {
    public GameRun GameRun;
    public GameHandler Handler;
    public PokerGameConfig Config;
    public PokerHandsManager HandsManager;

    [SetUp]
    public void Setup() {
        Config = new PokerGameConfig();
        HandsManager = new PokerHandsManager(new PokerHandConfig());
        HandsManager.RegisterBasicPokerHands();
        GameRun = new GameRun(Config, HandsManager, 0);
        Handler = GameRun.CreateGameHandler(0, DeckBuilder.ClassicPokerDeck());
    }

    [Test]
    public void TestLevelProgression() {
        var state = Handler.State;

        // Test that Level affects scoring for a specific hand type
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        var scoreAtLevel0 = Handler.CalculateScore(hand);

        GameRun.State.SetPokerHandLevel(hand.HandType, 1);
        var scoreAtLevel1 = Handler.CalculateScore(hand);
        Assert.That(scoreAtLevel1, Is.GreaterThan(scoreAtLevel0));

        GameRun.State.SetPokerHandLevel(hand.HandType, 2);
        var scoreAtLevel2 = Handler.CalculateScore(hand);
        Assert.That(scoreAtLevel2, Is.GreaterThan(scoreAtLevel1));
    }

    [Test]
    public void TestScoringAtDifferentLevels() {
        var pairHand = new PokerHand(PokerHandType.Pair, new List<Card> {
            new(12, 'H'), // Queen of Hearts
            new(12, 'S'), // Queen of Spades
        });

        // Test at Level 0
        var scoreLevel0 = Handler.CalculateScore(pairHand);
        var expectedLevel0 = (10 + (12 + 12)) * 2; // (initialScore + ranks) * initialMultiplier
        Assert.That(scoreLevel0, Is.EqualTo(expectedLevel0));

        // Test at Level 1
        GameRun.State.SetPokerHandLevel(pairHand.HandType, 1);
        var scoreLevel1 = Handler.CalculateScore(pairHand);
        var expectedLevel1 = (25 + (12 + 12)) * 3; // ((10 + 15) + ranks) * (2 + 1)
        Assert.That(scoreLevel1, Is.EqualTo(expectedLevel1));

        // Test at Level 2
        GameRun.State.SetPokerHandLevel(pairHand.HandType, 2);
        var scoreLevel2 = Handler.CalculateScore(pairHand);
        var expectedLevel2 = (40 + (12 + 12)) * 4; // ((10 + 30) + ranks) * (2 + 2)
        Assert.That(scoreLevel2, Is.EqualTo(expectedLevel2));
    }

    [Test]
    public void GameProgression_ShouldHandleMultipleLevels() {
        var levels = new[] { 0, 1, 2 };

        foreach (var level in levels) {
            var gameHandler = GameRun.CreateGameHandler(10, DeckBuilder.ClassicPokerDeck());

            while (!gameHandler.IsWon() && !gameHandler.IsGameOver()) {
                gameHandler.DrawCards();
                var bestHand = gameHandler.GetPossibleHands()[0];

                // Establecer el nivel específico para este tipo de mano
                GameRun.State.SetPokerHandLevel(bestHand.HandType, level);

                var result = gameHandler.PlayHand(bestHand.Cards);

                // Verify that score increases with level
                Assert.That(result.Score, Is.GreaterThan(0));
                if (level > 0) {
                    // Recreate the same hand but with level 0
                    GameRun.State.SetPokerHandLevel(bestHand.HandType, 0);
                    var baseScore = gameHandler.CalculateScore(bestHand);
                    GameRun.State.SetPokerHandLevel(bestHand.HandType, level);
                    var levelScore = gameHandler.CalculateScore(bestHand);
                    Assert.That(levelScore, Is.GreaterThan(baseScore));
                }
            }
        }
    }

    [Test]
    public void CompleteGame_ShouldAccumulateScore() {
        long totalScore = 0;
        List<long> handScores = [];
        Handler.State.LevelScore = 100000;

        // Play all hands
        for (int i = 0; i < Config.MaxHands; i++) {
            Handler.DrawCards();
            var hand = Handler.GetPossibleHands()[0]; // Always play best hand
            var result = Handler.PlayHand(hand.Cards);
            handScores.Add(result.Score);
            totalScore += result.Score;
        }

        var finalState = Handler.State;
        Assert.Multiple(() => {
            Assert.That(finalState.Score, Is.EqualTo(totalScore));
            Assert.That(Handler.IsGameOver(), Is.True);

            // Verify history matches our played hands
            var history = finalState.History.GetHistory().Where(a => a.Type == PlayHistory.PlayedActionType.Play).ToList();
            Assert.That(history.Count, Is.EqualTo(Config.MaxHands));
            for (int i = 0; i < handScores.Count; i++) {
                Assert.That(history[i].HandScore, Is.EqualTo(handScores[i]));
            }
        });
    }

    [Test]
    public void DrawCards_ShouldDrawCorrectNumber() {
        Handler.DrawCards();
        Assert.That(Handler.State.CurrentHand.Count, Is.EqualTo(Config.HandSize));
    }

    [Test]
    public void Discard_ShouldDecrementRemainingDiscards() {
        Handler.DrawCards();
        var initialDiscards = Handler.State.Discards;
        var cardToDiscard = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDiscard]);
        Assert.That(Handler.State.Discards, Is.EqualTo(initialDiscards + 1));
    }

    [Test]
    public void PlayHand_WithInvalidCards_ShouldThrowGameException() {
        Handler.DrawCards();
        var invalidCards = Handler.State.AvailableCards.Take(1).ToList();
        Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand(invalidCards));
    }

    [Test]
    public void IsWon_ShouldDependOnTotalScore() {
        // Should not be won initially
        Assert.That(Handler.IsWon(), Is.False);

        // Set score to meet total score
        Handler.State.Score = Handler.State.LevelScore;
        Assert.That(Handler.IsWon(), Is.True);
    }

    [Test]
    public void NewGame_ShouldStartWithCorrectInitialState() {
        var state = Handler.State;
        Assert.Multiple(() => {
            Assert.That(state.Level, Is.EqualTo(0)); // level defined in the test
            Assert.That(state.Score, Is.EqualTo(0));
            Assert.That(state.HandsPlayed, Is.EqualTo(0));
            Assert.That(state.LevelScore, Is.EqualTo(Config.GetLevelScore(0)));
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
        Handler.DrawCards();
        var state = Handler.State;
        Assert.Multiple(() => {
            Assert.That(state.Level, Is.EqualTo(0)); // level defined in the test
            Assert.That(state.Score, Is.EqualTo(0));
            Assert.That(state.HandsPlayed, Is.EqualTo(0));
            Assert.That(state.LevelScore, Is.EqualTo(Config.GetLevelScore(0)));
            Assert.That(state.Discards, Is.EqualTo(0));
            Assert.That(state.CurrentHand.Count, Is.EqualTo(Config.HandSize));
            Assert.That(state.DiscardedCards.Count, Is.EqualTo(0));
            Assert.That(state.PlayedCards.Count, Is.EqualTo(0));
            Assert.That(state.AvailableCards.Count, Is.EqualTo(52 - Config.HandSize));
            Assert.That(state.History.GetHistory(), Is.Empty);
        });
    }

    private GameHandler CreateGameWithInitialHand(PokerHandType type, int maxAttempts = 1000) {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var gameRun = new GameRun(Config, HandsManager, seed);
            var testGame = gameRun.CreateGameHandler(0, DeckBuilder.ClassicPokerDeck());
            testGame.DrawCards();
            var possibleHands = testGame.GetPossibleHands().Where(h => h.HandType == type).ToList();
            if (possibleHands.Count > 0) {
                return testGame;
            }
        }
        throw new InvalidOperationException($"Could not find a game with a {type} in the initial hand after {maxAttempts} attempts");
    }

    private GameHandler CreateGameWithInitialHandSize(int cardCount, int maxAttempts = 1000) {
        for (int seed = 1; seed <= maxAttempts; seed++) {
            var gameRun = new GameRun(Config, HandsManager, seed);
            var testGame = gameRun.CreateGameHandler(0, DeckBuilder.ClassicPokerDeck());
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
        Handler = CreateGameWithInitialHand(PokerHandType.Pair);
        var initialHand = new List<Card>(Handler.State.CurrentHand);

        // Find the pair
        var pairHand = Handler
            .GetPossibleHands()
            .First(h => h.HandType == PokerHandType.Pair); // We know it exists because CreateGameWithInitialHand succeeded

        var pairCards = pairHand.Cards.ToList();
        var unusedCards = initialHand.Except(pairCards).ToList();

        // Play the pair
        Handler.PlayHand(pairHand.Cards);

        // Draw new cards
        Handler.DrawCards();
        var newHand = Handler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(Handler.Config.HandSize));
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
        Handler = CreateGameWithInitialHandSize(5);
        var initialHand = new List<Card>(Handler.State.CurrentHand);

        var fiveCardHand = Handler.GetPossibleHands()
            .First(h => h.Cards.Count == 5);

        var usedCards = fiveCardHand.Cards.ToList();
        var unusedCards = initialHand.Except(usedCards).ToList();

        // Play the hand
        Handler.PlayHand(fiveCardHand.Cards);

        // Draw new cards
        Handler.DrawCards();
        var newHand = Handler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(Handler.Config.HandSize));
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
        Handler.DrawCards();
        var initialHand = new List<Card>(Handler.State.CurrentHand);
        Assert.That(initialHand.Count, Is.EqualTo(Handler.Config.HandSize));

        // Discard 2 cards
        var cardsToDiscard = initialHand.Take(2).ToList();
        var cardsToKeep = initialHand.Skip(2).ToList();
        Handler.Discard(cardsToDiscard);

        // Draw new cards
        Handler.DrawCards();
        var newHand = Handler.State.CurrentHand;

        Assert.Multiple(() => {
            // Verify we still have the same hand size
            Assert.That(newHand.Count, Is.EqualTo(Handler.Config.HandSize));
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
        Handler.State.LevelScore = 100000;
        for (int i = 0; i < Config.MaxHands; i++) {
            Handler.DrawCards();
            var hand = Handler.GetPossibleHands()[0];
            Handler.PlayHand(hand.Cards);
        }

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards errors: game is over"));
    }

    [Test]
    public void DrawCards_WhenHandAlreadyFull_ShouldThrowGameException() {
        Handler.DrawCards(); // Fill the hand

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.DrawCards());
        Assert.That(exception.Message, Is.EqualTo("DrawCards error: hand already full"));
    }

    [Test]
    public void PlayHand_WithValidHand_ShouldUpdateState() {
        Handler.DrawCards();
        var possibleHands = Handler.GetPossibleHands();
        Assert.That(possibleHands, Is.Not.Empty, "Should have at least one possible hand");

        var playedHand = possibleHands[0];
        var result = Handler.PlayHand(playedHand.Cards);
        Assert.Multiple(() => {
            Assert.That(result.Score, Is.EqualTo(Handler.CalculateScore(playedHand)));
            Assert.That(Handler.State.HandsPlayed, Is.EqualTo(1));
            Assert.That(Handler.State.Score, Is.EqualTo(result.Score));
            // Las cartas jugadas deben contener las cartas de la mano jugada (no ser igual)
            Assert.That(Handler.State.PlayedCards.Intersect(playedHand.Cards), Is.EquivalentTo(playedHand.Cards));
            Assert.That(Handler.State.DiscardedCards, Is.Empty);
            Assert.That(Handler.State.CurrentHand.Count, Is.EqualTo(Handler.Config.HandSize - playedHand.Cards.Count));
            Assert.That(Handler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void PlayHand_AfterMaxHands_ShouldThrowGameException() {
        Handler.State.LevelScore = 100000;
        for (int i = 0; i < Config.MaxHands - 1; i++) {
            Handler.DrawCards();
            Handler.PlayHand(Handler.GetBestHand().Cards);
        }

        // Play final hand
        Handler.DrawCards();
        Handler.PlayHand(Handler.GetBestHand().Cards);
        Assert.Multiple(() => {
            Assert.That(Handler.IsGameOver(), Is.True);
            // CurrentHand debe tener las cartas que quedaron sin usar de la última mano
            Assert.That(Handler.State.CurrentHand.Count, Is.LessThan(Handler.Config.HandSize));
            Assert.That(Handler.State.History.GetHistory().Count, Is.EqualTo(Config.MaxHands));
        });

        Assert.That(Handler.GetPossibleHands(), Is.Not.Empty); // Puede haber manos posibles con las cartas que quedaron

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand(Handler.State.CurrentHand));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game is over"));
        Assert.Throws<SolitairePokerGameException>(() => Handler.DrawCards());
    }

    [Test]
    public void PlayHand_WhenGameWon_ShouldThrowGameException() {
        Handler.State.LevelScore = 1; // Set a low score to make it winnable
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards); // This should make us win the game

        // Try to play another hand
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand([new Card(2, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: game won"));
    }

    [Test]
    public void PlayHand_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand([new Card(2, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("PlayHand error: cannot play hands yet, draw cards first."));
    }

    [Test]
    public void PlayHand_ShouldUpdateGameHistoryActionScores() {
        // Establecer un objetivo total
        Handler.State.LevelScore = 300;
        Handler.DrawCards();

        // Jugar dos manos para verificar la acumulación del score
        var firstHand = Handler.GetPossibleHands()[0];
        var firstResult = Handler.PlayHand(firstHand.Cards);
        Handler.DrawCards();

        var secondHand = Handler.GetPossibleHands()[0];
        var secondResult = Handler.PlayHand(secondHand.Cards);

        // Obtener las acciones del historial
        var history = Handler.State.History.GetHistory()
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
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];

        Handler.PlayHand(hand.Cards);

        Assert.Multiple(() => {
            // Las cartas jugadas no deberían estar en la mano actual
            Assert.That(Handler.State.CurrentHand.Intersect(hand.Cards), Is.Empty);
            // Las cartas jugadas deberían estar en la pila de jugadas
            Assert.That(Handler.State.PlayedCards.Intersect(hand.Cards), Is.EquivalentTo(hand.Cards));
            // Las cartas jugadas no deberían estar disponibles
            Assert.That(Handler.State.AvailableCards.Intersect(hand.Cards), Is.Empty);
        });
    }

    [Test]
    public void PlayHand_WithEmptyHand_ShouldThrowGameException() {
        Handler.DrawCards();

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand([]));
        Assert.That(exception.Message, Does.Contain("PlayHand error: cards cannot be empty"));
    }

    [Test]
    public void PlayHand_WithDiscardedCards_ShouldThrowGameException() {
        Handler.DrawCards();

        // Primero descartamos algunas cartas
        var cardsToDiscard = Handler.State.CurrentHand.Take(2).ToList();
        Handler.Discard(cardsToDiscard);

        // Robamos nuevas cartas para completar la mano
        Handler.DrawCards();

        // Intentamos jugar las cartas que fueron descartadas anteriormente
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.PlayHand(cardsToDiscard));
        Assert.That(exception.Message, Does.Contain("PlayHand error: hand to play contains cards not in current hand"));
    }

    [Test]
    public void PlayHand_ShouldUpdateHistory() {
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        var result = Handler.PlayHand(hand.Cards);

        var lastHistoryAction = Handler.State.History.GetHistory().Last();
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
        Handler.State.LevelScore = 300000;
        Handler.DrawCards();

        // Jugar tres manos y verificar la acumulación del score
        var firstHand = Handler.GetPossibleHands()[0];
        var firstResult = Handler.PlayHand(firstHand.Cards);
        Handler.DrawCards();

        var secondHand = Handler.GetPossibleHands()[0];
        var secondResult = Handler.PlayHand(secondHand.Cards);
        Handler.DrawCards();

        var thirdHand = Handler.GetPossibleHands()[0];
        var thirdResult = Handler.PlayHand(thirdHand.Cards);

        Assert.Multiple(() => {
            // Verificar el score final
            var expectedTotalScore = firstResult.Score + secondResult.Score + thirdResult.Score;
            Assert.That(Handler.State.Score, Is.EqualTo(expectedTotalScore));
            Assert.That(Handler.State.LevelScore, Is.EqualTo(300000));
        });
    }

    [Test]
    public void IsWon_ShouldDependOnTotalScoreAfterPlayHand() {
        // Caso 1: Score bajo, no debería ganar
        Handler.State.LevelScore = 1000;
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        Assert.That(Handler.IsWon(), Is.False, "Game should not be won with low score");

        // Caso 2: Score suficiente, debería ganar
        Handler = GameRun.CreateGameHandler(0, DeckBuilder.ClassicPokerDeck());
        Handler.State.LevelScore = 10; // Un valor bajo que seguro se puede alcanzar con una mano
        Handler.DrawCards();
        hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        Assert.That(Handler.IsWon(), Is.True, "Game should be won when score >= totalScore");
    }

    [Test]
    public void Discard_ValidCards_ShouldUpdateHandAndDecrementDiscards() {
        Handler.DrawCards();
        var initialState = Handler.State;
        var cardsToDiscard = initialState.CurrentHand.Take(3).ToList();

        var result = Handler.Discard(cardsToDiscard);

        Assert.Multiple(() => {
            Assert.That(result.DiscardedCards, Is.EquivalentTo(cardsToDiscard));
            Assert.That(Handler.State.Discards, Is.EqualTo(1));
            Assert.That(Handler.State.CurrentHand.Count, Is.EqualTo(Handler.Config.HandSize - cardsToDiscard.Count));
            Assert.That(Handler.State.CurrentHand.Intersect(cardsToDiscard), Is.Empty);
            // Las cartas descartadas deben contener las cartas que acabamos de descartar (no ser igual)
            Assert.That(Handler.State.DiscardedCards.Intersect(cardsToDiscard), Is.EquivalentTo(cardsToDiscard));
            Assert.That(Handler.State.PlayedCards, Is.Empty);
            Assert.That(Handler.State.History.GetHistory().Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void Discard_TooManyCards_ShouldThrowGameException() {
        Handler.DrawCards();
        var cardsToDiscard = Handler.State.CurrentHand.Take(Config.MaxDiscardCards + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.Discard(cardsToDiscard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: discard between 1 and {Config.MaxDiscardCards} cards: {cardsToDiscard.Count}"));
        Assert.That(Handler.State.Discards, Is.EqualTo(0));
    }

    [Test]
    public void Discard_AfterMaxDiscards_ShouldThrowGameException() {
        // Use all discards
        for (int i = 0; i < Config.MaxDiscards; i++) {
            Handler.DrawCards();
            var oneCard = Handler.State.CurrentHand.Take(1).ToList();
            _ = Handler.Discard(oneCard);
        }

        // Try one more discard
        Handler.DrawCards();
        var moreCard = Handler.State.CurrentHand.Take(1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.Discard(moreCard));
        Assert.That(exception.Message, Is.EqualTo($"Discard error: no discards remaining {Handler.State.Discards} of {Config.MaxDiscards}"));
    }

    [Test]
    public void Discard_WhenGameWon_ShouldThrowGameException() {
        Handler.State.LevelScore = 10; // Set a low score to make it winnable
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards); // This should make us win the game

        // Try to discard
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: game won"));
    }

    [Test]
    public void Discard_WithoutDrawingCards_ShouldThrowGameException() {
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Discard([new Card(14, 'S')]));
        Assert.That(exception.Message, Is.EqualTo("Discard error: cannot discard hands yet, draw cards first."));
    }

    [Test]
    public void Discard_WithInvalidCards_ShouldThrowGameException() {
        Handler.DrawCards();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Discard(Handler.State.AvailableCards.Take(1).ToList()));
        Assert.That(exception.Message, Does.Contain("Discard error: contains cards not in current hand"));
    }

    [Test]
    public void Discard_WithNoCards_ShouldThrowGameException() {
        Handler.DrawCards();

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.Discard([]));
        Assert.That(exception.Message, Does.Contain("Discard error: cards cannot be empty"));
    }

    [Test]
    public void DrawCards_WithSpecificCards_ShouldDrawCorrectly() {
        Handler.DrawCards();

        // Jugar una mano para tener espacio para robar más cartas
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);

        // Seleccionar cartas específicas para robar
        var availableCards = Handler.State.AvailableCards.Take(hand.Cards.Count).ToList();
        Handler.DrawCards(availableCards);

        Assert.Multiple(() => {
            // Verificar que las cartas específicas están en la mano actual
            Assert.That(Handler.State.CurrentHand.Intersect(availableCards), Is.EquivalentTo(availableCards));
            // Verificar que las cartas ya no están disponibles
            Assert.That(Handler.State.AvailableCards.Intersect(availableCards), Is.Empty);
        });
    }

    [Test]
    public void DrawCards_WithInvalidCards_ShouldThrowGameException() {
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);

        // Primero jugamos una mano para tener espacio para robar
        var playedCard = hand.Cards[0]; // Guardamos una carta que ya ha sido jugada

        // Intentar robar la carta que ya fue jugada (ahora está en PlayedCards, no en AvailableCards)
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.DrawCards([playedCard]));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cards to draw not found in available pile"));
    }

    [Test]
    public void DrawCards_WithTooManyCards_ShouldThrowGameException() {
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);

        // Intentar robar más cartas de las necesarias
        var tooManyCards = Handler.State.AvailableCards.Take(hand.Cards.Count + 1).ToList();
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.DrawCards(tooManyCards));
        Assert.That(exception.Message, Does.Contain("DrawCards error: cannot draw more cards"));
    }

    [Test]
    public void Recover_FromPlayedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in played pile
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        var cardToRecover = hand.Cards[0];

        // Act
        Handler.Recover([cardToRecover]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(Handler.State.PlayedCards, Does.Not.Contain(cardToRecover));
            Assert.That(Handler.State.AvailableCards, Does.Contain(cardToRecover));
        });
    }

    [Test]
    public void Recover_FromDiscardedPile_ShouldMoveCardToAvailablePile() {
        // Arrange: Prepare a card in discarded pile
        Handler.DrawCards();
        var cardToDiscard = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDiscard]);

        // Act
        Handler.Recover([cardToDiscard]);

        // Assert
        Assert.Multiple(() => {
            Assert.That(Handler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(Handler.State.AvailableCards, Does.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Recover_RandomCardsFromBothPiles_ShouldMoveCardsToAvailablePile() {
        // Prepare some cards in played pile
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);

        // Prepare some cards in discarded pile
        Handler.DrawCards();
        var cardToDiscard = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDiscard]);

        var initialPlayedCount = Handler.State.PlayedCards.Count;
        var initialDiscardedCount = Handler.State.DiscardedCards.Count;
        var initialAvailableCount = Handler.State.AvailableCards.Count;
        var cardsToRecover = 2;

        // Act
        Handler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(Handler.State.AvailableCards.Count,
                Is.EqualTo(initialAvailableCount + cardsToRecover));
            Assert.That(Handler.State.PlayedCards.Count + Handler.State.DiscardedCards.Count,
                Is.EqualTo(initialPlayedCount + initialDiscardedCount - cardsToRecover));
        });
    }

    [Test]
    public void Recover_WithCardsFromBothPiles_ShouldMoveSpecificCardsToAvailablePile() {
        // Prepare a card in played pile
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        var playedCard = hand.Cards[0];

        // Prepare a card in discarded pile
        Handler.DrawCards();
        var cardToDiscard = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDiscard]);

        var cardsToRecover = new List<Card> { playedCard, cardToDiscard };

        // Act
        Handler.Recover(cardsToRecover);

        // Assert
        Assert.Multiple(() => {
            Assert.That(Handler.State.PlayedCards, Does.Not.Contain(playedCard));
            Assert.That(Handler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
            Assert.That(Handler.State.AvailableCards.Intersect(cardsToRecover),
                Is.EquivalentTo(cardsToRecover));
        });
    }

    [Test]
    public void Recover_FromDestroyedPile_ShouldMoveCardToAvailablePile() {
        Handler.DrawCards();
        var cardToDestroy = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDestroy]);

        Handler.Recover([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(Handler.State.DestroyedCards, Does.Not.Contain(cardToDestroy));
            Assert.That(Handler.State.AvailableCards, Does.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_WithNoCards_ShouldThrowGameException() {
        Handler.DrawCards();

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.Destroy([]));
        Assert.That(exception.Message, Does.Contain("Destroy error: cards cannot be empty"));
    }

    [Test]
    public void Destroy_FromCurrentHand_ShouldMoveCardToDestroyedPile() {
        Handler.DrawCards();
        var cardToDestroy = Handler.State.CurrentHand.First();

        Handler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(Handler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(Handler.State.CurrentHand, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromAvailablePile_ShouldMoveCardToDestroyedPile() {
        var cardToDestroy = Handler.State.AvailableCards.First();

        Handler.Destroy([cardToDestroy]);

        Assert.Multiple(() => {
            Assert.That(Handler.State.DestroyedCards, Does.Contain(cardToDestroy));
            Assert.That(Handler.State.AvailableCards, Does.Not.Contain(cardToDestroy));
        });
    }

    [Test]
    public void Destroy_FromDiscardedPile_ShouldMoveCardToDestroyedPile() {
        Handler.DrawCards();
        var cardToDiscard = Handler.State.CurrentHand.First();
        Handler.Discard([cardToDiscard]);

        Handler.Destroy([cardToDiscard]);

        Assert.Multiple(() => {
            Assert.That(Handler.State.DestroyedCards, Does.Contain(cardToDiscard));
            Assert.That(Handler.State.DiscardedCards, Does.Not.Contain(cardToDiscard));
        });
    }

    [Test]
    public void Destroy_FromPlayedPile_ShouldMoveCardToDestroyedPile() {
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        var cardsToDestroy = hand.Cards;

        Handler.Destroy(cardsToDestroy);

        Assert.Multiple(() => {
            // Verificar que todas las cartas están en la pila de destruidas
            Assert.That(Handler.State.DestroyedCards, Is.EquivalentTo(cardsToDestroy));
            // Verificar que ninguna carta está en la pila de jugadas
            Assert.That(Handler.State.PlayedCards.Intersect(cardsToDestroy), Is.Empty);
        });
    }

    [Test]
    public void Destroy_NonExistentCard_ShouldThrowGameException() {
        var nonExistentCard = new Card(25, 'S'); // 25 of Spades que no existe en ninguna pila

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Destroy([nonExistentCard]));
        Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
    }

    [Test]
    public void Destroy_MultipleTimes_ShouldOnlyDestroyOnce() {
        Handler.DrawCards();
        var cardToDestroy = Handler.State.CurrentHand.First();

        Handler.Destroy([cardToDestroy]);
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Destroy([cardToDestroy]));

        Assert.Multiple(() => {
            Assert.That(exception.Message, Does.Contain("Destroy error: card not found in any pile"));
            Assert.That(Handler.State.DestroyedCards.Count(c => c == cardToDestroy), Is.EqualTo(1));
        });
    }

    [Test]
    public void Recover_NonExistentCard_ShouldThrowGameException() {
        var nonExistentCard = new Card(25, 'S'); // 25 of Spades no existe

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Recover([nonExistentCard]));
        Assert.That(exception.Message, Does.Contain("Recover error: cards to recover not found in played or discarded piles"));
    }

    [Test]
    public void Recover_CardAlreadyInAvailablePile_ShouldThrowGameException() {
        var availableCard = Handler.State.AvailableCards.First();

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Recover([availableCard]));
        Assert.That(exception.Message, Does.Contain("Recover error: cards to recover not found in played or discarded piles"));
    }

    [Test]
    public void Recover_WithNoCards_ShouldThrowGameException() {
        Handler.DrawCards();

        var exception = Assert.Throws<SolitairePokerGameException>(() => Handler.Recover([]));
        Assert.That(exception.Message, Does.Contain("Recover error: cards cannot be empty"));
    }

    [Test]
    public void Recover_MoreCardsThanAvailable_ShouldThrowGameException() {
        // Jugamos una mano para tener cartas en PlayedCards
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);

        // Intentamos recuperar más cartas de las que hay
        var totalPlayedAndDiscarded = Handler.State.PlayedCards.Count + Handler.State.DiscardedCards.Count;

        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Recover(totalPlayedAndDiscarded + 1));
        Assert.That(exception.Message, Does.Contain($"Recover error: cannot recover more cards"));
    }

    [Test]
    public void PlayHand_WithDestroyedCards_ShouldThrowGameException() {
        // Primer draw para obtener cartas
        Handler.DrawCards();
        var cardToDestroy = Handler.State.CurrentHand.First();
        Handler.Destroy([cardToDestroy]);

        // Necesitamos hacer otro draw después de destruir para tener una mano válida
        Handler.DrawCards();
    
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.PlayHand([cardToDestroy]));
        Assert.That(exception.Message, Does.Contain("PlayHand error: hand to play contains cards not in current hand"));
    }

    [Test]
    public void Discard_WithDestroyedCards_ShouldThrowGameException() {
        // Primer draw para obtener cartas
        Handler.DrawCards();
        var cardToDestroy = Handler.State.CurrentHand.First();
        Handler.Destroy([cardToDestroy]);
    
        // Necesitamos hacer otro draw después de destruir para tener una mano válida
        Handler.DrawCards();
    
        var exception = Assert.Throws<SolitairePokerGameException>(() =>
            Handler.Discard([cardToDestroy]));
        Assert.That(exception.Message, Does.Contain("Discard error: contains cards not in current hand"));
    }

    [Test]
    public void Destroy_AfterRecover_ShouldWork() {
        // Preparar una carta en played pile
        Handler.DrawCards();
        var hand = Handler.GetPossibleHands()[0];
        Handler.PlayHand(hand.Cards);
        var playedCard = hand.Cards[0];

        // Recuperar la carta
        Handler.Recover([playedCard]);

        // Debería poder destruirla
        Handler.Destroy([playedCard]);

        Assert.Multiple(() => {
            Assert.That(Handler.State.DestroyedCards, Does.Contain(playedCard));
            Assert.That(Handler.State.AvailableCards, Does.Not.Contain(playedCard));
        });
    }
}