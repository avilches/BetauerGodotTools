using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    public class FitToScreenResolutionService : BaseScreenResolutionService, IScreenService {
        public FitToScreenResolutionService(SceneTree tree) : base(tree) {
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
        }

        public void Disable() {
        }

        public List<ScaledResolution> GetResolutions() {
            return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
        }

        public override void SetFullscreen() {
            if (!OS.WindowFullscreen) {
                if (!FeatureFlags.IsMacOs()) {
                    OS.WindowBorderless = false;
                }
                OS.WindowFullscreen = true;
            }
        }

        protected override void DoSetBorderless(bool borderless) {
            if (!FeatureFlags.IsMacOs()) {
                if (OS.WindowBorderless == borderless) return;
                OS.WindowBorderless = borderless;
            }
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            Tree.SetScreenStretch(StretchMode, StretchAspect, resolution.Size, Zoom);
        }
    }
}