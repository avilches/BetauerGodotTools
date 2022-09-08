using Betauer.Nodes;
using Godot;

namespace Betauer.Restorer {
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
}