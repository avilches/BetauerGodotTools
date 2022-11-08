using Godot;

namespace Betauer.Core.Restorer {
    public class FocusRestorer : Restorer {
        private Control? _focused;
        private readonly Control _control;

        public FocusRestorer(Control control) {
            _control = control;
        }

        protected override void DoSave() {
            _focused = _control.GetViewport().GuiGetFocusOwner();
        }

        protected override void DoRestore() {
            _focused?.GrabFocus();
        }
    }
}