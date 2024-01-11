using System;
using System.Collections.Generic;

namespace Betauer.Bus;

public abstract class Multicast {
    public abstract void Purge();
    public abstract void Clear();
    public abstract int ConsumerCount { get; }
}

public class Multicast<TEvent> : Multicast {
    public readonly List<EventConsumer<TEvent>> Consumers = new();

    public override int ConsumerCount => Consumers.Count;

    public void Publish(TEvent @event) {
        if (Consumers.Count == 0) return;
        Consumers.RemoveAll(consumer => {
            if (!consumer.IsValid()) return true; // Deleting consumer
            consumer.Execute(@event);
            return false;
        });
    }

    public override void Purge() {
        Consumers.RemoveAll(consumer => !consumer.IsValid());
    }

    public EventConsumer<TEvent> Subscribe(Action<TEvent> action) {
        var consumer = new EventConsumer<TEvent>(action);
        Consumers.Add(consumer);
        return consumer;
    }

    // This allows to subscribe with a subtype of TEvent, which is valid but it will fail in the method above
    public EventConsumer<TEvent> Subscribe<T>(Action<T> action) where T : TEvent {
        var consumer = new EventConsumer<TEvent>(e => {
            if (e is null or T) action((T)e);
        });
        Consumers.Add(consumer);
        return consumer;
    }

    public override void Clear() {
        Consumers.Clear();
    }
}