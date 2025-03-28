using System;
using System.Numerics;
using Godot;

namespace Veronenger.Game.Deck.Hands;

/// <summary>
/// Tracks statistics for a specific hand type, including occurrences, scores, and probabilities.
/// Used in Monte Carlo simulations to evaluate potential hand improvements.
/// </summary>
public class HandTypeStats(PokerHandType type, long score) {
    public PokerHandType HandType { get; } = type;
    public int Count { get; private set; } = 1;
    public long MaxScore { get; private set; } = score;
    public long MinScore { get; private set; } = score;
    public BigInteger AccumulatedScore { get; private set; } = score;

    public float Probability { get; set; } = 0;

    public float AvgScore => (float)(AccumulatedScore / Count);
    public int ScoreAdjust = 0;

    public int PotentialScore => Mathf.RoundToInt(AvgScore * Probability) - ScoreAdjust;

    public void AddScore(long score) {
        Count++;
        MaxScore = Math.Max(MaxScore, score);
        MinScore = Math.Min(MinScore, score);
        AccumulatedScore += score;
    }
}