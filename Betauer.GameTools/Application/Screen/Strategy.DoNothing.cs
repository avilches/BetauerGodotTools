using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen; 

public class DoNothingStrategy : BaseScreenResolutionService, IScreenStrategy {
    public static readonly DoNothingStrategy Instance = new();
        
    public List<ScaledResolution> GetResolutions() {
        return Resolutions.Clamp(DownScaledMinimumResolution.Size).ExpandResolutions(BaseResolution, AspectRatios).ToList();
    }

    protected override void DoApply() {
    }
}