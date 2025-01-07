using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class Card : IComparable<Card> {
    public int Rank { get; }
    public char Suit { get; }

    public Card(int rank, char suit) {
        if (rank < 2 || rank > 14) throw new ArgumentException("Rank must be between 2 and 14");
        if (!"SHDC".Contains(suit)) throw new ArgumentException("Suit must be S, H, D or C");
        Rank = rank;
        Suit = suit;
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

    public static Card Parse(string str) {
        if (string.IsNullOrEmpty(str) || str.Length != 2)
            throw new ArgumentException("Card string must be 2 characters");

        int rank = str[0] switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ when char.IsDigit(str[0]) => int.Parse(str[0].ToString()),
            _ => throw new ArgumentException("Invalid rank")
        };

        if (!("SHDC").Contains(str[1]))
            throw new ArgumentException("Invalid suit");

        return new Card(rank, str[1]);
    }

    public int CompareTo(Card other) => Rank.CompareTo(other.Rank);
}

public class Deck {
    private readonly List<Card> cards = new();
    private readonly Random random = new();

    public Deck() {
        foreach (char suit in "SHDC") {
            for (int rank = 2; rank <= 14; rank++) {
                cards.Add(new Card(rank, suit));
            }
        }
    }

    public void Shuffle() {
        int n = cards.Count;
        while (n > 1) {
            n--;
            int k = random.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
    }

    public List<Card> Draw(int count) {
        if (count > cards.Count) throw new InvalidOperationException("Not enough cards in deck");
        var drawn = cards.Take(count).ToList();
        cards.RemoveRange(0, count);
        return drawn;
    }

    public void ReturnCards(List<Card> returnedCards) {
        cards.AddRange(returnedCards);
    }

    public int RemainingCards => cards.Count;
}

public class GameHistory {
    public record GameAction(
        string Type, // "PLAY" or "DISCARD"
        List<Card> Cards, // Cards played or discarded
        PokerHand PlayedHand = null, // If Type is "PLAY"
        int Score = 0 // Score earned if Type is "PLAY"
    );

    private readonly List<GameAction> actions = new();

    public void AddPlay(PokerHand hand) {
        actions.Add(new GameAction("PLAY", hand.Cards, hand, hand.Score));
    }

    public void AddDiscard(List<Card> cards) {
        actions.Add(new GameAction("DISCARD", cards));
    }

    public List<GameAction> GetHistory() => actions.ToList();
}