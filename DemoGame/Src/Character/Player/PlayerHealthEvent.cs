namespace Veronenger.Character.Player;

public class PlayerHealthEvent {
    public float FromHealth { get; }
    public float ToHealth { get; }
    public float Max { get; }

    public PlayerHealthEvent(float fromHealth, float toHealth, float max) {
        FromHealth = fromHealth;
        ToHealth = toHealth;
        Max = max;
    }
}