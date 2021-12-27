using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float RotateDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/rotating_entrances

        internal static TweenSequenceTemplate RotateIn() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 200, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInDownLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, -45, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInDownRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 45, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInUpLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 45, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInUpRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, -45, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
    }
}