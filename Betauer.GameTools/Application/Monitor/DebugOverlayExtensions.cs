using Betauer.Memory;
using Godot;

namespace Betauer.Application.Monitor {
    public static partial class DebugOverlayExtensions {
        public static Monitor Create(this DebugOverlay overlay, string prefix = null) {
            return overlay.MonitorList
                .Create<Monitor>()
                .WithPrefix(prefix);
        }

        public static Monitor MonitorNode(this DebugOverlay overlay, Node node, NodePath property) {
            return overlay.MonitorList
                .Create<Monitor>()
                .WithPrefix($"{node.Name}:{property}")
                .Bind(node)
                .Show(() => node.GetIndexed(property).ToString());
        }

        public static Monitor MonitorObjectRunnerSize(this DebugOverlay overlay) {
            return overlay.MonitorList
                .Create<Monitor>()
                .WithPrefix("ObjectWatcher")
                .Show(() => $"{DefaultObjectWatcherRunner.Instance.Size.ToString()}/{DefaultObjectWatcherRunner.Instance.PeakSize.ToString()}");
        }

        public static Monitor MonitorFpsAndMemory(this DebugOverlay overlay) {
            return overlay.MonitorList
                .Create<Monitor>()
                .Show(() => {
                var txt = $"FPS {((int)Engine.GetFramesPerSecond()).ToString()}";
                #if DEBUG
                    txt += $" | Static Mem: {((long)OS.GetStaticMemoryUsage()).HumanReadableBytes()} / {((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes()} | Dynamic Mem: {((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()}";
                #endif
                return txt;
            });
        }

        
    }
}