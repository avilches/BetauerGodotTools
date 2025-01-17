using System;
using System.Collections.Generic;

namespace Betauer.Core.Deck;

public class GameState {
    public PokerGameConfig Config { get; }
    
    public int Seed { get; }

    public int Level { get; } = 0;
    public long LevelScore { get; set; } = 0;
    public long Score { get; set; } = 0;

    public int HandsPlayed { get; set; } = 0;
    public int Discards { get; set; } = 0;
    public int CardsPlayed { get; set; } = 0;
    public int CardsDiscarded { get; set; } = 0;

    
    public PlayHistory History { get; } = new();

    private readonly List<Card> _availableCards = [];
    private readonly List<Card> _currentHand = [];
    private readonly List<Card> _discardedCards = [];
    private readonly List<Card> _playedCards = [];
    private readonly List<Card> _destroyedCards = [];

    public IReadOnlyList<Card> AvailableCards { get; }
    public IReadOnlyList<Card> CurrentHand { get; }
    public IReadOnlyList<Card> DiscardedCards { get; }
    public IReadOnlyList<Card> PlayedCards { get; }
    public IReadOnlyList<Card> DestroyedCards { get; }

    public bool IsWon() => LevelScore > 0 && Score >= LevelScore;
    public bool IsGameOver() => IsWon() || HandsPlayed >= Config.MaxHands;
    public bool IsDrawPending() => !IsGameOver() && CurrentHand.Count < Config.HandSize;
    public bool CanDiscard() => !IsGameOver() && Discards < Config.MaxDiscards;
    
    public long RemainingScoreToWin => LevelScore - Score;
    public int RemainingHands => Config.MaxHands - HandsPlayed;
    public int RemainingDiscards => Config.MaxHands - Discards;
    public int RemainingCards => AvailableCards.Count;
    public int RemainingCardsToDraw => Config.HandSize - CurrentHand.Count;
    
    public GameState(PokerGameConfig config, int level, int seed) {
        Config = config;
        Level = level;
        LevelScore = config.GetLevelScore(level);
        Seed = seed;
        AvailableCards = _availableCards.AsReadOnly();
        CurrentHand = _currentHand.AsReadOnly();
        DiscardedCards = _discardedCards.AsReadOnly();
        PlayedCards = _playedCards.AsReadOnly();
        DestroyedCards = _destroyedCards.AsReadOnly();
    }

    public void Clear() {
        _availableCards.Clear();
        _currentHand.Clear();
        _discardedCards.Clear();
        _playedCards.Clear();
        _destroyedCards.Clear();
    }

    public void BuildPokerDeck(string suits, int minRank, int maxRank) {
        if (minRank > maxRank) throw new ArgumentException("minRank cannot be greater than maxRank");

        _availableCards.Clear();
        foreach (var suit in suits) {
            for (var rank = minRank; rank <= maxRank; rank++) {
                _availableCards.Add(new Card(rank, suit));
            }
        }
    }
    
    public void ShuffleAvailable(Random random) {
        random.Shuffle(_availableCards);
    }

    /// <summary>
    /// Move a card from the available pile to hand
    /// </summary>
    public bool Draw(Card card) {
        if (!_availableCards.Remove(card)) return false;
        _currentHand.Add(card);
        return true;
    }

    /// <summary>
    /// Move a card from the hand to the discard pile
    /// </summary>
    public bool Discard(Card card) {
        if (!_currentHand.Remove(card)) return false;
        _discardedCards.Add(card);
        return true;
    }

    /// <summary>
    /// Move a card from the hand to the played pile
    /// </summary>
    public bool Play(Card card) {
        if (!_currentHand.Remove(card)) return false;
        _playedCards.Add(card);
        return true;
    }

    /// <summary>
    /// Move a card from played to the available pile
    /// </summary>
    public bool Recover(Card card) {
        if (_playedCards.Remove(card)) {
            _availableCards.Add(card);
            return true;
        }
        if (_discardedCards.Remove(card)) {
            _availableCards.Add(card);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Move a card from any pile to destroyed pile
    /// </summary>
    public bool Destroy(Card card) {
        if (_currentHand.Remove(card)) {
            _destroyedCards.Add(card);
            return true;
        }
        if (_availableCards.Remove(card)) {
            _destroyedCards.Add(card);
            return true;
        }
        if (_discardedCards.Remove(card)) {
            _destroyedCards.Add(card);
            return true;
        }
        if (_playedCards.Remove(card)) {
            _destroyedCards.Add(card);
            return true;
        }
        return false;
    }
}