using System;
using System.Collections.Generic;

namespace Betauer.Memory {
    /// <summary>
    /// A ConditionalConsumer is a thread safe list. Calling to Consume() extracts all the elements where the predicate
    /// is true and calls to the consumer action with all of them. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionalConsumer<T> {
        private readonly LinkedList<T> _queue = new LinkedList<T>();
        private readonly Func<T, bool> _predicate;
        private readonly Action<IEnumerable<T>> _consumerAction;

        public ConditionalConsumer(Func<T, bool> predicate, Action<IEnumerable<T>> consumerAction) {
            _predicate = predicate;
            _consumerAction = consumerAction;
        }

        public int Size => _queue.Count;

        public void Add(T e) {
            lock (_queue) _queue.AddLast(e);
        }

        public void Add(IEnumerable<T> elements) {
            lock (_queue) foreach (var e in elements) _queue.AddLast(e);
        }

        public void Remove(T e) {
            lock (_queue) _queue.Remove(e);
        }

        public void Remove(IEnumerable<T> elements) {
            lock (_queue) foreach (var e in elements) _queue.Remove(e);
        }

        public List<T> ToList() {
            lock (_queue) return new List<T>(_queue);
        }

        public int Consume() {
            LinkedList<T> toEnqueue = null;
            lock (_queue) {
                var node = _queue.First;
                while (node != null) {
                    var next = node.Next;
                    var o = node.Value;
                    if (_predicate(o)) {
                        _queue.Remove(node);
                        toEnqueue ??= new LinkedList<T>();
                        toEnqueue.AddLast(o);
                    }
                    node = next;
                }
            }
            if (toEnqueue == null) return 0;
            _consumerAction.Invoke(toEnqueue);
            return toEnqueue.Count;
        }

        public int ForceConsumeAll() {
            LinkedList<T> toEnqueue = null;
            lock (_queue) {
                if (_queue.Count > 0) {
                    toEnqueue = new LinkedList<T>(_queue);
                    _queue.Clear();
                }
            }
            if (toEnqueue == null) return 0;
            _consumerAction.Invoke(toEnqueue);
            return toEnqueue.Count;
        }

        public void Reset() {
            lock (_queue) _queue.Clear();
        }
    }
}