using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Screen.Resolution;
using Betauer.Application.Settings;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Screen;

public class ScreenController {
    private static readonly SceneTree SceneTree = (SceneTree)Engine.GetMainLoop();
    private static readonly Logger Logger = LoggerFactory.GetLogger<ScreenController>();

    public ScreenConfig ScreenConfig { get; }
    
    protected Resolution.Resolution BaseResolution => ScreenConfig.BaseResolution;

    public SaveSetting<bool>? FullscreenSetting { get; set; }                  
    public SaveSetting<bool>? VSyncSetting { get; set; }                       
    public SaveSetting<bool>? BorderlessSetting { get; set; }                  
    public SaveSetting<Vector2I>? WindowedResolutionSetting { get; set; }       

    public ScreenController(ScreenConfig screenConfig) {
        ScreenConfig = screenConfig;
    }

    public void Start() {
        SceneTree.Root.SizeChanged -= Apply;
    }

    public void Stop() {
        SceneTree.Root.SizeChanged -= Apply;
    }

    public void Apply() {
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfig.IsResizeable);
        DoApply();
        var windowSize = DisplayServer.WindowGetSize();
        var viewport = SceneTree.Root;
        var viewportResolution = viewport.Size;
        Logger.Debug($"{GetType().Name}: {viewport.ContentScaleMode}/{viewport.ContentScaleAspect} | Zoom {viewport.ContentScaleFactor} | WindowSize {windowSize.X}x{windowSize.Y} | ContentScaleSize {viewport.ContentScaleSize.X}x{viewport.ContentScaleSize.Y} | Viewport {viewportResolution.X}x{viewportResolution.Y}");
    }

    public List<ScaledResolution> GetResolutions() {
        var minSize = BaseResolution.Size;
        var maxSize = DisplayServer.ScreenGetSize();
        return ScreenConfig.Resolutions.Clamp(minSize, maxSize).ExpandResolutions(BaseResolution, ScreenConfig.AspectRatios).ToList();
    }

    public bool IsFullscreen() {
        var mode = DisplayServer.WindowGetMode();
        return mode is DisplayServer.WindowMode.Fullscreen or DisplayServer.WindowMode.ExclusiveFullscreen;
    }

    public void SetFullscreen(bool fs) {
        if (FullscreenSetting != null) FullscreenSetting.Value = true;
        DoFullscreen(fs);
    }

    public void SetWindowed(Resolution.Resolution resolution) {
        if (WindowedResolutionSetting != null) WindowedResolutionSetting.Value = resolution.Size;
        DoWindowed(resolution);
    }

    public void SetBorderless(bool borderless) {
        if (BorderlessSetting != null) BorderlessSetting.Value = borderless;
        DoBorderless(borderless);
    }

    public void SetVSync(bool vsync) {
        if (VSyncSetting != null) VSyncSetting.Value = vsync;
        DoVSync(vsync);
    }

    public void DoVSync(bool vsync) {
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
    }

    public void DoFullscreen(bool fs) {
        if (fs == IsFullscreen()) return;
        if (fs) {
            if (Project.FeatureFlags.IsMacOs()) {
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
            }
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        } else {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        Apply();
    }

    public void DoBorderless(bool borderless) {
        if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Windowed) return;
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, borderless);
        Apply();
    }

    public void DoWindowed(Resolution.Resolution resolution) {
        var screenSize = DisplayServer.ScreenGetSize();
        if (resolution.X > screenSize.X || resolution.Y > screenSize.Y) {
            // SetFullscreen();
            return;
        }
        if (resolution.X < BaseResolution.X || 
            resolution.Y < BaseResolution.Y) {
            resolution = BaseResolution;
        }
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        DisplayServer.WindowSetSize(resolution.Size);
        DisplayServer.WindowSetMinSize(BaseResolution.Size);
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfig.IsResizeable);
        Apply();
    }

    public void OnScreenResized() {
        Apply();
    }

    public void DoApply() {
        if (_resizeViewport) {
            var windowSize = DisplayServer.WindowGetSize();
            var keepRatio = KeepRatio(new Resolution.Resolution(windowSize));
            SceneTree.Root.ContentScaleMode = ScreenConfig.ScaleMode;
            SceneTree.Root.ContentScaleAspect = ScreenConfig.ScaleAspect;
            SceneTree.Root.ContentScaleFactor = ScreenConfig.ScaleFactor;
            SceneTree.Root.ContentScaleSize = keepRatio.Size;
        } else {
            SceneTree.Root.ContentScaleMode = ScreenConfig.ScaleMode;
            SceneTree.Root.ContentScaleAspect = ScreenConfig.ScaleAspect;
            SceneTree.Root.ContentScaleFactor = ScreenConfig.ScaleFactor;
            SceneTree.Root.ContentScaleSize = BaseResolution.Size;
        }
    }

    public bool _resizeViewport = true;

    private Resolution.Resolution KeepRatio(Resolution.Resolution resolution) {
        return ScreenConfig.ScaleAspect switch {
            Window.ContentScaleAspectEnum.KeepHeight => new Resolution.Resolution(resolution.X, (int)(resolution.X / BaseResolution.AspectRatio.Ratio)),
            Window.ContentScaleAspectEnum.KeepWidth => new Resolution.Resolution((int)(resolution.Y * BaseResolution.AspectRatio.Ratio), resolution.Y),
            _ => resolution
        };
    }

    public void CenterWindow() {
        if (IsFullscreen()) return;
        var screenSize = DisplayServer.ScreenGetSize();
        var windowSize = DisplayServer.WindowGetSize();
        var centeredPos = (screenSize - windowSize) / 2;
        DisplayServer.WindowSetPosition(centeredPos);
    }
}