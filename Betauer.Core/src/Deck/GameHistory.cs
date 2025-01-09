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

    public class GameAction(GameActionType type, IReadOnlyList<Card> cards, PokerHand? playedHand = null, int score = 0) {
        public GameActionType Type { get; } = type;
        public List<Card> Cards { get; } = cards.Select(card => card.Clone()).ToList();
        public PokerHand? PlayedHand { get; } = playedHand;
        public int Score { get; } = score;
    }

    public void AddPlayAction(PokerHand hand, int score) {
        _actions.Add(new GameAction(GameActionType.Play, hand.Cards, hand, score));
    }

    public void AddDiscardAction(IReadOnlyList<Card> cards) {
        _actions.Add(new GameAction(GameActionType.Discard, cards));
    }

    public IReadOnlyList<GameAction> GetHistory() => _actions.AsReadOnly();

    public void Clear() {
        _actions.Clear();
    }
}