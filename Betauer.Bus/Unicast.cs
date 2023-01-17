using System ;

namespace Betauer.Bus {
    public class Unicast<TArgs> : Unicast<object, TArgs> {
    }

    public class Unicast<TPublisher, TArgs> {
        public readonly EventConsumer Consumer = new();

        public Func<TPublisher, TArgs, bool>? Condition { get; set; }

        public void Publish() {
            Publish(default, default);
        }

        public void Publish(TArgs args) {
            Publish(default, args);
        }

        public void Publish(TPublisher publisher, TArgs args) {
            if (Consumer.IsValid() &&
                (Condition == null || Condition(publisher, args))) {
                Execute(Consumer, publisher, args);
            }
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
            Consumer.Remove();
            Consumer.Do(action);
            return Consumer;
        }

        public void Dispose() {
            Consumer.Remove();
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
        }
    }
}