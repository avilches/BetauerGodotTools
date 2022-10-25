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

        public static MonitorText TextSpeed(this DebugOverlay overlay, string label = "Speed", string format = "000.00") {
            if (overlay.Target is not Node2D node2D) throw new Exception("TextSpeed() needs the overlay follows a Node2D");
            return overlay.TextSpeed(label, node2D, format);
        }

        public static MonitorText TextSpeed(this DebugOverlay overlay, string label, Node2D node2D, string format = "000.00") {
            var speedometer2D = Speedometer2D.Position(node2D);
            speedometer2D.UpdateOnPhysicsProcess(node2D);
            return overlay
                .CreateMonitor<MonitorText>()
                .Show(() => speedometer2D.GetInfo())
                .SetLabel(label);
        }

        public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label = "Speed", float limit = 0, string format = "000.00") {
            if (overlay.Target is not Node2D node2D) throw new Exception("GraphSpeed() needs the overlay follows a Node2D");
            var speedometer2D = Speedometer2D.Position(node2D);
            speedometer2D.UpdateOnPhysicsProcess(node2D);
            return overlay.GraphSpeed(label, speedometer2D, limit, format);
        }

        public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label, Speedometer2D speedometer2D, float limit = 0, string format = "000.00") {
            var monitorGraph = overlay
                .CreateMonitor<MonitorGraph>()
                .AddSerie()
                .Load(() => speedometer2D.Speed)
                .Format((v) => $"{speedometer2D.SpeedVector.ToString(format)} {speedometer2D.Speed.ToString(format)}")
                .SetLabel(label)
                .EndSerie();
            if (limit > 0) monitorGraph.Range(0, limit);
            return monitorGraph;
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<float> func) {
            return overlay.CreateMonitor<MonitorGraph>()
                .AutoRange()
                .AddSerie()
                .Load(func)
                .SetLabel(label)
                .EndSerie();
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<float> func, float min, float max, Color? color = null) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Range(min, max)
                .AddSerie()
                .Load(func)
                .SetLabel(label)
                .SetColor(color)
                .EndSerie();
        }

        public static MonitorGraph Graph(this DebugOverlay overlay, string label, Func<bool> func, Color? color = null) {
            return overlay.CreateMonitor<MonitorGraph>()
                .Range(0, 1)
                .AddSerie()
                .Load(func)
                .SetLabel(label)
                .SetColor(color)
                .EndSerie();
        }

        public static MonitorVector2 Vector(this DebugOverlay overlay, string label, Func<Vector2> func, float maxValue) {
            return overlay.CreateMonitor<MonitorVector2>()
                .Load(func, maxValue)
                .SetLabel(label);
        }


        public static DebugOverlay AddMonitorInternals(this DebugOverlay overlay) {
            overlay.OpenBox()
                .Text("Watching(peak)", () => $"{DefaultObjectWatcherTask.Instance.Size}({DefaultObjectWatcherTask.Instance.PeakSize})").UpdateEvery(1f).EndMonitor()
                .Text("Tweens(peak)", () => $"{DefaultTweenCallbackManager.Instance.Size.ToString()}({DefaultTweenCallbackManager.Instance.PeakSize.ToString()})").UpdateEvery(1f).EndMonitor()
                .Text("Signals(objects)", () => $"{DefaultSignalManager.Instance.Size.ToString()}({DefaultSignalManager.Instance.ObjectsSize.ToString()})").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            return overlay;

        }

        public static DebugOverlay AddMonitorFpsAndMemory(this DebugOverlay overlay) {
            overlay.OpenBox()
                .Text("FPS", () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.TargetFps.ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("TimeScale", () => Engine.TimeScale.ToString("0.0")).UpdateEvery(1f).EndMonitor();
           
            #if DEBUG
            overlay
                .Text("Dynamic", () => $"{((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()}").UpdateEvery(1f).SetMinWidth(60).EndMonitor()
                .Text("Static", () => ((long)OS.GetStaticMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).SetMinWidth(60).EndMonitor()
                .Text("Peak", () => ((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor();
            #endif
            overlay.CloseBox();
            return overlay;
        }
    }
}