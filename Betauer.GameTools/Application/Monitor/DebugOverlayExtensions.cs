using System;
using System.Linq;
using Betauer.Animation.Tween;
using Betauer.Memory;
using Betauer.Signal;
using Godot;

namespace Betauer.Application.Monitor {
    public static partial class DebugOverlayExtensions {
        public static T CreateMonitor<T>(this DebugOverlay overlay) where T : BaseMonitor {
            var monitorText = Activator.CreateInstance<T>();
            overlay.Add(monitorText);
            return monitorText;
        }

        public static MonitorText Text(this DebugOverlay overlay, string? label = null) {
            return overlay.CreateMonitor<MonitorText>().SetLabel(label);
        }

        public static MonitorText Text(this DebugOverlay overlay, Func<string> func) {
            return overlay.Text("", func);
        }

        public static MonitorText Text(this DebugOverlay overlay, string label, Func<string> func) {
            return overlay.CreateMonitor<MonitorText>()
                .Show(func)
                .SetLabel(label);
        }

        public static MonitorText Text(this DebugOverlay overlay, string label, Func<bool> func) {
            return overlay.CreateMonitor<MonitorText>()
                .Show(func)
                .SetLabel(label);
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<float> func, float min, float max) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Load(func)
                .SetLabel(label)
                .Range(min, max);
        }

        public static MonitorText MonitorInternals(this DebugOverlay overlay) {
            return overlay.Text(() => {
                    var watcherSize = DefaultObjectWatcherTask.Instance.Size.ToString();
                    var watcherPeakSize = DefaultObjectWatcherTask.Instance.PeakSize.ToString();
                    var tweenCallbacks = DefaultTweenCallbackManager.Instance.ActionsByTween.Count.ToString();
                    var objectsWithSignals = DefaultSignalManager.Instance.SignalsByObject.Count.ToString();
                    var signals = DefaultSignalManager.Instance.SignalsByObject.Values.SelectMany(o => o.Signals)
                        .Count().ToString();
                    return $"Watching(peak): {watcherSize}({watcherPeakSize}) | Tweens: {tweenCallbacks} | Objects/Signals {objectsWithSignals}/{signals}";
                });

        }

        public static MonitorText MonitorFpsAndMemory(this DebugOverlay overlay) {
            return overlay.Text(() => {
                var txt = $"FPS {((int)Engine.GetFramesPerSecond()).ToString()}";
                #if DEBUG
                    txt += $" | Static Mem: {((long)OS.GetStaticMemoryUsage()).HumanReadableBytes()} / {((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes()} | Dynamic Mem: {((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()}";
                #endif
                return txt;
            });
        }
    }
}