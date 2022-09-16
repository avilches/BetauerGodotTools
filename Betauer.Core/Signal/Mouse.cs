using Godot;

namespace Betauer.Signal {
    public static class Mouse {
        public abstract class MouseInside<T> where T : class {
            public SignalHandler MouseEnteredSignalHandler { get; protected set; }
            public SignalHandler MouseExitedSignalHandler { get; protected set; }

            public bool Inside { get; protected set; } = false;

            public T Connect() {
                MouseEnteredSignalHandler.Connect();
                MouseExitedSignalHandler.Connect();
                return this as T;
            }

            public T Disconnect() {
                MouseEnteredSignalHandler.Disconnect();
                MouseExitedSignalHandler.Disconnect();
                return this as T;
            }
        }

        public class InsideControl : MouseInside<InsideControl> {
            public readonly Control Target;

            public InsideControl(Control target) {
                Target = target;
                MouseEnteredSignalHandler = Target.OnMouseEntered(() => Inside = true);
                MouseExitedSignalHandler = Target.OnMouseExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideCollisionObject : MouseInside<InsideControl> {
            public readonly CollisionObject Target;

            public InsideCollisionObject(CollisionObject target) {
                Target = target;
                MouseEnteredSignalHandler = Target.OnMouseEntered(() => Inside = true);
                MouseExitedSignalHandler = Target.OnMouseExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideCollisionObject2D : MouseInside<InsideControl> {
            public readonly CollisionObject2D Target;

            public InsideCollisionObject2D(CollisionObject2D target) {
                Target = target;
                MouseEnteredSignalHandler = Target.OnMouseEntered(() => Inside = true);
                MouseExitedSignalHandler = Target.OnMouseExited(() => Inside = false);
            }
        }
    }
}