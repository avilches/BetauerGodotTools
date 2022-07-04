using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Memory {
    public interface IObjectLifeCycle : IDisposable {
        public bool MustBeDisposed();
    }

    public class ObjectLifeCycleManagerNode : Node {
        public override void _Process(float delta) {
            ObjectLifeCycleManager.Singleton.ProcessWatching();
            ObjectLifeCycleManager.Singleton.ProcessQueue();
        }
    }

    public class ObjectLifeCycleManager {
        public static readonly ObjectLifeCycleManager Singleton = new ObjectLifeCycleManager();
        
        private readonly LinkedList<IObjectLifeCycle> _watching = new LinkedList<IObjectLifeCycle>();
        private LinkedList<IDisposable> _queue = new LinkedList<IDisposable>();
        private LinkedList<IDisposable> _queueB = new LinkedList<IDisposable>();

        public int ProcessQueue() {
            lock (this) {
                if (_queue.Count == 0 && _queueB.Count == 0) return 0;
                var disposedCount = _queueB.Count;
                if (disposedCount > 0) {
                    foreach (var disposable in _queueB) disposable.Dispose();
                    _queueB.Clear();
                }
                // Swap queues
                (_queueB, _queue) = (_queue, _queueB);
                return disposedCount;
            }
        }

        public void Watch(IObjectLifeCycle o) {
            lock (_watching) _watching.AddLast(o);
        }

        public void Unwatch(IObjectLifeCycle o) {
            lock (_watching) _watching.Remove(o);
        }

        public void QueueDispose(IDisposable disposable) {
            lock (this) _queue.AddLast(disposable);
        }

        public void QueueDispose(IEnumerable<IDisposable> disposables) {
            lock (this)
                foreach (var disposable in disposables)
                    _queue.AddLast(disposable);
        }

        public List<IDisposable> GetQueue() {
            lock (_queue) return new List<IDisposable>(_queue);
        }

        public List<IObjectLifeCycle> GetWatching() {
            lock (_watching) return new List<IObjectLifeCycle>(_watching);
        }

        public int ProcessWatching() {
            LinkedList<IDisposable> toEnqueue = null;
            lock (_watching) {
                var node = _watching.First;
                while (node != null) {
                    var next = node.Next;
                    var o = node.Value;
                    if (o.MustBeDisposed()) {
                        _watching.Remove(node);
                        toEnqueue ??= new LinkedList<IDisposable>();
                        toEnqueue.AddLast(o);
                    }
                    node = next;
                }
            }
            if (toEnqueue == null) return 0;
            QueueDispose(toEnqueue);
            return toEnqueue.Count;
        }

        public int DisposeAllWatching() {
            LinkedList<IDisposable> toEnqueue = null;
            lock (_watching) {
                if (_watching.Count > 0) {
                    toEnqueue = new LinkedList<IDisposable>(_watching);
                    _watching.Clear();
                }
            }
            if (toEnqueue == null) return 0;
            QueueDispose(toEnqueue);
            return toEnqueue.Count;
        }

        public void Reset() {
            lock (_watching) _watching.Clear();
            lock (this) {
                _queue.Clear();
                _queueB.Clear();
            }
        }
    }
}