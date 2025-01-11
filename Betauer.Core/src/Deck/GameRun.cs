using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class GameRun {
    public DateTime StartTime { get; }
    public List<GameState> GameStates { get; } = [];

    public GameRun(Random random) {
        StartTime = DateTime.Now;
    }

    public void AddGameState(GameState state) {
        GameStates.Add(state);
    }

    public override string ToString() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null
            ? $"{StartTime:yyyy-MM-dd HH:mm:ss} - Max level {lastState.Level + 1} - Score {lastState.Score}/{lastState.TotalScore}"
            : $"{StartTime:yyyy-MM-dd HH:mm:ss} - No games played";
    }
}