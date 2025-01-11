using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class GameRun(int id) {
    public int Id { get; } = id;
    public DateTime StartTime { get; } = DateTime.Now;
    public List<GameState> GameStates { get; } = [];

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