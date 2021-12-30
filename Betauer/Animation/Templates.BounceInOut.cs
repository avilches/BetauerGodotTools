using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float BounceInNoDirectionDuration = 0.75f; // Animate.css: 0.75f
        private const float BounceEntranceDuration = 1f; // Animate.css: 1f

        internal static TweenSequenceTemplate BounceIn() {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceIn.css
            return TemplateBuilder.Create()
                .SetDuration(BounceInNoDirectionDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.30f, 0.30f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(1.00f, 1.00f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.40f, new Vector2(0.90f, 0.90f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.60f, new Vector2(1.03f, 1.03f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.80f, new Vector2(0.97f, 0.97f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(1.00f, Vector2.One, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(0.60f, 1.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceInUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInUp.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DY)
                .KeyframeTo(0.00f, 3.000f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(0.60f, 1.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(0.60f, -25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.75f, 10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.90f, -5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceInDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInDown.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DY)
                .KeyframeTo(0.00f, 3.000f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(0.60f, 1.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(0.60f, 25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.75f, -10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.90f, 5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceInLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInLeft.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DX)
                .KeyframeTo(0.00f, 3.000f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(0.60f, 1.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(0.60f, 25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.75f, -10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.90f, 5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceInRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInRight.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DX)
                .KeyframeTo(0.00f, 3.000f, null,node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 0.0f)
                .KeyframeTo(0.60f, 1.0f)
                .KeyframeTo(1.00f, 1.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(0.60f, -25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.75f, 10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(0.90f, -5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                .EndAnimate()
                .BuildTemplate();
        }




        internal static TweenSequenceTemplate BounceOut() {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceIn.css
            return TemplateBuilder.Create()
                .SetDuration(BounceInNoDirectionDuration)
                .AnimateKeys(property: Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.9f, 0.9f))
                .KeyframeTo(0.50f, new Vector2(1.1f, 1.1f))
                .KeyframeTo(0.55f, new Vector2(1.1f, 1.1f))
                .KeyframeTo(1.00f, new Vector2(0f, 0f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1f)
                .KeyframeTo(0.55f, 1f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceOutUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceOutUp.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DY)
                .KeyframeTo(0.00f, 1f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, 0.985f)
                .KeyframeTo(0.40f, 0.9f)
                .KeyframeTo(0.45f, 0.9f)
                .KeyframeTo(1.00f, 3f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(0.45f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, -10f)
                .KeyframeOffset(0.40f, 20f)
                .KeyframeOffset(0.45f, 20f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceOutDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceOutUp.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DY)
                .KeyframeTo(0.00f, 1f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, 0.985f)
                .KeyframeTo(0.40f, 0.9f)
                .KeyframeTo(0.45f, 0.9f)
                .KeyframeTo(1.00f, 3f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(0.45f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 10f)
                .KeyframeOffset(0.40f, -20f)
                .KeyframeOffset(0.45f, -20f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceOutLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceOutRight.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DX)
                .KeyframeTo(0.00f, 1f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, 0.90f)
                .KeyframeTo(1.00f, 2f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(0.20f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 20f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate BounceOutRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceOutRight.css
            return TemplateBuilder.Create()
                .SetDuration(BounceEntranceDuration)
                .AnimateKeys(property: Property.Scale2DX)
                .KeyframeTo(0.00f, 1f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, 0.90f)
                .KeyframeTo(1.00f, 2f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(0.20f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(property: Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, -20f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate()
                .BuildTemplate();
        }


    }
}