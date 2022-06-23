using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float BackInOutDuration = 0.75f; // Animate.css: 1f

        internal static SequenceTemplate BackInUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInUp.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackInDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInDown.css
            return TemplateBuilder.Create()
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .SetDuration(BackInOutDuration)
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackInLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInLeft.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackInRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInRight.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, Vector2.One)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 0.7f)
                .KeyframeTo(0.80f, 0.7f)
                .KeyframeTo(1.00f, 1)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(0.80f, 0f)
                .KeyframeOffset(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackOutUp(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutUp.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackOutDown(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutDown.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackOutLeft(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutLeft.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .EndAnimate()
                .BuildTemplate();
        }

        internal static SequenceTemplate BackOutRight(float distance = 0) {
            // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutRight.css
            return TemplateBuilder.Create()
                .SetDuration(BackInOutDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Scale2D)
                .KeyframeTo(0.00f, Vector2.One, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0.00f, 1)
                .KeyframeTo(0.20f, 0.7f)
                .KeyframeTo(1.00f, 0.7f)
                .EndAnimate()
                .Parallel()
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, 0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(1.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .EndAnimate()
                .BuildTemplate();
        }
    }
}