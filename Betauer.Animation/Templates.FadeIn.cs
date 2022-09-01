using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float FadeInDuration = 0.75f; // Animate.css: 0.75f

        // https://github.com/animate-css/animate.css/tree/main/source/fading_entrances

        internal static Sequence FadeIn() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInUp() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeY)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInUpBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenBottom())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInDown() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeY)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInDownBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Property.PositionY)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenTop())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInLeft() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, -1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInLeftBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? -Math.Abs(distance) : node.GetOutOfScreenLeft())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInRight() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySizeX)
                .KeyframeTo(0.00f, 1.0f)
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInRightBig(float distance = 0) {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateRelativeKeys(Property.PositionX)
                .KeyframeOffset(0.00f, node => distance != 0 ? Math.Abs(distance) : node.GetOutOfScreenRight())
                .KeyframeOffset(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }


        internal static Sequence FadeInTopLeft() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(-1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInTopRight() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(1f, -1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInBottomLeft() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(-1f, 1))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }

        internal static Sequence FadeInBottomRight() {
            return Sequence.Create()
                .SetDuration(FadeInDuration)
                // When position movement and fade in effects are combined, it's better to force start with Opacity zero 
                .OnStart(target => Property.Opacity.SetValue(target, 0f))
                .AnimateKeys(Property.PositionBySize2D)
                .KeyframeTo(0.00f, new Vector2(1f, 1f))
                .KeyframeTo(1.00f, Vector2.Zero)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(Property.Opacity)
                .KeyframeTo(0f, 0f)
                .KeyframeTo(1f, 1f)
                .EndAnimate();
        }
    }
}