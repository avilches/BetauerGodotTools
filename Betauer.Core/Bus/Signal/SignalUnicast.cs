using System;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalUnicast<TPublisher, TSignalArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly EventHandler Handler = new();

        protected SignalUnicast(string? name = null) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TSignalArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TSignalArgs signalArgs) {
            if (Handler.Action == null) return;
            
            if (!Handler.IsValid()) {
                Handler.Remove();
                return;
            }
            if (Handler.Action != null && 
                (Handler.Filter == null || Matches(signalArgs, Handler.Filter))) {
                Handler.Action(publisher, signalArgs);
            }
        }

        public EventHandler OnEvent(Action<TPublisher, TSignalArgs> action) {
            Handler.Action = action;
            return Handler;
        }

        public class EventHandler {
            public Action<TPublisher, TSignalArgs>? Action { get; internal set; }
            public Object? Watch { get; private set; }
            public TFilter? Filter { get; private set; }

            public EventHandler RemoveIfInvalid(Object? owner) {
                Watch = owner;
                return this;
            }

            public EventHandler WithFilter(TFilter? detect) {
                Filter = detect;
                return this;
            }

            public bool IsValid() {
                return (Watch == null || Object.IsInstanceValid(Watch)) && 
                       (Filter == null || Object.IsInstanceValid(Filter));
            }

            public void Remove() {
                Action = null;
                Watch = null;
                Filter = null;
            }
        }

    }
}