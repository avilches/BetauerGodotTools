using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class PlayHistory {
    private readonly List<PlayedAction> _actions = [];
    
    public enum PlayedActionType {
        Play,
        Discard
    }

    public class PlayedAction(int id, PlayedActionType type, IReadOnlyList<Card> cards, PokerHand? playedHand, int handScore, int gameGameScore, int totalScore ) {
        public int Id { get; } = id;
        public PlayedActionType Type { get; } = type;
        public List<Card> Cards { get; } = cards.Select(card => card.Clone()).ToList();
        public PokerHand? PlayedHand { get; } = playedHand;
        public int HandScore { get; } = handScore;
        public int GameScore { get; } = gameGameScore;
        public int TotalScore { get; } = totalScore;
    }


    public void AddPlayAction(PokerHand hand, int handScore, int score, int totalScore) {
        var count = _actions.Count(ga => ga.Type == PlayedActionType.Play);
        _actions.Add(new PlayedAction(count, PlayedActionType.Play, hand.Cards, hand, handScore, score, totalScore));
    }

    public void AddDiscardAction(IReadOnlyList<Card> cards, int score, int totalScore) {
        var count = _actions.Count(ga => ga.Type == PlayedActionType.Discard);
        _actions.Add(new PlayedAction(count, PlayedActionType.Discard, cards, null, 0, score, totalScore));
    }

    public IReadOnlyList<PlayedAction> GetHistory() => _actions.AsReadOnly();

    public void Clear() {
        _actions.Clear();
    }
}