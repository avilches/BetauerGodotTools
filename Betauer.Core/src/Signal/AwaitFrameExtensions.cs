using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.Signal; 

public static partial class AwaitExtensions {

    public static SignalAwaiter AwaitPhysicsFrame(this Node node, Action? onComplete = null) =>
        node.GetTree().AwaitPhysicsFrame(onComplete);

    public static SignalAwaiter AwaitProcessFrame(this Node node, Action? onComplete = null) =>
        node.GetTree().AwaitProcessFrame(onComplete);

    public static Task AwaitPhysicsFrameTimes(this Node node, int times, Action? onComplete = null) =>
        AwaitPhysicsFrameTimes(node.GetTree(), times, onComplete);

    public static Task AwaitProcessFrameTimes(this Node node, int times, Action? onComplete = null) =>
        AwaitProcessFrameTimes(node.GetTree(), times, onComplete);

    public static async Task AwaitPhysicsFrameTimes(this SceneTree tree, int times, Action? onComplete = null) {
        while (times-- > 0) await tree.AwaitPhysicsFrame(onComplete);
    }

    public static async Task AwaitProcessFrameTimes(this SceneTree tree, int times, Action? onComplete = null) {
        while (times-- > 0) await tree.AwaitProcessFrame(onComplete);
    }
}