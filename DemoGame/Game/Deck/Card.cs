using System;

namespace Veronenger.Game.Deck;

public class Card(int rank, char suit) : IComparable<Card> {
    public int Rank { get; } = rank;
    public char Suit { get; } = suit;

    public int CompareTo(Card other) => Rank.CompareTo(other.Rank);

    public Card Clone() {
        return new Card(rank, suit);
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
}