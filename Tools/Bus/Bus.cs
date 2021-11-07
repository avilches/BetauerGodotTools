using System.Collections.Generic;

namespace Tools.Bus {
    public interface EventListener<T> {
        string Name { get; }
        void OnEvent(T @event);
    }

    public interface ITopic<E, T> where E : EventListener<T> {
        string Name { get; }

        void Subscribe(E eventListener);

        void Publish(T @event);
    }

    public class UnicastTopic<E, T> : ITopic<E, T> where E : EventListener<T> {
        public E Listener { get; protected set; }

        public string Name { get; }

        public UnicastTopic(string name) {
            Name = name;
        }

        public virtual void Subscribe(E eventListener) => Listener = eventListener;

        public virtual void Publish(T @event) {
            Listener?.OnEvent(@event);
        }
    }

    public class MulticastTopic<E, T> : ITopic<E, T> where E : EventListener<T> {
        public List<E> EventListeners { get; }

        public string Name { get; }

        protected MulticastTopic(string name) {
            Name = name;
            EventListeners = new List<E>();
        }

        public virtual void Subscribe(E eventListener) {
            if (eventListener != null) EventListeners.Add(eventListener);
        }

        public virtual void Publish(T @event) {
            EventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

    public class TopicMap {
        public static TopicMap Instance { get; } = new TopicMap();
        private readonly Dictionary<object, object> _topics = new Dictionary<object, object>();

        private TopicMap() {
        }

        public ITopic<E, T> AddTopic<E, T>(object key, ITopic<E, T> topic) where E : EventListener<T> {
            _topics[key] = topic;
            return topic;
        }

        public ITopic<E, T> GetTopic<E, T>(object key) where E : EventListener<T> {
            return _topics[key] as ITopic<E, T>;
        }

        public ITopic<EventListener<object>, object> GetTopic(object key) {
            return _topics[key] as ITopic<EventListener<object>, object>;
        }

        public void Subscribe<E, T>(object key, E listener) where E : EventListener<T> {
            var topic = GetTopic<E, T>(key);
            topic?.Subscribe(listener);
        }

        public void Publish<E, T>(object key, T evt) where E : EventListener<T> {
            var topic = GetTopic<E, T>(key);
            topic?.Publish(evt);
        }
    }
}