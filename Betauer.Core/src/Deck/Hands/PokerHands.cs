using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public record PokerHandConfig(PokerHand Prototype, int Multiplier);


public class PokerHands {
    private readonly List<PokerHandConfig> _handConfigs = [];

    public void RegisterBasicPokerHands() {
        RegisterHand(new HighCardHand(this, []), 1);
        RegisterHand(new PairHand(this, []), 2);
        RegisterHand(new TwoPairHand(this, []), 3);
        RegisterHand(new ThreeOfAKindHand(this, []), 4);
        RegisterHand(new StraightHand(this, []), 5);
        RegisterHand(new FlushHand(this, []), 6);
        RegisterHand(new FullHouseHand(this, []), 7);
        RegisterHand(new FourOfAKindHand(this, []), 8);
        RegisterHand(new StraightFlushHand(this, []), 9);
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
        if (cards.Count == 0) return [];

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

    public List<DiscardOption> GetDiscardOptions(IReadOnlyList<Card> currentHand, IReadOnlyList<Card> availableCards, int maxDiscardCards) {
        var improvements = new List<HandImprovement>();

        foreach (var config in _handConfigs) {
            var handImprovements = config.Prototype.FindPossibleImprovements(
                currentHand, 
                availableCards, 
                maxDiscardCards);
            improvements.AddRange(handImprovements);
        }

        return improvements
            .GroupBy(i => i.CardsToDiscard, new CardListEqualityComparer())
            .Select(group => new DiscardOption(group.Key, group.ToList()))
            .OrderByDescending(option => option.TotalScore)
            .Take(5)
            .ToList();
    }
    

    // Helper class para comparar listas de cartas
    private class CardListEqualityComparer : IEqualityComparer<List<Card>> {
        public bool Equals(List<Card> x, List<Card> y) {
            if (x == null || y == null) return x == y;
            return x.Count == y.Count && x.All(y.Contains);
        }

        public int GetHashCode(List<Card> obj) {
            return obj.Aggregate(0, (current, card) => current ^ card.GetHashCode());
        }
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