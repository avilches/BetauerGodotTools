using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Memory {
    public interface IObjectLifeCycle : IDisposable {
        public bool MustBeDisposed();
    }

    public class ObjectLifeCycleManagerNode : Node {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ObjectLifeCycleManager));
        public ObjectLifeCycleManager Manager;
        private readonly ulong _frames;
        private ulong _frameCount = 0;

        public ObjectLifeCycleManagerNode(int frames = 600, ObjectLifeCycleManager? manager = null) {
            _frames = (ulong)frames;
            Manager = manager ?? ObjectLifeCycleManager.Singleton;
        }
        
        public override void _Process(float delta) {
            ++_frameCount;
            if (_frameCount < _frames) return;
            _frameCount = 0;
            var moved = Manager.ProcessWatching();
            var processed = Manager.ProcessQueue();
#if DEBUG
            if (moved > 0 || processed > 0) {
                var qSize = Manager.GetQueue().Count;
                var iqSize = Manager.GetDisposablesNextFrame().Count;
                var wSize = Manager.GetWatching().Count;
                Logger.Debug("W:" + wSize + " / Q:" +
                             qSize + " / IQ:" +
                             iqSize + " ");
            }
#endif
        }

        protected override void Dispose(bool disposing) {
            Manager.Dispose(disposing);
        }
    }

    public class ObjectLifeCycleManager {
        public static readonly ObjectLifeCycleManager Singleton = new ObjectLifeCycleManager();
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ObjectLifeCycleManager));

        private readonly LinkedList<IObjectLifeCycle> _watching = new LinkedList<IObjectLifeCycle>();
        private LinkedList<IDisposable> _queue = new LinkedList<IDisposable>();
        private LinkedList<IDisposable> _queueB = new LinkedList<IDisposable>();

        public int ProcessQueue() {
            lock (this) {
                if (_queue.Count == 0 && _queueB.Count == 0) return 0;
                var disposedCount = _queueB.Count;
                if (disposedCount > 0) {
#if DEBUG
                    Logger.Debug($"ProcessQueue. Disposed {_queueB.Count} elements from IQ");
#endif                    
                    foreach (var disposable in _queueB) disposable.Dispose();
                    _queueB.Clear();
                }
                // Swap queues
#if DEBUG
                if (_queue.Count > 0) {
                    Logger.Debug($"ProcessQueue. Moved {_queue.Count} elements from Q to IQ");
                }
#endif                    
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
            lock (this) {
                foreach (var disposable in disposables) _queue.AddLast(disposable);
            }
        }

        public List<IDisposable> GetQueue() {
            lock (this) return new List<IDisposable>(_queue);
        }

        public List<IDisposable> GetDisposablesNextFrame() {
            lock (this) return new List<IDisposable>(_queueB);
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
#if DEBUG
            Logger.Debug("ProcessWatching. Moved " + toEnqueue.Count + " elements");
#endif                    
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

        public void Dispose(bool disposing) {
            lock (_watching)
                foreach (var o in _watching) o.Dispose();
            lock (this) {
                foreach (var o in _queue) o.Dispose();
                foreach (var o in _queueB) o.Dispose();
            }
        }
    }
}