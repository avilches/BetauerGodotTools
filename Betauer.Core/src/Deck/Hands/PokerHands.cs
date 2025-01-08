using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class PokerHands {
    private readonly List<PokerHandConfig> _handConfigs = [];

    public void RegisterBasicPokerHands() {
        RegisterHand(new HighCardHand([]), 1);
        RegisterHand(new PairHand([]), 2);
        RegisterHand(new TwoPairHand([]), 3);
        RegisterHand(new ThreeOfAKindHand([]), 4);
        RegisterHand(new StraightHand([]), 5);
        RegisterHand(new FlushHand([]), 6);
        RegisterHand(new FullHouseHand([]), 7);
        RegisterHand(new FourOfAKindHand([]), 8);
        RegisterHand(new StraightFlushHand([]), 9);
        RegisterHand(new RoyalFlushHand([]), 10);
    }

    public void RegisterHand(PokerHand prototype, int multiplier) {
        // Remove any existing config for this hand type if it exists
        _handConfigs.RemoveAll(config => config.Prototype.GetType() == prototype.GetType());
        _handConfigs.Add(new PokerHandConfig(prototype, multiplier));
        // Re-sort configs by multiplier in descending order
        _handConfigs.Sort((a, b) => b.Multiplier.CompareTo(a.Multiplier));
    }

    public List<PokerHand> IdentifyAllHands(IReadOnlyList<Card> cards) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        
        var allHands = _handConfigs
            .SelectMany(config => config.Prototype.FindAll(cards))
            .OrderByDescending(CalculateScore)
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

    public int GetMultiplier(PokerHand hand) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        var config = _handConfigs.Find(c => c.Prototype.GetType() == hand.GetType());
        if (config == null) {
            throw new ArgumentException($"No configuration found for hand type: {hand.GetType()}");
        }
        return config.Multiplier;
    }

    public int CalculateScore(PokerHand hand) {
        return hand.Cards.Sum(c => c.Rank) * GetMultiplier(hand);
    }
}