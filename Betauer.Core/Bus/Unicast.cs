using System ;
using System.Collections.Generic;
using Object = Godot.Object;

namespace Betauer.Bus {
    public class Unicast<TArgs> : Unicast<object, TArgs> {
    }

    public class Unicast<TPublisher, TArgs> {
        public readonly EventConsumer Consumer = new();

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TArgs args) {
            Publish(default, args);
        }

        public void Publish(TPublisher origin, TArgs args) {
            if (Consumer.IsValid()) {
                Consumer.Execute(origin, args);
            }
        }

        public EventConsumer Subscribe(Action action) {
            return Subscribe((_, _) => action());
        }

        public EventConsumer Subscribe(Action<TArgs> action) {
            return Subscribe((_, args) => action(args));
        }

        public EventConsumer Subscribe(Action<TPublisher, TArgs> action) {
            Consumer.Remove();
            Consumer.Do(action);
            return Consumer;
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
        }
    }
}