using System.Collections.Generic;
using System.Dynamic;
using Godot;

namespace Tools.Statemachine {
    public abstract class State {
        public abstract NextState Execute(NextState nextState);

        public virtual void _UnhandledInput(InputEvent @event) {
        }

        public virtual void Start(StateConfig config) {
        }

        public virtual void End() {
        }

        public string Name => GetType().Name;
    }

    public class StateConfig {
        private HashSet<string> _flags = new HashSet<string>();

        public bool HasFlag(string flag) {
            return _flags.Contains(flag);
        }

        public StateConfig AddFlag(string flag) => Flag(flag, true);
        public StateConfig RemoveFlag(string flag) => Flag(flag, false);

        public StateConfig Flag(string flag, bool value) {
            if (value) {
                _flags.Add(flag);
            } else {
                _flags.Remove(flag);
            }
            return this;
        }
    }
}