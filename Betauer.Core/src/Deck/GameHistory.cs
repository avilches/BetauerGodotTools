using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Deck;

public class GameHistory {
    private readonly List<GameAction> _actions = [];
    
    public enum GameActionType {
        Play,
        Discard
    }

    public class GameAction(int id, GameActionType type, IReadOnlyList<Card> cards, PokerHand? playedHand, int handScore, int gameGameScore, int totalScore ) {
        public int Id { get; } = id;
        public GameActionType Type { get; } = type;
        public List<Card> Cards { get; } = cards.Select(card => card.Clone()).ToList();
        public PokerHand? PlayedHand { get; } = playedHand;
        public int HandScore { get; } = handScore;
        public int GameScore { get; } = gameGameScore;
        public int TotalScore { get; } = totalScore;
    }


    public void AddPlayAction(PokerHand hand, int handScore, int score, int totalScore) {
        var count = _actions.Count(ga => ga.Type == GameActionType.Play);
        _actions.Add(new GameAction(count, GameActionType.Play, hand.Cards, hand, handScore, score, totalScore));
    }

    public void AddDiscardAction(IReadOnlyList<Card> cards, int score, int totalScore) {
        var count = _actions.Count(ga => ga.Type == GameActionType.Discard);
        _actions.Add(new GameAction(count, GameActionType.Discard, cards, null, 0, score, totalScore));
    }

    public IReadOnlyList<GameAction> GetHistory() => _actions.AsReadOnly();

    public void Clear() {
        _actions.Clear();
    }
}