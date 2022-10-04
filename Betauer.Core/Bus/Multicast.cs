using System ;
using System.Collections.Generic;

namespace Betauer.Bus {
    public class Multicast<TArgs> : Multicast<object, TArgs> {
    }

    public class Multicast<TPublisher, TArgs> {
        public readonly List<EventConsumer> Consumers = new();

        public Func<TPublisher, TArgs, bool>? Condition;

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TArgs args) {
            Publish(default, args);
        }

        public void Publish(TPublisher publisher, TArgs args) {
            Consumers.RemoveAll(consumer => {
                if (!consumer.IsValid()) return true; // Deleting consumer
                if (Condition == null || Condition(publisher, args)) Execute(consumer, publisher, args);
                return false;
            });
        }

        protected virtual void Execute(EventConsumer consumer, TPublisher publisher, TArgs args) {
            consumer.Execute(publisher, args);
        }

        public EventConsumer Subscribe(Action action) {
            return Subscribe((_, _) => action());
        }

        public EventConsumer Subscribe(Action<TArgs> action) {
            return Subscribe((_, args) => action(args));
        }

        public EventConsumer Subscribe(Action<TPublisher, TArgs> action) {
            var consumer = new EventConsumer().Do(action);
            Consumers.Add(consumer);
            return consumer;
        }

        public void Dispose() {
            Consumers.Clear();
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
        }
    }
}
