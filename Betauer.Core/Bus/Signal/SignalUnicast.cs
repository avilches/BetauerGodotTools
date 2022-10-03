using System;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalUnicast<TPublisher, TSignalArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public Action<TPublisher, TSignalArgs>? EventHandler;

        protected SignalUnicast(string name) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TSignalArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TSignalArgs signalArgs) {
            EventHandler?.Invoke(publisher, signalArgs);
        }

        public void RemoveEventFilter() => EventHandler = null;

        public void OnEventFilter(TFilter filter, Action<TPublisher> action) {
            OnEventFilter(filter, (publisher, signalArgs) => action(publisher));
        }

        public void OnEventFilter(TFilter filter, Action<TPublisher, TSignalArgs> action) {
            EventHandler = (publisher, signalArgs) => {
                if (Matches(signalArgs, filter)) action(publisher, signalArgs);
            };
        }

        public void OnEvent(Action<TPublisher, TSignalArgs> action) {
            EventHandler = action;
        }

        public void OnEvent(Action<TSignalArgs> action) {
            EventHandler = (_, args) => action(args);
        }

    }
}