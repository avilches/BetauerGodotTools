using Godot;

namespace Betauer {
    public static partial class SignalExtensions {
        public static SignalAwaiter AwaitPhysicsFrame(this Node node) {
            return AwaitPhysicsFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitIdleFrame(this Node node) {
            return AwaitIdleFrame(node.GetTree());
        }

        public static SignalAwaiter AwaitPhysicsFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, SignalConstants.SceneTree_PhysicsFrameSignal);
        }

        public static SignalAwaiter AwaitIdleFrame(this SceneTree sceneTree) {
            return sceneTree.ToSignal(sceneTree, SignalConstants.SceneTree_IdleFrameSignal);
        }
    }

}