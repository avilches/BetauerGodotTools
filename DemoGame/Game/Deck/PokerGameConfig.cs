using System;

namespace Veronenger.Game.Deck;

public class PokerGameConfig {
    public long[] BaseLevel { get; set; }= [
        300,
        800,
        2000,
        5000,
        11000,
        20000,
        35000,
        50000,
        110000,
        560000,
        7200000,
        300000000,
        47000000000
    ];

    public int MaxHandSizeToPlay { get; set; } = 5;
    public int HandSize { get; set; } = 8;

    public int MaxHands { get; set; } = 4;
    public int MaxDiscards { get; set; } = 4;
    public int MaxCardsToDiscard { get; set; } = 5;
    public bool AutoRecover { get; set; } = false;

    public int MaxLevel => BaseLevel.Length * 3 - 1; // level starts in 0

    public long GetLevelScore(int level) {
        var baseScoreIndex = level / 3; // Cada 3 niveles cambiamos de score base
        var multiplierIndex = level % 3; // 0 = x1, 1 = x1.5, 2 = x2

        if (baseScoreIndex >= BaseLevel.Length) {
            throw new ArgumentException($"Level {level} too high. Max level is {MaxLevel}");
        }

        var baseScore = BaseLevel[baseScoreIndex];
        var multiplier = multiplierIndex switch {
            0 => 1.0f,
            1 => 1.5f,
            2 => 2.0f,
            _ => throw new ArgumentException($"Invalid multiplier index {multiplierIndex}")
        };

        return (int)(baseScore * multiplier);
    }

}