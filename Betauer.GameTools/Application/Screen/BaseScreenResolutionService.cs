using System.Collections.Generic;
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
        protected SceneTree.StretchMode StretchMode => ScreenConfiguration.StretchMode;
        protected SceneTree.StretchAspect StretchAspect => ScreenConfiguration.StretchAspect;
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

        public virtual bool IsFullscreen() => OS.WindowFullscreen;

        public virtual void SetFullscreen() {
            if (OS.WindowFullscreen) return;
            if (!FeatureFlags.IsMacOs()) OS.WindowBorderless = false;
            OS.WindowFullscreen = true;
            Setup();
        }

        public virtual void SetBorderless(bool borderless) {
            if (IsFullscreen() || FeatureFlags.IsMacOs() || OS.WindowBorderless == borderless) return;
            OS.WindowBorderless = borderless;
            Setup();
        }

        public virtual void SetWindowed(Resolution resolution) {
            var screenSize = OS.GetScreenSize();
            if (resolution.x > screenSize.x || resolution.y > screenSize.y) {
                SetFullscreen();
                return;
            }
            if (resolution.x < DownScaledMinimumResolution.x || 
                resolution.y < DownScaledMinimumResolution.y) {
                resolution = DownScaledMinimumResolution;
            }
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            Setup();
        }

        public virtual void OnScreenResized() {
            Setup();
        }

        public virtual void CenterWindow() {
            if (OS.WindowFullscreen) return;
            OS.CenterWindow();
            // TODO why this instead of OS.CenterWindow()
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