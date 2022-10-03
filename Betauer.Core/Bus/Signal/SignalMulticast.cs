using System;
using System.Collections.Generic;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalMulticast<TPublisher, TSignalArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly List<EventHandler> Handlers = new();

        protected SignalMulticast(string? name = null) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TSignalArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TSignalArgs signalArgs) {
            Handlers.RemoveAll((handler) => {
                if (!handler.IsValid()) return true; // Deleting handler
                if (handler.Filter == null || Matches(signalArgs, handler.Filter)) {
                    handler.Action(publisher, signalArgs);
                }
                return false;
            });
        }

        public EventHandler OnEvent(Action<TPublisher, TSignalArgs> action) {
            var eventHandler = new EventHandler(action);
            Handlers.Add(eventHandler);
            return eventHandler;
        }

        public void Dispose() {
            Handlers.Clear();
        }

        public class EventHandler {
            public readonly Action<TPublisher, TSignalArgs> Action;
            public Object? Watch { get; private set; }
            public TFilter? Filter { get; private set; }
            private bool _removed = false;

            internal EventHandler(Action<TPublisher, TSignalArgs> action) {
                Action = action;
            }

            internal EventHandler WithFilter(TFilter? detect) {
                Filter = detect;
                return this;
            }

            internal EventHandler RemoveIfInvalid(Object? owner) {
                Watch = owner;
                return this;
            }

            internal void Remove() {
                _removed = true;
            }

            public bool IsValid() {
                return !_removed &&
                       (Watch == null || Object.IsInstanceValid(Watch)) && 
                       (Filter == null || Object.IsInstanceValid(Filter));
            }
        }
    }
}