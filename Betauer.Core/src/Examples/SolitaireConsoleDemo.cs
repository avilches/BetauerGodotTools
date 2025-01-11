using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Examples;

public class SolitaireConsoleDemo {
    public readonly Random Random;

    public readonly int[] BaseLevel = [
        300,
        800,
        2000,
        5000,
        11000,
        20000,
        35000,
        50000,
        110000,
        560000,
        7200000,
        300000000
        // 47000000000
    ];
    
    public int MaxLevel => (BaseLevel.Length * 3) - 1; // level starts in 0

    private int GetScoreFromLevel(int level) {
        var baseScoreIndex = level / 3;  // Cada 3 niveles cambiamos de score base
        var multiplierIndex = level % 3;  // 0 = x1, 1 = x1.5, 2 = x2
                      
        if (baseScoreIndex >= BaseLevel.Length) {
            throw new ArgumentException($"Level {level} too high. Max level is {MaxLevel}");
        }
    
        var baseScore = BaseLevel[baseScoreIndex];
        var multiplier = multiplierIndex switch {
            0 => 1.0f,
            1 => 1.5f,
            2 => 2.0f,
            _ => throw new ArgumentException($"Invalid multiplier index {multiplierIndex}")
        };
    
        return (int)(baseScore * multiplier);
    }


    public readonly bool _autoPlay;
    public readonly List<GameRun> GameRuns = new();
    public GameRun CurrentRun;
    public SolitairePokerGame Game;
    private readonly AutoPlayer autoPlayer = new AutoPlayer();
    private AutoPlayer.AutoPlayDecision? currentDecision;

    public SolitaireConsoleDemo(bool autoPlay = false) {
        _autoPlay = autoPlay;
    }

    public static void Main() {
        Console.WriteLine("Auto-play mode? (Y/N)");
        var autoPlay = Console.ReadLine()?.ToUpper() == "Y";
        new SolitaireConsoleDemo(autoPlay).Play();
    }

    public void Play() {
        InitializeCurrentRun();

        while (Game.State.Level < MaxLevel) {
            while (!Game.IsGameOver()) {
                currentDecision = autoPlayer.GetNextAction(Game);
                DisplayGameState();

                if (_autoPlay) {
                    ProcessAutoPlay();
                    // System.Threading.Thread.Sleep(1000);
                } else {
                    if (ProcessUserInput() == true) {
                        return; // El usuario quiere salir
                    }
                }
            }

            // Guardamos el estado del juego actual
            CurrentRun.AddGameState(Game.State);

            if (Game.IsWon()) {
                DisplayWinScreen();

                if (Game.State.Level + 1 < MaxLevel) {
                    // Preparamos el siguiente nivel
                    InitializeGame(Game.State.Level + 1);

                    Console.WriteLine($"\nAdvancing to level {Game.State.Level + 1}! New target: {Game.State.TotalScore}");
                    if (!_autoPlay) {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
            } else {
                // El juego terminó sin alcanzar la puntuación
                DisplayGameOverScreen();
                if (!_autoPlay && !PlayAgain()) {
                    return;
                }
                InitializeCurrentRun();
            }
        }

        DisplayFinalVictoryScreen();
    }

    private void InitializeCurrentRun() {
        CurrentRun = new GameRun();
        GameRuns.Add(CurrentRun);
        InitializeGame(0);
    }

    private void InitializeGame(int level) {
        Game = new SolitairePokerGame(Random, new PokerGameConfig());
        Game.Hands.RegisterBasicPokerHands();
        Game.DrawCards();
        Game.State.Level = level;
        Game.State.TotalScore = GetScoreFromLevel(level);
    }

    private void DisplayGameState() {
        var state = Game.State;
        Console.Clear();

        // Display runs history
        if (GameRuns.Count > 0) {
            Console.WriteLine("=== Previous Runs ===");
            // foreach (var run in gameRuns.OrderByDescending(r => r.StartTime)) {
            foreach (var run in GameRuns) {
                Console.WriteLine(run);
                foreach (var gameState in run.GameStates) {
                    Console.Write($"  Level {gameState.Level + 1}:");
                    foreach (var action in gameState.History.GetHistory()) {
                        if (action.Type == GameHistory.GameActionType.Play)
                            Console.Write($"Play #{action.Id + 1}: {action.PlayedHand} (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
                        else
                            Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        Console.WriteLine($"=== Solitaire Poker - Level {state.Level + 1} ===");
        Console.WriteLine($"Score: {state.Score}/{state.TotalScore}");
        Console.WriteLine($"Hand {state.HandsPlayed + 1}/{Game.Config.MaxHands}");
        Console.WriteLine($"Discards: {state.Discards}/{Game.Config.MaxDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(state.CurrentHand);
        Console.WriteLine("\nHistory: ");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
                Console.Write($"Play #{action.Id + 1}: {action.PlayedHand} (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
            else
                Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
        }
        Console.WriteLine();
    }

    private void DisplayWinScreen() {
        Console.Clear();
        Console.WriteLine("=== WINNER! ===");
        Console.WriteLine($"Congratulations! You've reached {Game.State.Score} points!");
        Console.WriteLine($"Target was: {Game.State.TotalScore}");
    }

    private void DisplayGameOverScreen() {
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        Console.WriteLine($"Final Score: {Game.State.Score}");
        Console.WriteLine($"Target Score: {Game.State.TotalScore}");
        Console.WriteLine($"You needed {Game.State.TotalScore - Game.State.Score} more points to win");
    }

    private void DisplayFinalVictoryScreen() {
        Console.Clear();
        Console.WriteLine("=== CONGRATULATIONS! ===");
        Console.WriteLine("You've completed all levels!");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private bool PlayAgain() {
        if (_autoPlay) return true;
        Console.WriteLine("\nPlay again? (Y/N)");
        return Console.ReadLine()?.ToUpper() == "Y";
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

    private void DisplayPotentialHands() {
        Console.WriteLine("\nPossible hands you can play:");
        int i = 0;
        for (; i < currentDecision.PossibleHands.Count; i++) {
            Console.WriteLine($"{i + 1}: {currentDecision.PossibleHands[i]}: " + currentDecision.PossibleHands[i].CalculateScore());
        }
        var currentBestHand = currentDecision.PossibleHands[0];
        var options = currentDecision.DiscardOptions.GetBestDiscards(Risk);
        foreach (var option in options.Where(option => option.GetBestPotentialScore(Risk) > Risk)) {
            i++;
            var discardBestHand = option.GetBestHand(Risk)!;
            Console.WriteLine($"{i}. Discarding {option.CardsToDiscard.Count}: {string.Join(" ", option.CardsToDiscard)}, keeping: {string.Join(" ", option.CardsToKeep)}  Score: {discardBestHand.PotentialScore:F2}");
            var handsByScore = option.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.GetType() ? "redundant" : "")}");
            }
        }
        foreach (var option in options.Where(option => option.GetBestPotentialScore(Risk) <= Risk)) {
            i++;
            var discardBestHand = option.GetBestHand(0)!;
            Console.WriteLine($"{i}. Discarding {option.CardsToDiscard.Count}: {string.Join(" ", option.CardsToDiscard)}, keeping: {string.Join(" ", option.CardsToKeep)}  Score: {discardBestHand.PotentialScore:F2}");
            var handsByScore = option.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.GetType() ? "redundant" : "")}");
            }
        }
        Console.WriteLine($"Analysis time: {currentDecision.DiscardOptions.ElapsedTime.TotalSeconds:F3} seconds");
        Console.WriteLine($"Total simulations: {currentDecision.DiscardOptions.TotalSimulations:N0}/{currentDecision.DiscardOptions.TotalCombinations:N0} ({(float)currentDecision.DiscardOptions.TotalSimulations / currentDecision.DiscardOptions.TotalCombinations:0%})");
    }

    private bool ProcessUserInput() {
        currentDecision = autoPlayer.GetNextAction(Game);
        DisplayPotentialHands();

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1-N: Play hand/discard from the list");
        Console.WriteLine("M: Make your own hand by selecting cards");
        Console.WriteLine("D: Discard cards");
        Console.WriteLine("A: Auto play: " + GetAutoPlay());
        Console.WriteLine("Q: Quit game");

        var option = Console.ReadLine()?.ToUpper();

        if (option == "Q") {
            return true;
        }

        if (option == "A") {
            ProcessAutoPlay();
        } else if (option == "M") {
            ProcessManualHand();
        } else if (option == "D") {
            if (Game.CanDiscard())
                ProcessManualDiscard();
            else
                Console.WriteLine("No discards remaining!");
        } else if (int.TryParse(option, out int choice) && choice > 0) {
            if (choice <= currentDecision.PossibleHands.Count) {
                var hand = currentDecision.PossibleHands[choice - 1];
                ProcessHand(hand);
            } else {
                choice -= currentDecision.PossibleHands.Count;
                var cardsToDiscard = currentDecision.DiscardOptions.DiscardOptions[choice - 1].CardsToDiscard;
                ProcessDiscard(cardsToDiscard);
            }
        } else {
            Console.WriteLine("Invalid option! (press any key to continue)");
            Console.ReadKey();
        }
        return false;
    }

    private string GetAutoPlay() {
        if (currentDecision == null) throw new InvalidOperationException("currentDecision is null");
        var stats = currentDecision.DiscardOption?.GetBestHand(0);
        return $"{currentDecision.Reason} | " + (currentDecision.ShouldPlay
            ? $"Play {currentDecision.HandToPlay!}"
            : $"Score {stats.AvgScore:0} x {stats.Probability:0.0%} = {stats.PotentialScore:0.0}");
    }

    private void ProcessAutoPlay() {
        if (currentDecision == null) throw new InvalidOperationException("currentDecision is null");
        if (currentDecision.ShouldPlay) {
            ProcessHand(currentDecision.HandToPlay!);
        } else {
            ProcessDiscard(currentDecision.DiscardOption!.CardsToDiscard);
        }
    }

    private void ProcessManualHand() {
        while (true) {
            var state = Game.State;
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
            var possibleHands = Game.Hands.IdentifyAllHands(selectedCards);

            if (possibleHands.Count == 0) {
                Console.WriteLine("No valid poker hand can be formed with these cards!");
                continue;
            }

            var hand = possibleHands[0];
            ProcessHand(hand);
            break;
        }
    }

    private void ProcessHand(PokerHand hand) {
        var result = Game.PlayHand(hand);

        Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.Score} ({Game.State.Score}/{Game.State.TotalScore})");

        if (Game.DrawPending()) {
            Game.DrawCards();
            Console.WriteLine($"* New hand: {string.Join(", ", Game.State.CurrentHand)}");
        }
        if (!_autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private void ProcessManualDiscard() {
        while (true) {
            var state = Game.State;
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

            if (indices == null || indices.Count == 0 || indices.Count > Game.Config.MaxDiscardCards) {
                Console.WriteLine($"Error. Please select between 1 and {Game.Config.MaxDiscardCards} valid cards!");
                continue;
            }

            var cardsToDiscard = indices.Select(i => state.CurrentHand[i]).ToList();
            ProcessDiscard(cardsToDiscard);
            break;
        }
    }

    private void ProcessDiscard(List<Card> cardsToDiscard) {
        Console.WriteLine($"Discarded: {string.Join(", ", cardsToDiscard)}");
        var result = Game.Discard(cardsToDiscard);
        Game.DrawCards();
        Console.WriteLine($"* Cards discarded. Remaining discards: {Game.Config.MaxDiscards - Game.State.Discards}");
        Console.WriteLine($"* New hand: {string.Join(", ", Game.State.CurrentHand)}");
        if (!_autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private const float Risk = 0.7f;
}