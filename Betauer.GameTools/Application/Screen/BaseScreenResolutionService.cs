using System.Collections.Generic;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Screen;
public abstract class BaseScreenResolutionService : IScreenResizeHandler {
    protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(BaseScreenResolutionService));
    protected ScreenService ScreenService;
    protected SceneTree SceneTree => ScreenService.SceneTree;

    protected ScreenConfiguration ScreenConfiguration => ScreenService.ScreenConfiguration;
    protected Resolution DownScaledMinimumResolution => ScreenConfiguration.DownScaledMinimumResolution;
    protected Resolution BaseResolution => ScreenConfiguration.BaseResolution;

    protected List<Resolution> Resolutions => ScreenConfiguration.Resolutions;
    protected List<AspectRatio> AspectRatios => ScreenConfiguration.AspectRatios;
    protected Window.ContentScaleModeEnum ScaleMode => ScreenConfiguration.ScaleMode;
    protected Window.ContentScaleAspectEnum ScaleAspect => ScreenConfiguration.ScaleAspect;
    protected float ScaleFactor => ScreenConfiguration.ScaleFactor;

    public virtual void Apply() {
        DoApply();
        var windowSize = DisplayServer.WindowGetSize();
        var viewport = SceneTree.Root;
        var viewportResolution = viewport.Size;
        Logger.Debug($"{GetType().Name}: {viewport.ContentScaleMode}/{viewport.ContentScaleAspect} | Zoom {viewport.ContentScaleFactor} | WindowSize {windowSize.X}x{windowSize.Y} | ContentScaleSize {viewport.ContentScaleSize.X}x{viewport.ContentScaleSize.Y} | Viewport {viewportResolution.X}x{viewportResolution.Y}");
    }

    protected abstract void DoApply();

    public virtual void Disable() {
    }

    public void SetScreenService(ScreenService screenService) {
        ScreenService = screenService;
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

    public virtual void SetWindowed(Resolution resolution) {
        var screenSize = DisplayServer.ScreenGetSize();
        if (resolution.X > screenSize.X || resolution.Y > screenSize.Y) {
            // SetFullscreen();
            return;
        }
        if (resolution.X < DownScaledMinimumResolution.X || 
            resolution.Y < DownScaledMinimumResolution.Y) {
            resolution = DownScaledMinimumResolution;
        }
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        DisplayServer.WindowSetSize(resolution.Size);
        DisplayServer.WindowSetMinSize(DownScaledMinimumResolution.Size);
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfiguration.IsResizeable);
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