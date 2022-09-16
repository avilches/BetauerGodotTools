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

    public static class IsVisibilityNotifier2D {
        
        // TODO: this class is not tested
        public class InsideScreen : EnterExitHandler<InsideScreen> {
            public readonly VisibilityNotifier2D Origin;

            public InsideScreen(VisibilityNotifier2D origin) {
                Origin = origin;
                EnteredSignalHandler = Origin.OnScreenEntered(() => Inside = true);
                ExitedSignalHandler = Origin.OnScreenExited(() => Inside = false);
            }
        }
        
        // TODO: this class is not tested
        public class InsideViewport : EnterExitHandler<InsideViewport> {
            public readonly VisibilityNotifier2D Origin;
            public readonly Viewport Viewport;

            public InsideViewport(VisibilityNotifier2D origin, Viewport viewport) {
                Origin = origin;
                Viewport = viewport;
                EnteredSignalHandler = Origin.OnViewportEntered((v) => {
                    if (viewport == v) Inside = true;
                });
                ExitedSignalHandler = Origin.OnViewportExited((v) => {
                    if (viewport == v) Inside = false;
                    Inside = false;
                });
            }

            public InsideViewport DisconnectIfInvalidViewport() {
                EnteredSignalHandler.DisconnectIfInvalid(Viewport);
                ExitedSignalHandler.DisconnectIfInvalid(Viewport);
                return this;
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
        public class InsideCollisionObject : EnterExitHandler<InsideControl> {
            public readonly CollisionObject Origin;

            public InsideCollisionObject(CollisionObject origin) {
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