using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Application.Notifications;
using Betauer.Application.Screen;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;

public static class DebugOverlayBuilder {

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, Action<MonitorText>? config = null) where T : Node {
        var monitor = builder.Create<MonitorText>();
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, string label, Action<MonitorText>? config = null) where T : Node {
        var monitor = builder.Create<MonitorText>()
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, string label, Func<int> updater, int updateEvery = 0, Action<MonitorText>? config = null) where T : Node {
        return TextField(builder, label, () => updater().ToString(), updateEvery, config);
    }

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, string label, Func<float> updater, int updateEvery = 0, Action<MonitorText>? config = null) where T : Node {
        return TextField(builder, label, () => updater().ToString(), updateEvery, config);
    }

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, string label, Func<string> updater, int updateEvery = 0, Action<MonitorText>? config = null) where T : Node {
        var monitor = builder.Create<MonitorText>()
            .UpdateEvery(updateEvery)
            .SetLabel(label)
            .Show(updater);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> TextField<T>(this NodeBuilder<T> builder, string label, Func<bool> updater, int updateEvery = 0, Action<MonitorText>? config = null) where T : Node {
        var monitor = builder.Create<MonitorText>()
            .UpdateEvery(updateEvery)
            .SetLabel(label)
            .Show(updater);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> TextSpeed<T>(this NodeBuilder<T> builder, string label, Node2D node2D, string format = "000.00", Action<MonitorText>? config = null) where T : Node {
        var speedometer2D = Speedometer2D.Position(node2D);
        speedometer2D.UpdateOnPhysicsProcess(node2D);
        var monitor = builder.Create<MonitorText>()
            .Follow(node2D)
            .SetLabel(label)
            .Show(() => speedometer2D.GetInfo());
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> GraphSpeed<T>(this NodeBuilder<T> builder, string label, Speedometer2D speedometer2D, float limit = 0, string format = "000.00", Action<MonitorGraph>? config = null) where T : Node {
        var monitorGraph = builder.Create<MonitorGraph>()
            .AddSerie()
            .Load(() => speedometer2D.Speed)
            .Format((v) => $"{speedometer2D.Speed.ToString(format)} {speedometer2D.SpeedVector.ToString(format)}")
            .SetLabel(label)
            .EndSerie();
        if (limit > 0) monitorGraph.Range(0, limit);
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder<T> GraphSpeed<T>(this NodeBuilder<T> builder, string label, CharacterBody2D characterBody2D, float limit = 0, string format = "000.00", Action<MonitorGraph>? config = null) where T : Node {
        var monitorGraph = builder.Create<MonitorGraph>()
            .Follow(characterBody2D)
            .AddSerie()
            .Load(() => characterBody2D.GetRealVelocity().Length())
            .Format((v) => $"{characterBody2D.GetRealVelocity().Length().ToString(format)} {characterBody2D.GetRealVelocity().ToString(format)}")
            .SetLabel(label)
            .EndSerie();
        if (limit > 0) monitorGraph.Range(0, limit);
        config?.Invoke(monitorGraph);
        return builder;
    }

    public static NodeBuilder<T> Graph<T>(this NodeBuilder<T> builder, string label, Func<float> func, Color? color = null, Action<MonitorGraph>? config = null) where T : Node {
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

    public static NodeBuilder<T> Graph<T>(this NodeBuilder<T> builder, string label, Func<float> func, float min, float max, Color? color = null, Action<MonitorGraph>? config = null) where T : Node {
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

    public static NodeBuilder<T> Graph<T>(this NodeBuilder<T> builder, string label, Func<bool> func, Color? color = null, Action<MonitorGraph>? config = null) where T : Node {
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

    public static NodeBuilder<T> Vector<T>(this NodeBuilder<T> builder, string label, Func<Vector2> func, float maxValue, Action<MonitorVector2>? config = null) where T : Node {
        var monitor = builder.Create<MonitorVector2>()
            .Load(func, maxValue)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> VectorNormalized<T>(this NodeBuilder<T> builder, string label, Func<Vector2> func, Action<MonitorVector2>? config = null) where T : Node {
        var monitor = builder.Create<MonitorVector2>()
            .LoadNormalized(func)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> Angle<T>(this NodeBuilder<T> builder, string label, Func<float> func, Action<MonitorVector2>? config = null) where T : Node {
        var monitor = builder.Create<MonitorVector2>()
            .LoadAngle(func)
            .SetLabel(label);
        config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, int initialValue, Action<int> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, initialValue.ToString, (s) => update(s.ToInt()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, float initialValue, Action<float> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, initialValue.ToString, (s) => update(s.ToFloat()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, double initialValue, Action<double> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, initialValue.ToString, (s) => update(s.ToFloat()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, string initialValue, Action<string> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, initialValue.ToString, update, config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, Func<int> initialValueLoader, Action<int> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, () => initialValueLoader().ToString(), (s) => update(s.ToInt()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, Func<float> initialValueLoader, Action<float> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, () => initialValueLoader().ToString(), (s) => update(s.ToFloat()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, Func<double> initialValueLoader, Action<double> update, Action<MonitorEditValue>? config = null) where T : Node {
        builder.Edit(label, () => initialValueLoader().ToString(), (s) => update(s.ToFloat()), config);
        return builder;
    }

    public static NodeBuilder<T> Edit<T>(this NodeBuilder<T> builder, string label, Func<string> initialValueLoader, Action<string> update, Action<MonitorEditValue>? config = null) where T : Node {
         var monitor = builder.Create<MonitorEditValue>()
            .SetLabel(label)
            .SetValueLoader(initialValueLoader)
            .OnUpdate(update);
         config?.Invoke(monitor);
        return builder;
    }

    public static NodeBuilder<T> AddMonitorVideoInfo<T>(this NodeBuilder<T> builder) where T : Node {
        // TODO Godot 4
        // overlay.OpenBox()
            // .Text("Driver", () => OS.GetCurrentVideoDriver().ToString()).UpdateEvery(1f).EndMonitor()
            // .Text("Screen", () => $"#{(OS.CurrentScreen + 1).ToString()}/{OS.GetScreenCount().ToString()}").UpdateEvery(1f).EndMonitor()
            // .Text("Resolution", () => $"{OS.GetScreenSize(OS.CurrentScreen).ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .Text("Window", () => $"{OS.WindowSize.ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .CloseBox();
        return builder;
    }

    public static NodeBuilder<T> AddMonitorInternals<T>(this NodeBuilder<T> builder) where T : Node {
        var maxNodes = 0;
        var maxOrphans = 0;
        var maxResources = 0;
        var maxObjects = 0;
        builder.Add(new HBoxContainer().Children()
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
                .Node)
            .Add(new HBoxContainer().Children()
                .TextField("Resources/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectResourceCount);
                    maxResources = Math.Max(maxResources, count);
                    return $"{count.ToString()}/{maxResources.ToString()}";
                }, 1)
                .TextField("Objects/max", () => {
                    var count = (int)Performance.GetMonitor(Performance.Monitor.ObjectCount);
                    maxObjects = Math.Max(maxObjects, count);
                    return $"{count.ToString()}/{maxObjects.ToString()}";
                }, 1).Node);
        return builder;
    }

    public static NodeBuilder<T> AddWindowNotificationStatus<T>(this NodeBuilder<T> builder, WindowNotificationStatus windowNotificationStatus) where T : Node {
        builder.Add(new HBoxContainer().Children()
            .TextField("Window Focus", () => windowNotificationStatus.IsWindowFocused)
            .TextField("Application Focus", () => windowNotificationStatus.IsApplicationFocused)
            .TextField("Mouse inside game", () => windowNotificationStatus.IsMouseInsideScreen)
            .Node);
        return builder;
    }

    public static NodeBuilder<T> AddMonitorFpsTimeScaleAndUptime<T>(this NodeBuilder<T> builder) where T : Node {
        builder.Add(new HBoxContainer().Children()
            .TextField("FPS/limit",
                () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.MaxFps.ToString()}", 1)
            .TextField("TimeScale", () => Engine.TimeScale.ToString("0.0"), 1)
            .TextField("Uptime", () => {
                var timespan = TimeSpan.FromMilliseconds(Time.GetTicksMsec());
                return $"{(int)timespan.TotalMinutes}:{timespan.Seconds:00}";
            }, 1).Node);
        return builder;
    }

    public static NodeBuilder<T> AddMonitorMemory<T>(this NodeBuilder<T> builder) where T : Node {
        #if DEBUG
        builder.Add(new HBoxContainer().Children()
            .TextField("Static", () => ((long)OS.GetStaticMemoryUsage()).HumanReadableBytes(), 1)
            .TextField("Max", () => ((long)OS.GetStaticMemoryPeakUsage()).HumanReadableBytes(), 1)
            .Node);
            // TODO Godot 4
            // .Text("Dynamic", () => ((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
            // .Text("Max", () => ((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax)).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
        #endif
        return builder;
    }
    
    public static NodeBuilder<T> AddMonitorInputEvent<T>(this NodeBuilder<T> builder, InputActionsContainer inputActionsContainer, int history = 10) where T : Node {
        var inputs = new LinkedList<string>();
        builder.Node.OnInput(e => {
            var pressed = e.IsJustPressed()?"Just Pressed":e.IsPressed()?"Pressed":e.IsReleased()?"Released": "Unknown";
            var modifiers = new List<string>(5);
            if (e.HasShift()) modifiers.Add("Shift");
            if (e.HasAlt()) modifiers.Add("Alt");
            if (e.HasControl()) modifiers.Add("Ctrl");
            if (e.HasMeta()) modifiers.Add("Meta");
            var actions = inputActionsContainer.InputActionList.Where(a => a.IsEvent(e)).Select(a => a.Name).ToList();
            var actionName = actions.Count > 0 ? $" | Action [{string.Join(",", actions)}]" : "";
            if (e.IsAnyKey()) {
                modifiers.Add(e.GetKeyString());
                inputs.AddLast($"Key {(int)e.GetKey()} [{string.Join('+', modifiers)}] {pressed} {actionName}");
            } else if (e.IsAnyClick()) {
                modifiers.Add(e.GetClick().ToString());
                inputs.AddLast($"Click {(int)e.GetClick()} [{string.Join('+', modifiers)}] {pressed} {actionName}");
            } else if (e.IsAnyButton()) {
                modifiers.Add(e.GetButton().ToString());
                inputs.AddLast($"Button {(int)e.GetButton()} [{string.Join('+', modifiers)}] {pressed} | {e.GetButtonPressure()} {actionName}");
            } else if (e.IsAnyAxis())
                inputs.AddLast($"Axis {(int)e.GetAxis()} [{e.GetAxis()}] {e.GetAxisValue():0.00} {actionName}");
            // else if (e.IsMouseMotion())
                // inputs.AddLast($"Mouse motion {e.GetMouseGlobalPosition()} {actionName}");
            if (inputs.Count > history) inputs.RemoveFirst();
        });
        builder.TextField("", () => string.Join('\n', inputs));
        return builder;
    }
    
    public static NodeBuilder<T> AddMonitorInputAction<T>(this NodeBuilder<T> builder, InputActionsContainer inputActionsContainer) where T : Node {
        builder.TextField("", () => {
            var s = new StringBuilder();
            foreach (var a in inputActionsContainer.InputActionList) {
                if (a is InputAction inputAction) {
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
                } else if (a is AxisAction axis) {
                    s.Append($"{axis.Name}: {axis.Strength:0.00} ({axis.RawStrength:0.00})");
                    s.Append('\n');
                }
            }
            return s.ToString();
        });
        return builder;
    }

    public static NodeBuilder<T> AddMonitorScreenSettings<T>(this NodeBuilder<T> builder, ScreenSettingsManager screenSettingsManager) where T : Node {
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

        builder.Add(new HBoxContainer().Children()
                .TextField("Strategy", () => sc.GetType().Name)
                .TextField("Stretch", () => $"{sc.ScreenConfig.ScaleMode.ToString()}/{sc.ScreenConfig.ScaleAspect.ToString()}")
                .TextField("Zoom", () => sc.ScreenConfig.ScaleFactor.ToString())
                .TextField("Viewport", () => $"{window.Size.ToString("0")}")
                .Node)
            .Add(new HBoxContainer().Children()
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
                .Node)
            .Add(new HBoxContainer().Children()
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
                .Node)
            .Add(new HBoxContainer().Children()
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
                .Node);
        return builder;
    }
}