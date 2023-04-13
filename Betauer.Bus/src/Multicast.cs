using System ;
using System.Collections.Generic;
using Betauer.Core;

namespace Betauer.Bus {
    public class Multicast<TEvent> : Multicast<object, TEvent> {
    }

    public class Multicast<TPublisher, TEvent> {
        public readonly List<EventConsumer> Consumers = new();

        public Func<TPublisher, TEvent, bool>? Condition;

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TEvent args) {
            Publish(default, args);
        }

        public void Publish(TPublisher publisher, TEvent args) {
            Consumers.RemoveAll(consumer => {
                if (!consumer.IsValid()) return true; // Deleting consumer
                if (Condition == null || Condition(publisher, args)) Execute(consumer, publisher, args);
                return false;
            });
        }

        protected virtual void Execute(EventConsumer consumer, TPublisher publisher, TEvent args) {
            consumer.Execute(publisher, args);
        }

        public EventConsumer Subscribe(Action action) {
            return Subscribe<TPublisher, TEvent>((_, _) => action());
        }

        public EventConsumer Subscribe(Action<TEvent> action) {
            return Subscribe((_, args) => action(args));
        }

        public EventConsumer Subscribe(Action<TPublisher, TEvent> action) {
            return Subscribe<TPublisher, TEvent>(action);
        }


        public EventConsumer Subscribe<T>(Action<T> action) where T : TEvent {
            return Subscribe<TPublisher, T>((_, args) => action(args));
        }

        public EventConsumer Subscribe<TP, T>(Action<TP, T> action) where T : TEvent where TP : TPublisher {
            var consumer = new EventConsumer().Do(ActionTools.Convert<TPublisher, TEvent, TP, T>(action));
            Consumers.Add(consumer);
            return consumer;
        }

        public void Dispose() {
            Consumers.Clear();
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TEvent> {
        }
    }
}
