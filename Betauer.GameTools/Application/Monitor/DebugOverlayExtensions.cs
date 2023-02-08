using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Application.Notifications;
using Betauer.Application.Screen;
using Betauer.Core;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.UI;
using Godot;

namespace Betauer.Application.Monitor;
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
        if (overlay.Target is CharacterBody2D characterBody2D)
            return overlay.GraphSpeed(label, characterBody2D, limit, format);
        if (overlay.Target is not Node2D node2D) 
            throw new Exception("GraphSpeed() needs the overlay follows a Node2D");
        var speedometer2D = Speedometer2D.Position(node2D);
        speedometer2D.UpdateOnPhysicsProcess(node2D);
        return overlay.GraphSpeed(label, speedometer2D, limit, format);
    }

    public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label, Speedometer2D speedometer2D, float limit = 0, string format = "000.00") {
        var monitorGraph = overlay
            .CreateMonitor<MonitorGraph>()
            .AddSerie()
            .Load(() => speedometer2D.Speed)
            .Format((v) => $"{speedometer2D.Speed.ToString(format)} {speedometer2D.SpeedVector.ToString(format)}")
            .SetLabel(label)
            .EndSerie();
        if (limit > 0) monitorGraph.Range(0, limit);
        return monitorGraph;
    }

    public static MonitorGraph GraphSpeed(this DebugOverlay overlay, string label, CharacterBody2D characterBody2D, float limit = 0, string format = "000.00") {
        var monitorGraph = overlay
            .CreateMonitor<MonitorGraph>()
            .AddSerie()
            .Load(() => characterBody2D.GetRealVelocity().Length())
            .Format((v) => $"{characterBody2D.GetRealVelocity().Length().ToString(format)} {characterBody2D.GetRealVelocity().ToString(format)}")
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

    public static MonitorVector2 VectorNormalized(this DebugOverlay overlay, string label, Func<Vector2> func) {
        return overlay.CreateMonitor<MonitorVector2>()
            .LoadNormalized(func)
            .SetLabel(label);
    }

    public static MonitorVector2 Angle(this DebugOverlay overlay, string label, Func<float> func) {
        return overlay.CreateMonitor<MonitorVector2>()
            .LoadAngle(func)
            .SetLabel(label);
    }

    public static MonitorEditValue Edit(this DebugOverlay overlay, string label, string initialValue, Action<string> update) {
        return overlay.CreateMonitor<MonitorEditValue>()
            .SetLabel(label)
            .SetValue(initialValue)
            .OnUpdate(update);
    }

    public static DebugOverlay AddMonitorVideoInfo(this DebugOverlay overlay) {
        // TODO Godot 4
        // overlay.OpenBox()
            // .Text("Driver", () => OS.GetCurrentVideoDriver().ToString()).UpdateEvery(1f).EndMonitor()
            // .Text("Screen", () => $"#{(OS.CurrentScreen + 1).ToString()}/{OS.GetScreenCount().ToString()}").UpdateEvery(1f).EndMonitor()
            // .Text("Resolution", () => $"{OS.GetScreenSize(OS.CurrentScreen).ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .Text("Window", () => $"{OS.WindowSize.ToString("0")}").UpdateEvery(1f).EndMonitor()
            // .CloseBox();
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
            .CloseBox();
        return overlay;
    }

    public static DebugOverlay AddWindowNotificationStatus(this DebugOverlay overlay, WindowNotificationStatus windowNotificationStatus) {
        return overlay
            .OpenBox()
                .Text("Window Focus", () => windowNotificationStatus.IsWindowFocused).EndMonitor()
                .Text("Application Focus", () => windowNotificationStatus.IsApplicationFocused).EndMonitor()
                .Text("Mouse inside game", () => windowNotificationStatus.IsMouseInsideScreen).EndMonitor()
            .CloseBox();
    }

    public static DebugOverlay AddMonitorFpsTimeScaleAndUptime(this DebugOverlay overlay) {
        return overlay
            .OpenBox()
                .Text("FPS/limit", () => $"{((int)Engine.GetFramesPerSecond()).ToString()}/{Engine.MaxFps.ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("TimeScale", () => Engine.TimeScale.ToString("0.0")).UpdateEvery(1f).EndMonitor()
                .Text("Uptime", () => {
                    var timespan = TimeSpan.FromMilliseconds(Time.GetTicksMsec());
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
            // TODO Godot 4
            // .Text("Dynamic", () => ((long)OS.GetDynamicMemoryUsage()).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
            // .Text("Max", () => ((long)Performance.GetMonitor(Performance.Monitor.MemoryDynamicMax)).HumanReadableBytes()).UpdateEvery(1f).EndMonitor()
            .CloseBox();
        #endif
        return overlay;
    }
    
    public static DebugOverlay AddMonitorInputEvent(this DebugOverlay overlay, InputActionsContainer inputActionsContainer, int history = 10) {
        var inputs = new LinkedList<string>();
        overlay.OnInput(e => {
            var pressed = e.IsJustPressed()?"Just Pressed":e.IsPressed()?"Pressed":e.IsReleased()?"Released": "Unknown";
            var modifiers = new List<string>(5);
            if (e.HasShift()) modifiers.Add("Shift");
            if (e.HasAlt()) modifiers.Add("Alt");
            if (e.HasControl()) modifiers.Add("Ctrl");
            if (e.HasMeta()) modifiers.Add("Meta");
            var actions = inputActionsContainer.FindActions(e).Select(a=>a.Name).ToList();
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
        overlay.Text(() => string.Join('\n', inputs)).EndMonitor();
        return overlay;
    }
    
    public static DebugOverlay AddMonitorInputAction(this DebugOverlay overlay, InputActionsContainer inputActionsContainer) {
        overlay
            .Text(() => {
                var s = new StringBuilder();
                foreach (var a in inputActionsContainer.InputActionList) {
                    if (a is InputAction inputAction) {
                        var keys = inputAction.Keys.Select(key => $"Key:{key}").ToList();
                        keys.AddRange(inputAction.Buttons.Select(button => $"Button:{button}"));
                        s.Append($"{inputAction.Name}: {string.Join(" | ", keys)}");
                        if (inputAction.HasAxis()) {
                            s.Append($" {(inputAction.AxisSign > 0 ? "Positive":"Negative")} DeadZone:{inputAction.DeadZone}");
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
            }).EndMonitor();
        return overlay;
    }
    
    public static DebugOverlay AddMonitorScreenSettings(this DebugOverlay overlay, ScreenSettingsManager screenSettingsManager) {
        return overlay
            .OpenBox()
                .Text("Strategy", () => screenSettingsManager.ScreenService.GetType().Name).UpdateEvery(1f).EndMonitor()
                .Text("Stretch", () => $"{screenSettingsManager.ScreenService.ScreenConfiguration.ScaleMode.ToString()}/{screenSettingsManager.ScreenService.ScreenConfiguration.ScaleAspect.ToString()}").UpdateEvery(1f).EndMonitor()
                .Text("Zoom", () => screenSettingsManager.ScreenService.ScreenConfiguration.ScaleFactor.ToString()).UpdateEvery(1f).EndMonitor()
                .Text("Viewport", () => $"{overlay.GetTree().Root.Size.ToString("0")}").UpdateEvery(1f).EndMonitor()
            .CloseBox()
            .Add(new HBoxContainer().NodeBuilder()
                .Label("Strategy").End()
                .ToggleButton(nameof(DoNothingStrategy), 
                    (button) => {
                        screenSettingsManager.SetStrategy(DoNothingStrategy.Instance);
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenService.ScreenStrategy is DoNothingStrategy)
                .End()
                .ToggleButton(nameof(FixedViewportStrategy), 
                    (button) => {
                        screenSettingsManager.SetStrategy(FixedViewportStrategy.Instance);
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenService.ScreenStrategy is FixedViewportStrategy)
                .End()
                .ToggleButton(nameof(ResizeViewportStrategy), 
                    (button) => {
                        screenSettingsManager.SetStrategy(ResizeViewportStrategy.Instance);
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenService.ScreenStrategy is ResizeViewportStrategy)
                .End()
                .ToggleButton(nameof(ResizeIntegerScaledStrategy), 
                    (button) => {
                        screenSettingsManager.SetStrategy(ResizeIntegerScaledStrategy.Instance);
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenService.ScreenStrategy is ResizeIntegerScaledStrategy)
                .End()
                .TypedNode)
            .Add(new HBoxContainer().NodeBuilder()
                .Label("Mode").End()
                .ToggleButton(nameof(Window.ContentScaleModeEnum.Disabled), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleMode = Window.ContentScaleModeEnum.Disabled;
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleMode == Window.ContentScaleModeEnum.Disabled)
                .End()
                .ToggleButton(nameof(Window.ContentScaleModeEnum.CanvasItems), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleMode = Window.ContentScaleModeEnum.CanvasItems;
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleMode == Window.ContentScaleModeEnum.CanvasItems)
                .End()
                .ToggleButton(nameof(Window.ContentScaleModeEnum.Viewport), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleMode = Window.ContentScaleModeEnum.Viewport;
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleMode == Window.ContentScaleModeEnum.Viewport)
                .End()
                .TypedNode)
            .Add(new HBoxContainer().NodeBuilder()
                .Label("Aspect").End()
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Ignore), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleAspect = Window.ContentScaleAspectEnum.Ignore;
                        screenSettingsManager.ScreenService.Apply();
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleAspect == Window.ContentScaleAspectEnum.Ignore)
                .End()
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Expand), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleAspect = Window.ContentScaleAspectEnum.Expand;
                        screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleAspect == Window.ContentScaleAspectEnum.Expand)
                .End()
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.Keep), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleAspect = Window.ContentScaleAspectEnum.Keep;
                        screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleAspect == Window.ContentScaleAspectEnum.Keep)
                .End()
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.KeepHeight), 
                    (button) => {
                        screenSettingsManager.ScreenConfiguration.ScaleAspect = Window.ContentScaleAspectEnum.KeepHeight;
                        screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                        button.GetParent().GetChildren().OfType<ToggleButton>().ForEach(b => b.Refresh());
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleAspect == Window.ContentScaleAspectEnum.KeepHeight)
                .End()
                .ToggleButton(nameof(Window.ContentScaleAspectEnum.KeepWidth), 
                    () => {
                        screenSettingsManager.ScreenConfiguration.ScaleAspect = Window.ContentScaleAspectEnum.KeepWidth;
                        screenSettingsManager.SetScreenConfiguration(screenSettingsManager.ScreenConfiguration);
                    }, 
                    () => screenSettingsManager.ScreenConfiguration.ScaleAspect == Window.ContentScaleAspectEnum.KeepWidth)
                .End()
            .TypedNode);
    }
}