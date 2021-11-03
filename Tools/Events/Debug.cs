using Godot;

namespace Tools.Events {
    public class Debug {
        public const bool DEBUG_STAGE = true;
        public const bool DEBUG_REGISTER = false;
        public const bool DEBUG_RESOLUTION = false;
        public const bool DEBUG_EVENT_LISTENER = false;

        public static void Register(string type, Node node) {
            if (!DEBUG_REGISTER) return;
            GD.Print("+"+ type+": \""+node.Name+ "\" ("+node.GetType()+") 0x"+node.GetHashCode().ToString("X"));
        }
        public static void Stage(string message) {
            if (!DEBUG_STAGE) return;
            GD.Print("[Stage] " + message);
        }
    }
}