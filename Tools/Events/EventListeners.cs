using System;

namespace Tools.Events {
    public interface EventListener<T> {
        string Name { get; }
        void OnEvent(T @event);
    }
}