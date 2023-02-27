namespace Veronenger.Character.Player;

public class PlayerUpdateHealthEvent {
    public float FromHealth { get; }
    public float ToHealth { get; }
    public float Max { get; }

    public PlayerUpdateHealthEvent(float fromHealth, float toHealth, float max) {
        FromHealth = fromHealth;
        ToHealth = toHealth;
        Max = max;
    }
}