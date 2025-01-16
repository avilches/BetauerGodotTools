using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class GameRun(int id, PokerGameConfig config, PokerHandsManager pokerHandsManager, int seed) {
    public int Id { get; } = id;
    public PokerGameConfig Config { get; } = config;
    public PokerHandsManager PokerHandsManager { get; } = pokerHandsManager;
    public int Seed { get; } = seed;
    
    public DateTime StartTime { get; } = DateTime.Now;
    
    public List<GameState> GameStates { get; } = [];

    public GameHandler CreateGameHandler(int level) {
        var gameHandler = new GameHandler(Config, PokerHandsManager, level, Seed);
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