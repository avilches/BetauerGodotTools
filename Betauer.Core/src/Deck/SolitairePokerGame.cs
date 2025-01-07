using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class GameState {
    private readonly PokerGameConfig config;
    private readonly PokerHandScoring scoring;
    
    public int HandsPlayed { get; }
    public int TotalScore { get; }
    public int RemainingDiscards { get; }
    public IReadOnlyList<Card> CurrentHand { get; }
    public GameHistory History { get; }

    public GameState(
        PokerGameConfig config,
        PokerHandScoring scoring,
        int handsPlayed = 0,
        int totalScore = 0,
        int remainingDiscards = -1,
        IReadOnlyList<Card>? currentHand = null,
        GameHistory? history = null
    ) {
        this.config = config;
        this.scoring = scoring;
        HandsPlayed = handsPlayed;
        TotalScore = totalScore;
        RemainingDiscards = remainingDiscards == -1 ? config.MaxDiscards : remainingDiscards;
        CurrentHand = currentHand ?? new List<Card>();
        History = history ?? new GameHistory();
    }

    public bool IsGameOver() => HandsPlayed >= config.MaxHands;

    public bool CanDiscard() => RemainingDiscards > 0 && !IsGameOver();

    public GameState Clone() {
        return new GameState(
            config.Clone(),
            scoring.Clone(),
            HandsPlayed,
            TotalScore,
            RemainingDiscards,
            new List<Card>(CurrentHand),
            History.Clone()
        );
    }
}

public class SolitairePokerGame {
    private GameState state;
    private Deck deck;
    public HandIdentifier HandIdentifier { get; }
    public PokerGameConfig Config;
    public PokerHandScoring Scoring;

    public SolitairePokerGame(PokerGameConfig? config = null, PokerHandScoring? scoring = null) {
        this.Config = config ?? new PokerGameConfig();
        this.Scoring = scoring ?? new PokerHandScoring();
        this.deck = new Deck();
        this.HandIdentifier = new HandIdentifier(this.Scoring);
        this.deck.Shuffle();
        this.state = new GameState(
            this.Config,
            this.Scoring,
            currentHand: this.deck.Draw(this.Config.HandSize)
        );
    }

    public GameState GetState() => state.Clone();

    public (bool Success, string? Message, GameState State) PlayHand(PokerHand hand) {
        if (state.IsGameOver()) {
            return (false, "Game is already over", state);
        }

        if (!hand.Cards.All(c => state.CurrentHand.Contains(c))) {
            return (false, "Invalid hand: contains cards not in current hand", state);
        }

        // Return played cards to deck
        deck.ReturnCards(state.CurrentHand);

        var score = Scoring.CalculateScore(hand);
        var newHand = state.HandsPlayed + 1 < Config.MaxHands 
            ? deck.Draw(Config.HandSize) 
            : new List<Card>();

        state = new GameState(
            Config,
            Scoring,
            handsPlayed: state.HandsPlayed + 1,
            totalScore: state.TotalScore + score,
            remainingDiscards: state.RemainingDiscards,
            currentHand: newHand,
            history: state.History
        );
        // Aquí pasamos el score calculado
        state.History.AddPlay(hand, score);

        return (true, null, state);
    }

    public (bool Success, string? Message, GameState State) Discard(IReadOnlyList<Card> cards) {
        if (state.IsGameOver()) {
            return (false, "No discards remaining or game is over", state);
        }

        if (state.RemainingDiscards <= 0) {
            return (false, "No discards remaining or game is over", state);
        }

        if (cards.Count < 1 || cards.Count > Config.MaxDiscardCards) {
            return (false, $"Must discard between 1 and {Config.MaxDiscardCards} cards", state);
        }

        if (!cards.All(c => state.CurrentHand.Contains(c))) {
            return (false, "Invalid discard: contains cards not in current hand", state);
        }

        // Return discarded cards to deck
        deck.ReturnCards(cards);

        // Remove discarded cards and draw new ones
        var remainingCards = state.CurrentHand
            .Where(c => !cards.Contains(c))
            .ToList();
        var newCards = deck.Draw(cards.Count);
        var newHand = remainingCards.Concat(newCards).ToList();

        state = new GameState(
            Config,
            Scoring,
            handsPlayed: state.HandsPlayed,
            totalScore: state.TotalScore,
            remainingDiscards: state.RemainingDiscards - 1,
            currentHand: newHand,
            history: state.History
        );
        state.History.AddDiscard(cards);

        return (true, null, state);
    }

    public List<PokerHand> GetPossibleHands() {
        return HandIdentifier.IdentifyAllHands(state.CurrentHand);
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

        if (indices == null || indices.Count == 0 || indices.Count > _game.Config.MaxDiscardCards) {
            Console.WriteLine($"Please select between 1 and {_game.Config.MaxDiscardCards} valid cards!");
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