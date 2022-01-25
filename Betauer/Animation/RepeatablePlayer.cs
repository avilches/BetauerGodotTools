using System;
using System.Threading.Tasks;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public interface ITweenPlayer<out TBuilder> {
        public DisposableTween DisposableTween { get; }
        public TBuilder CreateNewTween(Node node);
        public TBuilder WithTween(DisposableTween tween);
        public TBuilder RemoveTween();
        public bool IsRunning();
        public TBuilder Start();
        public TBuilder Stop();
        public TBuilder Reset();
    }

    public abstract class RepeatablePlayer<TBuilder> :
        DisposableGodotObject /* needed to be callable by interpolate_callback */,
        ITweenPlayer<TBuilder> where TBuilder : class {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(RepeatablePlayer<>));

        protected bool Started = false;
        protected bool Running = false;
        protected bool DisposeOnFinish = false;
        protected SimpleLinkedList<Action> OnFinishAll;

        public DisposableTween DisposableTween { get; private set; }

        public RepeatablePlayer() {
        }

        public RepeatablePlayer(DisposableTween tween, bool disposeOnFinish = false) {
            WithTween(tween, disposeOnFinish);
        }

        public TBuilder CreateNewTween(Node node, bool disposeOnFinish) {
            DisposeOnFinish = disposeOnFinish;
            return CreateNewTween(node);
        }

        public TBuilder CreateNewTween(Node node) {
            var tween = new DisposableTween();
            node.AddChild(tween);
            return WithTween(tween);
        }

        public TBuilder WithTween(DisposableTween tween) {
            RemoveTween();
            DisposableTween = tween;
            // _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
            // _tween.Connect("tween_started", this, nameof(TweenStarted));
            // _tween.Connect("tween_completed", this, nameof(TweenCompleted));
            return this as TBuilder;
        }

        public TBuilder WithTween(DisposableTween tween, bool disposeOnFinish) {
            DisposeOnFinish = disposeOnFinish;
            WithTween(tween);
            return this as TBuilder;
        }

        public TBuilder RemoveTween() {
            if (DisposableTween != null) {
                Running = false;
                Reset();
                DisposableTween = null;
            }
            return this as TBuilder;
        }

        public TBuilder AddOnFinishAll(Action onFinishAll) {
            if (OnFinishAll == null) {
                OnFinishAll = new SimpleLinkedList<Action>
                    { onFinishAll };
            } else {
                OnFinishAll.Add(onFinishAll);
            }
            return this as TBuilder;
        }

        public TBuilder SetDisposeOnFinish(bool disposeOnFinish) {
            DisposeOnFinish = disposeOnFinish;
            return this as TBuilder;
        }

        public bool IsRunning() => Running;

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public TBuilder Start() {
            if (Disposed) return this as TBuilder;
            if (!IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Start with a freed DisposableTween instance");
                return this as TBuilder;
            }
            if (!Started) {
                DoStart();
            } else {
                if (!Running) {
                    Logger.Info("DisposableTween.ResumeAll()");
                    DisposableTween.ResumeAll();
                    Running = true;
                }
            }
            return this as TBuilder;
        }

        public TBuilder Stop() {
            if (Disposed) return this as TBuilder;
            if (!IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Stop with a freed DisposableTween instance");
                return this as TBuilder;
            }
            if (Running) {
                Logger.Info("DisposableTween.StopAll()");
                DisposableTween.StopAll();
                Running = false;
            }
            return this as TBuilder;
        }

        public TBuilder Reset() {
            if (Disposed) return this as TBuilder;
            if (!IsInstanceValid(DisposableTween)) {
                Logger.Warning("Can't Reset with a freed DisposableTween instance");
                return this as TBuilder;
            }
            Logger.Info("DisposableTween.RemoveAll()");
            DisposableTween.RemoveAll();
            OnReset();
            if (Running) {
                DoStart();
            } else {
                Started = false;
            }
            return this as TBuilder;
        }


        protected override void Dispose(bool disposing) {
            try {
                DisposableTween.Dispose();
            } finally {
                base.Dispose(disposing);
            }
        }

        protected abstract void OnStart();
        protected abstract void OnReset();

        private TaskCompletionSource<TBuilder> _promise;

        private void DoStart() {
            if (_promise != null && !_promise.Task.IsCompleted) {
                // This can't happen, but just in case...
                _promise.SetCanceled();
            }
            _promise = new TaskCompletionSource<TBuilder>();
            Started = true;
            Running = true;
            OnStart();
            Logger.Info("DisposableTween.Start()");
            DisposableTween.Start();
        }

        protected void Finished() {
            Logger.Debug($"Finished. End: stop & reset. Kill {DisposeOnFinish}");
            Running = false; // this will avoid to start again in the next Reset() call
            Reset();
            // Started = false;
            // End of ALL THE LOOPS of all the sequences of the player
            if (OnFinishAll != null)
                foreach (var callback in OnFinishAll)
                    callback.Invoke();
            if (DisposeOnFinish) {
                Dispose();
            }
            _promise.TrySetResult(this as TBuilder);
            // EmitSignal(nameof(finished));
        }

        public Task<TBuilder> Await() {
            return _promise.Task;
        }
    }
}