using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.Signal; 

public static partial class AwaitExtensions {

    public static SignalAwaiter AwaitPhysicsFrame(this Node node, Action? onComplete = null) =>
        node.GetTree().AwaitPhysicsFrame(onComplete);

    public static SignalAwaiter AwaitProcessFrame(this Node node, Action? onComplete = null) =>
        node.GetTree().AwaitProcessFrame(onComplete);

    public static async Task AwaitPhysicsFrame(this Node node, int times, Action? onComplete = null) {
        var tree = node.GetTree();
        while (times-- > 0) await tree.AwaitPhysicsFrame(onComplete);
    }

    public static async Task AwaitProcessFrame(this Node node, int times, Action? onComplete = null) {
        var tree = node.GetTree();
        while (times-- > 0) await tree.AwaitProcessFrame(onComplete);
    }
}