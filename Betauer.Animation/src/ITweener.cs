using Godot;

namespace Betauer.Animation {
    public interface ITweener {
        public float Start(Tween sceneTreeTween, float initialDelay, Node target);
        public bool IsCompatibleWith(Node node);
    }
}