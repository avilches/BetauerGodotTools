using Betauer.Signal;
using Godot;

namespace Betauer {
    public static class SceneTreeHolder {
        public static SceneTree SceneTree;

        public static SignalAwaiter AwaitPhysicsFrame() {
            return AwaitExtensions.AwaitPhysicsFrame(SceneTree);
        }

        public static SignalAwaiter AwaitIdleFrame() {
            return AwaitExtensions.AwaitIdleFrame(SceneTree);
        }

        public static SignalAwaiter AwaitPhysicsFrame(this object _) =>
            AwaitPhysicsFrame();

        public static SignalAwaiter AwaitIdleFrame(this object _) =>
            AwaitIdleFrame();

    }
}