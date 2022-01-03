using System;
using System.Threading.Tasks;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public interface ITweenPlayer<out TBuilder> {
        public Tween Tween { get; }
        public TBuilder CreateNewTween(Node node);
        public TBuilder WithTween(Tween tween);
        public TBuilder RemoveTween();
        public bool IsRunning();
        public TBuilder Start();
        public TBuilder Stop();
        public TBuilder Reset();
        public TBuilder Kill();
    }

    public abstract class RepeatablePlayer<TBuilder> : Object /* needed to be callable by interpolate_callback */,
        ITweenPlayer<TBuilder> where TBuilder : class {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(RepeatablePlayer<>));

        protected bool Started = false;
        protected bool Running = false;
        protected bool FreeTweenOnFinish = false;
        protected SimpleLinkedList<Action> OnFinishAll;

        public Tween Tween { get; private set; }

        public RepeatablePlayer() {
        }

        public RepeatablePlayer(Tween tween, bool freeTweenOnFinish = false) {
            WithTween(tween, freeTweenOnFinish);
        }

        public TBuilder CreateNewTween(Node node, bool freeTweenOnFinish) {
            FreeTweenOnFinish = freeTweenOnFinish;
            return CreateNewTween(node);
        }

        public TBuilder CreateNewTween(Node node) {
            var tween = new Tween();
            node.AddChild(tween);
            return WithTween(tween);
        }

        public TBuilder WithTween(Tween tween) {
            RemoveTween();
            Tween = tween;
            // _tween.Connect("tween_all_completed", this, nameof(OnTweenAllCompletedSignaled));
            // _tween.Connect("tween_started", this, nameof(TweenStarted));
            // _tween.Connect("tween_completed", this, nameof(TweenCompleted));
            return this as TBuilder;
        }

        public TBuilder WithTween(Tween tween, bool freeTweenOnFinish) {
            FreeTweenOnFinish = freeTweenOnFinish;
            WithTween(tween);
            return this as TBuilder;
        }

        public TBuilder RemoveTween() {
            if (Tween != null) {
                Running = false;
                Reset();
                Tween = null;
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

        public TBuilder SetFreeTweenOnFinish(bool freeTweenOnFinish) {
            FreeTweenOnFinish = freeTweenOnFinish;
            return this as TBuilder;
        }

        public bool IsRunning() => Running;

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public TBuilder Start() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Start with a freed Tween instance");
                return this as TBuilder;
            }
            if (!Started) {
                DoStart();
            } else {
                if (!Running) {
                    Logger.Info("Tween.ResumeAll()");
                    Tween.ResumeAll();
                    Running = true;
                }
            }
            return this as TBuilder;
        }

        public TBuilder Stop() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Stop with a freed Tween instance");
                return this as TBuilder;
            }
            if (Running) {
                Logger.Info("Tween.StopAll()");
                Tween.StopAll();
                Running = false;
            }
            return this as TBuilder;
        }

        public TBuilder Reset() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Reset with a freed Tween instance");
                return this as TBuilder;
            }
            Logger.Info("Tween.RemoveAll()");
            Tween.RemoveAll();
            OnReset();
            if (Running) {
                DoStart();
            } else {
                Started = false;
            }
            return this as TBuilder;
        }

        public TBuilder Kill() {
            if (!Object.IsInstanceValid(Tween)) return this as TBuilder;
            if (Running) {
                Stop();
            }
            Reset();
            Logger.Info("Tween.QueueFree()");
            Tween.QueueFree();
            return this as TBuilder;
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
            Logger.Info("Tween.Start()");
            Tween.Start();
        }

        protected void Finished() {
            Logger.Debug($"Finished. End: stop & reset. Kill {FreeTweenOnFinish}");
            Running = false; // this will avoid to start again in the next Reset() call
            Reset();
            // Started = false;
            // End of ALL THE LOOPS of all the sequences of the player
            if (OnFinishAll != null)
                foreach (var callback in OnFinishAll)
                    callback.Invoke();
            _promise.TrySetResult(this as TBuilder);
            // EmitSignal(nameof(finished));
            if (FreeTweenOnFinish) Kill();
        }

        public Task<TBuilder> Await() {
            return _promise.Task;
        }
    }
}