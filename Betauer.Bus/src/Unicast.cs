using System ;
using Betauer.Core;

namespace Betauer.Bus; 

public class Unicast<TEvent> {
    public readonly EventConsumer<TEvent> Consumer = new();

    public void Execute(TEvent @event) {
        if (Consumer.IsValid()) {
            Consumer.Execute(@event);
        }
    }

    public EventConsumer<TEvent> Subscribe(Action<TEvent> action) {
        Consumer.Unsubscribe();
        Consumer.Do(action);
        return Consumer;
    }

    // This allows to subscribe with a subtype of TEvent only, which is valid but it will fail in the method above
    public EventConsumer<TEvent> Subscribe<T>(Action<T> action) where T : TEvent {
        Consumer.Unsubscribe();
        Consumer.Do(e => {
            if (e is null or T) action((T)e);
        });
        return Consumer;
    }

    public void Dispose() {
        Consumer.Unsubscribe();
    }
}