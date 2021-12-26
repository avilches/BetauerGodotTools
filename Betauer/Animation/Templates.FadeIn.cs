using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float FadeInDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_entrances

        internal static TweenSequenceTemplate FadeIn() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInUp() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPositionY)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInUpBig(float distance = 2000) {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, -Math.Abs(distance))
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInDown() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPositionY)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInDownBig(float distance = 2000) {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, Math.Abs(distance))
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPositionX)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInLeftBig(float distance = 2000) {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, -Math.Abs(distance))
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPositionX)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInRightBig(float distance = 2000) {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, Math.Abs(distance))
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }


        internal static TweenSequenceTemplate FadeInTopLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPosition2D)
                .KeyframeTo(0.00f, new Vector2(-1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInTopRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPosition2D)
                .KeyframeTo(0.00f, new Vector2(1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInBottomLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPosition2D)
                .KeyframeTo(0.00f, new Vector2(-1f, 1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInBottomRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PercentPosition2D)
                .KeyframeTo(0.00f, new Vector2(1f, 1f))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }


    }
}