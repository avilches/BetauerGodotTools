using Godot;

namespace Veronenger.Game.Dungeon.World;

/// <summary>
/// Representa una acción realizada por una entidad
/// </summary>
public class ActionCommand {
    public ActionType Type { get; }
    public ActionTypeConfig Config => ActionTypeConfig.Get(Type);
    public Entity Actor { get; }
    public Entity? Target { get; }

    public int EnergyCost { get; set; }

    public ActionCommand(ActionType type, Entity actor, Entity? target = null) {
        Type = type;
        Actor = actor;
        Target = target;
        EnergyCost = Config.EnergyCost;
    }

    public void ModifyEnergyCost(float multiplier) {
        EnergyCost = Mathf.RoundToInt(multiplier * EnergyCost);
    }
}