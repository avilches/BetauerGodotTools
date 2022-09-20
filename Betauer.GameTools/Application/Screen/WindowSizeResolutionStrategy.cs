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
    public class WindowSizeResolutionStrategy : BaseScreenResolutionService, IScreenStrategy {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(WindowSizeResolutionStrategy));
        public WindowSizeResolutionStrategy(SceneTree tree) : base(tree) {
        }

        public List<ScaledResolution> GetResolutions() {
            return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
        }

        protected override void Setup() {
            // Enforce minimum resolution.
            OS.MinWindowSize = ScreenConfiguration.DownScaledMinimumResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
            OS.WindowResizable = ScreenConfiguration.IsResizeable;
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
            Logger.Debug($"{StretchMode}/{StretchAspect} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {BaseResolution.x}x{BaseResolution.y}");
        }
    }
}