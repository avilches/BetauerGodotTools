using System.Collections.Generic;
using Godot;

namespace Betauer.Bus.Signal {
    public abstract class SignalCollection<TPublisher, TArgs, TObject>
        where TPublisher : Object
        where TObject : Object {

        public readonly string? Name;

        protected SignalCollection(string? name = null) {
            Name = name;
        }

        protected readonly HashSet<TObject> Set = new();

        public bool Contains(TObject o) {
            lock (Set) {
                return Set.Contains(o);
            }
        }

        public int Size() {
            lock (Set) {
                return Set.Count;
            }
        }

        protected abstract TObject Extract(TArgs signalArgs);

        protected void OnEnter(TPublisher publisher, TArgs signalArgs) {
            lock (Set) {
                Set.RemoveWhere((e) => !Object.IsInstanceValid(e));
                Set.Add(Extract(signalArgs));
            }
        }

        protected void OnExit(TPublisher publisher, TArgs signalArgs) {
            var toRemove = Extract(signalArgs);
            lock (Set) Set.RemoveWhere((e) => e == toRemove || !Object.IsInstanceValid(e));
        }
    }
}