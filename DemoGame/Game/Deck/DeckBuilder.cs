using System.Collections.Generic;

namespace Veronenger.Game.Deck;

public class DeckBuilder {
    public static IEnumerable<Card> ClassicPokerDeck() {
        return Cards("SHDC", 2, 14);
    }

    public static IEnumerable<Card> Cards(string suits, int minRank, int maxRank) {
        foreach (var suit in suits) {
            for (var rank = minRank; rank <= maxRank; rank++) {
                yield return new Card(rank, suit);
            }
        }
    }

    public static IEnumerable<Card> Cards(char suit, int minRank, int maxRank) {
        for (var rank = minRank; rank <= maxRank; rank++) {
            yield return new Card(rank, suit);
        }
    }
}