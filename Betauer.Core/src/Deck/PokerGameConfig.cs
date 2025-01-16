using System;

namespace Betauer.Core.Deck;

public class PokerGameConfig {
    public int MaxHandSizeToPlay { get; set; } = 5;
    public int HandSize { get; set; } = 8;
    public int MaxHands { get; set; } = 4;
    public int MaxDiscards { get; set; } = 4;
    public int MaxDiscardCards { get; set; } = 5;
    public int MinRank { get; set; } = 2;
    public int MaxRank { get; set; } = 14;
    public string ValidSuits { get; set; } = "SHDC";

    public int StraightSize { get; set; } = 5;
    public int FlushSize { get; set; } = 5;
    public int AnalysisMinFlushSize { get; set; } = 3;

    public Card Parse(string str) {
        if (string.IsNullOrEmpty(str) || str.Length != 2)
            throw new ArgumentException("Card string must be 2 characters");

        var rank = str[0] switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ when char.IsDigit(str[0]) => int.Parse(str[0].ToString()),
            _ => throw new ArgumentException("Invalid rank")
        };

        if (!ValidSuits.Contains(str[1]))
            throw new ArgumentException("Invalid suit");

        return new Card(rank, str[1]);
    }
}