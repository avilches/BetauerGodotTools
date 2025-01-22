using System;
using System.Collections.Generic;
using System.Linq;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game;

public class SolitaireAnalyzer {
    public record HandStats(PokerHandType HandType, int Count, double Percentage);

    public static void Main() {
        const int MIN_HAND = 5;
        const int MAX_HAND = 11;
        AnalyzeHands(MIN_HAND, MAX_HAND);
    }

    public static void AnalyzeHands(int minHand, int maxHand) {
        var handSizes = maxHand - minHand + 1;
        var allStats = new List<HandStats>[handSizes];
        Console.WriteLine($"=== Analyzing hands from {minHand} to {maxHand} cards ===");

        // Get stats for each hand size
        for (int i = 0; i < handSizes; i++) {
            var handSize = i + minHand;
            Console.WriteLine($"Analyzing {handSize} card hands...");
            allStats[i] = Stats(handSize);
        }

        // Get all unique hand types across all stats
        var allHandTypes = new HashSet<PokerHandType>();
        foreach (var statsList in allStats) {
            foreach (var stat in statsList) {
                allHandTypes.Add(stat.HandType);
            }
        }

        var colSize = 5;

        // Print header
        Console.WriteLine("\n=== Hand Statistics Comparison ===");
        Console.Write($"{"Hand Type",-20} |");
        for (int size = minHand; size <= maxHand; size++) {
            Console.Write($" {(size + "").PadLeft(colSize)} |");
        }
        Console.WriteLine();

        // Print separator
        Console.Write(new string('-', 21) + "+");
        for (int i = 0; i < handSizes; i++) {
            Console.Write(new string('-', colSize + 2) + "+");
        }
        Console.WriteLine();

        // Print each hand type statistics
        foreach (var handType in allHandTypes.OrderByDescending(ht =>
                     allStats.Select(s => s.FirstOrDefault(x => x.HandType == ht)?.Percentage ?? 0).Max())) {
            Console.Write($"{handType,-20} |");
            foreach (var statsList in allStats) {
                var stat = statsList.FirstOrDefault(s => s.HandType == handType);
                var value = stat != null ? (Math.Round(stat.Percentage)).ToString() : "0";
                Console.Write($" {value.PadLeft(colSize)} |");
            }
            Console.WriteLine();
        }
    }

    public static List<HandStats> Stats(int handSize) {
        var times = 1000;
        var random = new Random(0);
        // Initialize game components
        var config = new PokerGameConfig();
        var handsManager = new PokerHandsManager();
        handsManager.RegisterBasicPokerHands();
        var gameRun = new GameRun(config, handsManager, 0);
        var handler = gameRun.CreateGameHandler(0);
        handler.State.ShuffleAvailable(random);

        handler.State.Config.MaxDiscardCards = handSize;
        handler.State.Config.HandSize = handSize;
        handler.State.Config.MaxDiscards = times;

        var stats = new Dictionary<PokerHandType, int>();
        var progressInterval = times / 10; // Show progress every 10%

        for (int i = 0; i < times; i++) {
            if (i % progressInterval == 0) {
                Console.Write($"\rProgress: {i * 100 / times}%");
            }

            handler.DrawCards();

            var handTypes = handler.GetPossibleHands().GroupBy(h => h.HandType).Select(g => g.Key);

            foreach (var handType in handTypes) {
                stats.TryAdd(handType, 0);
                stats[handType]++;
            }

            handler.Discard([..handler.State.CurrentHand]);
            handler.Recover([..handler.State.DiscardedCards]);
        }
        Console.WriteLine($"\rProgress: 100%");

        return stats
            .Select(entry => new HandStats(
                entry.Key,
                entry.Value,
                (entry.Value * 100.0) / times))
            .OrderByDescending(x => x.Percentage)
            .ToList();
    }
}