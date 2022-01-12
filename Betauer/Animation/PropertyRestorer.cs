using Godot;

namespace Betauer.Animation {
    public interface IRestorer {
        public void Restore();
    }

    public static class RestoreExtensions {
        public static IRestorer Save(this Node2D node) {
            return new Node2DRestorer(node).Save();
        }

        public static IRestorer Save(this Control node) {
            return new ControlRestorer(node).Save();
        }
    }

    public class Node2DRestorer : IRestorer {
        private IRestorer? _pivotOffsetRestorer;
        private Color _modulate;
        private Color _selfModulate;
        private Transform2D _transform;
        private readonly Node2D _node2D;
        private bool _saved = false;

        public Node2DRestorer(Node2D node2D) {
            _node2D = node2D;
        }

        public Node2DRestorer Save() {
            _pivotOffsetRestorer = _node2D is Sprite sprite ? new SpritePivotOffsetRestorer(sprite) : null;
            _modulate = _node2D.Modulate;
            _selfModulate = _node2D.SelfModulate;
            _transform = _node2D.Transform;
            _saved = true;
            return this;
        }

        public void Restore() {
            if (!_saved) return;
            _pivotOffsetRestorer?.Restore();
            _node2D.Modulate = _modulate;
            _node2D.SelfModulate = _selfModulate;
            _node2D.Transform = _transform;
        }
    }

    public class ControlRestorer : IRestorer {
        private IRestorer _pivotOffsetRestorer;
        private Color _modulate;
        private Color _selfModulate;
        private Vector2 _position;
        private Vector2 _scale;
        private float _rotation;
        private readonly Control _control;
        public bool HasSavedState { get; private set; } = false;

        public ControlRestorer(Control control) {
            _control = control;
        }

        public ControlRestorer Save() {
            _pivotOffsetRestorer = new RectPivotOffsetRestorer(_control);
            _modulate = _control.Modulate;
            _selfModulate = _control.SelfModulate;
            _position = _control.RectPosition;
            _scale = _control.RectScale;
            _rotation = _control.RectRotation;
            HasSavedState = true;
            return this;
        }

        public void Restore() {
            if (!HasSavedState) return;
            _control.Modulate = _modulate;
            _control.SelfModulate = _selfModulate;
            _control.RectPosition = _position;
            _control.RectScale = _scale;
            _control.RectRotation = _rotation;
            _pivotOffsetRestorer.Restore();
        }
    }

    public class RectPivotOffsetRestorer : IRestorer {
        private readonly Control _node;
        private readonly Vector2 _originalRectPivotOffset;

        public RectPivotOffsetRestorer(Control node) {
            _node = node;
            _originalRectPivotOffset = node.RectPivotOffset;
        }

        public void Restore() {
            _node.RectPivotOffset = _originalRectPivotOffset;
        }
    }

    public class SpritePivotOffsetRestorer : IRestorer {
        private readonly Sprite _node;
        private readonly Vector2 _offset;
        private readonly Vector2 _globalPosition;

        public SpritePivotOffsetRestorer(Sprite node) {
            _node = node;
            _offset = node.Offset;
            _globalPosition = node.GlobalPosition;
        }

        public void Restore() {
            _node.Offset = _offset;
            _node.GlobalPosition = _globalPosition;
        }
    }

    public class DummyRestorer : IRestorer {
        public static readonly IRestorer Instance = new DummyRestorer();

        private DummyRestorer() {
        }

        public void Restore() {
        }
    }
}