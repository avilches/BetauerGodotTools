using System;
using System.Collections.Generic;
using System.Linq;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game;

public class SolitaireMultiRunner {
    public readonly List<SolitaireConsoleDemo> Demos = [];

    public static void Main() {
        new SolitaireMultiRunner().Run(10);
    }

    private const int MaxSimulations = 100;
    private const float SimulationPercentage = 1f;

    public void Run(int iterations) {
        for (var i = 0; i < iterations; i++) {
            var demo = new SolitaireConsoleDemo(true, MaxSimulations, SimulationPercentage);
            Demos.Add(demo);
            demo.Run(i, DisplayGameState, () => {
                demo.IncreaseRandomPokerHandLevel();
                demo.IncreaseRandomPokerHandLevel();
            });
        }
        DisplayGameState();
    }

    private void DisplayGameState() {
        Console.Clear();
        Console.WriteLine("=== Now playing ===");
        Console.WriteLine(Demos.Last().GameRun);
        var runs = Demos
            .Where(demo => demo.GameRun.RunIsGameOver())
            .Select(run => run.GameRun).ToList();

        Console.WriteLine("=== Runs ===");
        foreach (var demo in runs) {
            Console.WriteLine(demo);
            // demo.DisplayHistory();
        }
        Console.WriteLine("=== Stats ===");
        Console.WriteLine($"- Simulating {SimulationPercentage:0%} (capped to {MaxSimulations})");

        if (runs.Count > 0) {
            DisplayWinLostStats(runs);
            DisplayRunDistribution(runs);
            DisplayMostPlayedHands(runs);
        }
        Console.WriteLine();
        Console.WriteLine("=== Current game ... ===");
    }

    private static void DisplayMostPlayedHands(List<GameRun> runs) {
        Console.WriteLine("=== Most played hands ===");
        var handStats = new Dictionary<PokerHandType, int>();
        foreach (var run in runs) {
            foreach (var (handType, count) in run.State.HandTypePlayed) {
                handStats.TryAdd(handType, 0);
                handStats[handType] += count;
            }
        }

        foreach (var (handType, count) in handStats.OrderByDescending(x => x.Value)) {
            Console.WriteLine($"- {handType,-15}: {count} times");
        }
    }

    private static void DisplayRunDistribution(List<GameRun> runs) {
        var runsByLevel = runs
            .GroupBy(run => run.GameStates.Last().Level)
            .OrderBy(group => group.Key)
            .ToDictionary(group => group.Key, group => group.Count());

        Console.WriteLine("=== Runs distribution by level ===");
        foreach (var (level, count) in runsByLevel) {
            Console.WriteLine($"- Level {level + 1}: {count} run{(count > 1 ? "s" : "")}");
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

        Console.WriteLine($"- Wins: {gamesWon}/{runs.Count} Lost: {gamesLost}/{runs.Count} - Min level lost: {minLevelLost + 1} (seed {minLevelState?.Seed}) | Max level lost: {maxLevelLost + 1} (seed {maxLevelState?.Seed})]");
    }
}