using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

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
        var last = GameStates.LastOrDefault();
        var state = RunIsWon() ? "[Won]": RunIsGameOver() ? "[Lost]" : $"Hand {last.HandsPlayed + 1}/{Config.MaxHands} | Discards: {last.Discards}/{Config.MaxDiscards}";
        return last != null
            ? $"{StartTime:yyyy-MM-dd HH:mm:ss} | Seed: {Seed} | Level {last.Level + 1}/{Config.MaxLevel + 1} | Score: {last.Score}/{last.LevelScore} | {state}"
            : $"{StartTime:yyyy-MM-dd HH:mm:ss} | Seed: {Seed} | {state}";
    }

    public bool RunIsWon() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null && lastState.IsWon() && lastState.Level == Config.MaxLevel;
    }

    public bool RunIsLost() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null && lastState.IsGameOver() && lastState.Level < Config.MaxLevel;
    }

    public bool RunIsGameOver() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null && lastState.IsGameOver();
    }
}