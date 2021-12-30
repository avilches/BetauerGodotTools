using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float HingeDuration = 1.5f;
        private const float JackInTheBoxDuration = 0.5f;
        private const float RollDuration = 0.5f;

        // https://github.com/animate-css/animate.css/tree/main/source/specials

        internal static SequenceTemplate Hinge() {
            return TemplateBuilder.Create()
                .SetDuration(HingeDuration)
                .AnimateRelativeKeys(property: Property.PositionY) // TODO: try this one instead,Easing.QuadInOut)
                .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToTopLeft())
                .KeyframeOffset(0.80f, 0.0f)
                .KeyframeOffset(1.00f, 700f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(0.20f, 80.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.40f, 60.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.60f, 80.0f)
                .KeyframeTo(0.80f, 60.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1.0f)
                .KeyframeTo(0.80f, 1.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate JackInTheBox() {
            return TemplateBuilder.Create()
                .SetDuration(JackInTheBoxDuration)
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 30, null, node => node.SetRotateOriginToBottomCenter())
                .KeyframeTo(0.50f, -10)
                .KeyframeTo(0.70f, 3)
                .KeyframeTo(1.00f, 0)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(0.50f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate RollIn() {
            return TemplateBuilder.Create()
                .SetDuration(RollDuration)
                .AnimateRelativeKeys(property: Property.PositionBySizeX)
                .KeyframeOffset(0.00f, -1.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, -120)
                .KeyframeTo(1.00f, 0)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate RollOut() {
            return TemplateBuilder.Create()
                .SetDuration(RollDuration)
                .AnimateRelativeKeys(property: Property.PositionBySizeX)
                .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Rotate2D)
                .KeyframeTo(0.00f, 0)
                .KeyframeTo(1.00f, 120)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .BuildTemplate();
        }
    }
}