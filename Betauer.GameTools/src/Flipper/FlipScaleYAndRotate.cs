using Godot;

namespace Betauer.Flipper;

public class FlipScaleYAndRotate : Flipper {
    private readonly Node2D _node2D;

    private static readonly Vector2 FlipY = new(1, -1);

    public FlipScaleYAndRotate(Node2D node2D) {
        _node2D = node2D;
    }

    public override bool LoadIsFacingRight() {
        return (int)_node2D.Scale.Y == 1 && (_node2D.Transform.Rotation == 0);
    }

    public override void SetFacingRight(bool right) {
        if (right) {
            // Return to normal position
            _node2D.Scale = Vector2.One; // 1,1
            _node2D.Rotation = 0;
        } else {
            // Flip to the left = flip Y axis + rotate 180 degrees
            _node2D.Scale = FlipY;
            _node2D.Rotate(Mathf.Pi);
        }
    }
}