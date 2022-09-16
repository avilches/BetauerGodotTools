using System;
using System.Collections.Generic;
using Object = Godot.Object;

namespace Betauer.Signal.Bus {
    public abstract class Multicast<TEmitter, TSignalParams, TFilter>
        where TEmitter : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly List<EventHandler> EventHandlers = new List<EventHandler>();

        protected Multicast(string name) {
            Name = name;
        }

        public abstract SignalHandler Connect(TEmitter area2D);

        public void Emit(TEmitter origin, TSignalParams signalParams) {
            EventHandlers.RemoveAll((handler) => {
                if (!handler.IsValid()) return true; // Deleting handler
                if (handler.Filter == null || Matches(signalParams, handler.Filter)) {
                    handler.Action(origin, signalParams);
                }
                return false;
            });
        }

        public EventHandler OnEventFilter(TFilter filter, Action<TEmitter> action) {
            return OnEventFilter(filter, (origin, _) => action(origin));
        }

        public EventHandler OnEventFilter(TFilter filter, Action<TEmitter, TSignalParams> action) {
            var eventHandler = new EventHandler(action).WithFilter(filter);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public EventHandler OnEvent(Object watch, Action<TEmitter, TSignalParams> action) {
            var eventHandler = new EventHandler(action).RemoveIfInvalid(watch);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public EventHandler OnEvent(Action<TEmitter, TSignalParams> action) {
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

        protected abstract bool Matches(TSignalParams e, TFilter detect);

        public class EventHandler {
            public readonly Action<TEmitter, TSignalParams> Action;
            public Object? Watch { get; private set; }
            public TFilter? Filter { get; private set; }

            internal EventHandler(Action<TEmitter, TSignalParams> action) {
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