using System;
using System.Collections.Generic;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalMulticast<TPublisher, TSignalArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly List<EventHandler> EventHandlers = new();

        protected SignalMulticast(string name) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TSignalArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TSignalArgs signalArgs) {
            EventHandlers.RemoveAll((handler) => {
                if (!handler.IsValid()) return true; // Deleting handler
                if (handler.Filter == null || Matches(signalArgs, handler.Filter)) {
                    handler.Action(publisher, signalArgs);
                }
                return false;
            });
        }

        public EventHandler OnEventFilter(TFilter filter, Action<TPublisher> action) {
            return OnEventFilter(filter, (publisher, _) => action(publisher));
        }

        public EventHandler OnEventFilter(TFilter filter, Action<TPublisher, TSignalArgs> action) {
            var eventHandler = new EventHandler(action).WithFilter(filter);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public EventHandler OnEvent(Object watch, Action<TPublisher, TSignalArgs> action) {
            var eventHandler = new EventHandler(action).RemoveIfInvalid(watch);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public EventHandler OnEvent(Action<TPublisher, TSignalArgs> action) {
            var eventHandler = new EventHandler(action);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public void AddEventHandler(EventHandler eventHandler) {
            EventHandlers.Add(eventHandler);
        }

        public void RemoveEventHandler(EventHandler eventHandler) {
            EventHandlers.Remove(eventHandler);
        }

        public class EventHandler {
            public readonly Action<TPublisher, TSignalArgs> Action;
            public Object? Watch { get; private set; }
            public TFilter? Filter { get; private set; }

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

            public bool IsValid() {
                return (Watch == null || Object.IsInstanceValid(Watch)) && 
                       (Filter == null || Object.IsInstanceValid(Filter));
            }
        }
    }
}