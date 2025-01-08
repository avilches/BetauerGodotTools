using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class SolitairePokerGame {
    public GameState State = new();
    public PokerHands Hands { get; } = new();
    public PokerGameConfig Config;

    public SolitairePokerGame(PokerGameConfig config) {
        Config = config;

        State.BuildPokerDeck(config.ValidSuits, config.MinRank, config.MaxRank);
    }

    public void DrawCards() {
        if (!DrawPending()) {
            throw new InvalidOperationException("Cannot draw cards with current hand");
        }
        if (IsGameOver()) {
            throw new InvalidOperationException("Cannot draw cards: no more hands to play");
        }

        State.CurrentHand = State.Draw(Math.Min(Config.HandSize, State.RemainingCards));
    }

    public bool IsGameOver() => State.HandsPlayed >= Config.MaxHands;
    public bool DrawPending() => !IsGameOver() && State.CurrentHand.Count == 0;
    public bool CanDiscard() => !IsGameOver() && State.Discards < Config.MaxDiscards;


    public (bool Success, string? Message, int score) PlayHand(PokerHand hand) {
        if (IsGameOver()) {
            return (false, "Game is already over", 0);
        }
        if (DrawPending()) {
            return (false, "Cannot play hands yet. Draw cards first.", 0);
        }
        if (!hand.Cards.All(c => State.CurrentHand.Contains(c))) {
            return (false, "Invalid hand: contains cards not in current hand", 0);
        }

        State.HandsPlayed++;

        var score = Hands.CalculateScore(hand);
        State.TotalScore += score;

        State.CurrentHand = [];

        State.PlayCards(hand.Cards);
        State.History.AddPlay(hand, score);
        return (true, null, score);
    }

    public (bool Success, string? Message) Discard(IReadOnlyList<Card> cards) {
        if (IsGameOver()) {
            return (false, "No discards remaining or game is over");
        }
        if (DrawPending()) {
            return (false, "Cannot discard hands yet. Draw cards first.");
        }
        if (!CanDiscard()) {
            return (false, "No discards remaining or game is over");
        }
        if (cards.Count < 1 || cards.Count > Config.MaxDiscardCards) {
            return (false, $"Must discard between 1 and {Config.MaxDiscardCards} cards");
        }

        if (!cards.All(c => State.CurrentHand.Contains(c))) {
            return (false, "Invalid discard: contains cards not in current hand");
        }

        State.Discards++;

        State.CurrentHand = [];

        State.DiscardCards(cards);
        State.History.AddDiscard(cards);
        return (true, null);
    }

    public List<PokerHand> GetPossibleHands() {
        return Hands.IdentifyAllHands(State.CurrentHand);
    }
}