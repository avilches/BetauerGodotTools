using Godot;

namespace Betauer.Signal {
    public class MouseInsideControlStatus {
        public readonly Control Target;
        public readonly SignalHandler MouseEnteredSignalHandler;
        public readonly SignalHandler MouseExitedSignalHandler;

        public bool MouseInside { get; private set; } = false;

        public MouseInsideControlStatus(Control target) {
            Target = target;
            MouseEnteredSignalHandler = Target.OnMouseEntered(() => MouseInside = true);
            MouseExitedSignalHandler = Target.OnMouseExited(() => MouseInside = false);
        }
        
        public MouseInsideControlStatus Connect() {
            MouseEnteredSignalHandler.Connect();
            MouseExitedSignalHandler.Connect();
            return this;
        }

        public MouseInsideControlStatus Disconnect() {
            MouseEnteredSignalHandler.Disconnect();
            MouseExitedSignalHandler.Disconnect();
            return this;
        }
    }
}