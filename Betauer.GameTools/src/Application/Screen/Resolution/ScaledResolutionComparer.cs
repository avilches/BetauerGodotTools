using System.Collections.Generic;

namespace Betauer.Application.Screen.Resolution;

internal class ScaledResolutionComparer : IEqualityComparer<ScaledResolution> {
    internal ScaledResolutionComparer() {
    }

    public bool Equals(ScaledResolution x, ScaledResolution y) => x.Size.Equals(y.Size);
    public int GetHashCode(ScaledResolution scaledResolution) => scaledResolution.Size.GetHashCode();
}