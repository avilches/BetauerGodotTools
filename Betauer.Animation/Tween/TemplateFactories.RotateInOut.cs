using Betauer.Nodes;
using Betauer.Nodes.Property;

namespace Betauer.Animation.Tween {
    internal static partial class TemplateFactories {
        private const float RotateDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/rotating_entrances

        internal static KeyframeAnimation RotateIn() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 200, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateInDownLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateInDownRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateInUpLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateInUpRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate();
        }

        internal static KeyframeAnimation RotateOut() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 200f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateOutDownLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateOutDownRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateOutUpLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
        internal static KeyframeAnimation RotateOutUpRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate();
        }
    }
}