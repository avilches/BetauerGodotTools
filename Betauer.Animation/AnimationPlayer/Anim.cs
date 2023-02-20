using Godot;

namespace Betauer.Animation.AnimationPlayer;

public class Anim {
    public readonly StringName StringName;
    public readonly string Name;
    public readonly Godot.AnimationPlayer AnimationPlayer;
    public readonly Godot.Animation Animation;

    public Anim(Godot.AnimationPlayer animationPlayer, string name) {
        AnimationPlayer = animationPlayer;
        Name = name;
        StringName = name;
        Animation = animationPlayer.GetAnimation(StringName);
    }

    public void PlayFrom(double from, bool update = false, double customBlend = -1.0, float customSpeed = 1f) {
        AnimationPlayer.Play(StringName, customBlend, customSpeed);
        AnimationPlayer.Seek(from, update);
    }

    public void Play(double customBlend = -1.0, float customSpeed = 1f, bool fromEnd = false) => AnimationPlayer.Play(StringName, customBlend, customSpeed, fromEnd);
    public void PlayBackwards(double customBlend = -1.0) => AnimationPlayer.PlayBackwards(StringName, customBlend);
    public bool IsPlaying() => AnimationPlayer.CurrentAnimation == Name && AnimationPlayer.IsPlaying();
    public void Queue() => AnimationPlayer.Queue(StringName);

}