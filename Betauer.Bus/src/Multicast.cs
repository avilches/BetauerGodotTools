using System ;
using System.Collections.Generic;

namespace Betauer.Bus; 

public class Multicast<TEvent> {
    public readonly List<EventConsumer<TEvent>> Consumers = new();

    public void Publish(TEvent @event) {
        Consumers.RemoveAll(consumer => {
            if (!consumer.IsValid()) return true; // Deleting consumer
            consumer.Execute(@event);
            return false;
        });
    }

    public EventConsumer<TEvent> Subscribe(Action<TEvent> action) {
        var consumer = new EventConsumer<TEvent>();
        consumer.Do(action);
        Consumers.Add(consumer);
        return consumer;
    }

    // This allows to subscribe with a subtype of TEvent, which is valid but it will fail in the method above
    public EventConsumer<TEvent> Subscribe<T>(Action<T> action) where T : TEvent {
        var consumer = new EventConsumer<TEvent>();
        consumer.Do(e => {
            if (e is null or T) action((T)e);
        });
        Consumers.Add(consumer);
        return consumer;
    }

    public void Dispose() {
        Consumers.Clear();
    }
}