using System;
using Betauer.Nodes;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    internal static partial class TemplateFactories {
        private const float BackInOutDuration = 0.75f; // Animate.css: 1f

        internal static KeyframeAnimation BackInUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInUp.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation BackInDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInDown.css
            return KeyframeAnimation.Create()
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation BackInLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInLeft.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation BackInRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInRight.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation BackOutUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutUp.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate();
        }

        internal static KeyframeAnimation BackOutDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutDown.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate();
        }

        internal static KeyframeAnimation BackOutLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutLeft.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate();
        }

        internal static KeyframeAnimation BackOutRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutRight.css
            return KeyframeAnimation.Create()
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .AnimateRelativeKeys(Properties.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate();
        }
    }
}