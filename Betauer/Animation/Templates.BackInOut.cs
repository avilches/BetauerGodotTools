using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float BackInOutDuration = 0.75f; // Animate.css: 1f

        internal static TweenSequenceTemplate BackInUp(float distance = 1200f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInUp.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, Math.Abs(distance))
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackInDown(float distance = 1200f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInDown.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, -Math.Abs(distance))
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackInLeft(float distance = 2000f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInLeft.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, -Math.Abs(distance))
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackInRight(float distance = 2000f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInRight.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, Math.Abs(distance))
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackOutUp(float distance = 700f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutUp.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, -Math.Abs(distance))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackOutDown(float distance = 700f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutDown.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, Math.Abs(distance))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackOutLeft(float distance = 2000f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutLeft.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, -Math.Abs(distance))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BackOutRight(float distance = 2000f) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutRight.css
            return TweenSequenceBuilder.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, Math.Abs(distance))
                .EndAnimate()
                .BuildTemplate();
        }
    }
}