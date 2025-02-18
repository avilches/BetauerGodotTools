using System;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class PlayerEntity(string name, EntityStats stats) : SchedulingEntityBase(name, stats) {
    public override void DoExecute(ActionCommand actionCommand) {
        if (actionCommand.Type == ActionType.Walk) {
            MoveTo(actionCommand.TargetPosition ?? throw new ArgumentNullException(nameof(actionCommand.TargetPosition)));
        }
    }

    public void MoveTo(Vector2I position) {
        Location.Position = position;
    }
}