namespace Veronenger.Game.Dungeon.World;

/// <summary>
/// Representa una acción realizada por una entidad
/// </summary>
public class EntityAction {
    public ActionConfig Config { get; }
    public Entity Actor { get; }
    public Entity Target { get; }
    public int EnergyCost { get; private set; }
    public int AnimationDurationMillis { get; set; } = 1;

    public EntityAction(ActionType type, Entity actor, Entity target = null) {
        Config = ActionConfig.Get(type);
        Actor = actor;
        Target = target;
        EnergyCost = Config.EnergyCost;
    }

    // Modificadores pueden alterar el coste de energía
    public void ModifyEnergyCost(float multiplier) {
        EnergyCost = (int)(EnergyCost * multiplier);
    }
}