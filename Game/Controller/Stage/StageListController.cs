using System;
using Godot;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Stage {
    /**
     * Add this script to wrap all the stages in every scene.
     * Your Scene
     *    +-- StageList (with this script)
     *        +- Area2D
     *        |  +- RectangleShape2D
     *        +- Area2D with RectangleShape2D
     *        |  +- RectangleShape2D
     *        +- ...
     * Non Area2D elements will be ignored. Non RectangleShape2D collision shapes will be ignored too.
     */
    public class StageListController : Node {
        public override void _EnterTree() {
            foreach (var node in GetChildren()) {
                if (node is Area2D area2D) {
                    ValidateStageArea2D(area2D);
                    GameManager.Instance.StageManager.ConfigureStage(area2D);
                }
            }
        }

        private void ValidateStageArea2D(Area2D area2D) {
            var hasValidShape = false;
            foreach (var nodeChild in area2D.GetChildren()) {
                if (nodeChild is CollisionShape2D collisionShape2D && collisionShape2D.Shape is RectangleShape2D) {
                    if (hasValidShape) throw new Exception($"Stage {area2D.Name} with more than 1 RectangleShape2D");
                    hasValidShape = true;
                }
            }
            if (!hasValidShape) throw new Exception($"Stage {area2D.Name} needs 1 RectangleShape2D");
        }
    }
}