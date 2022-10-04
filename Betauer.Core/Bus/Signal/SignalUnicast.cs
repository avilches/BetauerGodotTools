using System;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalUnicast<TPublisher, TArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly EventConsumer Consumer = new();

        protected SignalUnicast(string? name = null) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TArgs signalArgs) {
            if ((Consumer.IsValid()) && 
                (Consumer.Filter == null || Matches(signalArgs, Consumer.Filter))) {
                Consumer.Execute(publisher, signalArgs);
            }
        }

        public EventConsumer OnEvent(Action<TPublisher, TArgs> action) {
            Consumer.Remove();
            Consumer.Do(action);
            return Consumer;
        }

        public class EventConsumer : BaseEventConsumer<EventConsumer, TPublisher, TArgs> {
            public TFilter? Filter { get; private set; }

            public EventConsumer WithFilter(TFilter? detect) {
                Filter = detect;
                return this;
            }

            public override bool IsValid() {
                return base.IsValid() && (Filter == null || Object.IsInstanceValid(Filter));
            }

            public override void Remove() {
                Filter = null;
            }
        }

    }
}