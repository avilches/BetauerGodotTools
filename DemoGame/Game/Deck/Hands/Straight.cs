using System.Collections.Generic;
using System.Linq;

namespace Veronenger.Game.Deck.Hands;

public class Straight {
    public List<Card> Cards { get; }
    public int RankStart { get; }
    public int Size { get; }
    public int RankEnd => RankStart + Size - 1;
    public int MissingCards { get; }
    public int Gaps { get; }
    public bool IsFlush { get; }
    public bool IsComplete => MissingCards == 0;

    public Straight(PokerHandConfig config, List<Card> cards, int startRank) {
        Cards = cards.OrderBy(c => c.Rank).ToList();
        // Si el minRank es un (As), lo convertimos a 1
        RankStart = startRank == config.MaxRank ? 1 : startRank;
        Size = config.StraightSize;
        MissingCards = config.StraightSize - Cards.Count;
        IsFlush = Cards.Count > 0 && Cards.All(c => c.Suit == Cards[0].Suit);
        Gaps = CalculateGaps(config);
    }

    private int CalculateGaps(PokerHandConfig config) {
        if (Cards.Count <= 1) return 0;

        var gaps = 0;
        var sortedRanks = Cards.Select(c => c.Rank == config.MaxRank ? 1 : c.Rank)
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
        return RankEnd > other.RankEnd;
    }

    public bool IsEquivalent(Straight other) {
        if (other == null) return false;
        if (RankStart != other.RankStart || Size != other.Size ||
            MissingCards != other.MissingCards || Cards.Count != other.Cards.Count) return false;

        // Comparamos los rangos de las cartas
        return Cards.Select(c => c.Rank).SequenceEqual(other.Cards.Select(c => c.Rank));
    }

    public override string ToString() {
        return $"Straight [{RankStart}-{RankEnd}] Size={Size} MissingCards={MissingCards} Gaps={Gaps} Cards=[{string.Join(",", Cards)}]";
    }
}