using System;
using Godot;

namespace Betauer.Animation.Tween {
    internal static partial class Templates {
        private const float BackInOutDuration = 0.75f; // Animate.css: 1f

        internal static Sequence BackInUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInUp.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static Sequence BackInDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInDown.css
            return Sequence.Create()
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static Sequence BackInLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInLeft.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static Sequence BackInRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInRight.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static Sequence BackOutUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutUp.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate();
        }

        internal static Sequence BackOutDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutDown.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate();
        }

        internal static Sequence BackOutLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutLeft.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate();
        }

        internal static Sequence BackOutRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutRight.css
            return Sequence.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate();
        }
    }
}