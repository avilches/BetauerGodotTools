using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Core.Restorer {
    public static class RestoreExtensions {
        // Node -> CanvasItem
        public static readonly string[] CanvasItemProperties = { "modulate", "self_modulate" };

        // Node -> CanvasItem -> Node2D
        public static readonly string[] Node2DProperties =
            CanvasItemProperties.Concat(new[] {
                "global_position", "transform",
            }).ToArray();

        // Node -> CanvasItem -> Node2D -> Sprite2D
        public static readonly string[] SpriteProperties = Node2DProperties.Concat(new[] { "offset",
            // Not tested
            "frame", "flip_h", "flip_v" }).ToArray();

        // Node -> CanvasItem -> Control
        public static readonly string[] ControlProperties = CanvasItemProperties.Concat(new [] {
            "rect_position",
            "rect_scale",
            "rect_rotation",
            "rect_pivot_offset",
            "focus_mode",
        }).ToArray();

        // Node -> CanvasItem -> Control -> BaseButton
        public static readonly string[] BaseButtonProperties = ControlProperties.Concat(new [] {
            "disabled",
        }).ToArray();


        public static FocusRestorer CreateFocusOwnerRestorer(this Control control) {
            return new FocusRestorer(control);
        }

        public static ChildFocusRestorer CreateChildFocusedRestorer(this Container container) {
            return new ChildFocusRestorer(container);
        }

        public static Restorer CreateRestorer(this Node node, params string[] properties) {
            return new PropertyNameRestorer(node, properties);
        }

        public static Restorer CreateRestorer(this Node node, params IProperty[] property) {
            return new PropertyRestorer(node, property);
        }

        public static MultiRestorer CreateMultiRestorer(this IEnumerable<Node> nodes, params string[] properties) {
            var multiRestorer = new MultiRestorer();
            nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer(properties)));
            return multiRestorer;
        }

        public static MultiRestorer CreateMultiRestorer(this IEnumerable<Node> nodes, params IProperty[] properties) {
            var multiRestorer = new MultiRestorer();
            nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer(properties)));
            return multiRestorer;
        }

        public static MultiRestorer CreateMultiRestorer(this IEnumerable<Node> nodes) {
            var multiRestorer = new MultiRestorer();
            nodes.ForEach(node => multiRestorer.Add(node.CreateRestorer()));
            return multiRestorer;
        }


        public static PropertyNameRestorer CreateRestorer(this Node node) {
            return node switch {
                Sprite2D sprite => new PropertyNameRestorer(node, SpriteProperties),
                Node2D node2D => new PropertyNameRestorer(node, Node2DProperties),
                BaseButton button => new PropertyNameRestorer(node, BaseButtonProperties),
                Control control => new PropertyNameRestorer(node, ControlProperties),
                _ => DummyPropertyNameRestorer.Instance
            };
        }

        private class DummyPropertyNameRestorer : PropertyNameRestorer {
            internal static readonly DummyPropertyNameRestorer Instance = new DummyPropertyNameRestorer();

            private DummyPropertyNameRestorer() : base(null) {
            }

            protected override void DoSave() {
            }

            protected override void DoRestore() {
            }
        }
    }
}