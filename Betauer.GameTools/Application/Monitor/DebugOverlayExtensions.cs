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

        public static MonitorText TextSpeed(this DebugOverlay overlay, string label = "Speed", string format = "000") {
            if (overlay.Target is not Node2D node2D) throw new Exception("TextSpeed() needs the overlay follows a Node2D");
            return overlay.TextSpeed(label, Speedometer2D.Position(node2D), format);
        }

        public static MonitorText TextSpeed(this DebugOverlay overlay, string label, Speedometer2D speedometer2D, string format = "000") {
            return overlay
                .AddSpeedometer(speedometer2D)
                .CreateMonitor<MonitorText>()
                .Show(() => $"{speedometer2D.SpeedVector.ToString(format)} {speedometer2D.Speed.ToString(format)}")
                .SetLabel(label);
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<float> func, float min, float max) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Load(func)
                .SetLabel(label)
                .Range(min, max);
        }

        public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label = "Speed", float limit = 0, string format = "000") {
            if (overlay.Target is not Node2D node2D) throw new Exception("GraphSpeed() needs the overlay follows a Node2D");
            return overlay.GraphSpeed(label, Speedometer2D.Position(node2D), limit, format);
        }

        public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label, Speedometer2D speedometer2D, float limit = 0, string format = "000") {
            var monitorGraph = overlay
                .AddSpeedometer(speedometer2D)
                .CreateMonitor<MonitorGraph>()
                .Load(() => speedometer2D.Speed)
                .Format((v) => $"{speedometer2D.SpeedVector.ToString(format)} {speedometer2D.Speed.ToString(format)}")
                .SetLabel(label);
            if (limit > 0) monitorGraph.Range(0, limit);
            return monitorGraph;
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<float> func) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Load(func)
                .SetLabel(label)
                .AutoRange(true);
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<bool> func) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Load(func)
                .SetLabel(label)
                .Range(0, 1);
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