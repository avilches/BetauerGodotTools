using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class GameHandler {
    public class PlayResult(PokerHand hand, long score) {
        public long Score { get; } = score;
        public PokerHand? Hand { get; } = hand;
    }

    public class DiscardResult(IReadOnlyList<Card> discardedCards) {
        public IReadOnlyList<Card> DiscardedCards { get; } = discardedCards;
    }

    public PokerHandsManager PokerHandsManager { get; }
    public PokerGameConfig Config { get; }
    public GameState State { get; }
    public GameRunState GameRunState { get; }
    
    public bool IsWon() => State.IsWon();
    public bool IsGameOver() => State.IsGameOver();
    public bool IsDrawPending() => State.IsDrawPending();
    public bool CanDiscard() => State.CanDiscard();

    public long RemainingScoreToWin => State.RemainingScoreToWin;
    public int RemainingHands => State.RemainingHands;
    public int RemainingDiscards => State.RemainingDiscards;
    public int RemainingCards => State.RemainingCards;
    public int RemainingCardsToDraw => State.RemainingCardsToDraw;

    private readonly Random _drawCardsRandom;
    private readonly Random _recoverCardsRandom;

    public GameHandler(GameRunState gameRunState, PokerGameConfig config, PokerHandsManager pokerHandsManager, int level, int seed) {
        GameRunState = gameRunState;
        Config = config;
        PokerHandsManager = pokerHandsManager;
        State = new GameState(config, level, seed);
        State.BuildPokerDeck(config.ValidSuits, config.MinRank, config.MaxRank);

        _drawCardsRandom = new Random(seed * 100 + level);
        _recoverCardsRandom = new Random(seed * 100 + level);
    }

    public void DrawCards() {
        DrawCards(RemainingCardsToDraw);
    }

    public void DrawCards(int n) {
        if (IsWon()) {
            throw new SolitairePokerGameException("DrawCards error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("DrawCards errors: game is over");
        }
        if (!IsDrawPending()) {
            throw new SolitairePokerGameException("DrawCards error: hand already full");
        }
        if (n <= 0) {
            throw new SolitairePokerGameException("DrawCards error: number of cards to draw must be greater than 0");
        }
        if (n > RemainingCards) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({n}) than available ({RemainingCards}). Recover ({n - RemainingCards}) cards first.");
        }
        if (n > RemainingCardsToDraw) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({n}) than remaining to fulfill the current hand ({RemainingCardsToDraw})");
        }

        var cards = _drawCardsRandom.Take(State.AvailableCards as IList<Card>, n).ToList();
        cards.ForEach(card => State.Draw(card));
    }

    public void DrawCards(IReadOnlyList<Card> cards) {
        if (IsWon()) {
            throw new SolitairePokerGameException("DrawCards error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("DrawCards errors: game is over");
        }
        if (!IsDrawPending()) {
            throw new SolitairePokerGameException("DrawCards error: hand already full");
        }
        var invalidCards = cards.Except(State.AvailableCards).ToList();
        if (invalidCards.Count > 0) {
            var invalidCardsString = string.Join(", ", invalidCards);
            throw new SolitairePokerGameException($"DrawCards error: cards to draw not found in available pile ({invalidCardsString})");
        }
        if (cards.Count > RemainingCards) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({cards.Count}) than available ({RemainingCards}). Recover ({cards.Count - RemainingCards}) cards first.");
        }
        if (cards.Count > RemainingCardsToDraw) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({cards.Count}) than remaining to fulfill the current hand ({RemainingCardsToDraw})");
        }
        cards.ForEach(card => State.Draw(card));
    }

    public PlayResult PlayHand(IReadOnlyList<Card> cards) {
        if (IsWon()) {
            throw new SolitairePokerGameException("PlayHand error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("PlayHand error: game is over");
        }
        if (IsDrawPending()) {
            throw new SolitairePokerGameException("PlayHand error: cannot play hands yet, draw cards first.");
        }
        var invalidCards = cards.Except(State.CurrentHand).ToList();
        if (invalidCards.Count > 0) {
            var invalidCardsString = string.Join(", ", invalidCards);
            throw new SolitairePokerGameException($"PlayHand error: hand to play contains cards not in current hand ({invalidCardsString})");
        }

        State.HandsPlayed++;
        State.CardsPlayed += cards.Count;

        var hand = PokerHandsManager.IdentifyBestHand(this, cards);
        var score = hand != null ? CalculateScore(hand) : 0;
        State.Score += score;
        cards.ForEach(card => State.Play(card));
        State.History.AddPlayAction(hand, cards, score, State.Score, State.LevelScore);
        GameRunState.AddPlayAction(hand, cards);
        return new PlayResult(hand, score);
    }

    public DiscardResult Discard(IReadOnlyList<Card> cards) {
        if (IsWon()) {
            throw new SolitairePokerGameException("Discard error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Discard error: game is over");
        }
        if (IsDrawPending()) {
            throw new SolitairePokerGameException("Discard error: cannot discard hands yet, draw cards first.");
        }
        if (!CanDiscard()) {
            throw new SolitairePokerGameException($"Discard error: no discards remaining {State.Discards} of {Config.MaxDiscards}");
        }
        if (cards.Count < 1 || cards.Count > Config.MaxDiscardCards) {
            throw new SolitairePokerGameException($"Discard error: discard between 1 and {Config.MaxDiscardCards} cards: {cards.Count}");
        }
        var invalidCards = cards.Except(State.CurrentHand).ToList();
        if (invalidCards.Count > 0) {
            var invalidCardsString = string.Join(", ", invalidCards);
            throw new SolitairePokerGameException($"Discard error: contains cards not in current hand ({invalidCardsString})");
        }

        State.Discards++;
        State.CardsDiscarded += cards.Count;

        cards.ForEach(card => State.Discard(card));
        State.History.AddDiscardAction(cards, State.Score, State.LevelScore);
        GameRunState.AddDiscardAction(cards);
        return new DiscardResult(cards);
    }

    public void Recover(IReadOnlyList<Card> cards) {
        if (IsWon()) {
            throw new SolitairePokerGameException("Recover error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Recover error: game is over");
        }

        foreach (var card in cards) {
            // Comprobar si la carta está en alguna de las dos pilas
            if (!State.PlayedCards.Contains(card) && !State.DiscardedCards.Contains(card)) {
                throw new SolitairePokerGameException($"Recover error: cards to recover not found in played or discarded piles ({card})");
            }
        }

        cards.ForEach(card => State.Recover(card));
    }

    public void Recover(int n) {
        if (IsWon()) {
            throw new SolitairePokerGameException("Recover error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Recover error: game is over");
        }
        if (n <= 0) {
            throw new SolitairePokerGameException("Recover error: number of cards to recover must be greater than 0");
        }

        var playedCount = State.PlayedCards.Count;
        var discardedCount = State.DiscardedCards.Count;
        var totalCards = playedCount + discardedCount;

        if (n > totalCards) {
            throw new SolitairePokerGameException($"Recover error: cannot recover more cards ({n}) than available in played and discarded piles ({totalCards})");
        }

        var indices = _recoverCardsRandom.Take(totalCards, n);
        foreach (var index in indices) {
            if (index < playedCount) {
                // El índice corresponde a una carta de PlayedCards
                State.Recover(State.PlayedCards[index]);
            } else {
                // El índice corresponde a una carta de DiscardedCards, asi que
                // restamos playedCount para obtener el índice correcto en DiscardedCards
                State.Recover(State.DiscardedCards[index - playedCount]);
            }
        }
    }

    public void Destroy(IReadOnlyList<Card> cards) {
        foreach (var card in cards) {
            if (!State.Destroy(card)) {
                throw new SolitairePokerGameException($"Destroy error: card not found in any pile ({card})");
            }
        }
    }

    public List<PokerHand> GetPossibleHands() {
        return PokerHandsManager.IdentifyAllHands(this, State.CurrentHand);
    }

    public PokerHand GetBestHand() {
        return PokerHandsManager.IdentifyBestHand(this, State.CurrentHand)!;
    }

    public DiscardOptionsResult GetDiscardOptions(int maxSimulations, float simulationPercentage) {
        return PokerHandsManager.GetDiscardOptions(this, State.CurrentHand, State.AvailableCards, Config.MaxDiscardCards, maxSimulations, simulationPercentage);
    }

    public long CalculateScore(PokerHand hand) {
        var config = PokerHandsManager.GetPokerHandConfig(hand.HandType);
        var level = GameRunState.GetPokerHandLevel(hand.HandType);

        var score = config.InitialScore + level * config.ScorePerLevel;
        var multiplier = config.InitialMultiplier + level * config.MultiplierPerLevel;

        return (score + hand.Cards.Sum(c => c.Rank)) * multiplier;
    }
}