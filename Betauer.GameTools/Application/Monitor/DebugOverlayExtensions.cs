using System.Linq;
using Betauer.Animation.Tween;
using Betauer.Memory;
using Betauer.Signal;
using Godot;

namespace Betauer.Application.Monitor {
    public static partial class DebugOverlayExtensions {
        public static Monitor CreateMonitor(this DebugOverlay overlay, string prefix = null) {
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

        public static Monitor MonitorInternals(this DebugOverlay overlay) {
            return overlay.MonitorList
                .Create<Monitor>()
                .Show(() => {
                    var watcherSize = DefaultObjectWatcherTask.Instance.Size.ToString();
                    var watcherPeakSize = DefaultObjectWatcherTask.Instance.PeakSize.ToString();
                    var tweenCallbacks = DefaultTweenCallbackManager.Instance.ActionsByTween.Count.ToString();
                    var objectsWithSignals = DefaultSignalManager.Instance.SignalsByObject.Count.ToString();
                    var signals = DefaultSignalManager.Instance.SignalsByObject.Values.SelectMany(o => o.Signals)
                        .Count().ToString();
                    return $"Watching(peak): {watcherSize}({watcherPeakSize}) | Tweens: {tweenCallbacks} | Objects/Signals {objectsWithSignals}/{signals}";
                });

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