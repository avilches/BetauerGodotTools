using Godot;

namespace Veronenger.Game.Dungeon.World;

/// <summary>
/// Representa una acción realizada por una entidad
/// </summary>
public class ActionCommand {
    public ActionType Type { get; }
    public ActionTypeConfig Config => ActionTypeConfig.Get(Type);

    public EntityBase? Target { get; }
    public Vector2I? TargetPosition { get; set; }

    public int EnergyCost { get; set; }

    public ActionCommand(ActionType type, EntityBase? target = null, Vector2I? targetPosition = null) {
        Type = type;
        Target = target;
        TargetPosition = targetPosition;
        EnergyCost = Config.EnergyCost;
    }

    public void ModifyEnergyCost(float multiplier) {
        EnergyCost = Mathf.RoundToInt(multiplier * EnergyCost);
    }
}