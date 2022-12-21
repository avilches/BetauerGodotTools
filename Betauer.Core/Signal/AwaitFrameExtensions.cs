using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Core.Signal {
    public static partial class AwaitExtensions {
        
        public static SignalAwaiter AwaitPhysicsFrame(this Node node) =>
            node.GetTree().ToSignal(node.GetTree(), "physics_frame");

        public static SignalAwaiter AwaitProcessFrame(this Node node) =>
            node.GetTree().ToSignal(node.GetTree(), "process_frame");

        public static async Task AwaitPhysicsFrame(this Node node, int times) {
            times = Math.Max(1, times);
            var tree = node.GetTree();
            while (times-- > 0) await tree.ToSignal(tree, "physics_frame");
        }

        public static async Task AwaitProcessFrame(this Node node, int times) {
            times = Math.Max(1, times);
            var tree = node.GetTree();
            while (times-- > 0) await tree.ToSignal(tree, "process_frame");
        }
    }
}