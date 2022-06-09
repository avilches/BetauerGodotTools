using System;
using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Game.Managers;

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
        [Inject] public StageManager StageManager;

        public override void _Ready() {
            foreach (var node in GetChildren()) {
                if (node is Area2D area2D) {
                    ValidateStageArea2D(area2D);
                    StageManager.ConfigureStage(area2D);
                }
            }
        }

        private void ValidateStageArea2D(Area2D area2D) {
            var childCount = area2D.GetChildCount();
            if (childCount != 1) {
                throw new Exception(
                    $"Stage {area2D.Name} has {childCount} children. It should have only 1 RectangleShape2D");
            }
            var nodeChild = area2D.GetChild(0);
            if (nodeChild is CollisionShape2D collisionShape2D && collisionShape2D.Shape is RectangleShape2D) {
                return;
            }
            throw new Exception(
                $"Stage {area2D.Name}/{nodeChild.Name} is not a CollisionShape2D with a RectangleShape2D shape");
        }
    }
}