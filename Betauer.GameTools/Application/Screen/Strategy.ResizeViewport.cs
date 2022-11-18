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
            var windowSize = DisplayServer.WindowGetSize();
            var keepRatio = KeepRatio(new Resolution(windowSize));
            Tree.Root.ContentScaleMode = StretchMode;
            Tree.Root.ContentScaleAspect = StretchAspect;
            Tree.Root.ContentScaleFactor = Zoom;
            Tree.Root.ContentScaleSize = keepRatio.Size;
            
            _state = $"ResizeViewport: {StretchMode}/{StretchAspect} | Zoom {Zoom} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {keepRatio.x}x{keepRatio.y}";
            Logger.Debug(_state);
        }

        private string _state;
        public override string GetStateAsString() => _state;

        public Resolution KeepRatio(Resolution resolution) {
            return StretchAspect switch {
                Window.ContentScaleAspectEnum.KeepHeight => new Resolution(resolution.x, (int)(resolution.x / BaseResolution.AspectRatio.Ratio)),
                Window.ContentScaleAspectEnum.KeepWidth => new Resolution((int)(resolution.y * BaseResolution.AspectRatio.Ratio), resolution.y),
                _ => resolution
            };
        }
    }
}