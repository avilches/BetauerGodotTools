using System;
using System.Collections.Generic;

namespace Betauer.Bus {
    public class Multicast<TArgs> : Multicast<object, TArgs> {
    }

    public class Multicast<TPublisher, TArgs> {
        public readonly List<EventHandler> EventHandlers = new();

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TArgs args) {
            Publish(default, args);
        }

        public void Publish(TPublisher origin, TArgs args) {
            EventHandlers.RemoveAll(handler => {
                if (!handler.IsValid()) return true; // Deleting handler
                handler.Action(origin, args);
                return false;
            });
        }

        public EventHandler Subscribe(Action action) {
            return Subscribe((_, _) => action());
        }

        public EventHandler Subscribe(Action<TArgs> action) {
            return Subscribe((_, args) => action(args));
        }

        public EventHandler Subscribe(Action<TPublisher, TArgs> action) {
            var eventHandler = new EventHandler(action);
            AddEventHandler(eventHandler);
            return eventHandler;
        }

        public void AddEventHandler(EventHandler eventHandler) {
            EventHandlers.Add(eventHandler);
        }

        public class EventHandler {
            public readonly Action<TPublisher, TArgs> Action;
            public Godot.Object? Watch { get; private set; }
            private bool _removed = false;

            internal EventHandler(Action<TPublisher, TArgs> action) {
                Action = action;
            }

            internal EventHandler RemoveIfInvalid(Godot.Object? owner) {
                Watch = owner;
                return this;
            }

            internal void Remove() {
                _removed = true;
            }

            public bool IsValid() {
                return !_removed && 
                       (Watch == null || Godot.Object.IsInstanceValid(Watch));
            }
        }
    }
}