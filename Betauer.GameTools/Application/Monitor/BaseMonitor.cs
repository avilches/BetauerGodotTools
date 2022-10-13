using Godot;

namespace Betauer.Application.Monitor {
    public abstract class BaseMonitor : VBoxContainer {
        public Object? NodeToFollow { get; set; }
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
            var nodeToFollow = NodeToFollow ?? DebugOverlayOwner.NodeToFollow;
            if (nodeToFollow != null && !IsInstanceValid(nodeToFollow)) {
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