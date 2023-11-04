using System.Collections.Generic;

namespace Betauer.Application.Screen.Resolution;

internal class ResolutionByHeightComparer : IComparer<Resolution> {
    /// <summary>
    /// Compare first the height. If equals, compare the width. 
    /// </summary>
    public int Compare(Resolution left, Resolution right) {
        var height = left.Y.CompareTo(right.Y);
        return height != 0 ? height : left.X.CompareTo(right.X);
    }
}