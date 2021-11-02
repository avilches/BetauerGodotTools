using Godot;
using Godot.Collections;

namespace Game.Tools.Statemachine {
    public abstract class State {

        public abstract void Execute();

        public virtual void _UnhandledInput(InputEvent @event) {
        }

        public virtual void Configure(System.Collections.Generic.Dictionary<string, object> config) {
        }

        public virtual void Start() {
        }

        public virtual void End() {
        }

    }
}
