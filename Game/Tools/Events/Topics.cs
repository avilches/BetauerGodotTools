using System;
using System.Collections.Generic;

namespace Veronenger.Game.Tools.Events {

    public interface ITopic<E, T> where E : EventListener<T> {
        void Subscribe(E eventListener);

        void Publish(T @event);
    }

    public class UnicastTopic<E, T> : ITopic<E, T> where E : EventListener<T> {
        protected E _eventListener;

        public UnicastTopic() {
        }

        public virtual void Subscribe(E eventListener) => _eventListener = eventListener;

        public virtual void Publish(T @event) {
            _eventListener?.OnEvent(@event);
        }
    }

    public class MulticastTopic<E, T> : ITopic<E, T> where E : EventListener<T> {
        protected readonly List<E> _eventListeners;

        public MulticastTopic() {
            _eventListeners = new List<E>();
        }

        public virtual void Subscribe(E eventListener) => _eventListeners.Add(eventListener);

        public virtual void Publish(T @event) {
            _eventListeners.ForEach(listener => listener.OnEvent(@event));
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

        public void Subscribe<E, T>(object key, E listener) where E : EventListener<T> {
            GetTopic<E, T>(key)?.Subscribe(listener);
        }

        public void Emit<E, T>(object key, T evt) where E : EventListener<T> {
            GetTopic<E, T>(key)?.Publish(evt);
        }
    }
}