using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float LightSpeedDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/lightspeed

        internal static SequenceTemplate LightSpeedInLeft() {
            return TemplateBuilder.Create()
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
                .KeyframeTo(1.00f, 0f, Easing.CircOut)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate LightSpeedInRight() {
            return TemplateBuilder.Create()
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
                .KeyframeTo(1.00f, 0f, Easing.CircOut)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate LightSpeedOutLeft() {
            return TemplateBuilder.Create()
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
                .KeyframeTo(1.00f, -1f, Easing.CircOut)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate LightSpeedOutRight() {
            return TemplateBuilder.Create()
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
                .KeyframeTo(1.00f, 1f, Easing.CircOut)
                .EndAnimate()
                .BuildTemplate();
        }
    }
}