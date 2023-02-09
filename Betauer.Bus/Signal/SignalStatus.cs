using Godot;

namespace Betauer.Bus.Signal {
    public abstract class SignalStatus<TPublisher, TArgs, TObject>
        where TPublisher : GodotObject
        where TObject : GodotObject {

        public readonly string? Name;

        protected SignalStatus(string? name = null) {
            Name = name;
        }
        public TObject? Filter { get; private set; }
        
        public bool Status { get; private set; }

        public void WithFilter(TObject filter) {
            Filter = filter;
        }

        protected abstract TObject Extract(TArgs signalArgs);

        protected void OnEnter(TPublisher publisher, TArgs signalArgs) {
            if (Filter == null || Extract(signalArgs) == publisher) Status = true;
        }

        protected void OnExit(TPublisher publisher, TArgs signalArgs) {
            if (Filter == null || Extract(signalArgs) == publisher) Status = false;
        }
    }
}