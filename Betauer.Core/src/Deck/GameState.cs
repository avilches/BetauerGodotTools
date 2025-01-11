using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class GameState {
    public int Score { get; set; } = 0;
    public int TotalScore { get; set; } = 0;
    public int Level { get; set; } = 0;

    public int HandsPlayed { get; set; } = 0;
    public int Discards { get; set; } = 0;
    public IReadOnlyList<Card> CurrentHand { get; set; } = [];
    public GameHistory History { get; } = new();

    private readonly List<Card> _cards = [];
    private readonly List<Card> _discardedCards = [];
    private readonly List<Card> _playedCards = [];
    public int RemainingCards => _cards.Count;

    public IReadOnlyList<Card> DiscardedCards => _discardedCards.AsReadOnly();
    public IReadOnlyList<Card> PlayedCards => _playedCards.AsReadOnly();
    public IReadOnlyList<Card> AvailableCards => _cards.AsReadOnly();

    public void BuildPokerDeck(string suits, int minRank, int maxRank) {
        if (minRank > maxRank) throw new ArgumentException("minRank cannot be greater than maxRank");

        _cards.Clear();
        foreach (var suit in suits) {
            for (var rank = minRank; rank <= maxRank; rank++) {
                _cards.Add(new Card(rank, suit));
            }
        }
    }

    public void Shuffle(Random random) {
        random.Shuffle(_cards);
    }

    public List<Card> Draw(int count) {
        if (count > _cards.Count) throw new InvalidOperationException("Not enough cards in deck");
        var drawn = _cards.Take(count).ToList();
        _cards.RemoveRange(0, count);
        return drawn;
    }

    public void AddToDiscardedCards(IReadOnlyList<Card> returnedCards) {
        // Clone each card before adding to discarded pile
        _discardedCards.AddRange(returnedCards.Select(card => card.Clone()));
    }

    public void AddToPlayedCards(IReadOnlyList<Card> playedCards) {
        // Clone each card before adding to played pile
        _playedCards.AddRange(playedCards.Select(card => card.Clone()));
    }
}