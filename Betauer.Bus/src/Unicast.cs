using System ;
using Betauer.Core;

namespace Betauer.Bus {
    public class Unicast<TEvent> : Unicast<object, TEvent> {
    }

    public class Unicast<TPublisher, TEvent> {
        public readonly EventConsumer Consumer = new();

        public Func<TPublisher, TEvent, bool>? Condition { get; set; }

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TEvent args) {
            Publish(default, args);
        }

        public void Publish(TPublisher publisher, TEvent args) {
            if (Consumer.IsValid() &&
                (Condition == null || Condition(publisher, args))) {
                Execute(Consumer, publisher, args);
            }
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
            Consumer.Unsubscribe();
            Consumer.Do(ActionTools.Convert<TPublisher, TEvent, TP, T>(action));
            return Consumer;
        }

        public void Dispose() {
            Consumer.Unsubscribe();
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TEvent> {
        }
    }
}