using Godot;

namespace Betauer.Animation {
    public interface IRestorer {
        public void Rollback();
    }

    public static class RestoreExtensions {
        public static IRestorer Save(this Node2D node) {
            return new Node2DRestorer(node);
        }

        public static IRestorer Save(this Control node) {
            return new ControlRestorer(node);
        }
    }

    public class Node2DRestorer : IRestorer {
        private readonly IRestorer? _pivotOffsetRestorer;
        private readonly Color _modulate;
        private readonly Color _selfModulate;
        private readonly Transform2D _transform;
        private readonly Node2D _node2D;

        public Node2DRestorer(Node2D node2D) {
            _node2D = node2D;
            _pivotOffsetRestorer = node2D is Sprite sprite ? new SpritePivotOffsetRestorer(sprite) : null;
            _modulate = node2D.Modulate;
            _selfModulate = node2D.SelfModulate;
            _transform = node2D.Transform;
        }

        public void Rollback() {
            _node2D.Modulate = _modulate;
            _node2D.SelfModulate = _selfModulate;
            _node2D.Transform = _transform;
            _pivotOffsetRestorer?.Rollback();
        }
    }

    public class ControlRestorer : IRestorer {
        private readonly IRestorer _pivotOffsetRestorer;
        private readonly Color _modulate;
        private readonly Color _selfModulate;
        private readonly Vector2 _position;
        private readonly Vector2 _scale;
        private readonly float _rotation;
        private readonly Control _control;

        public ControlRestorer(Control control) {
            _control = control;
            _pivotOffsetRestorer = new RectPivotOffsetRestorer(control);
            _modulate = control.Modulate;
            _selfModulate = control.SelfModulate;
            _position = control.RectPosition;
            _scale = control.RectScale;
            _rotation = control.RectRotation;
        }

        public void Rollback() {
            _control.Modulate = _modulate;
            _control.SelfModulate = _selfModulate;
            _control.RectPosition = _position;
            _control.RectScale = _scale;
            _control.RectRotation = _rotation;
            _pivotOffsetRestorer.Rollback();
        }
    }


    public class RectPivotOffsetRestorer : IRestorer {
        private readonly Control _node;
        private readonly Vector2 _originalRectPivotOffset;

        public RectPivotOffsetRestorer(Control node) {
            _node = node;
            _originalRectPivotOffset = node.RectPivotOffset;
        }

        public void Rollback() {
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

        public void Rollback() {
            _node.Offset = _offset;
            _node.GlobalPosition = _globalPosition;
        }
    }

    public class DummyRestorer : IRestorer {
        public static readonly IRestorer Instance = new DummyRestorer();

        private DummyRestorer() {
        }

        public void Rollback() {
        }
    }
}