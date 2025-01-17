using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using static System.Int32;

namespace Betauer.Core.Examples;

public class SolitaireMultiRunner {
    private readonly List<SolitaireConsoleDemo> Demos = new();

    public static void Main() {
        new SolitaireMultiRunner().RunMultipleGames(100);
    }

    public void RunMultipleGames(int iterations) {
        for (var i = 0; i < iterations; i++) {
            var demo = new SolitaireConsoleDemo(true);
            Demos.Add(demo);
            demo.Run(i, DisplayGameState);
        }
    }

    private void DisplayGameState() {
        Console.Clear();
        var last = Demos.Last();
        Console.WriteLine($"=== Now playing: {last.GameRun}");
        Console.WriteLine("=== Previous Runs ===");
        foreach (var demo in Demos) {
            Console.WriteLine(demo.GameRun);
            demo.DisplayHistory();
        }
        if (Demos.Count >= 2) {
            Console.WriteLine("=== Stats ===");
            var gameRunsWithoutLast = Demos.Take(Demos.Count - 1);
            var runsWithStates = gameRunsWithoutLast
                .Where(run => run.GameRun.GameStates.Count > 0)
                .Select(run => run.GameRun).ToList();

            if (runsWithStates.Count != 0) {
                var minLevelWon = runsWithStates.Min(run => run.GameStates.Last().Level);
                var maxLevelWon = runsWithStates.Max(run => run.GameStates.Last().Level);

                // Encontrar los runs que tienen el nivel mínimo y máximo para obtener sus seeds
                var runWithMinLevel = runsWithStates.First(run => run.GameStates.Last().Level == minLevelWon);
                var runWithMaxLevel = runsWithStates.First(run => run.GameStates.Last().Level == maxLevelWon);
                var minLevelState = runWithMinLevel.GameStates.Last();
                var maxLevelState = runWithMaxLevel.GameStates.Last();

                Console.WriteLine($"- Min level won: {(minLevelWon + 1)} (seed {minLevelState.Seed}) | Max level won: {(maxLevelWon + 1)} (seed {maxLevelState.Seed})]");

                // Añadimos el resumen de runs por nivel
                var runsByLevel = runsWithStates
                    .GroupBy(run => run.GameStates.Last().Level)
                    .OrderBy(group => group.Key)
                    .ToDictionary(group => group.Key, group => group.Count());

                Console.WriteLine("=== Runs distribution by level ===");
                foreach (var (level, count) in runsByLevel) {
                    Console.WriteLine($"Level {level + 1}: {count} run{(count > 1 ? "s" : "")}");
                }
            }
        }
        Console.WriteLine();
        Console.WriteLine("=== Current game ... ===");
    }
}