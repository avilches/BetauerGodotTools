using Godot;

namespace Betauer.Animation.Tween {
    public interface ITweener {
        float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration);
    }
}