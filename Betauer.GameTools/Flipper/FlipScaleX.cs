using Godot;

namespace Betauer.Flipper;

public class FlipScaleX : Flipper {
    private readonly Node2D _node2D;

    private static readonly Vector2 FlipX = new(-1, 1);

    public FlipScaleX(Node2D node2D) {
        _node2D = node2D;
    }

    public override bool LoadIsFacingRight() {
        return (int)_node2D.Scale.X == 1;
    }

    public override void SetFacingRight(bool right) {
        SetFacingRight(_node2D, right);
    }

    public static void SetFacingRight(Node2D node2D, bool right) {
        node2D.Scale = right ? Vector2.One : FlipX;
    }
}