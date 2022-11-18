using System.Collections.Generic;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Screen {
    public abstract class BaseScreenResolutionService : IScreenResizeHandler {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(BaseScreenResolutionService));
        protected readonly SceneTree Tree;
        protected ScreenConfiguration ScreenConfiguration;
        protected Resolution DownScaledMinimumResolution => ScreenConfiguration.DownScaledMinimumResolution;
        protected Resolution BaseResolution => ScreenConfiguration.BaseResolution;

        protected List<Resolution> Resolutions => ScreenConfiguration.Resolutions;
        protected List<AspectRatio> AspectRatios => ScreenConfiguration.AspectRatios;
        protected Window.ContentScaleModeEnum StretchMode => ScreenConfiguration.StretchMode;
        protected Window.ContentScaleAspectEnum StretchAspect => ScreenConfiguration.StretchAspect;
        protected float Zoom => ScreenConfiguration.Zoom;

        protected BaseScreenResolutionService(SceneTree tree) {
            Tree = tree;
        }

        protected abstract void Setup();

        public virtual void Disable() {
        }

        public virtual void SetScreenConfiguration(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            Setup();
        }

        public virtual bool IsFullscreen() {
            var mode = DisplayServer.WindowGetMode();
            return mode is DisplayServer.WindowMode.Fullscreen or DisplayServer.WindowMode.ExclusiveFullscreen;
        }

        public virtual void SetFullscreen() {
            if (IsFullscreen()) return;
            // if (!Project.FeatureFlags.IsMacOs()) DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
            Setup();
        }

        public virtual void SetBorderless(bool borderless) {
            if (IsFullscreen() || Project.FeatureFlags.IsMacOs() || DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless) == borderless) return;
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, borderless);
            Setup();
        }

        public virtual void SetWindowed(Resolution resolution) {
            var screenSize = DisplayServer.WindowGetRealSize();
            if (resolution.x > screenSize.x || resolution.y > screenSize.y) {
                // SetFullscreen();
                return;
            }
            if (resolution.x < DownScaledMinimumResolution.x || 
                resolution.y < DownScaledMinimumResolution.y) {
                resolution = DownScaledMinimumResolution;
            }
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            DisplayServer.WindowSetSize(resolution.Size);
            DisplayServer.WindowSetMinSize(DownScaledMinimumResolution.Size);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, !ScreenConfiguration.IsResizeable);
            Setup();
        }

        public virtual void OnScreenResized() {
            Setup();
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

        public abstract string GetStateAsString();
    }
}