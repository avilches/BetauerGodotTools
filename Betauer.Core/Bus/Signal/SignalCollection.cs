using System.Collections.Generic;
using Godot;

namespace Betauer.Bus.Signal {
    public abstract class SignalCollection<TEmitter, TSignalParams, TObject>
        where TEmitter : Object
        where TObject : Object {

        public readonly string? Name;

        protected SignalCollection(string? name = null) {
            Name = name;
        }

        protected readonly HashSet<TObject> Set = new HashSet<TObject>();

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

        protected abstract TObject Extract(TSignalParams signalParams);

        protected void OnEnter(TEmitter origin, TSignalParams signalParams) {
            lock (Set) {
                Set.RemoveWhere((e) => !Object.IsInstanceValid(e));
                Set.Add(Extract(signalParams));
            }
        }

        protected void OnExit(TEmitter origin, TSignalParams signalParams) {
            var toRemove = Extract(signalParams);
            lock (Set) Set.RemoveWhere((e) => e == toRemove || !Object.IsInstanceValid(e));
        }
    }
}