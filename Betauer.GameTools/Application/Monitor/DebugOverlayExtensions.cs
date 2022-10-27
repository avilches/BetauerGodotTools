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


        public static DebugOverlay AddVideoInfo(this DebugOverlay overlay) {
            overlay.OpenBox()
                .Text("Resolution", () => $"{OS.GetScreenSize(OS.CurrentScreen).ToString("0")}").UpdateEvery(1f).EndMonitor()
                .Text("Driver", () => OS.GetCurrentVideoDriver().ToString()).UpdateEvery(1f).EndMonitor()
                .Text("Screen", () => $"#{(OS.CurrentScreen + 1).ToString()}/{OS.GetScreenCount().ToString()}").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            return overlay;
        }

        public static DebugOverlay AddMonitorInternals(this DebugOverlay overlay) {
            var maxNodes = 0;
            var maxOrphans = 0;
            var maxResources = 0;
            var maxObjects = 0;

            overlay.OpenBox()
                .Text("Nodes/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectNodeCount);
                    maxNodes = Math.Max(maxNodes, count);
                    return $"{count.ToString()}/{maxNodes.ToString()}";
                }).UpdateEvery(1f).EndMonitor()
                .Text("Orphans nodes/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount);
                    maxOrphans = Math.Max(maxOrphans, count);
                    return $"{count.ToString()}/{maxOrphans.ToString()}";
                }).UpdateEvery(1f).EndMonitor()
                .CloseBox()
                .OpenBox()
                .Text("Resources in use/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectResourceCount);
                    maxResources = Math.Max(maxResources, count);
                    return $"{count.ToString()}/{maxResources.ToString()}";
                }).UpdateEvery(1f).EndMonitor()
                .Text("Objects/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectCount);
                    maxObjects = Math.Max(maxObjects, count);
                    return $"{count.ToString()}/{maxObjects.ToString()}";
                }).UpdateEvery(1f).EndMonitor()
                .CloseBox()
                .OpenBox()
                .Text("Watching/max", () => $"{DefaultObjectWatcherTask.Instance.Size}/{DefaultObjectWatcherTask.Instance.PeakSize}").UpdateEvery(1f).EndMonitor()
                .Text("Tweens/max", () => $"{DefaultTweenCallbackManager.Instance.Size.ToString()}/{DefaultTweenCallbackManager.Instance.PeakSize.ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("Signals", () => $"{DefaultSignalManager.Instance.Size.ToString()}").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            return overlay;

        }

        public static DebugOverlay AddMonitorFpsAndMemory(this DebugOverlay overlay) {
            overlay.OpenBox()
                .Text("FPS/limit", () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.TargetFps.ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("TimeScale", () => Engine.TimeScale.ToString("0.0")).UpdateEvery(1f).EndMonitor()
                .Text("Uptime", () => {
                    var timespan = TimeSpan.FromMilliseconds(OS.GetTicksMsec());
                    return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
                }).UpdateEvery(1f).EndMonitor()
                .CloseBox();
           
            #if DEBUG
            overlay
                .Text("Static/max", () => $"{((long)Performance.GetMonitor(Performance.Monitor.MemoryStatic)).HumanReadableBytes()} / {((long)Performance.GetMonitor(Performance.Monitor.MemoryStaticMax)).HumanReadableBytes()}").UpdateEvery(1f).EndMonitor()
                .Text("Dynamic/max", () => $"{((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamic)).HumanReadableBytes()} / {((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax)).HumanReadableBytes()}").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            #endif
            return overlay;
        }
    }
}