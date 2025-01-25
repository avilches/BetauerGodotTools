using System;
using System.Collections.Generic;
using System.Linq;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game;

public class SolitaireAnalyzer {
    public record HandStats(PokerHandType HandType, int Count, double Percentage);

    public static IList<PokerHandType> Duplicated = [
        PokerHandType.Pair, PokerHandType.TwoPair, PokerHandType.ThreeOfAKind, PokerHandType.FourOfAKind, PokerHandType.FiveOfAKind, PokerHandType.FullHouse
    ];

    public static IList<PokerHandType> Straights = [
        PokerHandType.Straight, PokerHandType.StraightFlush
    ];

    public static IList<PokerHandType> Flushes = [
        PokerHandType.FlushHouse, PokerHandType.Flush, PokerHandType.FlushFive
    ];


    public const int Montecarlo = 1000;
    public static  IList<PokerHandType> Filter = [..Duplicated, ..Flushes, ..Straights];

    public static void MainBalatro() {
        const int MIN_HAND = 5;
        const int MAX_HAND = 10;
        Console.WriteLine($"=== Classic poker deck ===");
        AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SHDC", 2, 14));
        Console.WriteLine($"=== 2 to 10 ===");
        AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SHDC", 2, 10));
    }

    public static void Main() {
        const int MIN_HAND = 3;
        const int MAX_HAND = 5;
        Console.WriteLine($"=== Classic poker deck ===");
        AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SHDC", 2, 14));
        Console.WriteLine($"=== 2 to 10 ===");
        // AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SHDC", 2, 10));
        // Console.WriteLine($"=== 2 to 6 ===");
        // AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SHDC", 2, 6));
        // Console.WriteLine($"=== 2 to 6 ===");
        // AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SH", 2, 10));
        // Console.WriteLine($"=== 2 suits ===");
        // AnalyzeHands(MIN_HAND, MAX_HAND, DeckBuilder.Cards("SH", 2, 14));
    }

    public static void AnalyzeHands(int minHand, int maxHand, IEnumerable<Card> cards) {
        var handSizes = maxHand - minHand + 1;
        var allStats = new List<HandStats>[handSizes];

        // Get stats for each hand size
        for (var i = 0; i < handSizes; i++) {
            var handSize = i + minHand;
            allStats[i] = Stats(handSize, cards);
        }
        Console.Write("\r                                         \r");

        // Get all unique hand types across all stats
        var allHandTypes = new HashSet<PokerHandType>();
        foreach (var statsList in allStats) {
            foreach (var stat in statsList) {
                allHandTypes.Add(stat.HandType);
            }
        }

        var colSize = 5;

        // Print header
        // Console.WriteLine();
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
                var value = stat != null ? Math.Round(stat.Percentage).ToString() : "0";
                Console.Write($" {value.PadLeft(colSize)} |");
            }
            Console.WriteLine();
        }
    }

    public static List<HandStats> Stats(int handSize, IEnumerable<Card> cards) {
        var seed = 0;
        var random = new Random(seed);
        // Initialize game components
        var config = new PokerGameConfig();
        var handsManager = new PokerHandsManager(new Random(seed), new PokerHandConfig());
        handsManager.RegisterBasicPokerHands();
        var gameRun = new GameRun(config, handsManager, 0);
        var handler = gameRun.CreateGameHandler(0, cards);
        handler.State.ShuffleAvailable(random);

        handler.State.Config.MaxDiscardCards = handSize;
        handler.State.Config.HandSize = handSize;
        handler.State.Config.MaxDiscards = Montecarlo;

        var stats = new Dictionary<PokerHandType, int>();

        for (int i = 0; i < Montecarlo; i++) {
            Console.Write($"\rAnalyzing {handSize} card hands: {i * 100 / Montecarlo}%");

            handler.DrawCards();

            var handTypes = handler.GetPossibleHands().GroupBy(h => h.HandType).Select(g => g.Key);
            var filter = Filter.ToHashSet();

            foreach (var handType in handTypes.Where(h => filter.Contains(h))) {
                stats.TryAdd(handType, 0);
                stats[handType]++;
            }

            handler.Discard([..handler.State.CurrentHand]);
            handler.Recover([..handler.State.DiscardedCards]);
        }
        Console.Write($"\rAnalyzing {handSize} card hands: 100%");

        return stats
            .Select(entry => new HandStats(
                entry.Key,
                entry.Value,
                (entry.Value * 100.0) / Montecarlo))
            .OrderByDescending(x => x.Percentage)
            .ToList();
    }
}