using System.Collections.Generic;
using Betauer.Animation.Easing;

namespace Betauer.Animation.Tween {
    internal static partial class TemplateFactories {
        private static readonly Dictionary<string, BezierCurve> CurveBeziersCache = new Dictionary<string, BezierCurve>();

        private static BezierCurve Bezier(float p1X, float p1Y, float p2X, float p2Y) {
            return BezierCurve.Create(p1X, p1Y, p2X, p2Y);
        }
    }
}