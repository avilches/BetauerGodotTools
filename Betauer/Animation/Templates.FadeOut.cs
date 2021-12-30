using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float FadeOutDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_exits

        internal static TweenSequenceTemplate FadeOut() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutUp() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutUpBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutDown() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutDownBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutLeft() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutLeftBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutRight() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutRightBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }


        internal static TweenSequenceTemplate FadeOutTopLeft() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, -1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutTopRight() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, -1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutBottomLeft() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, 1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeOutBottomRight() {
            return TemplateBuilder.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }


    }
}