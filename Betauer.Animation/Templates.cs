using System.Collections.Generic;

namespace Betauer.Animation {
    internal static partial class Templates {
        private static readonly Dictionary<string, BezierCurve> CurveBeziersCache = new Dictionary<string, BezierCurve>();

        private static BezierCurve Bezier(float p1x, float p1y, float p2x, float p2y) {
            return BezierCurve.Create(p1x, p1y, p2x, p2y);
        }
    }
}