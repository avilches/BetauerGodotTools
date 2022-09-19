using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    /// <summary>
    /// On resize/change resolution: change the window size only, viewport resolution is always baseResolution
    /// StretchAspect: it's applied
    /// User interface container changes their size with the viewport (following StretchAspect) and
    /// controls (and fonts!) doesn't keep the aspect ratio (they shrink or expand)
    /// </summary>
    public class WindowSizeResolutionStrategy : BaseScreenResolutionService, IScreenStrategy, IScreenResizeHandler {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(WindowSizeResolutionStrategy));
        public WindowSizeResolutionStrategy(SceneTree tree) : base(tree) {
        }

        public void Enable(ScreenConfiguration screenConfiguration) {
            ScreenConfiguration = screenConfiguration;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
            Logger.Debug($"Regular: {StretchMode}/{StretchAspect} | Viewport {BaseResolution.x}x{BaseResolution.y}");
        }

        public void Disable() {
        }

        public List<ScaledResolution> GetResolutions() {
            return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
        }

        public override void SetFullscreen() {
            if (!OS.WindowFullscreen) {
                if (!FeatureFlags.IsMacOs()) OS.WindowBorderless = false;
                OS.WindowFullscreen = true;
            }
        }

        protected override void DoSetBorderless(bool borderless) {
            if (!FeatureFlags.IsMacOs()) {
                if (OS.WindowBorderless == borderless) return;
                OS.WindowBorderless = borderless;
            }
        }

        // El StretchAspect se usa
        public void OnScreenResized() {
            var resolution = OS.WindowSize;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
            Logger.Debug($"Regular: {StretchMode}/{StretchAspect} | WindowSize {resolution.x}x{resolution.y} | Viewport {BaseResolution.x}x{BaseResolution.y}");
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
            Logger.Debug($"ScaleUi: {StretchMode}/{StretchAspect} | WindowSize {resolution.x}x{resolution.y} | Viewport {BaseResolution.x}x{BaseResolution.y}");
        }
    }
}