using System;

namespace Betauer.Core.Deck;

public readonly struct Card(int rank, char suit) : IComparable<Card> {
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
}