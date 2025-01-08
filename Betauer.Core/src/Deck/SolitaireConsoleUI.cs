using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class SolitaireConsoleUI(Random random) {
    public static void Main() {
        var ui = new SolitaireConsoleUI(new Random(1));
        ui.Play();
    }
    
    private readonly SolitairePokerGame _game = new(new PokerGameConfig());

    public void Play() {
        Console.WriteLine("Welcome to Solitaire Poker!");

        while (!_game.IsGameOver()) {
            DisplayGameState();
            var possibleHands = _game.GetPossibleHands();
            DisplayPossibleHands(possibleHands);
            ProcessUserInput(possibleHands);
        }

        DisplayEndGame();
    }

    private void DisplayGameState() {
        var state = _game.State;
        Console.Clear();
        Console.WriteLine($"=== Solitaire Poker - Hand {state.HandsPlayed + 1}/{_game.Config.MaxHands} ===");
        Console.WriteLine($"Total Score: {state.TotalScore}");
        Console.WriteLine($"Discards: {state.Discards}/{_game.Config.MaxDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(state.CurrentHand);
        Console.WriteLine("\nHistory:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
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

        Console.Write("By suit: ");
        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.Write($"{string.Join(" ", sortedCards)} | ");
        }
        Console.WriteLine();
        Console.Write("By rank: ");
        Console.WriteLine($"{string.Join(" ", cards.OrderByDescending(c => c.Rank))}");
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
            if (_game.CanDiscard())
                ProcessDiscard();
            else
                Console.WriteLine("No discards remaining!");
        } else if (int.TryParse(option, out int choice) && choice > 0 && choice <= possibleHands.Count) {
            var hand = possibleHands[choice - 1];
            ProcessHand(hand);
        } else {
            Console.WriteLine("Invalid option! (press any key to continue)");
            Console.ReadKey();
        }
    }

    private void ProcessHand(PokerHand hand) {
        var result = _game.PlayHand(hand);
        if (result.Success) {
            Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.score}");
        } else {
            Console.WriteLine(result.Message);
        }
        Console.WriteLine("(press any key to continue)");
        Console.ReadKey();
    }

    private void ProcessManualHand() {
        while (true) {
            var state = _game.State;
            Console.WriteLine("\nSelect cards to play (enter card positions, e.g., '1 3 5'):");
            Console.WriteLine("Current hand:");
            for (int i = 0; i < state.CurrentHand.Count; i++) {
                Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
            }
            Console.WriteLine("C: Cancel");

            var input = Console.ReadLine();
            if (input?.ToUpper() == "C") {
                break;
            }
            var selectedIndices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (selectedIndices == null || selectedIndices.Count == 0) {
                Console.WriteLine("Invalid selection!");
                continue;
            }

            var selectedCards = selectedIndices.Select(i => state.CurrentHand[i]).ToList();
            var possibleHands = _game.Hands.IdentifyAllHands(selectedCards);

            var hand = possibleHands[0];
            var result = _game.PlayHand(hand);
            if (!result.Success) {
                Console.WriteLine(result.Message);
            } else {
                Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.score}");
                Console.WriteLine("(press any key to continue)");
                Console.ReadKey();
                break;
            }
        }
    }

    private void ProcessDiscard() {
        while (true) {

            var state = _game.State;
            Console.WriteLine("\nEnter the indices of cards to discard (1-7, separated by spaces):");
            Console.WriteLine("Current hand:");
            for (int i = 0; i < state.CurrentHand.Count; i++) {
                Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
            }
            Console.WriteLine("C: Cancel");

            var input = Console.ReadLine();
            if (input?.ToUpper() == "C") {
                break;
            }

            var indices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (indices == null || indices.Count == 0 || indices.Count > _game.Config.MaxDiscardCards) {
                Console.WriteLine($"Error. Please select between 1 and {_game.Config.MaxDiscardCards} valid cards!");
                continue;
            }

            var cardsToDiscard = indices.Select(i => state.CurrentHand[i]).ToList();
            var result = _game.Discard(cardsToDiscard);
            if (result.Success) {
                Console.WriteLine("Discarded cards: " + string.Join(", ", cardsToDiscard));
                Console.WriteLine("(press any key to continue)");
                Console.ReadKey();
                break;
            } else {
                Console.WriteLine(result.Message);
            }
        }
    }

    private void DisplayEndGame() {
        var state = _game.State;
        Console.Clear();
        Console.WriteLine("=== Game Over ===");
        Console.WriteLine($"Final Score: {state.TotalScore}");
        Console.WriteLine("\nHands played:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }
}