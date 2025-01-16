using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class GameRunState {

    private readonly Dictionary<Type, int> _handLevels = new();

    public int Discards { get; set; } = 0;
    public int HandsPlayed { get; set; } = 0;
    public int CardsPlayed { get; set; } = 0;
    public int CardsDiscarded { get; set; } = 0;
    
    public int GetPokerHandLevel(PokerHand hand) {
        return _handLevels.GetValueOrDefault(hand.GetType(), 0);
    }

    public void SetPokerHandLevel(PokerHand hand, int level) {
        _handLevels[hand.GetType()] = level;
    }
}

public class GameRun {
    public GameRunState State { get; }

    public int Id { get; }
    public PokerGameConfig Config { get; }
    public PokerHandsManager PokerHandsManager { get; }
    public int Seed { get; }

    public DateTime StartTime { get; } = DateTime.Now;
    public List<GameState> GameStates { get; } = [];

    public GameRun(int id, PokerGameConfig config, PokerHandsManager pokerHandsManager, int seed) {
        Id = id;
        Config = config;
        PokerHandsManager = pokerHandsManager;
        Seed = seed;
        State = new GameRunState();
    }

    public GameHandler CreateGameHandler(int level) {
        var gameHandler = new GameHandler(State, Config, PokerHandsManager, level, Seed);
        GameStates.Add(gameHandler.State);
        return gameHandler;
    }

    public override string ToString() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null
            ? $"{StartTime:yyyy-MM-dd HH:mm:ss} - Seed: {Seed} - Max level {lastState.Level + 1} - Score {lastState.Score}/{lastState.LevelScore}"
            : $"{StartTime:yyyy-MM-dd HH:mm:ss} - No games played";
    }
}