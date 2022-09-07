using System;
using Betauer.Nodes;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    internal static partial class TemplateFactories {
        private const float FadeOutDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_exits

        internal static KeyframeAnimation FadeOut() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutUp() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutUpBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutDown() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySizeY)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutDownBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, -1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutLeftBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySizeX)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutRightBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, 0.0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }


        internal static KeyframeAnimation FadeOutTopLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, -1))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutTopRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, -1))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutBottomLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(-1f, 1))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeOutBottomRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeOutDuration)
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(1.00f, new Vector2(1f, 1f))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 1f)
                .KeyframeTo(1f, 0f)
                .EndAnimate();
        }


    }
}