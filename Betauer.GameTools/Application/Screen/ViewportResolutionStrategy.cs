using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    /// <summary>
    /// On resize/change resolution change the viewport resolution along with the window size:
    /// - StretchAspect.Expand/Keep: User interface container changes their size with the viewport but controls (and fonts!) keep the aspect ratio
    /// - StretchAspect.KeepHeight: the viewport grows/shrink keeping the aspect ratio of base. Expand width only. The more width, the smaller the controls. Changing height keep aspect ratio of controls.
    /// - StretchAspect.KeepWidth: the viewport grows/shrink, but keeping the width aspect ratio of base. Expand height only. The more height, the smaller the controls. Changing width keep aspect ratio of controls.
    /// </summary>
    public class ViewportResolutionStrategy : BaseScreenResolutionService, IScreenStrategy, IScreenResizeHandler {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ViewportResolutionStrategy));
        public ViewportResolutionStrategy(SceneTree tree) : base(tree) {
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

        public void OnScreenResized() {
            var resolution = OS.WindowSize;
            var keepRatio = KeepRatio(new Resolution(resolution));
            Tree.SetScreenStretch(StretchMode, StretchAspect, keepRatio.Size, Zoom);
            Logger.Debug($"Regular: {StretchMode}/{StretchAspect} | WindowSize {resolution.x}x{resolution.y} | Viewport {keepRatio.x}x{keepRatio.y}");
        }

        protected override void DoSetWindowed(Resolution resolution) {
            if (OS.WindowFullscreen) OS.WindowFullscreen = false;
            OS.WindowSize = resolution.Size;
            var keepRatio = KeepRatio(resolution);
            Tree.SetScreenStretch(StretchMode, StretchAspect, keepRatio.Size, Zoom);
            Logger.Debug($"Regular: {StretchMode}/{StretchAspect} | WindowSize {resolution.x}x{resolution.y} | Viewport {keepRatio.x}x{keepRatio.y}");
        }

        public Resolution KeepRatio(Resolution resolution) {
            if (StretchAspect == SceneTree.StretchAspect.KeepHeight) {
                return new Resolution(resolution.x, (int)(resolution.x / BaseResolution.AspectRatio.Ratio));
            }
            if (StretchAspect == SceneTree.StretchAspect.KeepWidth) {
                return new Resolution((int)(resolution.y * BaseResolution.AspectRatio.Ratio), resolution.y);
            }
            return resolution;

        }
    }
}