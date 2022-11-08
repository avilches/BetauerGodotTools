using Godot;

namespace Betauer.Signal {
    public abstract class EnterExitHandler<T> where T : class {
        public SignalHandler EnteredSignalHandler { get; protected set; }
        public SignalHandler ExitedSignalHandler { get; protected set; }

        public bool Inside { get; protected set; } = false;

        public T Connect() {
            EnteredSignalHandler.Connect();
            ExitedSignalHandler.Connect();
            return this as T;
        }

        public T Disconnect() {
            EnteredSignalHandler.Disconnect();
            ExitedSignalHandler.Disconnect();
            return this as T;
        }
    }

    public static class IsVisibleOnScreenNotifier2D {
        
        // TODO: this class is not tested
        public class InsideScreen : EnterExitHandler<InsideScreen> {
            public readonly VisibleOnScreenNotifier2D Origin;

            public InsideScreen(VisibleOnScreenNotifier2D origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnScreenEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnScreenExited(() => Inside = false);
            }
        }
    }

    public static class IsVisibleOnScreenNotifier3D {
        
        // TODO: this class is not tested
        public class InsideScreen : EnterExitHandler<InsideScreen> {
            public readonly VisibleOnScreenNotifier3D Origin;

            public InsideScreen(VisibleOnScreenNotifier3D origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnScreenEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnScreenExited(() => Inside = false);
            }
        }
    }

    public static class Mouse {
        public class InsideControl : EnterExitHandler<InsideControl> {
            public readonly Control Origin;

            public InsideControl(Control origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnMouseEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnMouseExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideCollisionObject3D : EnterExitHandler<InsideControl> {
            public readonly CollisionObject3D Origin;

            public InsideCollisionObject3D(CollisionObject3D origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnMouseEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnMouseExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideWindow : EnterExitHandler<InsideControl> {
            public readonly Window Origin;

            public InsideWindow(Window origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnMouseEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnMouseExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideCollisionObject2D : EnterExitHandler<InsideControl> {
            public readonly CollisionObject2D Origin;

            public InsideCollisionObject2D(CollisionObject2D origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnMouseEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnMouseExited(() => Inside = false);
            }
        }
    }}