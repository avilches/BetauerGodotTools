using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float HingeDuration = 1.5f; // Animate.css: 2f

        // https://github.com/animate-css/animate.css/tree/main/source/specials

        internal static TweenSequenceTemplate Hinge() {
            return TweenSequenceBuilder.Create()
                .SetDuration(HingeDuration)
                .AnimateRelativeKeys(property: Property.PositionY) // TODO: try this one instead,Easing.QuadInOut)
                .KeyframeOffset(0.00f, 0.0f, node => node.SetRotateOriginToTopLeft())
                .KeyframeOffset(0.80f, 0.0f)
                .KeyframeOffset(1.00f, 700f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.0f,  0f)
                .KeyframeTo(0.20f, 80.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.40f, 60.0f) // TODO: try this one instead, Easing.QuadInOut)
                .KeyframeTo(0.60f, 80.0f)
                .KeyframeTo(0.80f, 60.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f,  1.0f)
                .KeyframeTo(0.80f, 1.0f)
                .KeyframeTo(1.00f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }


    }
}