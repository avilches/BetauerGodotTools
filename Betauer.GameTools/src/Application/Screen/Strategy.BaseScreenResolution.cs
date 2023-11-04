using System.Collections.Generic;
using Betauer.Application.Screen.Resolution;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Screen;
public abstract class BaseScreenResolutionStrategy : IScreenResizeHandler {
    protected static readonly SceneTree SceneTree = (SceneTree)Engine.GetMainLoop();

    protected static readonly Logger Logger = LoggerFactory.GetLogger<BaseScreenResolutionStrategy>();

    protected ScreenConfig ScreenConfig;
    protected Resolution.Resolution BaseResolution => ScreenConfig.BaseResolution;

    protected List<Resolution.Resolution> Resolutions => ScreenConfig.Resolutions;
    protected List<AspectRatio> AspectRatios => ScreenConfig.AspectRatios;
    protected Window.ContentScaleModeEnum ScaleMode => ScreenConfig.ScaleMode;
    protected Window.ContentScaleAspectEnum ScaleAspect => ScreenConfig.ScaleAspect;
    protected float ScaleFactor => ScreenConfig.ScaleFactor;

    public virtual void Apply() {
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfig.IsResizeable);
        DoApply();
        var windowSize = DisplayServer.WindowGetSize();
        var viewport = SceneTree.Root;
        var viewportResolution = viewport.Size;
        Logger.Debug($"{GetType().Name}: {viewport.ContentScaleMode}/{viewport.ContentScaleAspect} | Zoom {viewport.ContentScaleFactor} | WindowSize {windowSize.X}x{windowSize.Y} | ContentScaleSize {viewport.ContentScaleSize.X}x{viewport.ContentScaleSize.Y} | Viewport {viewportResolution.X}x{viewportResolution.Y}");
    }

    protected abstract void DoApply();

    public virtual void Disable() {
    }

    public void SetScreenConfig(ScreenConfig screenController) {
        ScreenConfig = screenController;
    }

    public virtual bool IsFullscreen() {
        var mode = DisplayServer.WindowGetMode();
        return mode is DisplayServer.WindowMode.Fullscreen or DisplayServer.WindowMode.ExclusiveFullscreen;
    }

    public virtual void SetFullscreen() {
        if (IsFullscreen()) return;
        // if (!Project.FeatureFlags.IsMacOs()) DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        Apply();
    }

    public virtual void SetBorderless(bool borderless) {
        if (IsFullscreen() || Project.FeatureFlags.IsMacOs() || DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless) == borderless) return;
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, borderless);
        Apply();
    }

    public virtual void SetWindowed(Resolution.Resolution resolution) {
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

    public virtual void OnScreenResized() {
        Apply();
    }

    public virtual void CenterWindow() {
        if (IsFullscreen()) return;
        // TODO Godot 4: 
        // var currentScreen = OS.CurrentScreen;
        // var screenSize = OS.GetScreenSize(currentScreen);
        // var windowSize = OS.WindowSize;
        // var centeredPos = (screenSize - windowSize) / 2;
        // OS.WindowPosition = centeredPos;
        // OS.CurrentScreen = currentScreen;
    }
}