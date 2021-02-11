using System;
using Godot;

namespace Betauer.Tools.Area {
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
                if (nodeChild is CollisionShape2D collisionShape2D) {
                    if (collisionShape2D.Shape is RectangleShape2D) {
                        if (added) {
                            throw new Exception("Stage " + area2D.Name + " with more than one collision shape!");
                        }

                        GameManager.Instance.AreaManager.RegisterStage(area2D, collisionShape2D);
                        added = true;
                    } else {
                        throw new Exception("Stage " + area2D.Name + " with non rectangle shape (" +
                                            collisionShape2D.Shape.GetType().FullName + ") detected");
                    }
                }
            }

            if (!added) {
                throw new Exception("Stage " + area2D.Name + " with 0 valid colliders");
            }
        }
    }
}