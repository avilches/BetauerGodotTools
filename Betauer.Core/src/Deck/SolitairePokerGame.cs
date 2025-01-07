using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class GameState {
    public readonly Deck Deck;
    public readonly IReadOnlyList<Card> CurrentHand;
    public readonly int RemainingDiscards;
    public readonly int TotalScore;
    public readonly int HandsPlayed;
    public readonly GameHistory History;

    // Constants moved from SolitairePokerGame to be accessible anywhere
    public const int MAX_DISCARDS = 4;
    public const int MAX_DISCARD_CARDS = 5;
    public const int HAND_SIZE = 7;
    public const int MAX_HANDS = 4;

    // Constructor for initial state
    public GameState() {
        Deck = new Deck();
        Deck.Shuffle();
        CurrentHand = Deck.Draw(HAND_SIZE);
        RemainingDiscards = MAX_DISCARDS;
        TotalScore = 0;
        HandsPlayed = 0;
        History = new GameHistory();
    }

    // Private constructor for creating new states
    private GameState(Deck deck, IReadOnlyList<Card> currentHand, int remainingDiscards, int totalScore, int handsPlayed, GameHistory history) {
        Deck = deck;
        CurrentHand = currentHand;
        RemainingDiscards = remainingDiscards;
        TotalScore = totalScore;
        HandsPlayed = handsPlayed;
        History = history;
    }

    // Creates a new state with updated values
    public GameState With(
        Deck? deck = null,
        IReadOnlyList<Card>? currentHand = null,
        int? remainingDiscards = null,
        int? totalScore = null,
        int? handsPlayed = null,
        GameHistory? history = null
    ) {
        return new GameState(
            deck ?? Deck,
            currentHand ?? CurrentHand,
            remainingDiscards ?? RemainingDiscards,
            totalScore ?? TotalScore,
            handsPlayed ?? HandsPlayed,
            history ?? History
        );
    }

    public bool IsGameOver() => HandsPlayed >= MAX_HANDS;

    public bool CanDiscard() => RemainingDiscards > 0 && !IsGameOver();
}

// Action result class to handle the result of game actions
public class ActionResult {
    public bool Success { get; }
    public string Message { get; }
    public GameState State { get; }

    private ActionResult(bool success, string message, GameState state) {
        Success = success;
        Message = message;
        State = state;
    }

    public static ActionResult Ok(GameState state, string message = "") => 
        new ActionResult(true, message, state);

    public static ActionResult Error(GameState state, string message) => 
        new ActionResult(false, message, state);
}

public class SolitairePokerGame {
    public readonly HandIdentifier HandIdentifier;
    private GameState state;

    public SolitairePokerGame() {
        HandIdentifier = new HandIdentifier();
        state = new GameState();
    }

    // Constructor for testing or loading saved games
    public SolitairePokerGame(GameState initialState) {
        HandIdentifier = new HandIdentifier();
        state = initialState;
    }

    public GameState GetState() => state;

    public List<PokerHand> GetPossibleHands() => 
        HandIdentifier.IdentifyAllHands(state.CurrentHand);

    public ActionResult PlayHand(PokerHand selectedHand) {
        if (state.IsGameOver()) {
            return ActionResult.Error(state, "Game is already over");
        }

        if (!state.CurrentHand.ContainsAll(selectedHand.Cards)) {
            return ActionResult.Error(state, "Selected hand contains invalid cards");
        }

        var remainingCards = state.CurrentHand
            .Where(c => !selectedHand.Cards.Contains(c))
            .ToList();

        var newDeck = state.Deck.Clone();
        IReadOnlyList<Card> newHand;

        if (state.HandsPlayed + 1 < GameState.MAX_HANDS) {
            var newCards = newDeck.Draw(GameState.HAND_SIZE - remainingCards.Count);
            newHand = remainingCards.Concat(newCards).ToList();
        } else {
            newHand = new List<Card>();
        }

        var newHistory = state.History.Clone();
        newHistory.AddPlay(selectedHand);

        state = state.With(
            deck: newDeck,
            currentHand: newHand,
            totalScore: state.TotalScore + selectedHand.Score,
            handsPlayed: state.HandsPlayed + 1,
            history: newHistory
        );

        return ActionResult.Ok(state);
    }

    public ActionResult Discard(IReadOnlyList<Card> cardsToDiscard) {
        if (!state.CanDiscard()) {
            return ActionResult.Error(state, "No discards remaining or game is over");
        }

        if (cardsToDiscard.Count == 0 || cardsToDiscard.Count > GameState.MAX_DISCARD_CARDS) {
            return ActionResult.Error(state, 
                $"Must discard between 1 and {GameState.MAX_DISCARD_CARDS} cards");
        }

        if (!state.CurrentHand.ContainsAll(cardsToDiscard)) {
            return ActionResult.Error(state, "Trying to discard cards not in hand");
        }

        var remainingCards = state.CurrentHand
            .Except(cardsToDiscard)
            .ToList();

        var newDeck = state.Deck.Clone();
        newDeck.ReturnCards(cardsToDiscard);
        newDeck.Shuffle();
        
        var newCards = newDeck.Draw(cardsToDiscard.Count);
        var newHand = remainingCards.Concat(newCards).ToList();

        var newHistory = state.History.Clone();
        newHistory.AddDiscard(cardsToDiscard);

        state = state.With(
            deck: newDeck,
            currentHand: newHand,
            remainingDiscards: state.RemainingDiscards - 1,
            history: newHistory
        );

        return ActionResult.Ok(state);
    }
}

// Extension method helper
public static class ListExtensions {
    public static bool ContainsAll<T>(this IReadOnlyList<T> list, IReadOnlyList<T> items) {
        return items.All(list.Contains);
    }
}

public class SolitaireConsoleUI {
    private readonly SolitairePokerGame _game = new();

    public void Play() {
        Console.WriteLine("Welcome to Solitaire Poker!");

        while (!_game.GetState().IsGameOver()) {
            DisplayGameState();
            var possibleHands = _game.GetPossibleHands();
            DisplayPossibleHands(possibleHands);
            ProcessUserInput(possibleHands);
        }

        DisplayEndGame();
    }

    private void DisplayGameState() {
        var state = _game.GetState();
        Console.Clear();
        Console.WriteLine($"=== Solitaire Poker - Hand {state.HandsPlayed + 1}/4 ===");
        Console.WriteLine($"Total Score: {state.TotalScore}");
        Console.WriteLine($"Remaining Discards: {state.RemainingDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(state.CurrentHand);
        Console.WriteLine("\nHistory:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == "PLAY")
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }

    private void DisplayCards(IReadOnlyList<Card> cards) {
        var groupedCards = cards
            .GroupBy(c => c.Suit)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.WriteLine($"{group.Key}: {string.Join(" ", sortedCards)}");
        }
    }

    private void DisplayPossibleHands(List<PokerHand> hands) {
        Console.WriteLine("\nPossible hands you can play:");
        for (int i = 0; i < hands.Count; i++) {
            Console.WriteLine($"{i + 1}: {hands[i]}");
        }
    }

    private void ProcessUserInput(List<PokerHand> possibleHands) {
        Console.WriteLine("\nOptions:");
        Console.WriteLine("1-N: Play a hand from the list");
        Console.WriteLine("M: Make your own hand by selecting cards");
        Console.WriteLine("D: Discard cards");
        Console.WriteLine("Q: Quit game");

        var option = Console.ReadLine()?.ToUpper();

        if (option == "Q") {
            DisplayEndGame();
            Environment.Exit(0);
        }

        if (option == "M") {
            ProcessManualHand();
        } else if (option == "D") {
            if (_game.GetState().CanDiscard())
                ProcessDiscard();
            else
                Console.WriteLine("No discards remaining!");
        } else if (int.TryParse(option, out int choice) && choice > 0 && choice <= possibleHands.Count) {
            var result = _game.PlayHand(possibleHands[choice - 1]);
            if (!result.Success) {
                Console.WriteLine(result.Message);
            }
        } else {
            Console.WriteLine("Invalid option!");
        }
    }

    private void ProcessManualHand() {
        var state = _game.GetState();
        Console.WriteLine("\nSelect cards to play (enter card positions, e.g., '1 3 5'):");
        Console.WriteLine("Current hand:");
        for (int i = 0; i < state.CurrentHand.Count; i++) {
            Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
        }

        var input = Console.ReadLine();
        var selectedIndices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
            .Where(n => n >= 0 && n < state.CurrentHand.Count)
            .ToList();

        if (selectedIndices == null || selectedIndices.Count == 0) {
            Console.WriteLine("Invalid selection!");
            return;
        }

        var selectedCards = selectedIndices.Select(i => state.CurrentHand[i]).ToList();
        var possibleHands = _game.HandIdentifier.IdentifyAllHands(selectedCards);

        if (possibleHands.Count == 0) {
            Console.WriteLine("These cards don't form a valid poker hand!");
            return;
        }

        var result = _game.PlayHand(possibleHands[0]);
        if (!result.Success) {
            Console.WriteLine(result.Message);
        }
    }

    private void ProcessDiscard() {
        var state = _game.GetState();
        Console.WriteLine("\nEnter the indices of cards to discard (1-7, separated by spaces):");
        Console.WriteLine("Current hand:");
        for (int i = 0; i < state.CurrentHand.Count; i++) {
            Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
        }

        var input = Console.ReadLine();
        var indices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
            .Where(n => n >= 0 && n < state.CurrentHand.Count)
            .ToList();

        if (indices == null || indices.Count == 0 || indices.Count > GameState.MAX_DISCARD_CARDS) {
            Console.WriteLine($"Please select between 1 and {GameState.MAX_DISCARD_CARDS} valid cards!");
            return;
        }

        var cardsToDiscard = indices.Select(i => state.CurrentHand[i]).ToList();
        var result = _game.Discard(cardsToDiscard);
        if (!result.Success) {
            Console.WriteLine(result.Message);
        }
    }

    private void DisplayEndGame() {
        var state = _game.GetState();
        Console.Clear();
        Console.WriteLine("=== Game Over ===");
        Console.WriteLine($"Final Score: {state.TotalScore}");
        Console.WriteLine("\nHands played:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == "PLAY")
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }
}

public class Program {
    public static void Main() {
        var ui = new SolitaireConsoleUI();
        ui.Play();
    }
}