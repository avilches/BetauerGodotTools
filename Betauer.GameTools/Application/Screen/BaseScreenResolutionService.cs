using System.Collections.Generic;
using Betauer.Memory;
using Godot;

namespace Betauer.Application.Screen {
    public abstract class BaseScreenResolutionService : DisposableObject {
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

        public bool IsFullscreen() => OS.WindowFullscreen;
        public abstract void SetFullscreen();

        public void SetBorderless(bool borderless) {
            if (IsFullscreen()) return;
            DoSetBorderless(borderless);
        }

        protected abstract void DoSetBorderless(bool borderless);

        public void SetWindowed(Resolution resolution) {
            var screenSize = OS.GetScreenSize();
            if (resolution.x > screenSize.x || resolution.y > screenSize.y) {
                SetFullscreen();
                return;
            }
            if (resolution.x < DownScaledMinimumResolution.x || resolution.y < DownScaledMinimumResolution.y) {
                DoSetWindowed(DownScaledMinimumResolution);
            } else {
                DoSetWindowed(resolution);
            }
        }

        protected abstract void DoSetWindowed(Resolution resolution);


        public void CenterWindow() {
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
    }
}