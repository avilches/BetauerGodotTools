using Godot;

namespace Tools.Events {
    public class Debug {
        public static bool TESTING = false;

        private const bool _DEBUG_STAGE = false;
        private const bool _DEBUG_REGISTER = false;
        private const bool _DEBUG_RESOLUTION = false;
        private const bool _DEBUG_EVENT_LISTENER = false;

        public static bool DEBUG_EVENT_LISTENER => _DEBUG_EVENT_LISTENER || TESTING;

        public static void Register(string type, Node node) {
            if (!TESTING && !_DEBUG_REGISTER) return;
            GD.Print("+"+ type+": \""+node.Name+ "\" ("+node.GetType()+") 0x"+node.GetHashCode().ToString("X"));
        }
        public static void Stage(string message) {
            if (!TESTING && !_DEBUG_STAGE) return;
            GD.Print("[Stage] " + message);
        }
    }
}