using static Godot.Mathf;

namespace Tools {
    public class GodotConstants {
        public const float CLOCK_THREE = Pi / 2;
        public const float CLOCK_NINE = -Pi / 2;
        public const float CLOCK_SIX = 0;
        public const float CLOCK_NOON = Pi;

        public const string GODOT_SIGNAL_body_entered = "body_entered";
        public const string GODOT_SIGNAL_body_exited = "body_exited";

        public const string GODOT_SIGNAL_area_entered = "area_entered";
        public const string GODOT_SIGNAL_area_exited = "area_exited";
        public const string GODOT_SIGNAL_area_shape_entered = "area_shape_entered";
        public const string GODOT_SIGNAL_area_shape_exited = "area_shape_exited";

        public const string GODOT_SIGNAL_screen_resized = "screen_resized";
        public const string GODOT_SIGNAL_animation_finished = "animation_finished";
    }
}