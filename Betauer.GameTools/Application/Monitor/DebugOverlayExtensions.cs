using System;
using System.Linq;
using Betauer.Animation.Tween;
using Betauer.Application.Screen;
using Betauer.Nodes;
using Betauer.Memory;
using Betauer.Signal;
using Betauer.UI;
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


        public static DebugOverlay AddMonitorVideoInfo(this DebugOverlay overlay) {
            overlay.OpenBox()
                .Text("Driver", () => OS.GetCurrentVideoDriver().ToString()).UpdateEvery(1f).EndMonitor()
                .Text("Screen", () => $"#{(OS.CurrentScreen + 1).ToString()}/{OS.GetScreenCount().ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("Resolution", () => $"{OS.GetScreenSize(OS.CurrentScreen).ToString("0")}").UpdateEvery(1f).EndMonitor()
                .Text("Window", () => $"{OS.WindowSize.ToString("0")}").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            return overlay;
        }

        public static DebugOverlay AddMonitorInternals(this DebugOverlay overlay) {
            var maxNodes = 0;
            var maxOrphans = 0;
            var maxResources = 0;
            var maxObjects = 0;

            overlay
                .OpenBox()
                    .Text("Nodes/max", () => {
                        var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectNodeCount);
                        maxNodes = Math.Max(maxNodes, count);
                        return $"{count.ToString()}/{maxNodes.ToString()}";
                    }).UpdateEvery(1f)
                    .EndMonitor()
                    .Text("Orphans nodes/max", () => {
                        var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount);
                        maxOrphans = Math.Max(maxOrphans, count);
                        return $"{count.ToString()}/{maxOrphans.ToString()}";
                    }).UpdateEvery(1f)
                    .EndMonitor()
                .CloseBox()
                .OpenBox()
                    .Text("Resources/max", () => {
                        var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectResourceCount);
                        maxResources = Math.Max(maxResources, count);
                        return $"{count.ToString()}/{maxResources.ToString()}";
                    }).UpdateEvery(1f)
                    .EndMonitor()
                    .Text("Objects/max", () => {
                        var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectCount);
                        maxObjects = Math.Max(maxObjects, count);
                        return $"{count.ToString()}/{maxObjects.ToString()}";
                    }).UpdateEvery(1f)
                    .EndMonitor()
                .CloseBox()
                .OpenBox()
                    .Text("Watching/max", () => $"{DefaultObjectWatcherTask.Instance.Size}/{DefaultObjectWatcherTask.Instance.PeakSize}").UpdateEvery(1f).EndMonitor()
                    .Text("Tweens/max", () => $"{DefaultTweenCallbackManager.Instance.Size.ToString()}/{DefaultTweenCallbackManager.Instance.PeakSize.ToString()}").UpdateEvery(1f).EndMonitor()
                    .Text("Signals", () => $"{DefaultSignalManager.Instance.Size.ToString()}").UpdateEvery(1f).EndMonitor()
                .CloseBox();
            return overlay;
        }

        public static DebugOverlay AddMonitorFpsTimeScaleAndUptime(this DebugOverlay overlay) {
            return overlay
                .OpenBox()
                    .Text("FPS/limit", () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.TargetFps.ToString()}").UpdateEvery(1f).EndMonitor()
                    .Text("TimeScale", () => Engine.TimeScale.ToString("0.0")).UpdateEvery(1f).EndMonitor()
                    .Text("Uptime", () => {
                        var timespan = TimeSpan.FromMilliseconds(OS.GetTicksMsec());
                        return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
                    }).UpdateEvery(1f)
                    .EndMonitor()
                .CloseBox();
        }

        public static DebugOverlay AddMonitorMemory(this DebugOverlay overlay) {
            #if DEBUG
            overlay
                .OpenBox()
                .Text("Static", () => ((long)OS.GetStaticMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
                .Text("Max", () => ((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
                .Text("Dynamic", () => ((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
                .Text("Max", () => ((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax)).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
                .CloseBox();
            #endif
            return overlay;
        }
        
        public static DebugOverlay AddMonitorScreenSettings(this DebugOverlay overlay, ScreenSettingsManager screenSettingsManager) {
            return overlay
                .OpenBox()
                    .Text("Strategy", () => screenSettingsManager.ScreenService.StrategyKey.ToString()).UpdateEvery(1f).EndMonitor()
                    .Text("Stretch", () => $"{screenSettingsManager.ScreenService.ScreenConfiguration.StretchMode.ToString()}/{screenSettingsManager.ScreenService.ScreenConfiguration.StretchAspect.ToString()}").UpdateEvery(1f).EndMonitor()
                    .Text("Zoom", () => screenSettingsManager.ScreenService.ScreenConfiguration.Zoom.ToString()).UpdateEvery(1f).EndMonitor()
                    .Text("Viewport", () => $"{overlay.GetTree().Root.Size.ToString("0")}").UpdateEvery(1f).EndMonitor()
                .CloseBox()
                .Add(new HBoxContainer().NodeBuilder()
                    .Label("Strategy").End()
                    .ToggleButton(nameof(ScreenService.ScreenStrategyKey.WindowSize), 
                        (button) => {
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.WindowSize);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenService.StrategyKey == ScreenService.ScreenStrategyKey.WindowSize)
                    .End()
                    .ToggleButton(nameof(ScreenService.ScreenStrategyKey.ViewportSize), 
                        (button) => {
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.ViewportSize);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenService.StrategyKey == ScreenService.ScreenStrategyKey.ViewportSize)
                    .End()
                    .ToggleButton(nameof(ScreenService.ScreenStrategyKey.IntegerScale), 
                        (button) => {
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration, ScreenService.ScreenStrategyKey.IntegerScale);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenService.StrategyKey == ScreenService.ScreenStrategyKey.IntegerScale)
                    .End()
                    .TypedNode)
                .Add(new HBoxContainer().NodeBuilder()
                    .Label("Mode").End()
                    .ToggleButton(nameof(SceneTree.StretchMode.Disabled), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchMode = SceneTree.StretchMode.Disabled;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchMode == SceneTree.StretchMode.Disabled)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchMode.Mode2d), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchMode = SceneTree.StretchMode.Mode2d;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchMode == SceneTree.StretchMode.Mode2d)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchMode.Viewport), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchMode = SceneTree.StretchMode.Viewport;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchMode == SceneTree.StretchMode.Viewport)
                    .End()
                    .TypedNode)
                .Add(new HBoxContainer().NodeBuilder()
                    .Label("Aspect").End()
                    .ToggleButton(nameof(SceneTree.StretchAspect.Ignore), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.Ignore;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchAspect == SceneTree.StretchAspect.Ignore)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchAspect.Expand), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.Expand;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchAspect == SceneTree.StretchAspect.Expand)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchAspect.Keep), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.Keep;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchAspect == SceneTree.StretchAspect.Keep)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchAspect.KeepHeight), 
                        (button) => {
                            screenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.KeepHeight;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                            button.GetParent().GetChildren<ToggleButton>().ForEach(b => b.Refresh());
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchAspect == SceneTree.StretchAspect.KeepHeight)
                    .End()
                    .ToggleButton(nameof(SceneTree.StretchAspect.KeepWidth), 
                        () => {
                            screenSettingsManager.ScreenConfiguration.StretchAspect = SceneTree.StretchAspect.KeepWidth;
                            screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                        }, 
                        () => screenSettingsManager.ScreenConfiguration.StretchAspect == SceneTree.StretchAspect.KeepWidth)
                    .End()
                    .TypedNode);
            }

        private static Action<Button> Toggle(Func<bool> func) {
            return (button) => {
                button.ToggleMode = true;
                button.Pressed = func();
            };
        }
    }
}