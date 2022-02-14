using System;
using System.Collections.Generic;
using Betauer.DI;
using Godot;

namespace Betauer.Statemachine {
    public interface IState {
        public string Name { get; }

        public NextState Execute(Context context);

        public void Start(Context context);

        public void End();
    }

    public abstract class BaseState : Di, IState {
        public string Name { get; }

        protected BaseState(string name) {
            Name = name;
        }

        public abstract NextState Execute(Context context);

        public virtual void Start(Context context) {
        }

        public virtual void End() {
        }
    }

    public class State : IState {
        private readonly Action<Context>? _start;
        private readonly Func<Context, NextState> _execute;
        private readonly Action? _end;
        public string Name { get; }


        public State(string name, Func<Context, NextState> execute, Action? end = null) :
            this(name, null, execute, end) {
        }

        public State(string name, Action<Context>? start, Func<Context, NextState> execute, Action? end = null) {
            Name = name;
            _start = start;
            _execute = execute;
            _end = end;
        }

        public NextState Execute(Context context) {
            return _execute(context);
        }

        public void Start(Context context) {
            _start?.Invoke(context);
        }

        public void End() {
            _end?.Invoke();
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