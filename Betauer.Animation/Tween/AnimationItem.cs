using System;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {
    public abstract class AnimationItem<TProperty> {
        public readonly IEasing? Easing;
        public readonly Action<Node>? CallbackNode;

        internal AnimationItem(IEasing? easing, Action<Node>? callbackNode) {
            Easing = easing;
            CallbackNode = callbackNode;
        }

        public abstract TProperty GetTo(Node target, TProperty from);
    }
}