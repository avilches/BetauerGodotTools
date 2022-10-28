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
    public class ResizeViewportStrategy : BaseScreenResolutionService, IScreenStrategy {
        public ResizeViewportStrategy(SceneTree tree) : base(tree) {
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
            var keepRatio = KeepRatio(new Resolution(windowSize));
            Tree.SetScreenStretch(StretchMode, StretchAspect, keepRatio.Size, Zoom);
            _state = $"ResizeViewport: {StretchMode}/{StretchAspect} | Zoom {Zoom} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {keepRatio.x}x{keepRatio.y}";
            Logger.Debug(_state);
        }

        public Resolution KeepRatio(Resolution resolution) {
            return StretchAspect switch {
                SceneTree.StretchAspect.KeepHeight => new Resolution(resolution.x, (int)(resolution.x / BaseResolution.AspectRatio.Ratio)),
                SceneTree.StretchAspect.KeepWidth => new Resolution((int)(resolution.y * BaseResolution.AspectRatio.Ratio), resolution.y),
                _ => resolution
            };
        }

        private string _state;
        public override string GetStateAsString() => _state;
    }
}