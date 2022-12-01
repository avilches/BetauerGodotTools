using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Nodes {
    public static partial class NodeExtensions {
        public static void DisableAllShapes(this Node parent) {
            parent.EnableAllShapes(false);
        }

        public static void EnableAllShapes(this Node parent, bool enable = true) {
            parent.GetChildren().ForEach(node => {
                if (node is CollisionPolygon2D collisionPolygon2D) collisionPolygon2D.Disabled = !enable;
                else if (node is CollisionShape2D collisionShape2D) collisionShape2D.Disabled = !enable; 
                else if (node is CollisionPolygon3D collisionPolygon3D) collisionPolygon3D.Disabled = !enable;
                else if (node is CollisionShape3D collisionShape3D) collisionShape3D.Disabled = !enable; 
            });
        }

        public static T? GetNode<T>(this Node parent) where T : Node {
            return parent.GetChildren().OfType<T>().First();
        }

        public static Node AddChildDeferred(this Node parent, Node child) {
            parent.CallDeferred("add_child", child);
            return parent;
        }

        public static Node RemoveChildDeferred(this Node parent, Node child) {
            parent.CallDeferred("remove_child", child);
            return parent;
        }

        public static List<T> GetChildren<T>(this Node parent) where T : class {
            return parent.GetChildren().OfType<T>().ToList();
        }

        public static void DisableAllNotifications(this Node node) {
            SetAllNotifications(node, false);
        }

        public static void EnableAllNotifications(this Node node) {
            SetAllNotifications(node, true);
        }

        public static void SetAllNotifications(this Node node, bool enable) {
            node.SetProcess(enable);
            node.SetProcessInput(enable);
            node.SetProcessUnhandledInput(enable);
            node.SetProcessUnhandledKeyInput(enable);
            node.SetPhysicsProcess(enable);
        }

        public static void FocusEnable(this BaseButton control) {
            SetFocusDisabled(control, false);
        }

        public static void FocusDisable(this BaseButton control) {
            SetFocusDisabled(control, true);
        }

        public static void SetFocusDisabled(this BaseButton control, bool isDisabled) {
            control.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;
            control.Disabled = isDisabled;
        }
    }
}