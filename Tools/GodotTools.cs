using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using static Godot.Mathf;

namespace Betauer.Tools {
    public delegate void BodyOnArea2DSignalMethod(Node body, Area2D area2D);

    public class GodotTools {
        public static void ConnectBodyWithArea2D(Area2D area2D, BodyOnArea2DSignalMethod enter, BodyOnArea2DSignalMethod exit = null) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, (Godot.Object)enter.Target, enter.Method.Name, new Array {area2D});
            if (exit != null) {
                area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, (Godot.Object)exit.Target, exit.Method.Name, new Array {area2D});
            }
        }

        public static T FindChild<T>(Node parent) where T : class {
            var nodes = parent.GetChildren();
            var count = nodes.Count;
            for (var i = 0; i < count; i++) {
                var node = nodes[i];
                if (node is T nodeTyped) return nodeTyped;
            }

            return null;
        }

        public static List<T> FindAllChildren<T>(Node parent) where T : class {
            return Filter<T>(parent.GetChildren());
        }

        public static List<T> Filter<T>(IList nodes) where T : class {
            var children = new List<T>();
            var count = nodes.Count;
            for (var i = 0; i < count; i++) {
                var node = nodes[i];
                if (node is T nodeTyped) {
                    children.Add(nodeTyped);
                }
            }

            return children;
        }

        /*
         * Alinea las plataformas como si fueran una aguja de un reloj y la gira
         */
        public static void RotateAligned(List<PhysicsBody2D> nodes, float angle, float radius) {
            var count = nodes.Count;
            var spacing = radius / count;
            for (var i = 0; i < count; i++) {
                var newX = Sin(angle) * spacing * (i + 1);
                var newY = Cos(angle) * spacing * (i + 1);
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }

        /*
         * Distribuye por el circulo de manera espaciado y las gira
         */
        public static void RotateSpaced(List<PhysicsBody2D> nodes, float angle, Vector2 radius) {
            var count = nodes.Count;
            var spacing = Tau / count;
            for (var i = 0; i < count; i++) {
                var newX = Sin(spacing * i + angle) * radius.x;
                var newY = Cos(spacing * i + angle) * radius.y;
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }

    }
}