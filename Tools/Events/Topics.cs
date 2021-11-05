using System.Collections.Generic;

namespace Tools.Events {

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
            Listener?.OnEvent(Name, @event);
        }
    }

    public class MulticastTopic<E, T> : ITopic<E, T> where E : EventListener<T> {
        protected readonly List<E> _eventListeners;

        public string Name { get; }

        protected MulticastTopic(string name) {
            Name = name;
            _eventListeners = new List<E>();
        }

        public virtual void Subscribe(E eventListener) => _eventListeners.Add(eventListener);

        public virtual void Publish(T @event) {
            _eventListeners.ForEach(listener => listener.OnEvent(Name, @event));
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
            GetTopic<E, T>(key)?.Subscribe(listener);
        }

        public void Publish<E, T>(object key, T evt) where E : EventListener<T> {
            GetTopic<E, T>(key)?.Publish(evt);
        }
    }
}