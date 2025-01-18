using System.Collections.Generic;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game.Deck;

public class GameRunState {

    public readonly Dictionary<PokerHandType, int> HandLevels = new();
    public readonly Dictionary<PokerHandType, int> HandTypePlayed = new();

    public int Discards { get; set; } = 0;
    public int HandsPlayed { get; set; } = 0;
    public int CardsPlayed { get; set; } = 0;
    public int CardsDiscarded { get; set; } = 0;

    public int GetPokerHandLevel(PokerHandType hand) {
        return HandLevels.GetValueOrDefault(hand, 0);
    }

    public void SetPokerHandLevel(PokerHandType hand, int level) {
        HandLevels[hand] = level;
    }

    public void AddPlayAction(PokerHand hand, IReadOnlyList<Card> cards) {
        HandsPlayed++;
        CardsPlayed += cards.Count;
        var times = HandTypePlayed.GetValueOrDefault(hand.HandType, 0);
        HandTypePlayed[hand.HandType] = times + 1;
    }

    public void AddDiscardAction(IReadOnlyList<Card> cards) {
        Discards++;
        CardsDiscarded += cards.Count;
    }
}