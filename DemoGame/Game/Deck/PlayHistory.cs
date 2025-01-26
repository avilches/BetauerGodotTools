using System.Collections.Generic;
using System.Linq;
using Veronenger.Game.Deck.Hands;

namespace Veronenger.Game.Deck;

public class PlayHistory {
    private readonly List<PlayedAction> _actions = [];
    
    public enum PlayedActionType {
        Play,
        Discard
    }

    public class PlayedAction(int id, PlayedActionType type, IReadOnlyList<Card> cards, PokerHand? playedHand, long handScore, long gameGameScore, long totalScore) {
        public int Id { get; } = id;
        public PlayedActionType Type { get; } = type;
        public List<Card> Cards { get; } = cards.Select(card => card.Clone()).ToList();
        public PokerHand? PlayedHand { get; } = playedHand;
        public long HandScore { get; } = handScore;
        public long GameScore { get; } = gameGameScore;
        public long TotalScore { get; } = totalScore;

        public override string ToString() {
            return $"{(Type == PlayedActionType.Play ? PlayedHand?.Name:"Discard")} ({string.Join(", ", Cards)}) +{HandScore} ({GameScore}/{TotalScore})";
        }
    }


    public void AddPlayAction(PokerHand hand, IReadOnlyList<Card> cards, long handScore, long score, long totalScore) {
        var count = _actions.Count(ga => ga.Type == PlayedActionType.Play);
        _actions.Add(new PlayedAction(count, PlayedActionType.Play, cards, hand, handScore, score, totalScore));
    }

    public void AddDiscardAction(IReadOnlyList<Card> cards, long score, long totalScore) {
        var count = _actions.Count(ga => ga.Type == PlayedActionType.Discard);
        _actions.Add(new PlayedAction(count, PlayedActionType.Discard, cards, null, 0, score, totalScore));
    }

    public IReadOnlyList<PlayedAction> GetHistory() => _actions.AsReadOnly();

    public void Clear() {
        _actions.Clear();
    }
}