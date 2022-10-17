using Godot;

namespace Betauer.Application.Monitor {
    public abstract class BaseMonitor : VBoxContainer {
        public Object? Watch { get; set; }
        public DebugOverlay DebugOverlayOwner { get; set; }
        public bool IsEnabled => Visible;

        public void Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
        }

        public void Disable() {
            Enable(false);
        }
        
        public override void _PhysicsProcess(float delta) {
            var watching = Watch ?? DebugOverlayOwner.Target;
            if (watching != null && !IsInstanceValid(watching)) {
                QueueFree();
            } else if (!Visible) {
                Disable();
            } else {
                Process(delta);
            }
        }

        public abstract void Process(float delta);
    }
}