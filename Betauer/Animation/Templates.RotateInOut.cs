using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float RotateDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/rotating_entrances

        internal static SequenceTemplate RotateIn() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 200, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateInDownLeft() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateInDownRight() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateInUpLeft() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateInUpRight() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate RotateOut() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 200f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateOutDownLeft() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateOutDownRight() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateOutUpLeft() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static SequenceTemplate RotateOutUpRight() {
            return TemplateBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
    }
}