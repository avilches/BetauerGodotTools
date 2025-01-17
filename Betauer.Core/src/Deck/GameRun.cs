using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class GameRunState {

    private readonly Dictionary<PokerHandType, int> _handLevels = new();

    public int Discards { get; set; } = 0;
    public int HandsPlayed { get; set; } = 0;
    public int CardsPlayed { get; set; } = 0;
    public int CardsDiscarded { get; set; } = 0;
    
    public int GetPokerHandLevel(PokerHandType hand) {
        return _handLevels.GetValueOrDefault(hand, 0);
    }

    public void SetPokerHandLevel(PokerHandType hand, int level) {
        _handLevels[hand] = level;
    }
}

public class GameRun {
    public GameRunState State { get; }

    public PokerGameConfig Config { get; }
    public PokerHandsManager PokerHandsManager { get; }
    public int Seed { get; }

    public DateTime StartTime { get; } = DateTime.Now;
    public List<GameState> GameStates { get; } = [];

    public GameRun(PokerGameConfig config, PokerHandsManager pokerHandsManager, int seed) {
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
        var state = GameStates.LastOrDefault();
        return state != null
            ? $"{StartTime:yyyy-MM-dd HH:mm:ss} - Seed: {Seed} - Level {state.Level + 1}/{Config.MaxLevel} | Score: {state.Score}/{state.LevelScore} | Hand {state.HandsPlayed + 1}/{Config.MaxHands} | Discards: {state.Discards}/{Config.MaxDiscards}"
            : $"{StartTime:yyyy-MM-dd HH:mm:ss} - No games played";
    }

    public bool RunIsWon() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null && lastState.IsWon() && lastState.Level == Config.MaxLevel;
    }

    public bool RunIsGameOver() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null && lastState.IsGameOver();
    }
}