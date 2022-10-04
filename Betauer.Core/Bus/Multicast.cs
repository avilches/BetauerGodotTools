using System ;
using System.Collections.Generic;
using Object = Godot.Object;

namespace Betauer.Bus {
    public class Multicast<TArgs> : Multicast<object, TArgs> {
    }

    public class Multicast<TPublisher, TArgs> {
        public readonly List<EventConsumer> Consumers = new();

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TArgs args) {
            Publish(default, args);
        }

        public void Publish(TPublisher origin, TArgs args) {
            Consumers.RemoveAll(consumer => {
                if (!consumer.IsValid()) return true; // Deleting consumer
                consumer.Execute(origin, args);
                return false;
            });
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

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
        }
    }
}