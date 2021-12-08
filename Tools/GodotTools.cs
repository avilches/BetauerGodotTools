using System.Collections;
using System.Collections.Generic;
using Godot;
using static Godot.Mathf;

namespace Tools {
    public class GodotTools {
        public static T FindChild<T>(Node parent) where T : class {
            var nodes = parent.GetChildren();
            var count = nodes.Count;
            for (var i = 0; i < count; i++) {
                if (nodes[i] is T result) {
                    return result;
                }
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
                if (nodes[i] is T nodeTyped) {
                    children.Add(nodeTyped);
                }
            }

            return children;
        }

        public static void DisableAllNotifications(Node node) {
            node.SetProcess(false);
            node.SetProcessInput(false);
            node.SetProcessUnhandledInput(false);
            node.SetProcessUnhandledKeyInput(false);
            node.SetPhysicsProcess(false);
        }
    }

    public static class AnimationTools {
        /*
         * Alinea las plataformas como si fueran una aguja de un reloj y la gira. La primera primera plataforma
         * mantiene su posicion y las demás se van espaciando hasta llegar al radius
         */
        public static void RotateAligned(List<PhysicsBody2D> nodes, float angle, float radius,
            float initialOffset = 20) {
            var count = nodes.Count;
            var spacing = radius / count;
            for (var i = 0; i < count; i++) {
                float offset = ((spacing * i) + initialOffset);
                var newX = Sin(angle) * offset;
                var newY = Cos(angle) * offset;
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