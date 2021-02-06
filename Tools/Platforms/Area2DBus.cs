using Godot;
using Godot.Collections;

namespace Betauer.Tools.Platforms {
    // public delegate void OnBodyEnterArea2D(Node body, Area2D area2D);
    // public delegate void OnMeEnterArea2D(Area2D area2D);

    public class Area2DBus : Node {

        private const string GODOT_SIGNAL_body_shape_entered = "body_shape_entered";
        private const string GODOT_SIGNAL_body_shape_exited = "body_shape_exited";


        void AddArea2DToTopic(Area2D area2D, string topic) {
            area2D.Connect(GODOT_SIGNAL_body_shape_entered, this, nameof(_on_Area2D_platform_exit_body_shape_entered), new Array() {area2D});
        }

        void _on_Area2D_platform_exit_body_shape_entered() {

        }

    }
}