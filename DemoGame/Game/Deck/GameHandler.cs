using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game.Deck;

public class GameHandler(GameRunState gameRunState, PokerGameConfig config, PokerHandsManager pokerHandsManager, int level, int seed) {
    public class PlayResult(PokerHand hand, long score) {
        public long Score { get; } = score;
        public PokerHand? Hand { get; } = hand;
    }

    public class DiscardResult(IReadOnlyList<Card> discardedCards) {
        public IReadOnlyList<Card> DiscardedCards { get; } = discardedCards;
    }

    public PokerHandsManager PokerHandsManager { get; } = pokerHandsManager;
    public PokerGameConfig Config { get; } = config;
    public GameState State { get; } = new(config, level, seed);
    public GameRunState GameRunState { get; } = gameRunState;

    public bool IsWon() => State.IsWon();
    public bool IsGameOver() => State.IsGameOver();
    public bool IsDrawPending() => State.IsDrawPending();
    public bool CanDiscard() => State.CanDiscard();

    public long RemainingScoreToWin => State.RemainingScoreToWin;
    public int RemainingHands => State.RemainingHands;
    public int RemainingDiscards => State.RemainingDiscards;
    public int RemainingCards => State.RemainingCards;
    public int RealCardsToDraw => State.RealCardsToDraw;
    public int CardsToDraw => State.CardsToDraw;

    private readonly Random _drawCardsRandom = new(seed * 100 + level);
    private readonly Random _recoverCardsRandom = new(seed * 100 + level);

    public void DrawCards() {
        DrawCards(CardsToDraw); // First consume all the cards from the available pile
        if (config.AutoRecover && RealCardsToDraw > 0) {
            // Then add them all back to the available pile
            RecoverAll();
            DrawCards(CardsToDraw);
        }
    }

    public void AddCards(IEnumerable<Card> cards) {
        cards.ToArray().ForEach(card => State.AddCard(card));
    }

    public void DrawCards(int n) {
        if (IsWon()) {
            throw new SolitairePokerGameException("DrawCards error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("DrawCards errors: game is over");
        }
        if (n == 0) {
            return;
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
        if (n > CardsToDraw) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({n}) than remaining to fulfill the current hand ({CardsToDraw})");
        }

        var cards = _drawCardsRandom.Take(State.AvailableCards as IList<Card>, n).ToList();
        cards.ToArray().ForEach(card => State.Draw(card));
    }

    public void DrawCards(IReadOnlyList<Card> cards) {
        if (cards.Count == 0) {
            throw new SolitairePokerGameException("DrawCards error: cards cannot be empty");
        }
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
        if (cards.Count > CardsToDraw) {
            throw new SolitairePokerGameException($"DrawCards error: cannot draw more cards ({cards.Count}) than remaining to fulfill the current hand ({CardsToDraw})");
        }
        cards.ToArray().ForEach(card => State.Draw(card));
    }

    public PlayResult PlayHand(IReadOnlyList<Card> cards) {
        if (cards.Count == 0) {
            throw new SolitairePokerGameException("PlayHand error: cards cannot be empty");
        }
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
        cards.ToArray().ForEach(card => State.Play(card));
        State.History.AddPlayAction(hand, cards, score, State.Score, State.LevelScore);
        GameRunState.AddPlayAction(hand, cards);
        return new PlayResult(hand, score);
    }

    public DiscardResult Discard(IReadOnlyList<Card> cards) {
        if (cards.Count == 0) {
            throw new SolitairePokerGameException("Discard error: cards cannot be empty");
        }
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
        if (cards.Count < 1 || cards.Count > Config.MaxCardsToDiscard) {
            throw new SolitairePokerGameException($"Discard error: discard between 1 and {Config.MaxCardsToDiscard} cards: {cards.Count}");
        }
        var invalidCards = cards.Except(State.CurrentHand).ToList();
        if (invalidCards.Count > 0) {
            var invalidCardsString = string.Join(", ", invalidCards);
            throw new SolitairePokerGameException($"Discard error: contains cards not in current hand ({invalidCardsString})");
        }

        State.Discards++;
        State.CardsDiscarded += cards.Count;

        cards.ToArray().ForEach(card => State.Discard(card));
        State.History.AddDiscardAction(cards, State.Score, State.LevelScore);
        GameRunState.AddDiscardAction(cards);
        return new DiscardResult(cards);
    }

    public void Recover(IReadOnlyList<Card> cards) {
        if (cards.Count == 0) {
            throw new SolitairePokerGameException("Recover error: cards cannot be empty");
        }
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

        cards.ToArray().ForEach(card => State.Recover(card));
    }

    public void RecoverAll() {
        if (IsWon()) {
            throw new SolitairePokerGameException("Recover error: game won");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Recover error: game is over");
        }
        State.DiscardedCards.ToArray().ForEach(card => State.Recover(card));
        State.PlayedCards.ToArray().ForEach(card => State.Recover(card));
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
        var cardsToRecover = new List<Card>();
        foreach (var index in indices) {
            if (index < playedCount) {
                // El índice corresponde a una carta de PlayedCards
                cardsToRecover.Add(State.PlayedCards[index]);
            } else {
                // El índice corresponde a una carta de DiscardedCards, asi que
                // restamos playedCount para obtener el índice correcto en DiscardedCards
                cardsToRecover.Add(State.DiscardedCards[index - playedCount]);
            }
        }
        cardsToRecover.ForEach(card => State.Recover(card));
    }

    public void Destroy(IReadOnlyList<Card> cards) {
        if (cards.Count == 0) {
            throw new SolitairePokerGameException("Destroy error: cards cannot be empty");
        }
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
        return PokerHandsManager.GetDiscardOptions(this, State.CurrentHand, State.AvailableCards, Config.MaxCardsToDiscard, maxSimulations, simulationPercentage);
    }

    public long CalculateScore(PokerHand hand) {
        var config = PokerHandsManager.GetPokerHandConfig(hand.HandType);
        var level = GameRunState.GetPokerHandLevel(hand.HandType);

        var score = config.InitialScore + level * config.ScorePerLevel;
        var multiplier = config.InitialMultiplier + level * config.MultiplierPerLevel;

        return (score + hand.Cards.Sum(c => c.Rank)) * multiplier;
    }
}