using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Betauer.Core;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;

public static class DebugOverlayBuilder {

    public static NodeBuilder TextField(this NodeBuilder builder, Action<MonitorText>? config = null) {
        var monitor = builder.Create<MonitorText>();
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder TextField(this NodeBuilder builder, string label, Action<MonitorText>? config = null) {
        var monitor = builder.Create<MonitorText>()
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder TextField(this NodeBuilder builder, string label, Func<int> updater, int updateEvery = 0, Action<MonitorText>? config = null) {
        return TextField(builder, label, () => updater().ToString(), updateEvery, config);
    }

    public static NodeBuilder TextField(this NodeBuilder builder, string label, Func<float> updater, int updateEvery = 0, Action<MonitorText>? config = null) {
        return TextField(builder, label, () => updater().ToString(), updateEvery, config);
    }

    public static NodeBuilder TextField(this NodeBuilder builder, string label, Func<string> updater, int updateEvery = 0, Action<MonitorText>? config = null) {
        var monitor = builder.Create<MonitorText>()
            .UpdateEvery(updateEvery)
            .SetLabel(label)
            .Show(updater);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder TextField(this NodeBuilder builder, string label, Func<bool> updater, int updateEvery = 0, Action<MonitorText>? config = null) {
        var monitor = builder.Create<MonitorText>()
            .UpdateEvery(updateEvery)
            .SetLabel(label)
            .Show(updater);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder TextSpeed(this NodeBuilder builder, string label, Speedometer2D speedometer2D, string format = "000.00", int updateEvery = 0, Action<MonitorText>? config = null) {
        var monitor = builder.Create<MonitorText>()
            .UpdateEvery(updateEvery)
            .SetLabel(label)
            .Show(() => $"{speedometer2D.Speed.ToString(format)} {speedometer2D.SpeedVector.ToString(format)}");
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder GraphSpeed(this NodeBuilder builder, string label, Speedometer2D speedometer2D, float limit = 0, string format = "000.00", Action<MonitorGraph>? config = null) {
        var monitorGraph = builder.Create<MonitorGraph>()
            .AddSerie()
            .Load(() => speedometer2D.Speed)
            .Format((v) => $"{v.ToString(format)} {speedometer2D.SpeedVector.ToString(format)}")
            .SetLabel(label)
            .EndSerie();
        if (limit > 0) monitorGraph.Range(0, limit);
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder Graph(this NodeBuilder builder, string label, Func<float> func, Color? color = null, Action<MonitorGraph>? config = null) {
        var monitorGraph = builder.Create<MonitorGraph>()
            .AutoRange()
            .AddSerie()
            .Load(func)
            .SetLabel(label)
            .SetColor(color)
            .EndSerie();
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder Graph(this NodeBuilder builder, string label, Func<float> func, float min, float max, Color? color = null, Action<MonitorGraph>? config = null) {
        var monitorGraph = builder.Create<MonitorGraph>()
            .Range(min, max)
            .AddSerie()
            .Load(func)
            .SetLabel(label)
            .SetColor(color)
            .EndSerie();
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder Graph(this NodeBuilder builder, string label, Func<bool> func, Color? color = null, Action<MonitorGraph>? config = null) {
        var monitorGraph = builder.Create<MonitorGraph>()
            .Range(0, 1)
            .AddSerie()
            .Load(func)
            .SetLabel(label)
            .SetColor(color)
            .EndSerie();
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder Vector(this NodeBuilder builder, string label, Func<Vector2> func, float maxValue, Action<MonitorVector2>? config = null) {
        var monitor = builder.Create<MonitorVector2>()
            .Load(func, maxValue)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder VectorNormalized(this NodeBuilder builder, string label, Func<Vector2> func, Action<MonitorVector2>? config = null) {
        var monitor = builder.Create<MonitorVector2>()
            .LoadNormalized(func)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder Angle(this NodeBuilder builder, string label, Func<float> func, Action<MonitorVector2>? config = null) {
        var monitor = builder.Create<MonitorVector2>()
            .LoadAngle(func)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder Edit(this NodeBuilder builder, string label, string initialValue, Action<string> update, Action<MonitorEditValue>? config = null) {
        var monitor = builder.Create<MonitorEditValue>()
            .SetLabel(label)
            .SetValue(initialValue)
            .OnUpdate(update);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder Edit(this NodeBuilder builder, string label, Func<int> valueLoader, Action<int> update, Action<MonitorEditValue>? config = null) {
        builder.Edit(label, () => valueLoader().ToString(CultureInfo.InvariantCulture), (s) => update(int.Parse(s, CultureInfo.InvariantCulture)), config);
        return builder;
    }

    public static NodeBuilder Edit(this NodeBuilder builder, string label, Func<float> valueLoader, Action<float> update, Action<MonitorEditValue>? config = null) {
        builder.Edit(label, () => valueLoader().ToString(CultureInfo.InvariantCulture), (s) => update(float.Parse(s, CultureInfo.InvariantCulture)), config);
        return builder;
    }

    public static NodeBuilder Edit(this NodeBuilder builder, string label, Func<double> valueLoader, Action<double> update, Action<MonitorEditValue>? config = null) {
        builder.Edit(label, () => valueLoader().ToString(CultureInfo.InvariantCulture), (s) => update(double.Parse(s, CultureInfo.InvariantCulture)), config);
        return builder;
    }

    public static NodeBuilder Edit(this NodeBuilder builder, string label, Func<string> valueLoader, Action<string> update, Action<MonitorEditValue>? config = null) {
        var monitor = builder.Create<MonitorEditValue>()
            .SetLabel(label)
            .SetValueLoader(valueLoader)
            .OnUpdate(update);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder AddMonitorVideoInfo(this NodeBuilder builder) {
        // TODO Godot 4
        // overlay.OpenBox()
            // .Text("Driver", () => OS.GetCurrentVideoDriver().ToString()).UpdateEvery(1f).EndMonitor()
            // .Text("Screen", () => $"#{(OS.CurrentScreen + 1).ToString()}/{OS.GetScreenCount().ToString()}").UpdateEvery(1f).EndMonitor()
            // .Text("Resolution", () => $"{OS.GetScreenSize(OS.CurrentScreen).ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .Text("Window", () => $"{OS.WindowSize.ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .CloseBox();
        return builder;
    }

    public static NodeBuilder AddMonitorInternals(this NodeBuilder builder) {
        var maxNodes = 0;
        var maxOrphans = 0;
        var maxResources = 0;
        var maxObjects = 0;
        builder.Add<HBoxContainer>(box => box.Children()
                .TextField("Nodes/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectNodeCount);
                    maxNodes = Math.Max(maxNodes, count);
                    return $"{count.ToString()}/{maxNodes.ToString()}";
                }, 1)
                .TextField("Orphans nodes/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount);
                    maxOrphans = Math.Max(maxOrphans, count);
                    return $"{count.ToString()}/{maxOrphans.ToString()}";
                }, 1)
            )
            .Add<HBoxContainer>(box => box.Children()
                .TextField("Resources/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectResourceCount);
                    maxResources = Math.Max(maxResources, count);
                    return $"{count.ToString()}/{maxResources.ToString()}";
                }, 1)
                .TextField("Objects/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectCount);
                    maxObjects = Math.Max(maxObjects, count);
                    return $"{count.ToString()}/{maxObjects.ToString()}";
                }, 1)
            );
        return builder;
    }

    public static NodeBuilder AddWindowNotificationStatus(this NodeBuilder builder) {
        builder.Add<HBoxContainer>(box => box.Children()
            .TextField("Window Focus", () => NodeManager.MainInstance.IsWindowFocused)
            .TextField("Application Focus", () => NodeManager.MainInstance.IsApplicationFocused)
            .TextField("Mouse inside game", () => NodeManager.MainInstance.IsMouseInsideScreen)
        );
        return builder;
    }

    public static NodeBuilder AddMonitorFpsTimeScaleAndUptime(this NodeBuilder builder) {
        builder.Add<HBoxContainer>(box => box.Children()
            .TextField("FPS/limit",
                () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.MaxFps.ToString()}", 1)
            .TextField("TimeScale", () => Engine.TimeScale.ToString("0.0"), 1)
            .TextField("Uptime", () => {
                var timespan = TimeSpan.FromMilliseconds(Time.GetTicksMsec());
                return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
            }, 1)
        );
        return builder;
    }

    public static NodeBuilder AddMonitorMemory(this NodeBuilder builder) {
#if DEBUG
        builder.Add<HBoxContainer>(box => box.Children()
            .TextField("Static", () => ((long)OS.GetStaticMemoryUsage()).HumanReadableBytes(), 1)
            .TextField("Max", () => ((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes(), 1)
        );
        // TODO Godot 4
        // .Text("Dynamic", () => ((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
        // .Text("Max", () => ((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax)).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
#endif
        return builder;
    }
    
    public static NodeBuilder AddMonitorInputAction(this NodeBuilder builder, InputActionsContainer inputActionsContainer) {
        builder.TextField("", () => {
            var s = new StringBuilder();
            foreach (var axis in inputActionsContainer.AxisActions) {
                s.Append($"{axis.Name}: {axis.Strength:0.00} ({axis.RawStrength:0.00})");
                s.Append('\n');
            }
            foreach (var inputAction in inputActionsContainer.InputActions) {
                var keys = inputAction.Keys.Select(key => $"Key:{key}").ToList();
                keys.AddRange(inputAction.Buttons.Select(button => $"Button:{button}"));
                s.Append($"{inputAction.Name}: {string.Join(" | ", keys)}");
                if (inputAction.HasAxis()) {
                    s.Append($" {(inputAction.AxisSign > 0 ? "Positive" : "Negative")} DeadZone:{inputAction.DeadZone}");
                }
                if (inputAction.HasMouseButton()) {
                    s.Append($" Mouse:{inputAction.MouseButton}");
                }
                if (inputAction.IsPressed) s.Append(" [Pressed]");
                s.Append('\n');
            }
            return s.ToString();
        });
        return builder;
    }
/*
    public static NodeBuilder AddMonitorScreenSettings(this NodeBuilder builder, ScreenSettingsManager screenSettingsManager) {
        var strategyGroup = new ButtonGroup();
        var modeGroup = new ButtonGroup();
        var aspectGroup = new ButtonGroup();
        var sc = screenSettingsManager.ScreenController;
        var overlay = builder.Node.FindParent<DebugOverlay>();
        var window = builder.Node.GetTree().Root;
        overlay?.OnDestroy(() => {
            strategyGroup.Dispose();
            modeGroup.Dispose();
            aspectGroup.Dispose();
        });

        builder.Add<HBoxContainer>(box => box.Children()
                .TextField("Strategy", () => sc.GetType().Name)
                .TextField("Stretch", () => $"{sc.ScreenConfig.ScaleMode.ToString()}/{sc.ScreenConfig.ScaleAspect.ToString()}")
                .TextField("Zoom", () => sc.ScreenConfig.ScaleFactor.ToString())
                .TextField("Viewport", () => $"{window.Size.ToString("0")}")
            )
            .Add<HBoxContainer>(box => box.Children()
                .Label("Strategy")
                .ToggleButton(nameof(FixedViewportStrategy),
                    () => sc.ScreenConfig.Strategy is FixedViewportStrategy, () => {
                        sc.ScreenConfig.Strategy = FixedViewportStrategy.Instance;
                        sc.Apply();
                    }, strategyGroup)
                .ToggleButton(nameof(ResizeViewportStrategy),
                    () => sc.ScreenConfig.Strategy is ResizeViewportStrategy, () => {
                        sc.ScreenConfig.Strategy = ResizeViewportStrategy.Instance;
                        sc.Apply();
                    }, strategyGroup)
                .ToggleButton(nameof(ResizeIntegerScaledStrategy),
                    () => sc.ScreenConfig.Strategy is ResizeIntegerScaledStrategy, () => {
                        sc.ScreenConfig.Strategy = ResizeIntegerScaledStrategy.Instance;
                        sc.Apply();
                    }, strategyGroup)
            )
            .Add<HBoxContainer>(box => box.Children()
                .Label("Mode")
                .ToggleButton(nameof(Window.ContentScaleModeEnum.Disabled),
                    () => sc.ScreenConfig.ScaleMode == Window.ContentScaleModeEnum.Disabled, () => {
                        sc.ScreenConfig.ScaleMode = Window.ContentScaleModeEnum.Disabled;
                        sc.Apply();
                    }, modeGroup)
                .ToggleButton(nameof(Window.ContentScaleModeEnum.CanvasItems),
                    () => sc.ScreenConfig.ScaleMode == Window.ContentScaleModeEnum.CanvasItems, () => {
                        sc.ScreenConfig.ScaleMode = Window.ContentScaleModeEnum.CanvasItems;
                        sc.Apply();
                    }, modeGroup)
                .ToggleButton(nameof(Window.ContentScaleModeEnum.Viewport),
                    () => sc.ScreenConfig.ScaleMode == Window.ContentScaleModeEnum.Viewport, () => {
                        sc.ScreenConfig.ScaleMode = Window.ContentScaleModeEnum.Viewport;
                        sc.Apply();
                    }, modeGroup)
            )
            .Add<HBoxContainer>(box => box.Children()
                .Label("Aspect")
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Ignore),
                    () => sc.ScreenConfig.ScaleAspect == Window.ContentScaleAspectEnum.Ignore, () => {
                        sc.ScreenConfig.ScaleAspect = Window.ContentScaleAspectEnum.Ignore;
                        sc.Apply();
                    }, aspectGroup)
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Expand),
                    () => sc.ScreenConfig.ScaleAspect == Window.ContentScaleAspectEnum.Expand, () => {
                        sc.ScreenConfig.ScaleAspect = Window.ContentScaleAspectEnum.Expand;
                        sc.Apply();
                    }, aspectGroup)
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Keep),
                    () => sc.ScreenConfig.ScaleAspect == Window.ContentScaleAspectEnum.Keep, () => {
                        sc.ScreenConfig.ScaleAspect = Window.ContentScaleAspectEnum.Keep;
                        sc.Apply();
                    }, aspectGroup)
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.KeepHeight),
                    () => sc.ScreenConfig.ScaleAspect == Window.ContentScaleAspectEnum.KeepHeight, () => {
                        sc.ScreenConfig.ScaleAspect = Window.ContentScaleAspectEnum.KeepHeight;
                        sc.Apply();
                    }, aspectGroup)
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.KeepWidth),
                    () => sc.ScreenConfig.ScaleAspect == Window.ContentScaleAspectEnum.KeepWidth, () => {
                        sc.ScreenConfig.ScaleAspect = Window.ContentScaleAspectEnum.KeepWidth;
                        sc.Apply();
                    }, aspectGroup)
            );
        return builder;
    }
        */
}