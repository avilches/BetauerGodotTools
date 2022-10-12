using Godot;

namespace Betauer.Application.Monitor {
    public abstract class BaseMonitor : VBoxContainer {
        public Object? Target;

        public bool IsEnabled => Visible;

        public void Enable(bool enabled = true) {
            Visible = enabled;
            SetProcess(enabled);
        }

        public void Disable() {
            Enable(false);
        }
        
        public override void _PhysicsProcess(float delta) {
            if (Target != null && !IsInstanceValid(Target)) {
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