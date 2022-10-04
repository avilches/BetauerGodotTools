using System;
using System.Collections.Generic;
using Betauer.Signal;
using Object = Godot.Object;

namespace Betauer.Bus.Signal {
    public abstract class SignalMulticast<TPublisher, TArgs, TFilter>
        where TPublisher : Object
        where TFilter : Object {
        public readonly string? Name;
        public readonly List<EventConsumer> Consumers = new();

        protected SignalMulticast(string? name = null) {
            Name = name;
        }

        public abstract SignalHandler Connect(TPublisher publisher);

        protected abstract bool Matches(TArgs signalArgs, TFilter detect);

        public void Publish(TPublisher publisher, TArgs signalArgs) {
            Consumers.RemoveAll((consumer) => {
                if (!consumer.IsValid()) return true; // Deleting consumer
                if (consumer.Filter == null || Matches(signalArgs, consumer.Filter)) {
                    consumer.Execute(publisher, signalArgs);
                }
                return false;
            });
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
            public TFilter? Filter { get; private set; }

            public EventConsumer WithFilter(TFilter filter) {
                Filter = filter;
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