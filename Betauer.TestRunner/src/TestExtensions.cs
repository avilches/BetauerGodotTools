using Godot;

namespace Betauer.TestRunner;

public static class TestExtensions {
    internal static SceneTree SceneTree = null!;

    public static SignalAwaiter AwaitPhysicsFrame(this object _) =>
        AwaitPhysicsFrame();
        
    public static SignalAwaiter AwaitPhysicsFrame() =>
        SceneTree.ToSignal(SceneTree, "physics_frame");

    public static SignalAwaiter AwaitProcessFrame(this object _) =>
        AwaitProcessFrame();
        
    public static SignalAwaiter AwaitProcessFrame() =>
        SceneTree.ToSignal(SceneTree, "process_frame");
        
}