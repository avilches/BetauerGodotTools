using System;
using System.Collections.Generic;

namespace Veronenger.Game.Tools.Events {
    public interface EventListener<T> {
        void OnEvent(T @event);
    }

    public abstract class ConditionalEventListener<T> : EventListener<T> {
        public Type GetEventType() {
            return typeof(T);
        }

        public void OnEvent(T @event) {
            if (CanBeExecuted(@event)) Execute(@event);
        }

        public abstract bool CanBeExecuted(T @event);
        public abstract void Execute(T @event);
    }

}