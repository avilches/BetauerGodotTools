using Godot;
using Godot.Collections;

namespace Betauer.Tools {
    public delegate void BodyOnArea2DSignalMethod(Node body, Area2D area2D);

    public class GodotTools {
        public static void ConnectBodyWithArea2D(Area2D area2D, BodyOnArea2DSignalMethod enter, BodyOnArea2DSignalMethod exit = null) {
            area2D.Connect(GodotConstants.GODOT_SIGNAL_body_entered, (Godot.Object)enter.Target, enter.Method.Name, new Array {area2D});
            if (exit != null) {
                area2D.Connect(GodotConstants.GODOT_SIGNAL_body_exited, (Godot.Object)exit.Target, exit.Method.Name, new Array {area2D});
            }
        }


    }
}