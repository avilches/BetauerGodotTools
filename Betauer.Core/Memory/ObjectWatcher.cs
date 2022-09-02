using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Memory {
    public interface IObjectWatched {
        public bool Process(bool force);
    }

    /// <summary>
    /// If any of the Watching objects is not valid, it will free (deferred or immediately) the targets 
    /// </summary>
    public class WatchAndFree : IObjectWatched {
        public readonly Object[] Targets;
        public readonly Object[] Watching;
        public readonly bool Deferred;

        public WatchAndFree(Object[] targets, Object[] watching, bool deferred) {
            Targets = targets;
            Watching = watching;
            Deferred = deferred;
        }

        public WatchAndFree(Object target, Object watching, bool deferred) :
            this(new[] { target }, watching, deferred) {
        }

        public WatchAndFree(Object target, Object[] watching, bool deferred) :
            this(new[] { target }, watching, deferred) {
        }

        public WatchAndFree(Object[] target, Object watching, bool deferred) :
            this(target, new[] { watching }, deferred) {
        }

        public bool Process(bool force) {
            if (force) {
                FreeNow();
                OnFree(true);
                return true;
            }
            if (IsWatchedValid()) {
                return false; // All watching objects are ok
            }
            // At least one watching object is not valid, free!
            if (Deferred) FreeDeferred();
            else FreeNow();
            OnFree(false);
            return true;
        }

        private void FreeNow() {
            Targets.Where(Object.IsInstanceValid).ForEach(o => o.Free());
        }

        private void FreeDeferred() {
            foreach (var target in Targets.Where(Object.IsInstanceValid)) {
                if (target is Node node) node.QueueFree();
                else target.CallDeferred("free");
            }
        }

        public virtual void OnFree(bool force) {
        }

        public virtual bool IsWatchedValid() {
            return Watching.All(Object.IsInstanceValid);
        }
    }

    public class DefaultObjectWatcher {
        public static ObjectWatcherRunner Instance = new ObjectWatcherRunner();
    }

    public class ObjectWatcher {
        private readonly List<IObjectWatched> _queue = new List<IObjectWatched>();

        public int WatchingCount => _queue.Count;

        public List<IObjectWatched> ToList() {
            lock (_queue) return new List<IObjectWatched>(_queue);
        }

        public int Process(bool force = false) {
            lock (_queue) return _queue.RemoveAll(e => e.Process(force));
        }

        public void Watch(IObjectWatched o) {
            lock (_queue) _queue.Add(o);
        }

        public void Unwatch(IObjectWatched o) {
            lock (_queue) _queue.RemoveAll(x => x == o);
        }

        public virtual void Dispose() {
            Process(true);
        }
    }

    public class ObjectWatcherRunner : ObjectWatcher {
        public readonly GodotScheduler Scheduler;

        public ObjectWatcherRunner() {
            // The Runner should run even if the SceneTree is paused
            Scheduler = new GodotScheduler(_OnSchedule, false);
        }

        public GodotScheduler Start(float seconds = 10f) {
            return Scheduler.Start(seconds);
        }

        private void _OnSchedule() => Process(false);

        public override void Dispose() {
            base.Dispose();
            Scheduler.Stop();
        }
    }
}