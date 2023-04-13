using Godot;

namespace Betauer.Flipper;

public class Sprite2DFlipH : Flipper {
    private readonly Sprite2D _sprite2D;

    public Sprite2DFlipH(Sprite2D sprite2D) {
        _sprite2D = sprite2D;
    }

    public override bool LoadIsFacingRight() {
        return IsSprite2DFacingRight(_sprite2D);
    }

    public static bool IsSprite2DFacingRight(Sprite2D node) {
        return !node.FlipH;
    }

    public override void SetFacingRight(bool right) {
        _sprite2D.FlipH = !right;
    }
}