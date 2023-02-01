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
            return Subscribe<TPublisher, TArgs>((_, _) => action());
        }

        public EventConsumer Subscribe(Action<TArgs> action) {
            return Subscribe((_, args) => action(args));
        }

        public EventConsumer Subscribe(Action<TPublisher, TArgs> action) {
            return Subscribe<TPublisher, TArgs>(action);
        }


        public EventConsumer Subscribe<T>(Action<T> action) where T : TArgs {
            return Subscribe<TPublisher, T>((_, args) => action(args));
        }

        public EventConsumer Subscribe<TP, T>(Action<TP, T> action) where T : TArgs where TP : TPublisher {
            Consumer.Unsubscribe();
            if (typeof(T) == typeof(TArgs) && typeof(TP) == typeof(TPublisher)) {
                Consumer.Do(action as Action<TPublisher, TArgs>);
            } else {
                // This allow to subscribe with subtypes of T
                Consumer.Do((publisher, args) => {
                    if (publisher is null or TP &&
                        args is null or T) {
                        action((TP)publisher, (T)args);                        
                    }
                });
            }
            return Consumer;
        }

        public void Dispose() {
            Consumer.Unsubscribe();
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
        }
    }
}