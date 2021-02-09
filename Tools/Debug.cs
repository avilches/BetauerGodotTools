using Godot;

namespace Betauer.Tools {
    public class Debug {
        public const bool DEBUG_STAGE = true;
        public const bool DEBUG_REGISTER = true;

        public static void Register(string type, Node node) {
            if (!DEBUG_REGISTER) return;
            GD.Print("+"+ type+": \""+node.Name+ "\" ("+node.GetType()+") 0x"+node.GetHashCode().ToString("X"));
        }
    }
}