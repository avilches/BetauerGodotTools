using System;
using Godot;

namespace Game.Tools.Area {

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
    public class StageList : Node {
        public override void _EnterTree() {
            foreach (var node in GetChildren()) {
                if (node is Area2D area2D) {
                    RegisterArea2D(area2D);
                }
            }
        }

        private static void RegisterArea2D(Area2D area2D) {
            var added = false;
            foreach (var nodeChild in area2D.GetChildren()) {
                if (nodeChild is CollisionShape2D collisionShape2D && collisionShape2D.Shape is RectangleShape2D) {
                    if (added) throw new Exception("Stage " + area2D.Name + " with more than one collision shape!");
                    GameManager.Instance.AreaManager.RegisterStage(area2D, collisionShape2D);
                    added = true;
                }
            }
            if (!added) throw new Exception("Stage " + area2D.Name + " with 0 valid colliders");
        }
    }
}