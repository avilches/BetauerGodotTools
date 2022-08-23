using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Betauer.Nodes;
using Godot;

namespace Betauer {
    public static class RestoreExtensions {
        // CanvasItem -> Node
        public static readonly string[] CanvasItemProperties = { "modulate", "self_modulate" };

        // Node2D -> CanvasItem -> Node
        public static readonly string[] Node2DProperties =
            CanvasItemProperties.Concat(new[] {
                "global_position", "transform",
            }).ToArray();

        // Sprite -> Node2D -> CanvasItem -> Node
        public static readonly string[] SpriteProperties = Node2DProperties.Concat(new[] { "offset",
            // Not tested
            "frame", "flip_h", "flip_v" }).ToArray();

        // Control -> CanvasItem -> Node
        public static readonly string[] ControlProperties = CanvasItemProperties.Concat(new [] {
            "rect_position",
            "rect_scale",
            "rect_rotation",
            "rect_pivot_offset",
        }).ToArray();


        public static FocusRestorer CreateFocusRestorer(this Control control) {
            return new FocusRestorer(control);
        }

        public static ChildFocusRestorer CreateChildFocusRestorer(this Container container) {
            return new ChildFocusRestorer(container);
        }

        public static Restorer CreateRestorer(this Node node, params string[] property) {
            if (property.Length > 0) {
                return new PropertyRestorer(node, property);
            }
            return node switch {
                Sprite sprite => new PropertyRestorer(node, SpriteProperties),
                Node2D node2D => new PropertyRestorer(node, Node2DProperties),
                Control control => new PropertyRestorer(node, ControlProperties),
                _ => DummyRestorer.Instance
            };
        }
    }

    public abstract class Restorer {
        public bool HasSavedState { get; private set; } = false;

        public Restorer Save() {
            DoSave();
            HasSavedState = true;
            return this;
        }

        /// <summary>
        /// Warning: always call to Restore() in a idle_frame, not in a tween/signal callback
        /// <code>
        /// await this.AwaitIdleFrame();
        /// restorer.Restore();
        /// </code>
        /// </summary>
        /// <returns></returns>
        public Restorer Restore() {
            if (!HasSavedState) {
#if DEBUG                
                GD.PushWarning("Restoring without saving before");
#endif                
                return this;
            }
            DoRestore();
            return this;
        }

        protected abstract void DoSave();
        protected abstract void DoRestore();
    }

    public class MultiRestorer : Restorer {
        public readonly List<Restorer> Restorers;

        public MultiRestorer(IEnumerable<Node> nodes, params string[] property) {
            Restorers = nodes.Select(node => node.CreateRestorer(property)).ToList();
        }

        public MultiRestorer(params Node[] nodes) {
            Restorers = nodes.Select(node => node.CreateRestorer()).ToList();
        }

        public MultiRestorer(params Restorer[] restorers) {
            Restorers = restorers.ToList();
        }

        public MultiRestorer Add(IEnumerable<Node> nodes, params string[] property) {
            foreach (var r in nodes.Select(node => node.CreateRestorer(property))) Restorers.Add(r);
            return this;
        }

        public MultiRestorer Add(params Node[] nodes) {
            foreach (var r in nodes.Select(node => node.CreateRestorer())) Restorers.Add(r);
            return this;
        }

        public MultiRestorer Add(params Restorer[] restorer) {
            foreach (var r in restorer) Restorers.Add(r);
            return this;
        }

        public MultiRestorer Add(IEnumerable<Restorer> toList) {
            Restorers.AddRange(toList);
            return this;
        }

        public MultiRestorer AddFocusRestorer(Control control) {
            Restorers.Add(control.CreateFocusRestorer());
            return this;
        }

        public MultiRestorer AddChildFocusRestorer(Container container) {
            Restorers.Add(container.CreateChildFocusRestorer());
            return this;
        }

        protected override void DoSave() {
            foreach (var restorer in Restorers) restorer.Save();
        }

        protected override void DoRestore() {
            foreach (var restorer in Restorers) restorer.Restore();
        }
    }

    public class FocusRestorer : Restorer {
        private Control? _focused;
        private readonly Control _control;

        public FocusRestorer(Control control) {
            _control = control;
        }

        protected override void DoSave() {
            _focused = _control.GetFocusOwner();
        }

        protected override void DoRestore() {
            _focused?.GrabFocus();
        }
    }

    public class ChildFocusRestorer : Restorer {
        private Control? _focused;
        private readonly Container _container;

        public ChildFocusRestorer(Container container) {
            _container = container;
        }

        protected override void DoSave() {
            _focused = _container.GetChildFocused<Control>();
        }

        protected override void DoRestore() {
            _focused?.GrabFocus();
        }
    }

    public class PropertyRestorer : Restorer {
        private readonly Node _node;
        private object[] _values;
        private readonly string[] _properties;

        public PropertyRestorer(Node node, params string[] properties) {
            _node = node;
            _properties = properties;
        }

        protected override void DoSave() {
            _values = _properties.Select(property => _node.GetIndexed(property)).ToArray();
        }

        protected override void DoRestore() {
            foreach (var tuple in _properties.Zip(_values, Tuple.Create)) {
                _node.SetIndexed(tuple.Item1, tuple.Item2);
            }
        }
    }
    
    public class RectPivotOffsetRestorer : Restorer {
        private readonly Control _node;
        private Vector2 _originalRectPivotOffset;

        public RectPivotOffsetRestorer(Control node) {
            _node = node;
        }

        protected override void DoSave() {
            _originalRectPivotOffset = _node.RectPivotOffset;
        }

        protected override void DoRestore() {
            _node.RectPivotOffset = _originalRectPivotOffset;
        }
    }

    public class SpritePivotOffsetRestorer : Restorer {
        private readonly Sprite _node;
        private Vector2 _offset;
        private Vector2 _globalPosition;

        public SpritePivotOffsetRestorer(Sprite node) {
            _node = node;
        }

        protected override void DoSave() {
            _offset = _node.Offset;
            _globalPosition = _node.GlobalPosition;
        }

        protected override void DoRestore() {
            _node.Offset = _offset;
            _node.GlobalPosition = _globalPosition;
        }
    }

    public class DummyRestorer : Restorer {
        public static readonly Restorer Instance = new DummyRestorer();

        private DummyRestorer() {
        }

        protected override void DoSave() {
        }

        protected override void DoRestore() {
        }
    }
}