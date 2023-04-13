using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen; 

/// <summary>
/// On resize/change resolution: change the window size only, viewport resolution is always baseResolution
/// StretchAspect: it's applied
/// User interface container changes their size with the viewport (following StretchAspect) and
/// controls (and fonts!) doesn't keep the aspect ratio (they shrink or expand)
/// </summary>
public class FixedViewportStrategy : BaseScreenResolutionService, IScreenStrategy {
    public static readonly FixedViewportStrategy Instance = new();
        
    public List<ScaledResolution> GetResolutions() {
        return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
    }

    protected override void DoApply() {
        SceneTree.Root.ContentScaleMode = ScaleMode;
        SceneTree.Root.ContentScaleAspect = ScaleAspect;
        SceneTree.Root.ContentScaleFactor = ScaleFactor;
        SceneTree.Root.ContentScaleSize = BaseResolution.Size;
    }
}