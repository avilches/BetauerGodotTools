namespace Veronenger.Character.Player;

public class PlayerUpdateHealthEvent {
    public PlayerNode Player { get; }

    public float FromHealth { get; }
    public float ToHealth { get; }
    public float Max { get; }

    public PlayerUpdateHealthEvent(PlayerNode player, float fromHealth, float toHealth, float max) {
        Player = player;
        FromHealth = fromHealth;
        ToHealth = toHealth;
        Max = max;
    }
}