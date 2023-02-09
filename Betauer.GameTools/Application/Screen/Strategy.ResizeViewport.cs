using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen; 

/// <summary>
/// On resize/change resolution change the viewport resolution along with the window size:
/// - StretchAspect.Expand/Keep: User interface container changes their size with the viewport but controls (and fonts!) keep the aspect ratio
/// - StretchAspect.KeepHeight: the viewport grows/shrink keeping the aspect ratio of base. Expand width only. The more width, the smaller the controls. Changing height keep aspect ratio of controls.
/// - StretchAspect.KeepWidth: the viewport grows/shrink, but keeping the width aspect ratio of base. Expand height only. The more height, the smaller the controls. Changing width keep aspect ratio of controls.
/// </summary>
public class ResizeViewportStrategy : BaseScreenResolutionService, IScreenStrategy {
    public static readonly ResizeViewportStrategy Instance = new();

    public List<ScaledResolution> GetResolutions() {
        return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
    }

    protected override void DoApply() {
        var windowSize = DisplayServer.WindowGetSize();
        var keepRatio = KeepRatio(new Resolution(windowSize));
        SceneTree.Root.ContentScaleMode = ScaleMode;
        SceneTree.Root.ContentScaleAspect = ScaleAspect;
        SceneTree.Root.ContentScaleFactor = ScaleFactor;
        SceneTree.Root.ContentScaleSize = keepRatio.Size;
    }

    public Resolution KeepRatio(Resolution resolution) {
        return ScaleAspect switch {
            Window.ContentScaleAspectEnum.KeepHeight => new Resolution(resolution.X, (int)(resolution.X / BaseResolution.AspectRatio.Ratio)),
            Window.ContentScaleAspectEnum.KeepWidth => new Resolution((int)(resolution.Y * BaseResolution.AspectRatio.Ratio), resolution.Y),
            _ => resolution
        };
    }
}