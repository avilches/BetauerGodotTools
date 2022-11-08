using System;
using Godot;

namespace Betauer.Animation.Tween {
    public interface IAnimation {
        public Node? DefaultTarget { get; }
        public float Speed { get; }
        public Godot.Tween.TweenProcessMode ProcessMode { get; }
        public Action<Node>? StartAction { get; }
        public Tween Play(Node? target = null, float initialDelay = 0);
        public bool IsCompatibleWith(Node node);
    }
}