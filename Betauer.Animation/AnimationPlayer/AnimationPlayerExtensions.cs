namespace Betauer.Animation.AnimationPlayer;

public static class AnimationPlayerExtensions {
    public static Anim Anim(this Godot.AnimationPlayer player, string name) => new Anim(player, name);
}