using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Memory {
    public interface IObjectWatched {
        public bool MustBeFreed();
        public Object Object { get; }
    }

    public class ObjectWatcherNode : Node {
        private readonly IObjectWatcher? _objectWatcher;
        private IObjectWatcher ObjectWatcher => _objectWatcher ?? DefaultObjectWatcher.Instance;
        private readonly float _seconds;
        private float _timeElapsed = 0;

        public ObjectWatcherNode(float seconds = 10, IObjectWatcher? objectWatcher = null) {
            _seconds = seconds;
            _objectWatcher = objectWatcher;
            PauseMode = PauseModeEnum.Process;
        }

        public override void _Ready() {
            this.DisableAllNotifications();
            SetProcess(true);
        }

        public override void _Process(float delta) {
            _timeElapsed += delta;
            if (_timeElapsed < _seconds) return;
            _timeElapsed -= _seconds;
            ObjectWatcher.Process();
        }

        protected override void Dispose(bool disposing) {
            ObjectWatcher.Dispose();
        }
    }

    public class DefaultObjectWatcher {
        public static IObjectWatcher Instance = new ObjectWatcher();
    }

    public interface IObjectWatcher : IDisposable {
        public void Watch(IObjectWatched o);
        public void Unwatch(IObjectWatched o);
        public void Process();
        public int WatchingCount { get; }
    }

    public class ObjectWatcher : IObjectWatcher {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ObjectWatcher));
        private readonly ConditionalConsumer<IObjectWatched> _watcher;

        public ObjectWatcher() {
            _watcher = new ConditionalConsumer<IObjectWatched>(
                o => o.MustBeFreed(),
                objects => {
                    foreach (var o in objects.Select(o => o.Object).Where(Object.IsInstanceValid)) {
                        o.CallDeferred("free");
                    }
                });
        }

        public void Watch(IObjectWatched o) => _watcher.Add(o);
        public void Unwatch(IObjectWatched o) => _watcher.Remove(o);

        public int WatchingCount => _watcher.Size;

        public void Process() {
            var freed = _watcher.Consume();
#if DEBUG
            if (freed > 0) {
                Logger.Debug($"Watching:{_watcher.Size} (-{freed})");
            }
#endif
        }

        public void Dispose() {
            _watcher.ForceConsumeAll();
        }
    }
}