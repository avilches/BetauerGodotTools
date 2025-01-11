using System;
using Godot;

namespace Betauer.Core.Deck.Hands;

/// <summary>
/// Tracks statistics for a specific hand type, including occurrences, scores, and probabilities.
/// Used in Monte Carlo simulations to evaluate potential hand improvements.
/// </summary>
public class HandTypeStats(Type type, int score) {
    public Type HandType { get; } = type;
    public int Count { get; private set; } = 1;
    public int MaxScore { get; private set; } = score;
    public int MinScore { get; private set; } = score;
    public int AccumulatedScore { get; private set; } = score;

    public float Probability { get; set; } = 0;

    public float AvgScore => (float)AccumulatedScore / Count;
    public int ScoreAdjust = 0;

    public int PotentialScore => Mathf.RoundToInt(AvgScore * Probability) - ScoreAdjust;

    public void AddScore(int score) {
        Count++;
        MaxScore = Math.Max(MaxScore, score);
        MinScore = Math.Min(MinScore, score);
        AccumulatedScore += score;
    }
}