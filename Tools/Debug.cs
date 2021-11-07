using Godot;

namespace Tools {
    public class Debug {
        public static bool TESTING = false;

        private const bool _DEBUG_STAGE = true;
        public const bool _DEBUG_RESOLUTION = false;
        private const bool _DEBUG_EVENT_PUBLISH = true;
        private const bool _DEBUG_EVENT_LISTENER = true;

        public static void Stage(string message) {
            if (!TESTING && !_DEBUG_STAGE) return;
            GD.Print($"[Stage] {message}");
        }

        public static void Topic(string topicName, string message) {
            if (!TESTING && !_DEBUG_EVENT_PUBLISH) return;
            GD.Print($"[{topicName}] {message}");
        }
        public static void Event(string topicName, string listenerName, string message) {
            if (!TESTING && !_DEBUG_EVENT_LISTENER) return;
            GD.Print($"[{topicName}].{listenerName}] {message}");
        }
    }
}