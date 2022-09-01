using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float RotateDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/rotating_entrances

        internal static Sequence RotateIn() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 200, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static Sequence RotateInDownLeft() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static Sequence RotateInDownRight() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static Sequence RotateInUpLeft() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static Sequence RotateInUpRight() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }

        internal static Sequence RotateOut() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 200f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static Sequence RotateOutDownLeft() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static Sequence RotateOutDownRight() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static Sequence RotateOutUpLeft() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static Sequence RotateOutUpRight() {
            return Sequence.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
    }
}