using Godot;

namespace Betauer.Bus.Signal {
    public abstract class SignalStatus<TPublisher, TSignalArgs, TObject>
        where TPublisher : Object
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

        protected abstract TObject Extract(TSignalArgs signalArgs);

        protected void OnEnter(TPublisher publisher, TSignalArgs signalArgs) {
            if (Filter == null || Extract(signalArgs) == publisher) Status = true;
        }

        protected void OnExit(TPublisher publisher, TSignalArgs signalArgs) {
            if (Filter == null || Extract(signalArgs) == publisher) Status = false;
        }
    }
}