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
            PauseMode = PauseModeEnum.Process;
        }

        public DebugOverlay Overlay(Object target) {
            return GetChildren()
                       .OfType<DebugOverlay>()
                       .FirstOrDefault(d => d.Target == target)
                   ?? CreateOverlay().RemoveIfInvalid(target);
        }

        public DebugOverlay Overlay(Node2D follow) {
            return GetChildren()
                       .OfType<DebugOverlay>()
                       .FirstOrDefault(d => d.Target == follow)
                   ?? CreateOverlay().Follow(follow);
        }

        public DebugOverlay CreateOverlay() {
            var overlay = new DebugOverlay(this, _count++);
            AddChild(overlay);
            return overlay;
        }

        public override void _Input(InputEvent @event) {
            if (DebugOverlayAction != null && DebugOverlayAction.IsEventPressed(@event)) {
                if (@event.HasShift()) {
                    var children = GetChildren().OfType<DebugOverlay>().ToList();
                    _current = (_current + 1) % children.Count;
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