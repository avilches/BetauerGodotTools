using System.Collections.Generic;

namespace Betauer.Application.Screen.Resolution;

internal class ResolutionByAreaComparer : IComparer<Resolution> {
    /// <summary>
    /// Multiply the height x width to get the area
    /// </summary>
    public int Compare(Resolution left, Resolution right) => (left.X * left.Y).CompareTo(right.X * right.Y);
}