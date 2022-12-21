using Godot;

namespace Betauer.Core.Restorer {
    public class SpritePivotOffsetRestorer : Restorer {
        private readonly Sprite2D _node;
        private Vector2 _offset;
        private Vector2 _globalPosition;

        public SpritePivotOffsetRestorer(Sprite2D node) {
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
}