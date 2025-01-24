using System;

namespace Veronenger.Game.Deck;

public class Card(int rank, char suit) : IComparable<Card> {
    public int Rank { get; } = rank;
    public char Suit { get; } = suit;
    public int BaseScore { get; set; } = rank;

    public int AdditionalScore { get; set; } = 0;

    public int Score => BaseScore + AdditionalScore;

    public int CompareTo(Card other) => Rank.CompareTo(other.Rank);

    public Card Clone() {
        var clone = new Card(Rank, Suit) {
            BaseScore = BaseScore,
            AdditionalScore = AdditionalScore
        };
        return clone;
    }

    public override string ToString() {
        var rankStr = Rank switch {
            14 => "A",
            13 => "K",
            12 => "Q",
            11 => "J",
            10 => "T",
            _ => Rank.ToString()
        };
        return $"{rankStr}{Suit}";
    }

    public override bool Equals(object? obj) {
        if (obj is not Card other) return false;
        return Rank == other.Rank && Suit == other.Suit;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Rank, Suit);
    }

    public static Card Parse(string str) {
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

        return new Card(rank, str[1]);
    }
}