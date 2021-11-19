using System;
using System.Collections.Generic;
using Godot;

namespace Tools.Statemachine {
    public abstract class State {
        public abstract NextState Execute(Context context);

        public virtual void _UnhandledInput(InputEvent @event) {
        }

        public virtual void Start(Context context, StateConfig config) {
        }

        public virtual void End() {
        }

        protected Logger Logger;

        public void OnAddedToStateMachine(StateMachine stateMachine) {
            ConfigureLogging(stateMachine);
        }

        public virtual void ConfigureLogging(StateMachine stateMachine) {
        }


        public string Name => GetType().Name;
    }

    public static class StateHelper {
        public static bool HasStartImplemented(State state) {
            var startMethod = ReflectionTools.FindMethod(state, "Start", typeof(void),
                new Type[] { typeof(Context), typeof(StateConfig) });
            return startMethod != null;
        }

        public static bool HasEndImplemented(State state) {
            var endMethod = ReflectionTools.FindMethod(state, "End", typeof(void));
            return endMethod != null;
        }

    }

    public class StateConfig {
        private HashSet<string> _flags = new HashSet<string>();
        public Dictionary<string, object> Dictionary { get; } = new Dictionary<string, object>();

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

        public StateConfig Add(string key, object value) {
            Dictionary.Add(key, value);
            return this;
        }

        public T Get<T>(string key) => (T)Dictionary[key];
        public bool ContainsKey<T>(string key) => Dictionary.ContainsKey(key);
        public bool Remove(string key) => Dictionary.Remove(key);
    }
}