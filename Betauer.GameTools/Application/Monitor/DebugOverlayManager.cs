using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Betauer.Application.Monitor {
    public class DebugOverlayManager : CanvasLayer {

        [Inject(Nullable = true)]
        public InputAction? DebugOverlayAction { get; set; }

        private int _count = 0;
        private int _current = -1;

        public override void _Ready() {
            Layer = 1000000;
        }

        public DebugOverlay CreateOverlay() {
            var overlay = new DebugOverlay(this, _count++);
            AddChild(overlay);
            return overlay;
        }

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                if (@event.HasShift()) {
                    _current = (_current+1) % _count;
                    var children = GetChildren().OfType<DebugOverlay>().ToList();
                    children.ForEach(overlay => overlay.Enable(overlay.Id == _current));
                } else {
                    var children = GetChildren().OfType<DebugOverlay>().ToList();
                    var enabled = !children[0].Visible;
                    children.ForEach(overlay => overlay.Enable(enabled));
                }
            }
        }
    }
}