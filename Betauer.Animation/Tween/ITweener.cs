using Godot;

namespace Betauer.Animation.Tween {
    public interface ITweener {
        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target);
        public bool IsCompatibleWith(Node node);
    }
}