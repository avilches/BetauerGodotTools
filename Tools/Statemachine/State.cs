using Godot;

namespace Betauer.Tools.Statemachine {
    public abstract class State {

        public abstract void Execute();

        public virtual void _UnhandledInput(InputEvent @event) {
        }

        public virtual void Start() {
        }

        public virtual void End() {
        }

    }
}
