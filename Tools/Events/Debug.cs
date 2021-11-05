using Godot;

namespace Tools.Events {
    public class Debug {
        public static bool TESTING = false;

        private const bool _DEBUG_STAGE = false;
        private const bool _DEBUG_REGISTER = false;
        private const bool _DEBUG_RESOLUTION = false;
        private const bool _DEBUG_EVENT_PUBLISH = true;
        private const bool _DEBUG_EVENT_LISTENER = true;

        public static void Register(string type, Node node) {
            if (!TESTING && !_DEBUG_REGISTER) return;
            GD.Print($"+{type}: \"{node.Name}\" ({node.GetType()}) 0x{node.GetHashCode():X}");
        }
        public static void Stage(string message) {
            if (!TESTING && !_DEBUG_STAGE) return;
            GD.Print($"[Stage] {message}");
        }

        public static void Topic(string topicName, string message) {
            if (!TESTING && !_DEBUG_EVENT_PUBLISH) return;
            GD.Print($"[Topic:{topicName}] {message}");
        }
        public static void Event(string topicName, string listenerName, string message) {
            if (!TESTING && !_DEBUG_EVENT_LISTENER) return;
            GD.Print($"[Topic:{topicName}][Listener:{listenerName}] {message}");
        }
    }
}