using System;
using Godot;

namespace Betauer.Animation {
    public interface IAnimation {
        public Node? DefaultTarget { get; }
        public float ScaleSpeed { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public Action<Node>? StartAction { get; }
        public Action<Node>? FinishAction { get; }
        public Action? FinishAllAction { get; }
        public Tween Play(Node? target = null, float initialDelay = 0);
        public bool IsCompatibleWith(Node node);
    }
}