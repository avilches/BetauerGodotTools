namespace Betauer.Core.Deck.Hands;

public class PokerHandConfig(PokerHand prototype, int multiplier) {
    public PokerHand Prototype { get; } = prototype;
    public int Multiplier { get; } = multiplier;
}