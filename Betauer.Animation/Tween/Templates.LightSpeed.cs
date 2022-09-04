using Betauer.Animation.Easing;

namespace Betauer.Animation.Tween {
    internal static partial class Templates {
        private const float LightSpeedDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/lightspeed

        internal static Sequence LightSpeedInLeft() {
            return Sequence.Create()
                .SetDuration(LightSpeedDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(0.60f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(0.6f, 1f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Skew2DX)
                .KeyframeTo(0.00f, 1f)
                .KeyframeTo(0.60f, -0.8f)
                .KeyframeTo(0.80f, 0.16f)
                .KeyframeTo(1.00f, 0f, Easings.CircOut)
                .EndAnimate();
        }

        internal static Sequence LightSpeedInRight() {
            return Sequence.Create()
                .SetDuration(LightSpeedDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(0.60f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(0.6f, 1f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Skew2DX)
                .KeyframeTo(0.00f, -1f)
                .KeyframeTo(0.60f, 0.8f)
                .KeyframeTo(0.80f, +0.16f)
                .KeyframeTo(1.00f, 0f, Easings.CircOut)
                .EndAnimate();
        }

        internal static Sequence LightSpeedOutLeft() {
            return Sequence.Create()
                .SetDuration(LightSpeedDuration)
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Skew2DX)
                .KeyframeTo(0.00f, 0f)
                .KeyframeTo(1.00f, -1f, Easings.CircOut)
                .EndAnimate();
        }

        internal static Sequence LightSpeedOutRight() {
            return Sequence.Create()
                .SetDuration(LightSpeedDuration)
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Skew2DX)
                .KeyframeTo(0.00f, 0f)
                .KeyframeTo(1.00f, 1f, Easings.CircOut)
                .EndAnimate();
        }
    }
}