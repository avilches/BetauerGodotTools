using Godot;

namespace Betauer.Restorer {
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
}