using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    internal static partial class TemplateFactories {
        private const float HingeDuration = 1.5f;
        private const float JackInTheBoxDuration = 0.5f;
        private const float RollDuration = 0.5f;

        // https://github.com/animate-css/animate.css/tree/main/source/specials

        internal static KeyframeAnimation Hinge() {
            return KeyframeAnimation.Create()
                .SetDuration(HingeDuration)
                .AnimateRelativeKeys(Properties.PositionY) // TODO: try this one instead,Easing.QuadInOut)
                .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToTopLeft())
                .KeyframeOffset(0.80f, 0.0f)
                .KeyframeOffset(1.00f, 700f)
                .EndAnimate()
                .AnimateKeys(Properties.Rotate2D)
                .From(0f)
                .KeyframeTo(0.20f, 80.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.40f, 60.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.60f, 80.0f)
                .KeyframeTo(0.80f, 60.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.0f, 1.0f)
                .KeyframeTo(0.80f, 1.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation JackInTheBox() {
            return KeyframeAnimation.Create()
                .SetDuration(JackInTheBoxDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 30, null, node => node.SetRotateOriginToBottomCenter())
                .KeyframeTo(0.50f, -10)
                .KeyframeTo(0.70f, 3)
                .KeyframeTo(1.00f, 0)
                .EndAnimate()
                .AnimateKeys(Properties.Scale2D)
                .KeyframeTo(0.00f, Vector2.Zero)
                .KeyframeTo(0.50f, Vector2.One)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation RollInLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RollDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionBySizeX)
                .KeyframeOffset(0.00f, -1.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, -120)
                .KeyframeTo(1.00f, 0)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation RollInRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RollDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionBySizeX)
                .KeyframeOffset(0.00f, 1.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, -120)
                .KeyframeTo(1.00f, 0)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation RollOutLeft() {
            return KeyframeAnimation.Create()
                .SetDuration(RollDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Properties.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Properties.PositionBySizeX)
                .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, -1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0)
                .KeyframeTo(1.00f, 120)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate();
        }

        internal static KeyframeAnimation RollOutRight() {
            return KeyframeAnimation.Create()
                .SetDuration(RollDuration)
                .AnimateRelativeKeys(Properties.PositionBySizeX)
                .KeyframeOffset(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeOffset(1.00f, 1.0f)
                .EndAnimate()
                .AnimateKeys(Properties.Rotate2D)
                .KeyframeTo(0.00f, 0)
                .KeyframeTo(1.00f, 120)
                .EndAnimate()
                .AnimateKeys(Properties.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate();
        }
    }
}