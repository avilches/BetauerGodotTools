using Betauer.Memory;
using Godot;

namespace Betauer.Application.Monitor {
    public static partial class DebugOverlayExtensions {
        public static Monitor AddNode(this DebugOverlay overlay, Node node, NodePath property) {
            return overlay.MonitorList.Create()
                .Bind(node)
                .Show(() => node.GetIndexed(property).ToString());
        }

        public static Monitor AddObjectRunnerSize(this DebugOverlay overlay) {
            return overlay.MonitorList.Create()
                .WithPrefix("ObjectWatcher")
                .Show(() => $"{DefaultObjectWatcherRunner.Instance.Size.ToString()}/{DefaultObjectWatcherRunner.Instance.PeakSize.ToString()}");
        }

        public static Monitor AddFpsAndMemory(this DebugOverlay overlay) {
            return overlay.MonitorList.Create()
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