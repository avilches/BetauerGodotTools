using System;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    internal static partial class TemplateFactories {
        private const float FadeInDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_entrances

        internal static KeyframeAnimation FadeIn() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInUp() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySizeY)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInUpBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInDown() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySizeY)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInDownBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySizeX)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInLeftBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySizeX)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInRightBig(float distance = 0) {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }


        internal static KeyframeAnimation FadeInTopLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(-1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInTopRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInBottomLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(-1f, 1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation FadeInBottomRight() {
            return KeyframeAnimation.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(1f, 1f))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }
    }
}