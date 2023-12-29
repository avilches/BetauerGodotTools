namespace Veronenger.Game.Platform.Character.Player;

public readonly struct PlayerHealthEvent {
    public PlayerNode PlayerNode { get; }
    public float FromHealth { get; }
    public float ToHealth { get; }
    public float Max { get; }

    public PlayerHealthEvent(PlayerNode playerNode, float fromHealth, float toHealth, float max) {
        PlayerNode = playerNode;
        FromHealth = fromHealth;
        ToHealth = toHealth;
        Max = max;
    }
}