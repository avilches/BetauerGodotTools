using System;
using Godot;

namespace Betauer.Animation.Tween {
    internal static partial class Templates {
        private const float FadeOutDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_exits

        internal static Sequence FadeOut() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutUp() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutUpBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutDown() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutDownBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutLeft() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutLeftBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutRight() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutRightBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }


        internal static Sequence FadeOutTopLeft() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, -1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutTopRight() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, -1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutBottomLeft() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, 1))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static Sequence FadeOutBottomRight() {
            return Sequence.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }


    }
}