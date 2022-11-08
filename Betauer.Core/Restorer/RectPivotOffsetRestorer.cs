using Godot;

namespace Betauer.Core.Restorer {
    public class PivotOffsetRestorer : Restorer {
        private readonly Control _node;
        private Vector2 _originalPivotOffset;

        public PivotOffsetRestorer(Control node) {
            _node = node;
        }

        protected override void DoSave() {
            _originalPivotOffset = _node.PivotOffset;
        }

        protected override void DoRestore() {
            _node.PivotOffset = _originalPivotOffset;
        }
    }
}