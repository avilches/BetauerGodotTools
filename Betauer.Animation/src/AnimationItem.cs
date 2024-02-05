using System;
using Betauer.Core.Easing;
using Godot;

namespace Betauer.Animation {
    public abstract class AnimationItem<[MustBeVariant] TProperty> {
        public readonly IInterpolation? Easing;
        public readonly Action<Node>? CallbackNode;

        internal AnimationItem(IInterpolation? easing, Action<Node>? callbackNode) {
            Easing = easing;
            CallbackNode = callbackNode;
        }

        public abstract TProperty GetTo(Node target, TProperty from);
    }
}