using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game;

public class SolitaireMultiRunner {
    public readonly List<SolitaireConsoleDemo> Demos = [];

    // Run values
    private const int Runs = 20;

    // Simulation values
    private const int MaxSimulations = 100;
    private const float SimulationPercentage = 1f;

    // UI
    private const int ShowLastRuns = 10;
    private const int PercentBarSize = 40;

    public static void Main() {
        new SolitaireMultiRunner().Run(Runs);
    }

    public void Run(int iterations) {
        for (var i = 0; i < iterations; i++) {
            var demo = new SolitaireConsoleDemo(true, MaxSimulations, SimulationPercentage);
            Demos.Add(demo);
            MiniGame(demo, i);
        }
        DisplayGameState(true);
    }

    private void Complete(SolitaireConsoleDemo demo, int i) {
        demo.Run(i,
            handler => {
                var cards = DeckBuilder.ClassicPokerDeck();
                cards.ForEach(handler.State.AddCard);
                handler.Config.HandSize = 8;
                handler.PokerHandsManager.Config.FlushSize = 5;
                handler.PokerHandsManager.Config.StraightSize = 5;
                handler.PokerHandsManager.Config.MinPotentialFlushSize = 3;
                handler.PokerHandsManager.Config.MinPotentialStraightSize = 3;
            },
            () => {
                DisplayGameState();
            }, () => {
                // demo.IncreaseRandomPokerHandLevel();
                // demo.IncreaseRandomPokerHandLevel();
            });
    }

    private void MiniGame(SolitaireConsoleDemo demo, int i) {
        demo.Run(i,
            handler => {
                Card[] cards = [
                    ..DeckBuilder.Cards('S', 2, 9),
                    // ..DeckBuilder.Cards('C', 2, 9),
                    // ..DeckBuilder.Cards('D', 2, 9),
                    // ..DeckBuilder.Cards('H', 2, 9),
                ];
                // var cards = DeckBuilder.ClassicPokerDeck();
                cards.ForEach(handler.State.AddCard);
                handler.Config.HandSize = 6;
                handler.PokerHandsManager.Config.FlushSize = 4;
                handler.PokerHandsManager.Config.StraightSize = 4;
                handler.PokerHandsManager.Config.MinPotentialFlushSize = 2;
                handler.PokerHandsManager.Config.MinPotentialStraightSize = 2;
            },
            () => {
                DisplayGameState();
            }, () => {
                // demo.IncreaseRandomPokerHandLevel();
                // demo.IncreaseRandomPokerHandLevel();
            });
    }

    private void DisplayGameState(bool final = false) {
        var runs = Demos
            .Where(demo => demo.GameRun.RunIsGameOver())
            .Select(run => run.GameRun).ToList();

        Console.Clear();
        if (!final) {
            Console.WriteLine("=== Now playing ===");
            Console.WriteLine(Demos.Last().GameRun);
            Console.WriteLine(string.Join(", ", Demos.Last().GameRun.GameStates.Last().History.GetHistory()));
            Console.WriteLine($"=== {Runs} Runs ===");
            var runsToDisplay = runs.Count > ShowLastRuns ? runs.Skip(runs.Count - ShowLastRuns) : runs;
            var startIndex = (runs.Count > ShowLastRuns ? runs.Count - ShowLastRuns : 0) + 1;
            foreach (var (demo, index) in runsToDisplay.Select((demo, i) => (demo, startIndex + i))) {
                Console.WriteLine($"{index}/{Runs}: {demo}");
            }
        }
        Console.WriteLine($"=== Runs: {Runs} ===");
        // Console.WriteLine($"- Simulating {SimulationPercentage:0%} (capped to {MaxSimulations})");

        if (runs.Count > 0) {
            DisplayWinLostStats(runs);
            DisplayRunDistribution(runs);
            DisplayMostPlayedHands(runs);
        }
    }

    private static void DisplayMostPlayedHands(List<GameRun> runs) {
        var handStats = new Dictionary<PokerHandType, int>();
        foreach (var run in runs) {
            foreach (var (handType, count) in run.State.HandTypePlayed) {
                handStats.TryAdd(handType, 0);
                handStats[handType] += count;
            }
        }

        var totalPlays = handStats.Values.Sum();
        Console.WriteLine($"=== Played hands: {totalPlays} ===");
        foreach (var (handType, count) in handStats.OrderByDescending(x => x.Value)) {
            var percentage = (float)count / totalPlays;
            var bar = new string('#', (int)(percentage * PercentBarSize)) + new string('.', PercentBarSize - (int)(percentage * PercentBarSize));
            Console.WriteLine($"- {handType,-15}: {bar} {percentage:00.0%} | {count}");
        }
    }

    private static void DisplayRunDistribution(List<GameRun> runs) {
        var runsByLevel = runs
            .GroupBy(run => run.GameStates.Last().Level)
            .OrderBy(group => group.Key)
            .ToDictionary(group => group.Key, group => group.Count());

        Console.WriteLine("=== Levels ===");
        foreach (var (level, count) in runsByLevel) {
            var percentage = (float)count / runs.Count;
            var bar = new string('#', (int)(percentage * PercentBarSize)) + new string('.', PercentBarSize - (int)(percentage * PercentBarSize));
            Console.WriteLine($"- Level {level + 1,2}: {bar} {percentage:00.0%} | {count}");
        }
    }

    private static void DisplayWinLostStats(List<GameRun> runs) {
        var minLevelLost = runs.Where(run => !run.RunIsWon()).DefaultIfEmpty().Min(run => run?.GameStates.Last().Level);
        var maxLevelLost = runs.Where(run => !run.RunIsWon()).DefaultIfEmpty().Max(run => run?.GameStates.Last().Level);
        var gamesWon = runs.Count(run => run.RunIsWon());
        var gamesLost = runs.Count(run => run.RunIsLost());

        var runWithMinLevel = runs.FirstOrDefault(run => run.GameStates.Last().Level == minLevelLost);
        var runWithMaxLevel = runs.FirstOrDefault(run => run.GameStates.Last().Level == maxLevelLost);
        var minLevelState = runWithMinLevel?.GameStates.Last();
        var maxLevelState = runWithMaxLevel?.GameStates.Last();

        Console.WriteLine($"- Wins: {gamesWon}/{runs.Count} Lost: {gamesLost}/{runs.Count} - Min level lost: {minLevelLost + 1} (seed {minLevelState?.Seed}) | Max level lost: {maxLevelLost + 1} (seed {maxLevelState?.Seed})");
    }
}