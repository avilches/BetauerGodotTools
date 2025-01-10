using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class SolitairePokerGame {
    public class PlayResult(PokerHand hand, int score) {
        public int Score { get; } = score;
        public PokerHand Hand { get; } = hand;
    }

    public class DiscardResult(IReadOnlyList<Card> discardedCards) {
        public IReadOnlyList<Card> DiscardedCards { get; } = discardedCards;
    }

    public GameState State = new();
    public PokerHands Hands { get; } = new();
    public PokerGameConfig Config;
    public Random Random;

    public SolitairePokerGame(Random random, PokerGameConfig config) {
        Random = random;
        Config = config;
        State.BuildPokerDeck(config.ValidSuits, config.MinRank, config.MaxRank);
    }

    public void DrawCards() {
        if (!DrawPending()) {
            throw new SolitairePokerGameException("Cannot draw cards: hand already full");
        }
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Cannot draw cards: no more hands to play");
        }

        State.Shuffle(Random);
        var pendingCardsToDraw = Config.HandSize - State.CurrentHand.Count;
        var newCards = State.Draw(pendingCardsToDraw);
        State.CurrentHand = [..State.CurrentHand, ..newCards];
    }

    public bool IsGameOver() => State.HandsPlayed >= Config.MaxHands;
    public bool DrawPending() => !IsGameOver() && State.CurrentHand.Count < Config.HandSize;
    public bool CanDiscard() => !IsGameOver() && State.Discards < Config.MaxDiscards;

    public PlayResult PlayHand(PokerHand hand) {
        if (IsGameOver()) {
            throw new SolitairePokerGameException("Game is already over");
        }
        if (DrawPending()) {
            throw new SolitairePokerGameException("Cannot play hands yet. Draw cards first.");
        }
        if (!hand.Cards.All(c => State.CurrentHand.Contains(c))) {
            throw new SolitairePokerGameException("Invalid hand: contains cards not in current hand");
        }

        State.HandsPlayed++;

        var score = Hands.CalculateScore(hand);
        State.TotalScore += score;
        // Remove played cards from current hand first
        State.CurrentHand = State.CurrentHand.Where(c => !hand.Cards.Contains(c)).ToList();
        // Then add to played cards
        State.AddToPlayedCards(hand.Cards);
        State.History.AddPlayAction(hand, score);
        return new PlayResult(hand, score);
    }

    public DiscardResult Discard(IReadOnlyList<Card> cards) {
        if (IsGameOver()) {
            throw new SolitairePokerGameException("No discards remaining or game is over");
        }
        if (DrawPending()) {
            throw new SolitairePokerGameException("Cannot discard hands yet. Draw cards first.");
        }
        if (!CanDiscard()) {
            throw new SolitairePokerGameException("No discards remaining or game is over");
        }
        if (cards.Count < 1 || cards.Count > Config.MaxDiscardCards) {
            throw new SolitairePokerGameException($"Must discard between 1 and {Config.MaxDiscardCards} cards");
        }
        if (!cards.All(c => State.CurrentHand.Contains(c))) {
            throw new SolitairePokerGameException("Invalid discard: contains cards not in current hand");
        }

        State.Discards++;
        // Remove discarded cards from current hand first
        State.CurrentHand = State.CurrentHand.Where(c => !cards.Contains(c)).ToList();
        // Then add to discarded pile
        State.AddToDiscardedCards(cards);
        State.History.AddDiscardAction(cards);
        return new DiscardResult(cards);
    }

    public List<PokerHand> GetPossibleHands() {
        return Hands.IdentifyAllHands(State.CurrentHand);
    }

    public DiscardOptionsResult GetDiscardOptions(IReadOnlyList<Card> neverDiscard) {
        return Hands.GetDiscardOptions(State.CurrentHand, neverDiscard, State.AvailableCards, Config.MaxDiscardCards);
    }
}

public record DiscardOptionsResult(
    List<DiscardOption> DiscardOptions,
    TimeSpan ElapsedTime,
    int TotalSimulations,
    int TotalCombinations) {
    public IOrderedEnumerable<DiscardOption> GetBestDiscards(float risk) {
        return DiscardOptions.OrderByDescending(option => option.GetBestPotentialScore(risk));
    }
}