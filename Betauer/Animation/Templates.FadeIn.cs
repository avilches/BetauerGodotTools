using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float FadeInDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_entrances

        internal static TweenSequenceTemplate FadeIn() {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate FadeInUp() {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySizeY)
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

        internal static TweenSequenceTemplate FadeInUpBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySizeY)
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

        internal static TweenSequenceTemplate FadeInDownBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySizeX)
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

        internal static TweenSequenceTemplate FadeInLeftBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySizeX)
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

        internal static TweenSequenceTemplate FadeInRightBig(float distance = 0) {
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
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
            return TemplateBuilder.Create()
                .SetDuration(FadeInDuration)
                .AnimateKeys(property: Property.PositionBySize2D)
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