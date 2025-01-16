using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public class Straight {
    public List<Card> Cards { get; }
    public int MinRank { get; }
    public int Size { get; }
    public int MaxRank => MinRank + Size - 1;
    public int MissingCards { get; }
    public int Gaps { get; }
    public bool HasAce => Cards.Any(c => c.Rank == 14);
    public bool IsFlush { get; }
    public bool IsComplete => MissingCards == 0;

    public Straight(List<Card> cards, int minRank, int size) {
        Cards = cards.OrderBy(c => c.Rank).ToList();
        // Si el minRank es 14 (As), lo convertimos a 1
        MinRank = minRank == 14 ? 1 : minRank;
        Size = size;
        MissingCards = size - Cards.Count;
        IsFlush = Cards.Count > 0 && Cards.All(c => c.Suit == Cards[0].Suit);
        Gaps = CalculateGaps();
    }

    private int CalculateGaps() {
        if (Cards.Count <= 1) return 0;

        var gaps = 0;
        var sortedRanks = Cards.Select(c => c.Rank == 14 ? 1 : c.Rank)
            .OrderBy(r => r)
            .ToList();

        for (var i = 0; i < sortedRanks.Count - 1; i++) {
            if (sortedRanks[i + 1] - sortedRanks[i] > 1) {
                gaps++;
            }
        }
        return gaps;
    }

    public bool IsHigherThan(Straight other) {
        // Primero comparamos por número de cartas que faltan (menos cartas que faltan es mejor)
        if (MissingCards != other.MissingCards) return MissingCards < other.MissingCards;
        // Si tienen los cartas que faltan, comparamos por el rango máximo
        return MaxRank > other.MaxRank;
    }

    public bool IsEquivalent(Straight other) {
        if (other == null) return false;
        if (MinRank != other.MinRank || Size != other.Size ||
            MissingCards != other.MissingCards || Cards.Count != other.Cards.Count) return false;

        // Comparamos los rangos de las cartas
        return Cards.Select(c => c.Rank).SequenceEqual(other.Cards.Select(c => c.Rank));
    }

    public override string ToString() {
        return $"Straight [{MinRank}-{MaxRank}] Size={Size} MissingCards={MissingCards} Gaps={Gaps} Cards=[{string.Join(",", Cards)}]";
    }
}