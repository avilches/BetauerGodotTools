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
    public class FixedViewportStrategy : BaseScreenResolutionService, IScreenStrategy {
        public FixedViewportStrategy(SceneTree tree) : base(tree) {
        }

        public List<ScaledResolution> GetResolutions() {
            return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
        }

        protected override void Setup() {
            // Enforce minimum resolution.
            // TODO Godot 4
            /*
            OS.MinWindowSize = ScreenConfiguration.DownScaledMinimumResolution.Size;
            if (OS.WindowSize < OS.MinWindowSize) {
                OS.WindowSize = OS.MinWindowSize;
            }
            OS.WindowResizable = ScreenConfiguration.IsResizeable;
            var windowSize = OS.WindowFullscreen ? OS.GetScreenSize() : OS.WindowSize;
            // Tree.SetScreenStretch(StretchMode, StretchAspect, BaseResolution.Size, Zoom);
            Tree.Root.ContentScaleMode = StretchMode;
            Tree.Root.ContentScaleAspect = StretchAspect;
            Tree.Root.ContentScaleFactor = Zoom;
            Tree.Root.ContentScaleSize = BaseResolution.Size;
            
            
            _state = $"FixedViewport: {StretchMode}/{StretchAspect} | Zoom {Zoom} | WindowSize {windowSize.x}x{windowSize.y} | Viewport {BaseResolution.x}x{BaseResolution.y}";
            Logger.Debug(_state);
            */
        }
        
        private string _state;
        public override string GetStateAsString() => _state;
    }
}