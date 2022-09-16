using Godot;

namespace Betauer.Signal.Bus {
    public abstract class SignalStatus<TEmitter, TSignalParams, TObject>
        where TEmitter : Object
        where TObject : Object {

        public readonly string? Name;

        protected SignalStatus(string? name = null) {
            Name = name;
        }
        public TObject? Filter { get; private set; }
        
        public bool Status { get; private set; }

        public void WithFilter(TObject filter) {
            Filter = filter;
        }

        protected abstract TObject Extract(TSignalParams signalParams);

        protected void OnEnter(TEmitter origin, TSignalParams signalParams) {
            if (Filter == null || Extract(signalParams) == origin) Status = true;
        }

        protected void OnExit(TEmitter origin, TSignalParams signalParams) {
            if (Filter == null || Extract(signalParams) == origin) Status = false;
        }
    }
}