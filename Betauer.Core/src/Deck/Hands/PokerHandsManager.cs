using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public record PokerHandConfig(
    PokerHand Prototype, 
    long InitialScore, 
    long InitialMultiplier, 
    long ScorePerLevel, 
    long MultiplierPerLevel,
    int Order,
    bool Enabled);

public class PokerHandsManager {
    private readonly List<PokerHandConfig> _handConfigs = [];

    public void RegisterBasicPokerHands() {
        RegisterHand(new HighCardHand(this, []), 5, 1, 15, 1, order: 1);
        RegisterHand(new PairHand(this, []), 10, 2, 15, 1, order: 2);
        RegisterHand(new TwoPairHand(this, []), 20, 2, 20, 1, order: 3);
        RegisterHand(new ThreeOfAKindHand(this, []), 30, 3, 20, 2, order: 4);
        RegisterHand(new StraightHand(this, []), 30, 4, 30, 3, order: 5);
        RegisterHand(new FlushHand(this, []), 35, 4, 15, 2, order: 6);
        RegisterHand(new FullHouseHand(this, []), 40, 4, 25, 2, order: 7);
        RegisterHand(new FourOfAKindHand(this, []), 60, 7, 30, 3, order: 8);
        RegisterHand(new StraightFlushHand(this, []), 100, 8, 40, 4, order: 9);
        RegisterHand(new FiveOfAKindHand(this, []), 120, 12, 40, 4, order: 10);
        RegisterHand(new FlushHouseHand(this, []), 140, 14, 40, 4, order: 11);
        RegisterHand(new FlushFiveHand(this, []), 160, 16, 40, 4, order: 12);
    }

    /// <summary>
    /// Registers a poker hand with its corresponding multiplier.
    /// If a hand of the same type already exists, it will be replaced.
    /// Hand configs are kept sorted by multiplier in descending order.
    /// </summary>
    public void RegisterHand(PokerHand prototype, long initialScore, long initialMultiplier, long scorePerLevel, long multiplierPerLevel, int order, bool enabled = true) {
        _handConfigs.RemoveAll(config => config.Prototype.GetType() == prototype.GetType());
        _handConfigs.Add(new PokerHandConfig(prototype, initialScore, initialMultiplier, scorePerLevel, multiplierPerLevel, order, enabled));
        var ordered = _handConfigs.OrderByDescending(c => c.Order).ToList();
        _handConfigs.Clear();
        _handConfigs.AddRange(ordered);
    }    
    public void ClearHands() => _handConfigs.Clear();

    /// <summary>
    /// Identifies all possible poker hands in the given cards.
    /// Returns hands ordered by score in descending order.
    /// For multiple hands of the same type, they are numbered (#1, #2, etc.).
    /// </summary>
    /// <param name="handler">GameHandler is needed to get the score, which is a value calculated using the GameState and the PokerGameConfig</param>
    /// <param name="cards">Cards to analyze</param>
    /// <returns>List of identified poker hands, ordered by score</returns>
    public List<PokerHand> IdentifyAllHands(GameHandler handler, IReadOnlyList<Card> cards) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        if (cards.Count == 0) return [];
        
        var analysis = new PokerHandAnalysis(handler.Config, cards);

        var allHands = _handConfigs
            .Where(config => config.Enabled)
            .SelectMany(config => config.Prototype.IdentifyHands(analysis))
            .OrderByDescending(handler.CalculateScore)
            .ToList();

        // Add unique identifier for hands of the same type
        var groupedHands = allHands.GroupBy(h => h.Name).ToList();
        foreach (var group in groupedHands.Where(g => g.Count() > 1)) {
            var i = 1;
            foreach (var hand in group) {
                hand.Name = $"{hand.Name} #{i}";
                i++;
            }
        }

        return allHands;
    }
    
    /// <summary>
    /// Identifies the best poker hand by checking hand types in descending order.
    /// Returns the best hand of the highest-ranking type found.
    /// </summary>
    /// <param name="handler">GameHandler is needed to get the score, which is a value calculated using the GameState and the PokerGameConfig</param>
    /// <param name="cards">Cards to analyze</param>
    /// <returns>Best poker hand found, or null if no valid hands exist</returns>
    public PokerHand? IdentifyBestHand(GameHandler handler, IReadOnlyList<Card> cards) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        if (cards.Count == 0) return null;

        var analysis = new PokerHandAnalysis(handler.Config, cards);

        // Iterate through configs in descending order
        foreach (var config in _handConfigs) {
            if (!config.Enabled) continue;
        
            var hands = config.Prototype.IdentifyHands(analysis);
            if (hands.Count > 0) {
                // Return the best hand of this type
                return hands.MaxBy(handler.CalculateScore);
            }
        }

        return null;
    }

    /// <summary>
    /// Analyzes current hand and provides discard options for improvement.
    /// Uses Monte Carlo simulation to estimate probabilities when exact calculation
    /// is impractical.
    /// 
    /// Algorithm:
    /// 1. For each hand type, if the hand doesn't already exist:
    ///    - Get suggested discards for that hand type
    /// 2. For each unique discard combination:
    ///    - Simulate drawing new cards multiple times
    ///    - Identify best possible hand for each simulation
    ///    - Track statistics for each hand type achieved
    /// 3. Calculate probabilities and potential scores
    /// 4. Return options ordered by potential score
    /// </summary>
    /// <param name="handler">GameHandler is needed to get the score, which is a value calculated using the GameState and the PokerGameConfig</param>
    /// <param name="cards">Current cards in hand</param>
    /// <param name="availableCards">Cards available to draw</param>
    /// <param name="maxDiscardCards">Maximum number of cards that can be discarded</param>
    /// <returns>Analysis results with discard options and statistics</returns>
    public DiscardOptionsResult GetDiscardOptions(GameHandler handler, IReadOnlyList<Card> cards, IReadOnlyList<Card> availableCards, int maxDiscardCards) {
        if (maxDiscardCards < 0) throw new ArgumentException("maxDiscardCards cannot be negative");

        const int MaxSimulations = 10000;
        const double MinSimulationPercentage = 0.10;
        
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var totalSimulations = 0;
        var totalCombinations = 0;
        
        var analysis = new PokerHandAnalysis(handler.Config, cards);

        // Get all possible discard combinations from all hand types
        var suggestedDiscards = _handConfigs
            .Where(config => config.Enabled)
            .SelectMany(config => config.Prototype.SuggestDiscards(analysis, maxDiscardCards))
            .Where(cardsToDiscard => cardsToDiscard.Count > 0)
            .Distinct(new CardListEqualityComparer())
            .ToList();


        var options = new List<DiscardOption>();
        var random = new Random();

        // Analyze each discard combination
        foreach (var cardsToDiscard in suggestedDiscards) {
            var cardsToKeep = cards.Except(cardsToDiscard).ToList();
            var handTypeOccurrences = new Dictionary<Type, HandTypeStats>();
            
            // Calculate total possible combinations
            int combinations = CombinationTools.Calculate(availableCards.Count, cardsToDiscard.Count);
            if (combinations == 0) continue;
            totalCombinations += combinations;

            // Determine number of simulations
            var simulations = Math.Min(
                MaxSimulations,
                Math.Max(
                    (int)(combinations * MinSimulationPercentage),
                    combinations
                )
            );
            totalSimulations += simulations;

            var availableCardsList = availableCards.ToList();
            // Generate and analyze random combinations
            var draws = simulations == combinations
                ? availableCardsList.Combinations(cardsToDiscard.Count)
                    .Select(combo => combo.ToList())
                : availableCardsList.RandomCombinations(cardsToDiscard.Count, simulations, random);

            foreach (var draw in draws) {
                var newHand = new List<Card>(cardsToKeep);
                newHand.AddRange(draw);

                var bestHand = IdentifyAllHands(handler, newHand).MaxBy(handler.CalculateScore);
                if (bestHand != null) {
                    var handType = bestHand.GetType();
                    var score = handler.CalculateScore(bestHand);
                    if (!handTypeOccurrences.TryGetValue(handType, out HandTypeStats? value)) {
                        handTypeOccurrences[handType] = new HandTypeStats(handType, score);
                    } else {
                        value.AddScore(score);
                    }
                }
            }

            if (handTypeOccurrences.Count != 0) {
                options.Add(new DiscardOption(
                    cardsToKeep,
                    cardsToDiscard,
                    handTypeOccurrences,
                    simulations,
                    combinations));
            }
        }

        watch.Stop();
        return new DiscardOptionsResult(
            options.OrderByDescending(option => option.GetBestHand().PotentialScore).ToList(),
            watch.Elapsed,
            totalSimulations,
            totalCombinations
        );
    }
    
    private class CardListEqualityComparer : IEqualityComparer<List<Card>> {
        public bool Equals(List<Card>? x, List<Card>? y) {
            if (x == null || y == null) return x == y;
            return x.Count == y.Count && x.All(y.Contains);
        }

        public int GetHashCode(List<Card> obj) {
            return obj.Aggregate(0, (current, card) => current ^ card.GetHashCode());
        }
    }

    public PokerHandConfig GetPokerHandConfig(PokerHand hand) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        var config = _handConfigs.Find(c => c.Prototype.GetType() == hand.GetType());
        if (config == null) {
            throw new ArgumentException($"No configuration found for hand type: {hand.GetType()}");
        }
        return config;
    }
}