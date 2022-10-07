using Godot;

namespace Betauer.StateMachine {
    public abstract class StateMachineNode : Node {
        public ProcessMode Mode { get; set; }
    }
}