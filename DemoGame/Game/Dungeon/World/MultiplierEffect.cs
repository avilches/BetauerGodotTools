namespace Veronenger.Game.Dungeon.World;

/// <summary>
/// Efectos de estado que pueden afectar a una entidad
/// </summary>
public class MultiplierEffect {
    /// <summary>
    /// Efectos de estado que pueden afectar a una entidad
    /// </summary>
    private MultiplierEffect(string name, float multiplier, int ticks) {
        Name = name;
        Multiplier = multiplier;
        RemainingTicks = ticks;
    }

    public string Name { get; }
    public float Multiplier { get; }
    public int RemainingTicks { get; set; }

    public static MultiplierEffect Ticks(string name, float multiplier, int ticks) {
        return new MultiplierEffect(name, multiplier, ticks);
    }
}